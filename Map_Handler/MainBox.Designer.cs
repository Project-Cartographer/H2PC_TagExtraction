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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainBox));
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.output_db_ = new System.Windows.Forms.CheckBox();
            this.override_tags_ = new System.Windows.Forms.CheckBox();
            this.recursive_radio_ = new System.Windows.Forms.CheckBox();
            this.current_tag_status = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.extract_button = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Version = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button6 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.clear_button = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
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
            this.extractTagToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.extractTagToolStripMenuItem.Text = "Add Selected";
            this.extractTagToolStripMenuItem.Click += new System.EventHandler(this.extractTagToolStripMenuItem_Click);
            // 
            // decompileMapToolStripMenuItem
            // 
            this.decompileMapToolStripMenuItem.Name = "decompileMapToolStripMenuItem";
            this.decompileMapToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.decompileMapToolStripMenuItem.Text = "Add All Tags";
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
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(406, 619);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(8, 8);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(325, 24);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(220, 34);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(113, 24);
            this.textBox2.TabIndex = 3;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 25);
            this.button1.TabIndex = 4;
            this.button1.Text = "Load Map";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(134, 60);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 25);
            this.button2.TabIndex = 5;
            this.button2.Text = "Add Selected";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(134, 34);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 25);
            this.button3.TabIndex = 6;
            this.button3.Text = "Add All Tags";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(8, 60);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(80, 25);
            this.button4.TabIndex = 7;
            this.button4.Text = "Close Map";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.output_db_);
            this.groupBox1.Controls.Add(this.override_tags_);
            this.groupBox1.Controls.Add(this.recursive_radio_);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Location = new System.Drawing.Point(8, 135);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(234, 62);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Flags :";
            // 
            // output_db_
            // 
            this.output_db_.AutoSize = true;
            this.output_db_.Location = new System.Drawing.Point(125, 19);
            this.output_db_.Name = "output_db_";
            this.output_db_.Size = new System.Drawing.Size(108, 17);
            this.output_db_.TabIndex = 15;
            this.output_db_.Text = "Output DataBase";
            this.output_db_.UseVisualStyleBackColor = true;
            // 
            // override_tags_
            // 
            this.override_tags_.AutoSize = true;
            this.override_tags_.Location = new System.Drawing.Point(21, 39);
            this.override_tags_.Name = "override_tags_";
            this.override_tags_.Size = new System.Drawing.Size(98, 17);
            this.override_tags_.TabIndex = 14;
            this.override_tags_.Text = "Overwrite Tags";
            this.override_tags_.UseVisualStyleBackColor = true;
            // 
            // recursive_radio_
            // 
            this.recursive_radio_.AutoSize = true;
            this.recursive_radio_.Location = new System.Drawing.Point(21, 19);
            this.recursive_radio_.Name = "recursive_radio_";
            this.recursive_radio_.Size = new System.Drawing.Size(74, 17);
            this.recursive_radio_.TabIndex = 13;
            this.recursive_radio_.Text = "Recursive";
            this.recursive_radio_.UseVisualStyleBackColor = true;
            // 
            // current_tag_status
            // 
            this.current_tag_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.current_tag_status.AutoSize = true;
            this.current_tag_status.BackColor = System.Drawing.Color.Transparent;
            this.current_tag_status.Location = new System.Drawing.Point(8, 608);
            this.current_tag_status.Name = "current_tag_status";
            this.current_tag_status.Size = new System.Drawing.Size(48, 13);
            this.current_tag_status.TabIndex = 27;
            this.current_tag_status.Text = "Progress";
            this.current_tag_status.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(8, 573);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(325, 32);
            this.progressBar1.TabIndex = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 200);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Selected Tags:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(8, 216);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(325, 351);
            this.richTextBox1.TabIndex = 24;
            this.richTextBox1.Text = "";
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Location = new System.Drawing.Point(358, 109);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(35, 23);
            this.button5.TabIndex = 23;
            this.button5.Text = "...";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Extract Path";
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.HideSelection = false;
            this.textBox3.Location = new System.Drawing.Point(76, 109);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(214, 24);
            this.textBox3.TabIndex = 21;
            // 
            // extract_button
            // 
            this.extract_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.extract_button.Enabled = false;
            this.extract_button.Location = new System.Drawing.Point(254, 140);
            this.extract_button.Name = "extract_button";
            this.extract_button.Size = new System.Drawing.Size(79, 25);
            this.extract_button.TabIndex = 20;
            this.extract_button.Text = "Extract";
            this.extract_button.UseVisualStyleBackColor = true;
            this.extract_button.Click += new System.EventHandler(this.extract_button_Click);
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.Location = new System.Drawing.Point(220, 60);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(113, 24);
            this.textBox4.TabIndex = 29;
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(8, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(385, 2);
            this.label3.TabIndex = 30;
            // 
            // Version
            // 
            this.Version.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Version.AutoSize = true;
            this.Version.BackColor = System.Drawing.Color.Transparent;
            this.Version.Location = new System.Drawing.Point(373, 608);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(28, 13);
            this.Version.TabIndex = 31;
            this.Version.Text = "2.01";
            this.Version.Visible = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Location = new System.Drawing.Point(12, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.button6);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.clear_button);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.textBox2);
            this.splitContainer1.Panel1.Controls.Add(this.textBox4);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.button2);
            this.splitContainer1.Panel1.Controls.Add(this.current_tag_status);
            this.splitContainer1.Panel1.Controls.Add(this.button3);
            this.splitContainer1.Panel1.Controls.Add(this.progressBar1);
            this.splitContainer1.Panel1.Controls.Add(this.button4);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.extract_button);
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox1);
            this.splitContainer1.Panel1.Controls.Add(this.textBox3);
            this.splitContainer1.Panel1.Controls.Add(this.button5);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1MinSize = 340;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView1);
            this.splitContainer1.Panel2.Controls.Add(this.Version);
            this.splitContainer1.Size = new System.Drawing.Size(760, 629);
            this.splitContainer1.SplitterDistance = 340;
            this.splitContainer1.TabIndex = 32;
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.Location = new System.Drawing.Point(298, 107);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(35, 25);
            this.button6.TabIndex = 33;
            this.button6.Text = "...";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 200);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 32;
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // clear_button
            // 
            this.clear_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clear_button.Enabled = false;
            this.clear_button.Location = new System.Drawing.Point(254, 166);
            this.clear_button.Name = "clear_button";
            this.clear_button.Size = new System.Drawing.Size(79, 25);
            this.clear_button.TabIndex = 31;
            this.clear_button.Text = "Clear";
            this.clear_button.UseVisualStyleBackColor = true;
            this.clear_button.Click += new System.EventHandler(this.clear_button_Click);
            // 
            // MainBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(784, 662);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 700);
            this.Name = "MainBox";
            this.Text = "H2PC Map Handler";
            this.Load += new System.EventHandler(this.MainBox_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.CheckBox output_db_;
        public System.Windows.Forms.CheckBox override_tags_;
        public System.Windows.Forms.CheckBox recursive_radio_;
        public System.Windows.Forms.Label current_tag_status;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox3;
        public System.Windows.Forms.Button extract_button;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label Version;
        private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.Button clear_button;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button6;
    }
}

