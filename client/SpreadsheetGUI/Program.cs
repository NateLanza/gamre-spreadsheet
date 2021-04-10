using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI {

    /// <summary>
    /// Keeps track of how many top-level forms are running
    /// </summary>
    class SpreadsheetApplicationContext : ApplicationContext {
        // Number of open forms
        private int formCount = 0;

        // Singleton ApplicationContext
        private static SpreadsheetApplicationContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private SpreadsheetApplicationContext() {
        }

        /// <summary>
        /// Returns the one SpreadsheetApplicationContext.
        /// </summary>
        public static SpreadsheetApplicationContext getAppContext() {
            if (appContext == null) {
                appContext = new SpreadsheetApplicationContext();
            }
            return appContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form) {
            // One more form is running
            formCount++;

            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }

    }

    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one form inside it
            SpreadsheetApplicationContext appContext = SpreadsheetApplicationContext.getAppContext();
            appContext.RunForm(new SpreadsheetForm());
            Application.Run(appContext);
        }
    }
}
