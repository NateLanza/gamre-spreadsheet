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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFile = new System.Windows.Forms.OpenFileDialog();
            this.SaveFile = new System.Windows.Forms.SaveFileDialog();
            this.HelpWindow = new System.Windows.Forms.HelpProvider();
            this.HelpButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SpreadsheetGrid
            // 
            this.SpreadsheetGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SpreadsheetGrid.Location = new System.Drawing.Point(0, 85);
            this.SpreadsheetGrid.Name = "SpreadsheetGrid";
            this.SpreadsheetGrid.Size = new System.Drawing.Size(1615, 537);
            this.SpreadsheetGrid.TabIndex = 1;
            // 
            // SelectedCellContent
            // 
            this.SelectedCellContent.Location = new System.Drawing.Point(12, 59);
            this.SelectedCellContent.Name = "SelectedCellContent";
            this.SelectedCellContent.Size = new System.Drawing.Size(333, 20);
            this.SelectedCellContent.TabIndex = 0;
            this.SelectedCellContent.TextChanged += new System.EventHandler(this.SelectedCellContent_TextChanged);
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
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1616, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "File";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // OpenFile
            // 
            this.OpenFile.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFile_FileOk);
            // 
            // SaveFile
            // 
            this.SaveFile.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveFile_FileOk);
            // 
            // HelpButton
            // 
            this.HelpButton.Location = new System.Drawing.Point(898, 56);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(75, 23);
            this.HelpButton.TabIndex = 6;
            this.HelpButton.Text = "Get Help";
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1616, 536);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CellValueBox);
            this.Controls.Add(this.SelectedCellLabel);
            this.Controls.Add(this.SelectedCellContent);
            this.Controls.Add(this.SpreadsheetGrid);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpreadsheetForm";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel SpreadsheetGrid;
        private System.Windows.Forms.TextBox SelectedCellContent;
        private System.Windows.Forms.Label SelectedCellLabel;
        private System.Windows.Forms.TextBox CellValueBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog OpenFile;
        private System.Windows.Forms.SaveFileDialog SaveFile;
        private System.Windows.Forms.HelpProvider HelpWindow;
        private System.Windows.Forms.Button HelpButton;
    }
}

