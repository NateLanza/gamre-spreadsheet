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
            this.SelectedCellContent = new System.Windows.Forms.TextBox();
            this.SelectedCellLabel = new System.Windows.Forms.Label();
            this.CellValueBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.HelpWindow = new System.Windows.Forms.HelpProvider();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.IPAddressLabel = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.SpreadsheetNameList = new System.Windows.Forms.ComboBox();
            this.SpreadsheetNameLabel = new System.Windows.Forms.Label();
            this.OpenButton = new System.Windows.Forms.Button();
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.revert_button = new System.Windows.Forms.Button();
            this.undo_button = new System.Windows.Forms.Button();
            this.newSSName = new System.Windows.Forms.TextBox();
            this.newSSLabel = new System.Windows.Forms.Label();
            this.newSSButton = new System.Windows.Forms.Button();
            this.SpreadsheetGrid = new SS.SpreadsheetPanel();
            this.SuspendLayout();
            // 
            // SelectedCellContent
            // 
            this.SelectedCellContent.Enabled = false;
            this.SelectedCellContent.Location = new System.Drawing.Point(12, 59);
            this.SelectedCellContent.Name = "SelectedCellContent";
            this.SelectedCellContent.Size = new System.Drawing.Size(333, 20);
            this.SelectedCellContent.TabIndex = 0;
            this.SelectedCellContent.TextChanged += new System.EventHandler(this.SelectedCellContent_TextChanged);
            this.SelectedCellContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SelectedCellContent_KeyDown);
            // 
            // SelectedCellLabel
            // 
            this.SelectedCellLabel.AutoSize = true;
            this.SelectedCellLabel.Location = new System.Drawing.Point(12, 43);
            this.SelectedCellLabel.Name = "SelectedCellLabel";
            this.SelectedCellLabel.Size = new System.Drawing.Size(88, 13);
            this.SelectedCellLabel.TabIndex = 2;
            this.SelectedCellLabel.Text = "Selected Cell: A1";
            // 
            // CellValueBox
            // 
            this.CellValueBox.Enabled = false;
            this.CellValueBox.Location = new System.Drawing.Point(351, 59);
            this.CellValueBox.Name = "CellValueBox";
            this.CellValueBox.ReadOnly = true;
            this.CellValueBox.Size = new System.Drawing.Size(407, 20);
            this.CellValueBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(348, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cell Value";
            // 
            // IPTextBox
            // 
            this.IPTextBox.Location = new System.Drawing.Point(12, 23);
            this.IPTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(258, 20);
            this.IPTextBox.TabIndex = 7;
            // 
            // IPAddressLabel
            // 
            this.IPAddressLabel.AutoSize = true;
            this.IPAddressLabel.Location = new System.Drawing.Point(12, 4);
            this.IPAddressLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.IPAddressLabel.Name = "IPAddressLabel";
            this.IPAddressLabel.Size = new System.Drawing.Size(61, 13);
            this.IPAddressLabel.TabIndex = 8;
            this.IPAddressLabel.Text = "IP Address:";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(430, 17);
            this.ConnectButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(70, 30);
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
            this.SpreadsheetNameList.Location = new System.Drawing.Point(522, 21);
            this.SpreadsheetNameList.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SpreadsheetNameList.Name = "SpreadsheetNameList";
            this.SpreadsheetNameList.Size = new System.Drawing.Size(184, 21);
            this.SpreadsheetNameList.TabIndex = 10;
            // 
            // SpreadsheetNameLabel
            // 
            this.SpreadsheetNameLabel.AutoSize = true;
            this.SpreadsheetNameLabel.Location = new System.Drawing.Point(519, 6);
            this.SpreadsheetNameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SpreadsheetNameLabel.Name = "SpreadsheetNameLabel";
            this.SpreadsheetNameLabel.Size = new System.Drawing.Size(101, 13);
            this.SpreadsheetNameLabel.TabIndex = 11;
            this.SpreadsheetNameLabel.Text = "Spreadsheet Name:";
            // 
            // OpenButton
            // 
            this.OpenButton.Enabled = false;
            this.OpenButton.Location = new System.Drawing.Point(710, 21);
            this.OpenButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(56, 22);
            this.OpenButton.TabIndex = 12;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // UsernameBox
            // 
            this.UsernameBox.Location = new System.Drawing.Point(275, 23);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(150, 20);
            this.UsernameBox.TabIndex = 13;
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(273, 6);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(55, 13);
            this.UsernameLabel.TabIndex = 14;
            this.UsernameLabel.Text = "Username";
            // 
            // revert_button
            // 
            this.revert_button.Enabled = false;
            this.revert_button.Location = new System.Drawing.Point(765, 55);
            this.revert_button.Name = "revert_button";
            this.revert_button.Size = new System.Drawing.Size(75, 23);
            this.revert_button.TabIndex = 15;
            this.revert_button.Text = "Revert Cell";
            this.revert_button.UseVisualStyleBackColor = true;
            this.revert_button.Click += new System.EventHandler(this.revert_button_Click);
            // 
            // undo_button
            // 
            this.undo_button.Enabled = false;
            this.undo_button.Location = new System.Drawing.Point(847, 55);
            this.undo_button.Name = "undo_button";
            this.undo_button.Size = new System.Drawing.Size(75, 23);
            this.undo_button.TabIndex = 16;
            this.undo_button.Text = "Undo";
            this.undo_button.UseVisualStyleBackColor = true;
            this.undo_button.Click += new System.EventHandler(this.undo_button_Click);
            // 
            // newSSName
            // 
            this.newSSName.Enabled = false;
            this.newSSName.Location = new System.Drawing.Point(771, 21);
            this.newSSName.Name = "newSSName";
            this.newSSName.Size = new System.Drawing.Size(100, 20);
            this.newSSName.TabIndex = 17;
            // 
            // newSSLabel
            // 
            this.newSSLabel.AutoSize = true;
            this.newSSLabel.Location = new System.Drawing.Point(762, 6);
            this.newSSLabel.Name = "newSSLabel";
            this.newSSLabel.Size = new System.Drawing.Size(123, 13);
            this.newSSLabel.TabIndex = 18;
            this.newSSLabel.Text = "New Spreadsheet Name";
            // 
            // newSSButton
            // 
            this.newSSButton.Enabled = false;
            this.newSSButton.Location = new System.Drawing.Point(877, 19);
            this.newSSButton.Name = "newSSButton";
            this.newSSButton.Size = new System.Drawing.Size(75, 23);
            this.newSSButton.TabIndex = 19;
            this.newSSButton.Text = "Create";
            this.newSSButton.UseVisualStyleBackColor = true;
            this.newSSButton.Click += new System.EventHandler(this.newSSButton_Click);
            // 
            // SpreadsheetGrid
            // 
            this.SpreadsheetGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SpreadsheetGrid.Location = new System.Drawing.Point(0, 85);
            this.SpreadsheetGrid.MinimumSize = new System.Drawing.Size(2200, 2200);
            this.SpreadsheetGrid.Name = "SpreadsheetGrid";
            this.SpreadsheetGrid.Size = new System.Drawing.Size(2300, 3300);
            this.SpreadsheetGrid.TabIndex = 1;
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(2200, 2200);
            this.ClientSize = new System.Drawing.Size(1443, 554);
            this.Controls.Add(this.newSSButton);
            this.Controls.Add(this.newSSLabel);
            this.Controls.Add(this.newSSName);
            this.Controls.Add(this.undo_button);
            this.Controls.Add(this.revert_button);
            this.Controls.Add(this.UsernameLabel);
            this.Controls.Add(this.UsernameBox);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.SpreadsheetNameLabel);
            this.Controls.Add(this.SpreadsheetNameList);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.IPAddressLabel);
            this.Controls.Add(this.IPTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CellValueBox);
            this.Controls.Add(this.SelectedCellLabel);
            this.Controls.Add(this.SelectedCellContent);
            this.Controls.Add(this.SpreadsheetGrid);
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
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.Label IPAddressLabel;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.ComboBox SpreadsheetNameList;
        private System.Windows.Forms.Label SpreadsheetNameLabel;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.TextBox UsernameBox;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.Button revert_button;
        private System.Windows.Forms.Button undo_button;
        private System.Windows.Forms.TextBox newSSName;
        private System.Windows.Forms.Label newSSLabel;
        private System.Windows.Forms.Button newSSButton;
    }
}

