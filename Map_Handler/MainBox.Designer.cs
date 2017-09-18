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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractImportInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hCEToH2VSoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hCEGBXModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hCECollisionModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpSelectedTagsListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tests1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TagToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.extractTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decompileMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractMetaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.injectMetaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getTagStructureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultMaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testToolStripMenuItem,
            this.TagToolStripMenu,
            this.metaToolStripMenuItem,
            this.settingsToolStripMenuItem});
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
            this.closeMapToolStripMenuItem,
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
            // closeMapToolStripMenuItem
            // 
            this.closeMapToolStripMenuItem.Name = "closeMapToolStripMenuItem";
            this.closeMapToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.closeMapToolStripMenuItem.Text = "Close Map";
            this.closeMapToolStripMenuItem.Click += new System.EventHandler(this.closeMapToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractImportInfoToolStripMenuItem,
            this.convertToolStripMenuItem,
            this.dumpSelectedTagsListToolStripMenuItem,
            this.tests1ToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // extractImportInfoToolStripMenuItem
            // 
            this.extractImportInfoToolStripMenuItem.Name = "extractImportInfoToolStripMenuItem";
            this.extractImportInfoToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.extractImportInfoToolStripMenuItem.Text = "Extract Import Info";
            this.extractImportInfoToolStripMenuItem.Click += new System.EventHandler(this.ExtractImportlStripMenuItem_Click);
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hCEToH2VSoundsToolStripMenuItem,
            this.hCEGBXModelToolStripMenuItem,
            this.hCECollisionModelToolStripMenuItem});
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.convertToolStripMenuItem.Text = "Convert";
            // 
            // hCEToH2VSoundsToolStripMenuItem
            // 
            this.hCEToH2VSoundsToolStripMenuItem.Name = "hCEToH2VSoundsToolStripMenuItem";
            this.hCEToH2VSoundsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.hCEToH2VSoundsToolStripMenuItem.Text = "HCE Sounds";
            this.hCEToH2VSoundsToolStripMenuItem.Click += new System.EventHandler(this.hCEToH2VSoundsToolStripMenuItem_Click);
            // 
            // hCEGBXModelToolStripMenuItem
            // 
            this.hCEGBXModelToolStripMenuItem.Name = "hCEGBXModelToolStripMenuItem";
            this.hCEGBXModelToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.hCEGBXModelToolStripMenuItem.Text = "HCE GBX Model";
            this.hCEGBXModelToolStripMenuItem.Click += new System.EventHandler(this.hCEGBXModelToolStripMenuItem_Click);
            // 
            // hCECollisionModelToolStripMenuItem
            // 
            this.hCECollisionModelToolStripMenuItem.Name = "hCECollisionModelToolStripMenuItem";
            this.hCECollisionModelToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.hCECollisionModelToolStripMenuItem.Text = "HCE Collision Model";
            this.hCECollisionModelToolStripMenuItem.Click += new System.EventHandler(this.hCECollisionModelToolStripMenuItem_Click);
            // 
            // dumpSelectedTagsListToolStripMenuItem
            // 
            this.dumpSelectedTagsListToolStripMenuItem.Name = "dumpSelectedTagsListToolStripMenuItem";
            this.dumpSelectedTagsListToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.dumpSelectedTagsListToolStripMenuItem.Text = "Dump Selected Tags List";
            this.dumpSelectedTagsListToolStripMenuItem.Click += new System.EventHandler(this.dumpSelectedTagsListToolStripMenuItem_Click);
            // 
            // tests1ToolStripMenuItem
            // 
            this.tests1ToolStripMenuItem.Name = "tests1ToolStripMenuItem";
            this.tests1ToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.tests1ToolStripMenuItem.Text = "Tests1";
            this.tests1ToolStripMenuItem.Visible = false;
            this.tests1ToolStripMenuItem.Click += new System.EventHandler(this.tests1ToolStripMenuItem_Click);
            // 
            // TagToolStripMenu
            // 
            this.TagToolStripMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractTagToolStripMenuItem,
            this.decompileMapToolStripMenuItem});
            this.TagToolStripMenu.Name = "TagToolStripMenu";
            this.TagToolStripMenu.Size = new System.Drawing.Size(39, 20);
            this.TagToolStripMenu.Text = "Tag";
            this.TagToolStripMenu.Visible = false;
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
            this.metaToolStripMenuItem.Visible = false;
            // 
            // extractMetaToolStripMenuItem
            // 
            this.extractMetaToolStripMenuItem.Name = "extractMetaToolStripMenuItem";
            this.extractMetaToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.extractMetaToolStripMenuItem.Text = "Extract meta";
            this.extractMetaToolStripMenuItem.Click += new System.EventHandler(this.extractMetaToolStripMenuItem_Click);
            // 
            // injectMetaToolStripMenuItem
            // 
            this.injectMetaToolStripMenuItem.Name = "injectMetaToolStripMenuItem";
            this.injectMetaToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.injectMetaToolStripMenuItem.Text = "Compile meta";
            this.injectMetaToolStripMenuItem.Click += new System.EventHandler(this.CompileMetaToolStripMenuItem_Click);
            // 
            // getTagStructureToolStripMenuItem
            // 
            this.getTagStructureToolStripMenuItem.Name = "getTagStructureToolStripMenuItem";
            this.getTagStructureToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.getTagStructureToolStripMenuItem.Text = "Get Tag Structure";
            this.getTagStructureToolStripMenuItem.Click += new System.EventHandler(this.getTagStructureToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultMaToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // defaultMaToolStripMenuItem
            // 
            this.defaultMaToolStripMenuItem.Name = "defaultMaToolStripMenuItem";
            this.defaultMaToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.defaultMaToolStripMenuItem.Text = "Default Map Directory";
            this.defaultMaToolStripMenuItem.Click += new System.EventHandler(this.defaultMaToolStripMenuItem_Click);
            // 
            // treeView1
            // 
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(12, 37);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(418, 441);
            this.treeView1.TabIndex = 1;
            // 
            // MainBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.ClientSize = new System.Drawing.Size(442, 504);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.MaximizeBox = false;
            this.Name = "MainBox";
            this.Text = "H2PC_Map_Handler";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem metaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractMetaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TagToolStripMenu;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripMenuItem injectMetaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getTagStructureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decompileMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractImportInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultMaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hCEToH2VSoundsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hCEGBXModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hCECollisionModelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpSelectedTagsListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tests1ToolStripMenuItem;
    }
}

