namespace SpreadsheetGUI {
    partial class SpreadsheetForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.SpreadsheetGrid = new SS.SpreadsheetPanel();
            this.SelectedCellContent = new System.Windows.Forms.TextBox();
            this.SelectedCellLabel = new System.Windows.Forms.Label();
            this.CellValueBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.HelpWindow = new System.Windows.Forms.HelpProvider();
            this.helpButton = new System.Windows.Forms.Button();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.IPAddressLabel = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.SpreadsheetNameList = new System.Windows.Forms.ComboBox();
            this.SpreadsheetNameLabel = new System.Windows.Forms.Label();
            this.OpenButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SpreadsheetGrid
            // 
            this.SpreadsheetGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SpreadsheetGrid.Location = new System.Drawing.Point(0, 105);
            this.SpreadsheetGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SpreadsheetGrid.Name = "SpreadsheetGrid";
            this.SpreadsheetGrid.Size = new System.Drawing.Size(2153, 661);
            this.SpreadsheetGrid.TabIndex = 1;
            // 
            // SelectedCellContent
            // 
            this.SelectedCellContent.Location = new System.Drawing.Point(16, 73);
            this.SelectedCellContent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SelectedCellContent.Name = "SelectedCellContent";
            this.SelectedCellContent.Size = new System.Drawing.Size(443, 22);
            this.SelectedCellContent.TabIndex = 0;
            this.SelectedCellContent.TextChanged += new System.EventHandler(this.SelectedCellContent_TextChanged);
            // 
            // SelectedCellLabel
            // 
            this.SelectedCellLabel.AutoSize = true;
            this.SelectedCellLabel.Location = new System.Drawing.Point(16, 53);
            this.SelectedCellLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SelectedCellLabel.Name = "SelectedCellLabel";
            this.SelectedCellLabel.Size = new System.Drawing.Size(115, 17);
            this.SelectedCellLabel.TabIndex = 2;
            this.SelectedCellLabel.Text = "Selected Cell: A1";
            // 
            // CellValueBox
            // 
            this.CellValueBox.Location = new System.Drawing.Point(468, 73);
            this.CellValueBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CellValueBox.Name = "CellValueBox";
            this.CellValueBox.ReadOnly = true;
            this.CellValueBox.Size = new System.Drawing.Size(541, 22);
            this.CellValueBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(464, 53);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cell Value";
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(1197, 69);
            this.helpButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(100, 28);
            this.helpButton.TabIndex = 6;
            this.helpButton.Text = "Get Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // IPTextBox
            // 
            this.IPTextBox.Location = new System.Drawing.Point(16, 28);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(343, 22);
            this.IPTextBox.TabIndex = 7;
            // 
            // IPAddressLabel
            // 
            this.IPAddressLabel.AutoSize = true;
            this.IPAddressLabel.Location = new System.Drawing.Point(16, 5);
            this.IPAddressLabel.Name = "IPAddressLabel";
            this.IPAddressLabel.Size = new System.Drawing.Size(80, 17);
            this.IPAddressLabel.TabIndex = 8;
            this.IPAddressLabel.Text = "IP Address:";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(366, 28);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(93, 23);
            this.ConnectButton.TabIndex = 9;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // SpreadsheetNameList
            // 
            this.SpreadsheetNameList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SpreadsheetNameList.Enabled = false;
            this.SpreadsheetNameList.FormattingEnabled = true;
            this.SpreadsheetNameList.Location = new System.Drawing.Point(468, 27);
            this.SpreadsheetNameList.Name = "SpreadsheetNameList";
            this.SpreadsheetNameList.Size = new System.Drawing.Size(244, 24);
            this.SpreadsheetNameList.TabIndex = 10;
            // 
            // SpreadsheetNameLabel
            // 
            this.SpreadsheetNameLabel.AutoSize = true;
            this.SpreadsheetNameLabel.Location = new System.Drawing.Point(465, 7);
            this.SpreadsheetNameLabel.Name = "SpreadsheetNameLabel";
            this.SpreadsheetNameLabel.Size = new System.Drawing.Size(134, 17);
            this.SpreadsheetNameLabel.TabIndex = 11;
            this.SpreadsheetNameLabel.Text = "Spreadsheet Name:";
            // 
            // OpenButton
            // 
            this.OpenButton.Enabled = false;
            this.OpenButton.Location = new System.Drawing.Point(738, 28);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(75, 23);
            this.OpenButton.TabIndex = 12;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 660);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.SpreadsheetNameLabel);
            this.Controls.Add(this.SpreadsheetNameList);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.IPAddressLabel);
            this.Controls.Add(this.IPTextBox);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CellValueBox);
            this.Controls.Add(this.SelectedCellLabel);
            this.Controls.Add(this.SelectedCellContent);
            this.Controls.Add(this.SpreadsheetGrid);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SpreadsheetForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel SpreadsheetGrid;
        private System.Windows.Forms.TextBox SelectedCellContent;
        private System.Windows.Forms.Label SelectedCellLabel;
        private System.Windows.Forms.TextBox CellValueBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HelpProvider HelpWindow;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.Label IPAddressLabel;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.ComboBox SpreadsheetNameList;
        private System.Windows.Forms.Label SpreadsheetNameLabel;
        private System.Windows.Forms.Button OpenButton;
    }
}

