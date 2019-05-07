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
            this.resyncshadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resyncStringIDsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultMaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
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
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
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
            this.openMapToolStripMenuItem.Text = "Load Map";
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
            this.convertToolStripMenuItem.Enabled = false;
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.convertToolStripMenuItem.Text = "Convert";
            this.convertToolStripMenuItem.Visible = false;
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
            this.getTagStructureToolStripMenuItem,
            this.resyncshadToolStripMenuItem,
            this.resyncStringIDsToolStripMenuItem});
            this.metaToolStripMenuItem.Name = "metaToolStripMenuItem";
            this.metaToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.metaToolStripMenuItem.Text = "Meta";
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
            // resyncshadToolStripMenuItem
            // 
            this.resyncshadToolStripMenuItem.Name = "resyncshadToolStripMenuItem";
            this.resyncshadToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.resyncshadToolStripMenuItem.Text = "Resync tagRefs";
            this.resyncshadToolStripMenuItem.Click += new System.EventHandler(this.resyncshadToolStripMenuItem_Click);
            // 
            // resyncStringIDsToolStripMenuItem
            // 
            this.resyncStringIDsToolStripMenuItem.Name = "resyncStringIDsToolStripMenuItem";
            this.resyncStringIDsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.resyncStringIDsToolStripMenuItem.Text = "Resync StringIDs";
            this.resyncStringIDsToolStripMenuItem.Click += new System.EventHandler(this.resyncStringIDsToolStripMenuItem_Click);
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
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.CheckBoxes = true;
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(12, 74);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(418, 392);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 36);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(303, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(321, 36);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(109, 20);
            this.textBox2.TabIndex = 3;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(12, 472);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Load Map";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(321, 472);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(109, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Extract Selection";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(206, 472);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(109, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Decompile Map";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(93, 472);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 7;
            this.button4.Text = "Close Map";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // MainBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(442, 504);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(458, 542);
            this.Name = "MainBox";
            this.Text = "H2PC_Map_Handler";
            this.Load += new System.EventHandler(this.MainBox_Load);
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
        private System.Windows.Forms.ToolStripMenuItem resyncshadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resyncStringIDsToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

