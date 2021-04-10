using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml;
using System.Linq.Expressions;

namespace SS {

    /// <summary>
    /// An Spreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet {

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved, else false
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Maps cell names to their contents
        /// </summary>
        private Dictionary<String, Object> cells;

        /// <summary>
        /// Maps cell names to their values
        /// </summary>
        private Dictionary<String, Object> values;

        /// <summary>
        /// Tracks all cell dependencies
        /// </summary>
        private DependencyGraph dependencies;

        /// <summary>
        /// Creates a new, empty spreadsheet
        /// </summary>
        public Spreadsheet(): base(s => true, s => s, "default") {
            cells = new Dictionary<string, object>();
            dependencies = new DependencyGraph();
            values = new Dictionary<string, object>();
            Changed = false;
        }

        /// <summary>
        /// Creates a new spreadsheet from a file on disk
        /// </summary>
        /// <param name="path">Path to saved spreadsheet</param>
        /// <param name="isValid">Function to validate variables</param>
        /// <param name="normalize">Function to normalize variables</param>
        /// <param name="version">Version of spreadsheet</param>
        /// <exception cref="SpreadsheetReadWriteException">
        /// If the saved spreadsheet's version doesn't match the version parameter,
        /// or any names in the saved spreadsheet are invalid,
        /// or invalid formulas or circular dependencies are encountered,
        /// or an error related to reading the file is thrown.
        /// </exception>
        public Spreadsheet(string path, Func<string, bool> isValid, Func<string, string> normalize, string version): base(isValid, normalize, version) {
            cells = new Dictionary<string, object>();
            dependencies = new DependencyGraph();
            values = new Dictionary<string, object>();

            try {
                using (XmlReader reader = XmlReader.Create(path)) {
                    // Make sure we have content & init reader
                    if (!reader.Read()) {
                        throw new SpreadsheetReadWriteException("No content found");
                    }

                    // Check for spreadsheet node & version
                    if (reader.IsStartElement() && reader.Name == "spreadsheet") {
                        if (reader.GetAttribute("version") != Version) {
                            throw new SpreadsheetReadWriteException("File version does not match version argument supplied");
                        }
                    } else {
                        throw new SpreadsheetReadWriteException("spreadsheet node not found");
                    }

                    // Loop through cells
                    while (reader.Read()) {
                        Console.WriteLine(reader.Name + "|" + reader.Value);
                        //continue;
                        // Check for cell
                        if (reader.IsStartElement() && reader.Name == "cell") {
                            // Check for cell name
                            if (reader.Read() && reader.MoveToContent() == XmlNodeType.Element && reader.Name == "name") {
                                reader.Read();
                                reader.MoveToContent();
                                String cellName = reader.Value;

                                // Check for closing tag
                                if (!reader.Read() || reader.Name != "name") {
                                    throw new SpreadsheetReadWriteException("Closing </name> tag not found");
                                }

                                // Check for cell contents
                                if (reader.Read() && reader.MoveToContent() == XmlNodeType.Element && reader.Name == "contents") {
                                    reader.Read();
                                    reader.MoveToContent();

                                    // Check for content
                                    if (reader.Value != "") {
                                        SetContentsOfCell(cellName, reader.Value);

                                        // Check for closing tag
                                        if (!reader.Read() || reader.Name != "contents") {
                                            throw new SpreadsheetReadWriteException("Closing </contents> tag not found");
                                        }
                                    } else {
                                        if (reader.Name != "cell") {
                                            throw new SpreadsheetReadWriteException("Closing </cell> tag not found");
                                        } else {
                                            continue;
                                        }
                                    }

                                } else {
                                    throw new SpreadsheetReadWriteException("Contents tag must follow name tag in cell element");
                                }

                            } else {
                                throw new SpreadsheetReadWriteException("Cell tag must immediately contain name tag");
                            }

                            //Check for closing tag
                            // Check for closing tag
                            reader.Read();
                            reader.MoveToContent();
                            if (reader.Name != "cell") {
                                throw new SpreadsheetReadWriteException("Closing </cell> tag not found");
                            }

                            // Check for end tag
                        } else if (reader.Name == "spreadsheet") {
                            break;
                        } else {
                            throw new SpreadsheetReadWriteException("Invalid tag name encountered: " + reader.Name);
                        }
                    }

                }
            // Catch any exceptions
            } catch (Exception e) {
                if (e.GetType() == typeof(SpreadsheetReadWriteException)) {
                    throw e;
                } else {
                    throw new SpreadsheetReadWriteException("An exception of type " + e.GetType() + " was thrown while reading the file, with message " + e.Message);
                }
            }

            Changed = false;
        }

        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version) {
            cells = new Dictionary<string, object>();
            dependencies = new DependencyGraph();
            values = new Dictionary<string, object>();
            Changed = false;
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file
        /// </summary>
        /// <param name="filename">Path to file</param>
        /// <exception cref="SpreadsheetReadWriteException">If there are any problems
        /// opening, reading, or closing the file</exception>
        /// <returns>Version information</returns>
        public override string GetSavedVersion(string filename) {
            try {
                using (XmlReader reader = XmlReader.Create(filename)) {
                    // Make sure we have content & init reader
                    if (!reader.Read()) {
                        throw new SpreadsheetReadWriteException("No content found");
                    }

                    // Check for spreadsheet node & version
                    if (reader.IsStartElement() && reader.Name == "spreadsheet") {
                        if (reader.GetAttribute("version") == null) {
                            throw new SpreadsheetReadWriteException("No version found");
                        } else {
                            return reader.GetAttribute("version");
                        }
                    } else {
                        throw new SpreadsheetReadWriteException("spreadsheet node not found");
                    }
                }
            } catch (Exception e) {
                throw new SpreadsheetReadWriteException("An exception of type " + e.GetType() + " was thrown while reading the file, with message " + e.Message);
            }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet ot a file using an XML format
        /// XML is structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// </summary>
        /// <exception cref="SpreadsheetReadWriteException">Uf there are any problems opening,
        /// writing, or closing the file</exception>
        /// <param name="filename">File to save to</param>
        public override void Save(string filename) {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            try {
                using (XmlWriter writer = XmlWriter.Create(filename, settings)) {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    // Write all cells to file
                    foreach (String cell in cells.Keys) {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell);

                        // Write based on cell type
                        Object contents = GetCellContents(cell);
                        if (typeof(Formula).IsInstanceOfType(contents)) {
                            writer.WriteElementString("contents", "=" + contents.ToString());
                        } else {
                            writer.WriteElementString("contents",  contents.ToString());
                        }

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    Console.WriteLine(writer.ToString());
                }
            } catch (Exception) {
                throw new SpreadsheetReadWriteException("The xml writer threw an exception while trying to write to file");
            }

            Changed = false;
        }

        /// <summary>
        /// Sets a cell's content. Content string will be parsed into a double, formula if it
        /// starts with =, or string if the other two fail. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <exception cref="ArgumentNullException">If content is null</exception>
        /// <exception cref="InvalidNameException">If name is null or invalid</exception>
        /// <exception cref="FormulaFormatException">If the formula is formatted incorrectly</exception>
        /// <exception cref="CircularException">If changing the named cell's contents
        /// would cause a circular dependency</exception>
        /// <returns>A list consisting of name plus the names of all other cells whose value depends
        /// on the named cell</returns>
        public override IList<string> SetContentsOfCell(string name, string content) {
            // Check for invalid values
            ValidateParameters(name, content);

            double value;
            IList<string> result;
            // Try formula
            if (content.Length > 0 && content[0] == '=') {
                result = SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            // Try double
            } else if (Double.TryParse(content, out value)) {
                result = SetCellContents(name, value);
            // Try string
            } else {
                result = SetCellContents(name, content);
            }

            // If an error was not thrown above, the cell is assigned and needs evaluation
            EvaluateCell(name);

            Changed = true;

            return result;
        }

        /// <summary>
        /// Returns the value of the named cell
        /// </summary>
        /// <param name="name">The named cell</param>
        /// <returns>String, double, or SpreadsheetUtilities.FormulaError</returns>
        public override object GetCellValue(string name) {
            if (values.ContainsKey(name)) {
                return values[name];
            } else {
                return "";
            }
            
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet
        /// </summary>
        /// <returns>Enumerable HashSet</returns>
        public override IEnumerable<String> GetNamesOfAllNonemptyCells() {
            // Get all cell names
            List<String> keys = cells.Keys.ToList();

            // Prune empty cells
            foreach (String key in cells.Keys) {
                if (typeof(String).IsInstanceOfType(cells[key]) && (String) cells[key] == "") {
                    keys.Remove(key);
                }
            }

            return keys;
        }

        /// <summary>
        /// Validates a cell name and an argument, for use in other methods. The argument must
        /// not be null, otherwise an ArgumentNullException is thrown. The cell name must be valid
        /// as defined in the class comment. If both parameters are valid, no action is taken.
        /// </summary>
        /// <param name="name">Cell name</param>
        /// <param name="arg">Object to check for null</param>
        /// <exception cref="InvalidNameException">If name is invalid</exception>
        /// <exception cref="ArgumentNullException">If arg is null</exception>
        private void ValidateParameters(String name, Object arg) {
            // Validate arg
            if (arg == null) {
                throw new ArgumentNullException();
            }
            // Validate cell name
            ValidateParameters(name);
        }

        /// <summary>
        /// Validates a cell name, for use in other methods. The cell name must be valid
        /// as defined in the class comment. If cell name is valid, no action is taken.
        /// </summary>
        /// <param name="name">Cell name</param>
        /// <exception cref="InvalidNameException">If name is invalid</exception>
        private void ValidateParameters(String name) {
            // Validate cell name
            if (name != null && Regex.IsMatch(name, @"^[a-zA-Z]+[0-9]+$") && IsValid(name)) {
                return;
            } else {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// Returns the contents (not value) of the named cell.
        /// </summary>
        /// <param name="name">Named cell</param>
        /// <exception cref="InvalidNameException">If name is null or invalid</exception>
        /// <returns>string, double, or Formula</returns>
        public override object GetCellContents(String name) {
            // Check name
            ValidateParameters(name);

            // Get value
            Object result;
            cells.TryGetValue(Normalize(name), out result);

            // Return "" if result was null, else result
            return result == null ? "" : result;
        }

        /// <summary>
        /// Sets the contents of a cell to a number, then returns a list consisting of name
        /// plus the names of all other cells whose value depends, directly or indirectly, on
        /// the named cell.
        /// </summary>
        /// <param name="name">Name of cell to set</param>
        /// <param name="number">Number to set cell to</param>
        /// <exception cref="InvalidNameException">If name is null or invalid</exception>
        /// <returns>List of cell names</returns>
        protected override IList<String> SetCellContents(String name, double number) {
            // Check name
            ValidateParameters(name, number);

            // Remove prior dependencies
            RemoveDependencies(name);

            // Assign cell
            cells[name] = number;

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Sets the contents of a cell to text, then returns a list consisting of name
        /// plus the names of all other cells whose value depends, directly or indirectly, on
        /// the named cell.
        /// </summary>
        /// <param name="name">Name of cell to set</param>
        /// <param name="text">Text to set cell to</param>
        /// <exception cref="ArgumentNullException">If text is null</exception>
        /// <exception cref="InvalidNameException">If name is null or invalid</exception>
        /// <returns>List of cell names</returns>
        protected override IList<String> SetCellContents(String name, String text) {
            // Check name
            ValidateParameters(name, text);

            // Remove prior dependencies
            RemoveDependencies(name);

            // Assign cell
            cells[name] = text;

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Sets the contents of a cell to text, then returns a list consisting of name
        /// plus the names of all other cells whose value depends, directly or indirectly, on
        /// the named cell.
        /// </summary>
        /// <param name="name">Name of cell to set</param>
        /// <param name="formula">Text to set cell to</param>
        /// <exception cref="ArgumentNullException">If formula is null</exception>
        /// <exception cref="InvalidNameException">If name is null or invalid</exception>
        /// <returns>List of cell names</returns>
        protected override IList<String> SetCellContents(String name, Formula formula) {
            // Check name
            ValidateParameters(name, formula);

            // Remove prior dependencies
            RemoveDependencies(name);

            // Assign cell
            cells[name] = formula;

            // Add dependencies
            foreach (String var in formula.GetVariables()) {
                dependencies.AddDependency(var, name);
            }

            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// Removes all dependencies associated with a cell when it is reassigned
        /// </summary>
        /// <param name="name"></param>
        private void RemoveDependencies(String name) {
            if (cells.ContainsKey(name) && typeof(Formula).IsInstanceOfType(cells[name])) {
                foreach (String var in ((Formula)cells[name]).GetVariables()) {
                    dependencies.RemoveDependency(var, name);
                }
            }
        }

        /// <summary>
        /// Helper method which reassigns the value of a cell and any cells which depend on it
        /// Should be run after the content of the cell in this.cells has been reassigned
        /// </summary>
        /// <param name="name">Cell to reevaluate</param>
        /// <exception cref="CircularException">If a circular dependency is found while evaluating cells</exception>
        private void EvaluateCell(String name) {
            // Reassign if string or double
            if (typeof(string).IsInstanceOfType(cells[name]) || typeof(double).IsInstanceOfType(cells[name])) {
                values[name] = cells[name];
            } else if (typeof(Formula).IsInstanceOfType(cells[name])) {
                values[name] = ((Formula)cells[name]).Evaluate(CellLookup);
            }

            // Recalculate all dependent cells
            foreach(String cell in GetCellsToRecalculate(name)) {
                if (cell == name) {
                    continue;
                }
                EvaluateCell(cell);
            }
        }

        /// <summary>
        /// Helper method for EvaluateCell which looks up the value of a cell as a double
        /// </summary>
        /// <param name="name">Name of cell</param>
        /// <returns>Value of cell as double</returns>
        /// <exception cref="InvalidOperationException">If the cell's value is a string, FormulaError, or null</exception>
        private double CellLookup(String name) {
            if (typeof(double).IsInstanceOfType(values[name])) {
                return (double) values[name];
            } else {
                throw new InvalidOperationException("Cell " + name + " is not a double");
            }
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// </summary>
        /// <param name="name">The named cell</param>
        /// <returns>Enumerable list of cell names</returns>
        protected override IEnumerable<String> GetDirectDependents(String name) {
            return dependencies.GetDependents(name);
        }

    }
}
