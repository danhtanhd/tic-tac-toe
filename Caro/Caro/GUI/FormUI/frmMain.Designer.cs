namespace Caro
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlChessBoard = new System.Windows.Forms.Panel();
            this.lblGamemode = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.rdoPVP = new System.Windows.Forms.RadioButton();
            this.rdoPVE = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnload = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlChessBoard
            // 
            resources.ApplyResources(this.pnlChessBoard, "pnlChessBoard");
            this.pnlChessBoard.BackColor = System.Drawing.Color.White;
            this.pnlChessBoard.Name = "pnlChessBoard";
            // 
            // lblGamemode
            // 
            resources.ApplyResources(this.lblGamemode, "lblGamemode");
            this.lblGamemode.Name = "lblGamemode";
            // 
            // lblSize
            // 
            resources.ApplyResources(this.lblSize, "lblSize");
            this.lblSize.Name = "lblSize";
            // 
            // rdoPVP
            // 
            resources.ApplyResources(this.rdoPVP, "rdoPVP");
            this.rdoPVP.Checked = true;
            this.rdoPVP.Name = "rdoPVP";
            this.rdoPVP.TabStop = true;
            this.rdoPVP.UseVisualStyleBackColor = true;
            // 
            // rdoPVE
            // 
            resources.ApplyResources(this.rdoPVE, "rdoPVE");
            this.rdoPVE.Name = "rdoPVE";
            this.rdoPVE.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnLanguage});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // mnLanguage
            // 
            this.mnLanguage.Name = "mnLanguage";
            resources.ApplyResources(this.mnLanguage, "mnLanguage");
            this.mnLanguage.Click += new System.EventHandler(this.englishToolStripMenuItem1_Click);
            // 
            // txtSize
            // 
            resources.ApplyResources(this.txtSize, "txtSize");
            this.txtSize.Name = "txtSize";
            this.txtSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSize_KeyPress);
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.Name = "btnStart";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnload
            // 
            resources.ApplyResources(this.btnload, "btnload");
            this.btnload.Name = "btnload";
            this.btnload.UseVisualStyleBackColor = true;
            this.btnload.Click += new System.EventHandler(this.btnload_Click);
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnload);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.pnlChessBoard);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.rdoPVE);
            this.Controls.Add(this.rdoPVP);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.lblGamemode);
            this.Controls.Add(this.menuStrip1);
            this.Name = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel pnlChessBoard;
        private System.Windows.Forms.Label lblGamemode;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.RadioButton rdoPVP;
        private System.Windows.Forms.RadioButton rdoPVE;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnload;
        private System.Windows.Forms.ToolStripMenuItem mnLanguage;
    }
}

