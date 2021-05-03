using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI {
    public partial class Help : Form {
        public Help() {
            InitializeComponent();
        }

        private void CloseHelp_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }
    }
}
