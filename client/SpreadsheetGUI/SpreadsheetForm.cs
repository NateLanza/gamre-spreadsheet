﻿using SS;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI {
    public partial class SpreadsheetForm : Form {
        private SpreadsheetState Spreadsheet;

        private SpreadsheetController Controller;

        /// <summary>
        /// Creates a new, empty spreadsheet
        /// </summary>
        public SpreadsheetForm() {
            InitializeComponent();

            // Create model and controller
            Spreadsheet = new SpreadsheetState();
            Controller = new SpreadsheetController();

            // Register events
            Controller.CellChanged += HandleCellChange;
            Controller.ChangeRejected += HandleInvalidChange;
            Controller.SelectionChanged += HandleSelectionChange;
            Controller.ConnectionAttempted += HandleServerConnection;
            Controller.OtherClientDisconnected += HandleClientDisconnect;
            Controller.Disconnected += HandleDisconnect;
            this.FormClosing += SpreadsheetClosing;
            SpreadsheetGrid.SelectionChanged += SpreadsheetChanged;

            // Add keyboard event handlers
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormKeyDown);
        }

        /// <summary>
        /// Event handler for when the server rejects a change request
        /// </summary>
        /// <param name="cell">Cell which the change was requested for</param>
        /// <param name="message">Server message</param>
        public void HandleInvalidChange(string cell, string message) {
            this.Invoke((MethodInvoker)delegate {
                MessageBox.Show("Invalid change to cell " + cell + ": " + message);
            });
        }

        /// <summary>
        /// Event handler for when the server indicates that a cell has changed
        /// </summary>
        /// <param name="cell">Changed cell</param>
        /// <param name="contents">Cell contents</param>
        public void HandleCellChange(string cell, string content) {
            this.Invoke((MethodInvoker) delegate {
                UpdateCellContents(cell, content);
            });
        }

        /// <summary>
        /// Event handler for when the server indicates that a cell selection has changed
        /// </summary>
        /// <param name="cell">Cell selected</param>
        /// <param name="name">Name of selector</param>
        /// <param name="id">ID of selector</param>
        public void HandleSelectionChange(string cell, string name, int id) {
            this.Invoke((MethodInvoker) delegate {
                if (id == Controller.ID) {
                    int[] cellCoords = CellToRowCol(cell);
                    SpreadsheetGrid.SetSelection(cellCoords[1], cellCoords[0]);
                    SelectedCellContent.Text = Spreadsheet.GetCellContents(cell).ToString();
                    SelectedCellLabel.Text = "Selected Cell: " + cell;
                    UpdateCellValueBox();
                } else {
                    SetNetworkSelection(id, cell);
                }
            });
        }

        /// <summary>
        /// Event handler for when this client connects to a server and receives a spreadsheet list
        /// </summary>
        /// <param name="error">Whether an error occurred on connection</param>
        /// <param name="spreadsheets">Spreadsheet list</param>
        public void HandleServerConnection(bool error, List<string> spreadsheets) {
            this.Invoke((MethodInvoker) delegate {
                if (error) {
                    IPTextBox.Enabled = true;
                    UsernameBox.Enabled = true;
                    ConnectButton.Enabled = true;
                    MessageBox.Show("Unable to connect to server", "", MessageBoxButtons.OK);
                    return;
                }
                PopulateSpreadsheetNameList(spreadsheets);
            });
        }

        /// <summary>
        /// Handler for when another client disconnects from the server
        /// </summary>
        /// <param name="ID">ID of client</param>
        public void HandleClientDisconnect(int ID) {
            this.Invoke((MethodInvoker)delegate {
                RemoveNetworkConnection(ID);
            });
        }

        /// <summary>
        /// Handler for when this client disconnects from the server
        /// </summary>
        public void HandleDisconnect(string controllerMessage) {
            this.Invoke((MethodInvoker)delegate {
                MessageBox.Show("Disconnected from the server: " + controllerMessage, "", MessageBoxButtons.OK);
                IPTextBox.Enabled = true;
                UsernameBox.Enabled = true;
                ConnectButton.Enabled = true;
            });
        }

        /// <summary>
        /// Sets cntrlPressed to true when the control key is pressed, or false when it is released
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Control) {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Handles pressing of keys for keyboard shortcuts
        /// </summary>
        private void FormKeyDown(object sender, KeyEventArgs args) {
            if (!args.Control) {
                return;
            }

            // Close when user presses control + x
            if (args.KeyCode == Keys.X) {
                this.Close();
            }
        }

        /// <summary>
        /// Changes the contents of a cell, both in the backend and in the display
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="content"></param>
        private void UpdateCellContents(String cell, String content) {
            IList<string> ToRecalculate;
            // Update backend
            try {
                ToRecalculate = Spreadsheet.SetContentsOfCell(cell, content);
            } catch (Exception e) {
                if (typeof(FormulaFormatException).IsInstanceOfType(e)) {
                    int[] rowCol = CellToRowCol(cell);
                    SpreadsheetGrid.SetValue(rowCol[1], rowCol[0], "Invalid formula: " + e.Message);
                } else if (typeof(ArgumentException).IsInstanceOfType(e)) {
                    int[] rowCol = CellToRowCol(cell);
                    SpreadsheetGrid.SetValue(rowCol[1], rowCol[0], "Circular Error: Cells cannot depend on each other in a circular fashion");
                }
                return;
            }   
            // Update display for all necessary cells
            foreach(String newCell in ToRecalculate) {
                int[] rowCol = CellToRowCol(newCell);
                SpreadsheetGrid.SetValue(rowCol[1], rowCol[0], Spreadsheet.GetCellValue(newCell).ToString());
            }
        }

        

        /// <summary>
        /// Converts a cell string to row and column values
        /// </summary>
        /// <param name="cell">Cell to convert</param>
        /// <returns>Row at index 0, col at index 1</returns>
        private int[] CellToRowCol(String cell) {
            char col = cell.ToUpper()[0];
            int.TryParse(cell.Substring(1), out int row);

            return new int[] { --row, (int)col - 65 };
        }

        /// <summary>
        /// Converts row and column ints to a cell string
        /// </summary>
        /// <param name="col">Column number, 0-based</param>
        /// <param name="row">Row number, 0-based</param>
        /// <returns>[letter][number] cell</returns>
        private string RowColToCell(int row, int col) {
            char colLetter = (char) (col + 65);

            return colLetter + (++row).ToString();
        }

        /// <summary>
        /// Updates the cell value box to reflect the selected cell
        /// </summary>
        private void UpdateCellValueBox() {
            SpreadsheetGrid.GetSelection(out int col, out int row);
            SpreadsheetGrid.GetValue(col, row, out string value);
            CellValueBox.Text = value;
        }

        private void SpreadsheetClosing(Object sender, FormClosingEventArgs args) {
            // Fires when the spreadsheet window closes. Needs to send remaining changes to server and disconnect
        }

        /// <summary>
        /// Runs when the spreadsheet grid changes
        /// </summary>
        /// <param name="sender">The grid that changed</param>
        private void SpreadsheetChanged(SpreadsheetPanel sender) {
            SpreadsheetGrid.GetSelection(out int col, out int row);
            String cell = RowColToCell(row, col);
            Controller.SendSelectRequest(cell);
        }

        /// <summary>
        /// Runs when the content of the selected cell box changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedCellContent_TextChanged(object sender, EventArgs e) {
            SpreadsheetGrid.GetSelection(out int col, out int row);
            String cell = RowColToCell(row, col);
            Controller.SendEditRequest(cell, SelectedCellContent.Text);
        }

        private void HelpButton_Click(object sender, EventArgs e) {
            Help help = new Help();
            help.ShowDialog();
        }

        /// <summary>
        /// When the connect button is pressed - starts connection process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = false;
            IPTextBox.Enabled = false;
            UsernameBox.Enabled = false;
            Controller.ConnectToServer(IPTextBox.Text, UsernameBox.Text);
        }

        /// <summary>
        /// Populates the list of spreadsheets available to open with the given list of strings.
        /// Also sets the gui elements to enabled for opening a spreadsheet.
        /// </summary>
        /// <param name="names">List of spreadsheet names</param>
        private void PopulateSpreadsheetNameList(List<string> names)
        {
            SpreadsheetNameList.Items.AddRange(names.ToArray());
            SpreadsheetNameList.Enabled = true;
            OpenButton.Enabled = true;
        }

        /// <summary>
        /// Sets the selected cell for a networked client
        /// </summary>
        /// <param name="id">ID of client</param>
        /// <param name="cellName">Name of client</param>
        private void SetNetworkSelection(int id, string cellName)
        {
            //we assume that the cellname is a single capital character followed by an integer
            int col = (int)cellName[0] - 65;
            int row = int.Parse(cellName.Substring(1));

            SpreadsheetGrid.SetNetworkSelection(id, col, row);
        }

        /// <summary>
        /// Removes a networked client & their selection indicator by ID
        /// </summary>
        /// <param name="id">ID of client to remove</param>
        private void RemoveNetworkConnection(int id) {
            SpreadsheetGrid.RemoveNetworkSelection(id);
        }

        /// <summary>
        /// Send the command to open a given spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenButton.Enabled = false;
            SpreadsheetNameList.Enabled = false;
            Controller.ConnectToSpreadsheet(SpreadsheetNameList.Text);
        }
    }
}
