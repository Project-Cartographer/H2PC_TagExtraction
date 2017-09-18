namespace Map_Handler
{
    partial class TagExtractor
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
            this.recursive_radio_ = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.extract_button = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.curent_tag_status = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.output_db_ = new System.Windows.Forms.CheckBox();
            this.override_tags_ = new System.Windows.Forms.CheckBox();
            this.tag_count_stats = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // recursive_radio_
            // 
            this.recursive_radio_.AutoSize = true;
            this.recursive_radio_.Location = new System.Drawing.Point(6, 28);
            this.recursive_radio_.Name = "recursive_radio_";
            this.recursive_radio_.Size = new System.Drawing.Size(74, 17);
            this.recursive_radio_.TabIndex = 13;
            this.recursive_radio_.Text = "Recursive";
            this.recursive_radio_.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(313, 16);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(96, 27);
            this.button2.TabIndex = 12;
            this.button2.Text = "Browse Directory";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Destination";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(89, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(218, 20);
            this.textBox1.TabIndex = 10;
            // 
            // extract_button
            // 
            this.extract_button.Location = new System.Drawing.Point(308, 79);
            this.extract_button.Name = "extract_button";
            this.extract_button.Size = new System.Drawing.Size(144, 40);
            this.extract_button.TabIndex = 9;
            this.extract_button.Text = "Extract";
            this.extract_button.UseVisualStyleBackColor = true;
            this.extract_button.Click += new System.EventHandler(this.extract_button_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 151);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(440, 96);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Tags In Que  :";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 253);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(440, 32);
            this.progressBar1.TabIndex = 17;
            // 
            // curent_tag_status
            // 
            this.curent_tag_status.AutoSize = true;
            this.curent_tag_status.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.curent_tag_status.Location = new System.Drawing.Point(22, 266);
            this.curent_tag_status.Name = "curent_tag_status";
            this.curent_tag_status.Size = new System.Drawing.Size(37, 13);
            this.curent_tag_status.TabIndex = 18;
            this.curent_tag_status.Text = "Status";
            this.curent_tag_status.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.output_db_);
            this.groupBox1.Controls.Add(this.override_tags_);
            this.groupBox1.Controls.Add(this.recursive_radio_);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Location = new System.Drawing.Point(89, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 74);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Flags :";
            // 
            // output_db_
            // 
            this.output_db_.AutoSize = true;
            this.output_db_.Location = new System.Drawing.Point(6, 51);
            this.output_db_.Name = "output_db_";
            this.output_db_.Size = new System.Drawing.Size(108, 17);
            this.output_db_.TabIndex = 15;
            this.output_db_.Text = "Output DataBase";
            this.output_db_.UseVisualStyleBackColor = true;
            // 
            // override_tags_
            // 
            this.override_tags_.AutoSize = true;
            this.override_tags_.Location = new System.Drawing.Point(101, 28);
            this.override_tags_.Name = "override_tags_";
            this.override_tags_.Size = new System.Drawing.Size(93, 17);
            this.override_tags_.TabIndex = 14;
            this.override_tags_.Text = "Override Tags";
            this.override_tags_.UseVisualStyleBackColor = true;
            // 
            // tag_count_stats
            // 
            this.tag_count_stats.AutoSize = true;
            this.tag_count_stats.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.tag_count_stats.Location = new System.Drawing.Point(103, 135);
            this.tag_count_stats.Name = "tag_count_stats";
            this.tag_count_stats.Size = new System.Drawing.Size(37, 13);
            this.tag_count_stats.TabIndex = 20;
            this.tag_count_stats.Text = "Status";
            this.tag_count_stats.Visible = false;
            // 
            // TagExtractor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.IndianRed;
            this.ClientSize = new System.Drawing.Size(464, 288);
            this.Controls.Add(this.tag_count_stats);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.curent_tag_status);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.extract_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "TagExtractor";
            this.Text = "Tag Extractor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button extract_button;
        public System.Windows.Forms.CheckBox recursive_radio_;
        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Label curent_tag_status;
        public System.Windows.Forms.CheckBox output_db_;
        public System.Windows.Forms.CheckBox override_tags_;
        private System.Windows.Forms.Label tag_count_stats;
    }
}