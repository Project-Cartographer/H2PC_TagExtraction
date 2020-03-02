namespace Map_Handler
{
    partial class MetaInjector
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
            this.metaFileTextBox = new System.Windows.Forms.TextBox();
            this.targetMapTextBox = new System.Windows.Forms.TextBox();
            this.saveMapTextBox = new System.Windows.Forms.TextBox();
            this.selectMetabtn = new System.Windows.Forms.Button();
            this.selectTargetMapbtn = new System.Windows.Forms.Button();
            this.saveMapAsbtn = new System.Windows.Forms.Button();
            this.injectbtn = new System.Windows.Forms.Button();
            this.addExistingTagsCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // metaFileTextBox
            // 
            this.metaFileTextBox.Location = new System.Drawing.Point(31, 31);
            this.metaFileTextBox.Name = "metaFileTextBox";
            this.metaFileTextBox.ReadOnly = true;
            this.metaFileTextBox.Size = new System.Drawing.Size(338, 22);
            this.metaFileTextBox.TabIndex = 0;
            // 
            // targetMapTextBox
            // 
            this.targetMapTextBox.Location = new System.Drawing.Point(31, 89);
            this.targetMapTextBox.Name = "targetMapTextBox";
            this.targetMapTextBox.ReadOnly = true;
            this.targetMapTextBox.Size = new System.Drawing.Size(338, 22);
            this.targetMapTextBox.TabIndex = 1;
            // 
            // saveMapTextBox
            // 
            this.saveMapTextBox.Location = new System.Drawing.Point(31, 146);
            this.saveMapTextBox.Name = "saveMapTextBox";
            this.saveMapTextBox.ReadOnly = true;
            this.saveMapTextBox.Size = new System.Drawing.Size(338, 22);
            this.saveMapTextBox.TabIndex = 2;
            // 
            // selectMetabtn
            // 
            this.selectMetabtn.Location = new System.Drawing.Point(407, 21);
            this.selectMetabtn.Name = "selectMetabtn";
            this.selectMetabtn.Size = new System.Drawing.Size(170, 40);
            this.selectMetabtn.TabIndex = 3;
            this.selectMetabtn.Text = "Select Meta File";
            this.selectMetabtn.UseVisualStyleBackColor = true;
            this.selectMetabtn.Click += new System.EventHandler(this.selectMeta_Click);
            // 
            // selectTargetMapbtn
            // 
            this.selectTargetMapbtn.Location = new System.Drawing.Point(407, 74);
            this.selectTargetMapbtn.Name = "selectTargetMapbtn";
            this.selectTargetMapbtn.Size = new System.Drawing.Size(170, 40);
            this.selectTargetMapbtn.TabIndex = 4;
            this.selectTargetMapbtn.Text = "Select Target Map";
            this.selectTargetMapbtn.UseVisualStyleBackColor = true;
            this.selectTargetMapbtn.Click += new System.EventHandler(this.selectTargetMap_Click);
            // 
            // saveMapAsbtn
            // 
            this.saveMapAsbtn.Location = new System.Drawing.Point(407, 135);
            this.saveMapAsbtn.Name = "saveMapAsbtn";
            this.saveMapAsbtn.Size = new System.Drawing.Size(170, 40);
            this.saveMapAsbtn.TabIndex = 5;
            this.saveMapAsbtn.Text = "Save Map As";
            this.saveMapAsbtn.UseVisualStyleBackColor = true;
            this.saveMapAsbtn.Click += new System.EventHandler(this.saveMapAs_Click);
            // 
            // injectbtn
            // 
            this.injectbtn.Location = new System.Drawing.Point(407, 193);
            this.injectbtn.Name = "injectbtn";
            this.injectbtn.Size = new System.Drawing.Size(170, 40);
            this.injectbtn.TabIndex = 6;
            this.injectbtn.Text = "Inject";
            this.injectbtn.UseVisualStyleBackColor = true;
            this.injectbtn.Click += new System.EventHandler(this.inject_Click);
            // 
            // addExistingTagsCheckBox
            // 
            this.addExistingTagsCheckBox.AutoSize = true;
            this.addExistingTagsCheckBox.Location = new System.Drawing.Point(31, 193);
            this.addExistingTagsCheckBox.Name = "addExistingTagsCheckBox";
            this.addExistingTagsCheckBox.Size = new System.Drawing.Size(143, 21);
            this.addExistingTagsCheckBox.TabIndex = 7;
            this.addExistingTagsCheckBox.Text = "Add Existing Tags";
            this.addExistingTagsCheckBox.UseVisualStyleBackColor = true;
            // 
            // MetaInjector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 258);
            this.Controls.Add(this.addExistingTagsCheckBox);
            this.Controls.Add(this.injectbtn);
            this.Controls.Add(this.saveMapAsbtn);
            this.Controls.Add(this.selectTargetMapbtn);
            this.Controls.Add(this.selectMetabtn);
            this.Controls.Add(this.saveMapTextBox);
            this.Controls.Add(this.targetMapTextBox);
            this.Controls.Add(this.metaFileTextBox);
            this.Name = "MetaInjector";
            this.Text = "MetaInjector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox metaFileTextBox;
        private System.Windows.Forms.TextBox targetMapTextBox;
        private System.Windows.Forms.TextBox saveMapTextBox;
        private System.Windows.Forms.Button selectMetabtn;
        private System.Windows.Forms.Button selectTargetMapbtn;
        private System.Windows.Forms.Button saveMapAsbtn;
        private System.Windows.Forms.Button injectbtn;
        private System.Windows.Forms.CheckBox addExistingTagsCheckBox;
    }
}