using System;
using System.Collections.Generic;
using System.Text;
using NetworkUtil;

namespace SS {

    /// <summary>
    /// Controls the spreadsheet, handling server connections
    /// and passing information to the SpreadsheetState
    /// </summary>
    public class SpreadsheetController {

        // Events
        public delegate void CellChangeHandler(string cell, string contents);
        public event CellChangeHandler CellChanged;

        /// <summary>
        /// Current state of this spreadsheet
        /// </summary>
        private SpreadsheetState State;

        /// <summary>
        /// Creates a new SpreadsheetController
        /// </summary>
        /// <param name="state">State object of current spreadsheet</param>
        public SpreadsheetController(SpreadsheetState state) {
            this.State = state;
        }

        /// <summary>
        /// Connects this client to a server
        /// </summary>
        /// <param name="server">Hostname </param>
        public void ConnectToServer(string server) {

        }

    }
}
