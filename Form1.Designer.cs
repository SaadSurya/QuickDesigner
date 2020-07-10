namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.ImagesBrowse = new System.Windows.Forms.Button();
            this.ZipsFolderPathTxt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CDRFilesLocationTxt = new System.Windows.Forms.TextBox();
            this.CDRBrowse = new System.Windows.Forms.Button();
            this.GenerateBtn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.errorConsoleTxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ImagesBrowse
            // 
            this.ImagesBrowse.Location = new System.Drawing.Point(669, 40);
            this.ImagesBrowse.Name = "ImagesBrowse";
            this.ImagesBrowse.Size = new System.Drawing.Size(75, 23);
            this.ImagesBrowse.TabIndex = 0;
            this.ImagesBrowse.Text = "Browse";
            this.ImagesBrowse.UseVisualStyleBackColor = true;
            this.ImagesBrowse.Click += new System.EventHandler(this.ImagesBrowse_Click);
            // 
            // ZipsFolderPathTxt
            // 
            this.ZipsFolderPathTxt.Location = new System.Drawing.Point(51, 40);
            this.ZipsFolderPathTxt.Name = "ZipsFolderPathTxt";
            this.ZipsFolderPathTxt.Size = new System.Drawing.Size(612, 20);
            this.ZipsFolderPathTxt.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Zip Files Source Folder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "\"CDR\" Files Destination Folder";
            // 
            // CDRFilesLocationTxt
            // 
            this.CDRFilesLocationTxt.Location = new System.Drawing.Point(51, 99);
            this.CDRFilesLocationTxt.Name = "CDRFilesLocationTxt";
            this.CDRFilesLocationTxt.Size = new System.Drawing.Size(612, 20);
            this.CDRFilesLocationTxt.TabIndex = 4;
            // 
            // CDRBrowse
            // 
            this.CDRBrowse.Location = new System.Drawing.Point(669, 97);
            this.CDRBrowse.Name = "CDRBrowse";
            this.CDRBrowse.Size = new System.Drawing.Size(75, 23);
            this.CDRBrowse.TabIndex = 3;
            this.CDRBrowse.Text = "Browse";
            this.CDRBrowse.UseVisualStyleBackColor = true;
            this.CDRBrowse.Click += new System.EventHandler(this.CDRBrowse_Click);
            // 
            // GenerateBtn
            // 
            this.GenerateBtn.Location = new System.Drawing.Point(360, 149);
            this.GenerateBtn.Name = "GenerateBtn";
            this.GenerateBtn.Size = new System.Drawing.Size(75, 23);
            this.GenerateBtn.TabIndex = 6;
            this.GenerateBtn.Text = "Generate";
            this.GenerateBtn.UseVisualStyleBackColor = true;
            this.GenerateBtn.Click += new System.EventHandler(this.GenerateBtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(51, 194);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(693, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 7;
            // 
            // errorConsoleTxt
            // 
            this.errorConsoleTxt.Location = new System.Drawing.Point(51, 243);
            this.errorConsoleTxt.Multiline = true;
            this.errorConsoleTxt.Name = "errorConsoleTxt";
            this.errorConsoleTxt.ReadOnly = true;
            this.errorConsoleTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.errorConsoleTxt.Size = new System.Drawing.Size(693, 154);
            this.errorConsoleTxt.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 227);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Error Console";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 422);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.errorConsoleTxt);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.GenerateBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CDRFilesLocationTxt);
            this.Controls.Add(this.CDRBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ZipsFolderPathTxt);
            this.Controls.Add(this.ImagesBrowse);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Quick Desinger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button ImagesBrowse;
        private System.Windows.Forms.TextBox ZipsFolderPathTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox CDRFilesLocationTxt;
        private System.Windows.Forms.Button CDRBrowse;
        private System.Windows.Forms.Button GenerateBtn;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox errorConsoleTxt;
        private System.Windows.Forms.Label label3;
    }
}

