using SS;
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

            SpreadsheetGrid.SelectionChanged += SpreadsheetChanged;

            Spreadsheet = new SpreadsheetState();
            Controller = new SpreadsheetController(Spreadsheet);

            this.FormClosing += SpreadsheetClosing;

            // Add keyboard event handlers
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormKeyDown);
        }

        /// <summary>
        /// Creates a new spreadsheet from a file path
        /// </summary>
        /// <param name="path"></param>
        public SpreadsheetForm(String path) {
            InitializeComponent();

            SpreadsheetGrid.SelectionChanged += SpreadsheetChanged;

            Spreadsheet = new SpreadsheetState();

            foreach (String newCell in Spreadsheet.GetNamesOfAllNonemptyCells()) {
                int[] rowCol = CellToRowCol(newCell);
                SpreadsheetGrid.SetValue(rowCol[1], rowCol[0], Spreadsheet.GetCellValue(newCell).ToString());
            }

            this.FormClosing += SpreadsheetClosing;

            // Add keyboard event handlers
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormKeyDown);
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
            SelectedCellContent.Text = Spreadsheet.GetCellContents(cell).ToString();
            SelectedCellLabel.Text = "Selected Cell: " + cell;
            UpdateCellValueBox();
        }

        /// <summary>
        /// Runs when the content of the selected cell box changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedCellContent_TextChanged(object sender, EventArgs e) {
            SpreadsheetGrid.GetSelection(out int col, out int row);
            String cell = RowColToCell(row, col);
            UpdateCellContents(cell, SelectedCellContent.Text);
            UpdateCellValueBox();
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
            Controller.ConnectToServer(IPTextBox.Text);
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cellName"></param>
        private void SetNetworkSelection(int id, string cellName)
        {
            //we assume that the cellname is a single capital character followed by an integer
            int col = (int)cellName[0] - 65;
            int row = int.Parse(cellName.Substring(1));

            SpreadsheetGrid.SetNetworkSelection(id, col, row);
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
