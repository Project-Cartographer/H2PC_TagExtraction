namespace Map_Handler
{
    partial class MainBox
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decompileMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractMetaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.injectMetaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getTagStructureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.metaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(442, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMapToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openMapToolStripMenuItem
            // 
            this.openMapToolStripMenuItem.Name = "openMapToolStripMenuItem";
            this.openMapToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.openMapToolStripMenuItem.Text = "Open Map";
            this.openMapToolStripMenuItem.Click += new System.EventHandler(this.openMapToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractTagToolStripMenuItem,
            this.decompileMapToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.helpToolStripMenuItem.Text = "Tag";
            // 
            // extractTagToolStripMenuItem
            // 
            this.extractTagToolStripMenuItem.Name = "extractTagToolStripMenuItem";
            this.extractTagToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.extractTagToolStripMenuItem.Text = "Extract Tag";
            this.extractTagToolStripMenuItem.Click += new System.EventHandler(this.extractTagToolStripMenuItem_Click);
            // 
            // decompileMapToolStripMenuItem
            // 
            this.decompileMapToolStripMenuItem.Name = "decompileMapToolStripMenuItem";
            this.decompileMapToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.decompileMapToolStripMenuItem.Text = "Decompile Map";
            this.decompileMapToolStripMenuItem.Click += new System.EventHandler(this.decompileMapToolStripMenuItem_Click);
            // 
            // metaToolStripMenuItem
            // 
            this.metaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractMetaToolStripMenuItem,
            this.injectMetaToolStripMenuItem,
            this.getTagStructureToolStripMenuItem});
            this.metaToolStripMenuItem.Name = "metaToolStripMenuItem";
            this.metaToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.metaToolStripMenuItem.Text = "Meta";
            // 
            // extractMetaToolStripMenuItem
            // 
            this.extractMetaToolStripMenuItem.Name = "extractMetaToolStripMenuItem";
            this.extractMetaToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.extractMetaToolStripMenuItem.Text = "Extract meta";
            this.extractMetaToolStripMenuItem.Click += new System.EventHandler(this.extractMetaToolStripMenuItem_Click);
            // 
            // injectMetaToolStripMenuItem
            // 
            this.injectMetaToolStripMenuItem.Name = "injectMetaToolStripMenuItem";
            this.injectMetaToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.injectMetaToolStripMenuItem.Text = "Compile meta";
            this.injectMetaToolStripMenuItem.Click += new System.EventHandler(this.injectMetaToolStripMenuItem_Click);
            // 
            // getTagStructureToolStripMenuItem
            // 
            this.getTagStructureToolStripMenuItem.Name = "getTagStructureToolStripMenuItem";
            this.getTagStructureToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.getTagStructureToolStripMenuItem.Text = "Get Tag Structure";
            this.getTagStructureToolStripMenuItem.Click += new System.EventHandler(this.getTagStructureToolStripMenuItem_Click);
            // 
            // treeView1
            // 
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(12, 55);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(418, 437);
            this.treeView1.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(51, 27);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(326, 22);
            this.progressBar1.TabIndex = 2;
            // 
            // timer2
            // 
            this.timer2.Interval = 2;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // MainBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 504);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "MainBox";
            this.Text = "Map_Handler";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem metaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractMetaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripMenuItem injectMetaToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem getTagStructureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decompileMapToolStripMenuItem;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer2;
    }
}

