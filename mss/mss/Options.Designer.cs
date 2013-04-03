namespace mss
{
    partial class Options
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.lblFlagThreshold = new System.Windows.Forms.Label();
            this.txtFlagThreshold = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkOpenLast = new System.Windows.Forms.CheckBox();
            this.txtMaxScore = new System.Windows.Forms.TextBox();
            this.lblMaxScore = new System.Windows.Forms.Label();
            this.gbxDoc = new System.Windows.Forms.GroupBox();
            this.lblFlagModeDesc = new System.Windows.Forms.Label();
            this.lblFlagMode = new System.Windows.Forms.Label();
            this.cmbFlagMode = new System.Windows.Forms.ComboBox();
            this.lblCompleteScore = new System.Windows.Forms.Label();
            this.txtCompletionScore = new System.Windows.Forms.TextBox();
            this.gbxApp = new System.Windows.Forms.GroupBox();
            this.chkIncludeDanger = new System.Windows.Forms.CheckBox();
            this.gbxDoc.SuspendLayout();
            this.gbxApp.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFlagThreshold
            // 
            this.lblFlagThreshold.AutoSize = true;
            this.lblFlagThreshold.Location = new System.Drawing.Point(7, 47);
            this.lblFlagThreshold.Name = "lblFlagThreshold";
            this.lblFlagThreshold.Size = new System.Drawing.Size(77, 13);
            this.lblFlagThreshold.TabIndex = 0;
            this.lblFlagThreshold.Text = "Flag Threshold";
            // 
            // txtFlagThreshold
            // 
            this.txtFlagThreshold.Location = new System.Drawing.Point(113, 44);
            this.txtFlagThreshold.Name = "txtFlagThreshold";
            this.txtFlagThreshold.Size = new System.Drawing.Size(89, 20);
            this.txtFlagThreshold.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(330, 156);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(330, 185);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkOpenLast
            // 
            this.chkOpenLast.AutoSize = true;
            this.chkOpenLast.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOpenLast.Location = new System.Drawing.Point(6, 19);
            this.chkOpenLast.Name = "chkOpenLast";
            this.chkOpenLast.Size = new System.Drawing.Size(180, 17);
            this.chkOpenLast.TabIndex = 4;
            this.chkOpenLast.Text = "Load Latest Document On Open";
            this.chkOpenLast.UseVisualStyleBackColor = true;
            // 
            // txtMaxScore
            // 
            this.txtMaxScore.Location = new System.Drawing.Point(113, 18);
            this.txtMaxScore.Name = "txtMaxScore";
            this.txtMaxScore.Size = new System.Drawing.Size(89, 20);
            this.txtMaxScore.TabIndex = 6;
            // 
            // lblMaxScore
            // 
            this.lblMaxScore.AutoSize = true;
            this.lblMaxScore.Location = new System.Drawing.Point(7, 21);
            this.lblMaxScore.Name = "lblMaxScore";
            this.lblMaxScore.Size = new System.Drawing.Size(58, 13);
            this.lblMaxScore.TabIndex = 7;
            this.lblMaxScore.Text = "Max Score";
            // 
            // gbxDoc
            // 
            this.gbxDoc.Controls.Add(this.lblFlagModeDesc);
            this.gbxDoc.Controls.Add(this.lblFlagMode);
            this.gbxDoc.Controls.Add(this.cmbFlagMode);
            this.gbxDoc.Controls.Add(this.lblCompleteScore);
            this.gbxDoc.Controls.Add(this.txtCompletionScore);
            this.gbxDoc.Controls.Add(this.txtFlagThreshold);
            this.gbxDoc.Controls.Add(this.lblFlagThreshold);
            this.gbxDoc.Controls.Add(this.lblMaxScore);
            this.gbxDoc.Controls.Add(this.txtMaxScore);
            this.gbxDoc.Location = new System.Drawing.Point(12, 6);
            this.gbxDoc.Name = "gbxDoc";
            this.gbxDoc.Size = new System.Drawing.Size(438, 126);
            this.gbxDoc.TabIndex = 8;
            this.gbxDoc.TabStop = false;
            this.gbxDoc.Text = "Document Options";
            // 
            // lblFlagModeDesc
            // 
            this.lblFlagModeDesc.AutoSize = true;
            this.lblFlagModeDesc.Location = new System.Drawing.Point(6, 77);
            this.lblFlagModeDesc.Name = "lblFlagModeDesc";
            this.lblFlagModeDesc.Size = new System.Drawing.Size(160, 13);
            this.lblFlagModeDesc.TabIndex = 12;
            this.lblFlagModeDesc.Text = "Description of current flag mode.";
            // 
            // lblFlagMode
            // 
            this.lblFlagMode.AutoSize = true;
            this.lblFlagMode.Location = new System.Drawing.Point(208, 47);
            this.lblFlagMode.Name = "lblFlagMode";
            this.lblFlagMode.Size = new System.Drawing.Size(57, 13);
            this.lblFlagMode.TabIndex = 11;
            this.lblFlagMode.Text = "Flag Mode";
            // 
            // cmbFlagMode
            // 
            this.cmbFlagMode.FormattingEnabled = true;
            this.cmbFlagMode.Location = new System.Drawing.Point(304, 43);
            this.cmbFlagMode.Name = "cmbFlagMode";
            this.cmbFlagMode.Size = new System.Drawing.Size(121, 21);
            this.cmbFlagMode.TabIndex = 10;
            this.cmbFlagMode.SelectedIndexChanged += new System.EventHandler(this.cmbFlagMode_SelectedIndexChanged);
            // 
            // lblCompleteScore
            // 
            this.lblCompleteScore.AutoSize = true;
            this.lblCompleteScore.Location = new System.Drawing.Point(208, 22);
            this.lblCompleteScore.Name = "lblCompleteScore";
            this.lblCompleteScore.Size = new System.Drawing.Size(90, 13);
            this.lblCompleteScore.TabIndex = 9;
            this.lblCompleteScore.Text = "Completion Score";
            // 
            // txtCompletionScore
            // 
            this.txtCompletionScore.Location = new System.Drawing.Point(304, 18);
            this.txtCompletionScore.Name = "txtCompletionScore";
            this.txtCompletionScore.Size = new System.Drawing.Size(89, 20);
            this.txtCompletionScore.TabIndex = 8;
            // 
            // gbxApp
            // 
            this.gbxApp.Controls.Add(this.chkIncludeDanger);
            this.gbxApp.Controls.Add(this.chkOpenLast);
            this.gbxApp.Location = new System.Drawing.Point(12, 141);
            this.gbxApp.Name = "gbxApp";
            this.gbxApp.Size = new System.Drawing.Size(288, 75);
            this.gbxApp.TabIndex = 9;
            this.gbxApp.TabStop = false;
            this.gbxApp.Text = "Application Settings";
            // 
            // chkIncludeDanger
            // 
            this.chkIncludeDanger.AutoSize = true;
            this.chkIncludeDanger.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkIncludeDanger.Location = new System.Drawing.Point(6, 48);
            this.chkIncludeDanger.Name = "chkIncludeDanger";
            this.chkIncludeDanger.Size = new System.Drawing.Size(233, 17);
            this.chkIncludeDanger.TabIndex = 6;
            this.chkIncludeDanger.Text = "Include Danger Words With Flagged Words";
            this.chkIncludeDanger.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 231);
            this.Controls.Add(this.gbxApp);
            this.Controls.Add(this.gbxDoc);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Options";
            this.Text = "Options";
            this.gbxDoc.ResumeLayout(false);
            this.gbxDoc.PerformLayout();
            this.gbxApp.ResumeLayout(false);
            this.gbxApp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFlagThreshold;
        private System.Windows.Forms.TextBox txtFlagThreshold;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkOpenLast;
        private System.Windows.Forms.TextBox txtMaxScore;
        private System.Windows.Forms.Label lblMaxScore;
        private System.Windows.Forms.GroupBox gbxDoc;
        private System.Windows.Forms.GroupBox gbxApp;
        private System.Windows.Forms.Label lblCompleteScore;
        private System.Windows.Forms.TextBox txtCompletionScore;
        private System.Windows.Forms.Label lblFlagModeDesc;
        private System.Windows.Forms.Label lblFlagMode;
        private System.Windows.Forms.ComboBox cmbFlagMode;
        private System.Windows.Forms.CheckBox chkIncludeDanger;
    }
}