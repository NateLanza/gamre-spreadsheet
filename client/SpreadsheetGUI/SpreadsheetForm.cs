using SpreadsheetUtilities;
using SS;
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
        private Spreadsheet spreadsheet;

        /// <summary>
        /// Creates a new, empty spreadsheet
        /// </summary>
        public SpreadsheetForm() {
            InitializeComponent();

            SpreadsheetGrid.SelectionChanged += SpreadsheetChanged;

            spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "ps6");

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

            spreadsheet = new Spreadsheet(path, s => true, s => s.ToUpper(), "ps6");

            foreach (String newCell in spreadsheet.GetNamesOfAllNonemptyCells()) {
                int[] rowCol = CellToRowCol(newCell);
                SpreadsheetGrid.SetValue(rowCol[1], rowCol[0], spreadsheet.GetCellValue(newCell).ToString());
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

            if (args.KeyCode == Keys.S) {
                SaveFile.Filter = "Spreadsheets (*.sprd)|*.sprd|All files (*.*)|*.*";
                SaveFile.ShowDialog();
            } else if (args.KeyCode == Keys.O) {
                OpenFile.Filter = "Spreadsheets (*.sprd)|*.sprd|All files (*.*)|*.*";
                OpenFile.ShowDialog();
            } else if (args.KeyCode == Keys.N) {
                NewSpreadsheet();
            } else if (args.KeyCode == Keys.X) {
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
                ToRecalculate = spreadsheet.SetContentsOfCell(cell, content);
            } catch (Exception e) {
                if (typeof(FormulaFormatException).IsInstanceOfType(e)) {
                    int[] rowCol = CellToRowCol(cell);
                    SpreadsheetGrid.SetValue(rowCol[1], rowCol[0], "Invalid formula: " + e.Message);
                } else if (typeof(CircularException).IsInstanceOfType(e)) {
                    int[] rowCol = CellToRowCol(cell);
                    SpreadsheetGrid.SetValue(rowCol[1], rowCol[0], "Circular Error: Cells cannot depend on each other in a circular fashion");
                }
                return;
            }   
            // Update display for all necessary cells
            foreach(String newCell in ToRecalculate) {
                int[] rowCol = CellToRowCol(newCell);
                SpreadsheetGrid.SetValue(rowCol[1], rowCol[0], spreadsheet.GetCellValue(newCell).ToString());
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
            if (spreadsheet.Changed) {
                DialogResult result = MessageBox.Show("Do you want to save your changes before closing?", "", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) {
                    SaveFile.Filter = "Spreadsheets (*.sprd)|*.sprd|All files (*.*)|*.*";
                    SaveFile.ShowDialog();
                } else if (result == DialogResult.Cancel) {
                    args.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Creates a new this and opens a window for it
        /// </summary>
        private void NewSpreadsheet() {
            SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetForm());
        }

        private void OpenSpreadsheet(String path) {
            try {
                SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetForm(path));
            } catch (SpreadsheetReadWriteException e) {
                Console.WriteLine(e.Message);
                MessageBox.Show("Unable to open file", "", MessageBoxButtons.OK);
            }
        }

        private void SaveSpreadsheet(string path) {
            // Guarantee extension
            path = path.Length >= 5 && path.Substring(path.Length - 5) == ".sprd" ? path : path + ".sprd";
            // Warn if file exists
            if (File.Exists(path)) {
                DialogResult result = MessageBox.Show("Are you sure you want to overwrite this file?", "", MessageBoxButtons.OKCancel);
                if (result != DialogResult.OK) {
                    return;
                }
            }

            spreadsheet.Save(path);
        }

        /// <summary>
        /// Runs when the spreadsheet grid changes
        /// </summary>
        /// <param name="sender">The grid that changed</param>
        private void SpreadsheetChanged(SpreadsheetPanel sender) {
            SpreadsheetGrid.GetSelection(out int col, out int row);
            String cell = RowColToCell(row, col);
            SelectedCellContent.Text = spreadsheet.GetCellContents(cell).ToString();
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

        /// <summary>
        /// Runs when file > new is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            NewSpreadsheet();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFile.Filter = "Spreadsheets (*.sprd)|*.sprd|All files (*.*)|*.*";
            OpenFile.ShowDialog();
        }

        private void OpenFile_FileOk(object sender, CancelEventArgs e) {
            OpenSpreadsheet(OpenFile.FileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFile.Filter = "Spreadsheets (*.sprd)|*.sprd|All files (*.*)|*.*";
            SaveFile.ShowDialog();
        }

        private void SaveFile_FileOk(object sender, CancelEventArgs e) {
            SaveSpreadsheet(SaveFile.FileName);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void HelpButton_Click(object sender, EventArgs e) {
            Help help = new Help();
            help.ShowDialog();
        }
    }
}
