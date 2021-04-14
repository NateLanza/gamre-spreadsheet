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
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(898, 56);
            this.helpButton.Name = "HelpButton";
            this.helpButton.Size = new System.Drawing.Size(75, 23);
            this.helpButton.TabIndex = 6;
            this.helpButton.Text = "Get Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1616, 536);
            this.Controls.Add(this.helpButton);
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
        private System.Windows.Forms.Button helpButton;
    }
}

