namespace CSReportEditor
{
    partial class fPageSetup
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_section = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.op_landscape = new System.Windows.Forms.RadioButton();
            this.op_portrait = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_paperSize = new System.Windows.Forms.ComboBox();
            this.cmd_cancel = new System.Windows.Forms.Button();
            this.cmd_apply = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pic_landscape = new System.Windows.Forms.PictureBox();
            this.pic_portrait = new System.Windows.Forms.PictureBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tx_width = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tx_height = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_landscape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_portrait)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lb_section);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(548, 70);
            this.panel1.TabIndex = 54;
            // 
            // lb_section
            // 
            this.lb_section.AutoSize = true;
            this.lb_section.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_section.Location = new System.Drawing.Point(94, 18);
            this.lb_section.Name = "lb_section";
            this.lb_section.Size = new System.Drawing.Size(121, 25);
            this.lb_section.TabIndex = 2;
            this.lb_section.Text = "Page setup";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.op_landscape);
            this.groupBox1.Controls.Add(this.op_portrait);
            this.groupBox1.Controls.Add(this.pic_landscape);
            this.groupBox1.Controls.Add(this.pic_portrait);
            this.groupBox1.Location = new System.Drawing.Point(21, 154);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(423, 106);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Orientation";
            // 
            // op_landscape
            // 
            this.op_landscape.AutoSize = true;
            this.op_landscape.Location = new System.Drawing.Point(145, 58);
            this.op_landscape.Name = "op_landscape";
            this.op_landscape.Size = new System.Drawing.Size(72, 17);
            this.op_landscape.TabIndex = 3;
            this.op_landscape.TabStop = true;
            this.op_landscape.Text = "Horizontal";
            this.op_landscape.UseVisualStyleBackColor = true;
            this.op_landscape.CheckedChanged += new System.EventHandler(this.op_landscape_CheckedChanged);
            // 
            // op_portrait
            // 
            this.op_portrait.AutoSize = true;
            this.op_portrait.Location = new System.Drawing.Point(145, 35);
            this.op_portrait.Name = "op_portrait";
            this.op_portrait.Size = new System.Drawing.Size(60, 17);
            this.op_portrait.TabIndex = 2;
            this.op_portrait.TabStop = true;
            this.op_portrait.Text = "Vertical";
            this.op_portrait.UseVisualStyleBackColor = true;
            this.op_portrait.CheckedChanged += new System.EventHandler(this.op_portrait_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Location = new System.Drawing.Point(12, 76);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(468, 278);
            this.groupBox3.TabIndex = 58;
            this.groupBox3.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.tx_width);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.tx_height);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cb_paperSize);
            this.groupBox2.Location = new System.Drawing.Point(21, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(423, 129);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Orientation";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Paper size";
            // 
            // cb_paperSize
            // 
            this.cb_paperSize.FormattingEnabled = true;
            this.cb_paperSize.Location = new System.Drawing.Point(105, 33);
            this.cb_paperSize.Name = "cb_paperSize";
            this.cb_paperSize.Size = new System.Drawing.Size(231, 21);
            this.cb_paperSize.TabIndex = 0;
            this.cb_paperSize.SelectedIndexChanged += new System.EventHandler(this.cb_paperSize_SelectedIndexChanged);
            // 
            // cmd_cancel
            // 
            this.cmd_cancel.Location = new System.Drawing.Point(405, 369);
            this.cmd_cancel.Name = "cmd_cancel";
            this.cmd_cancel.Size = new System.Drawing.Size(75, 23);
            this.cmd_cancel.TabIndex = 56;
            this.cmd_cancel.Text = "Cancel";
            this.cmd_cancel.UseVisualStyleBackColor = true;
            this.cmd_cancel.Click += new System.EventHandler(this.cmd_cancel_Click);
            // 
            // cmd_apply
            // 
            this.cmd_apply.Location = new System.Drawing.Point(324, 369);
            this.cmd_apply.Name = "cmd_apply";
            this.cmd_apply.Size = new System.Drawing.Size(75, 23);
            this.cmd_apply.TabIndex = 55;
            this.cmd_apply.Text = "Apply";
            this.cmd_apply.UseVisualStyleBackColor = true;
            this.cmd_apply.Click += new System.EventHandler(this.cmd_apply_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CSReportEditor.Properties.Resources.config_page;
            this.pictureBox1.Location = new System.Drawing.Point(24, 18);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(46, 39);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // pic_landscape
            // 
            this.pic_landscape.Image = global::CSReportEditor.Properties.Resources.page_landscape;
            this.pic_landscape.Location = new System.Drawing.Point(46, 26);
            this.pic_landscape.Name = "pic_landscape";
            this.pic_landscape.Size = new System.Drawing.Size(68, 61);
            this.pic_landscape.TabIndex = 1;
            this.pic_landscape.TabStop = false;
            // 
            // pic_portrait
            // 
            this.pic_portrait.Image = global::CSReportEditor.Properties.Resources.page_portrait;
            this.pic_portrait.Location = new System.Drawing.Point(46, 26);
            this.pic_portrait.Name = "pic_portrait";
            this.pic_portrait.Size = new System.Drawing.Size(68, 61);
            this.pic_portrait.TabIndex = 0;
            this.pic_portrait.TabStop = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(126, 89);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(35, 13);
            this.label20.TabIndex = 42;
            this.label20.Text = "Width";
            // 
            // tx_width
            // 
            this.tx_width.Location = new System.Drawing.Point(168, 86);
            this.tx_width.Name = "tx_width";
            this.tx_width.Size = new System.Drawing.Size(71, 20);
            this.tx_width.TabIndex = 41;
            this.tx_width.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(125, 63);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(38, 13);
            this.label19.TabIndex = 40;
            this.label19.Text = "Height";
            // 
            // tx_height
            // 
            this.tx_height.Location = new System.Drawing.Point(168, 60);
            this.tx_height.Name = "tx_height";
            this.tx_height.Size = new System.Drawing.Size(71, 20);
            this.tx_height.TabIndex = 39;
            this.tx_height.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // fPageSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 408);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.cmd_cancel);
            this.Controls.Add(this.cmd_apply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fPageSetup";
            this.Text = "fPageSetup";
            this.Load += new System.EventHandler(this.fPageSetup_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_landscape)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_portrait)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb_section;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pic_portrait;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button cmd_cancel;
        private System.Windows.Forms.Button cmd_apply;
        private System.Windows.Forms.PictureBox pic_landscape;
        private System.Windows.Forms.RadioButton op_landscape;
        private System.Windows.Forms.RadioButton op_portrait;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cb_paperSize;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tx_width;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tx_height;
    }
}