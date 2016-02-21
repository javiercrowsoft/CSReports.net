namespace CSReportEditor
{
    partial class fProgress
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
            this.cmd_cancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lb_curr_page = new System.Windows.Forms.Label();
            this.prg_bar = new System.Windows.Forms.ProgressBar();
            this.lb_record_count = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lb_curr_record = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lb_task = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmd_cancel
            // 
            this.cmd_cancel.Location = new System.Drawing.Point(224, 185);
            this.cmd_cancel.Name = "cmd_cancel";
            this.cmd_cancel.Size = new System.Drawing.Size(75, 23);
            this.cmd_cancel.TabIndex = 9;
            this.cmd_cancel.Text = "Cancel";
            this.cmd_cancel.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lb_task);
            this.panel1.Controls.Add(this.Label1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(531, 77);
            this.panel1.TabIndex = 7;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(77, 26);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(159, 24);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "Executing report :";
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(42, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Generating page :";
            // 
            // lb_curr_page
            // 
            this.lb_curr_page.AutoSize = true;
            this.lb_curr_page.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_curr_page.Location = new System.Drawing.Point(163, 102);
            this.lb_curr_page.Name = "lb_curr_page";
            this.lb_curr_page.Size = new System.Drawing.Size(15, 16);
            this.lb_curr_page.TabIndex = 11;
            this.lb_curr_page.Text = "0";
            // 
            // prg_bar
            // 
            this.prg_bar.Location = new System.Drawing.Point(24, 145);
            this.prg_bar.Name = "prg_bar";
            this.prg_bar.Size = new System.Drawing.Size(473, 23);
            this.prg_bar.TabIndex = 12;
            // 
            // lb_record_count
            // 
            this.lb_record_count.AutoSize = true;
            this.lb_record_count.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_record_count.Location = new System.Drawing.Point(278, 102);
            this.lb_record_count.Name = "lb_record_count";
            this.lb_record_count.Size = new System.Drawing.Size(15, 16);
            this.lb_record_count.TabIndex = 14;
            this.lb_record_count.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(224, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 16);
            this.label4.TabIndex = 13;
            this.label4.Text = "Rows :";
            // 
            // lb_curr_record
            // 
            this.lb_curr_record.AutoSize = true;
            this.lb_curr_record.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_curr_record.Location = new System.Drawing.Point(441, 102);
            this.lb_curr_record.Name = "lb_curr_record";
            this.lb_curr_record.Size = new System.Drawing.Size(15, 16);
            this.lb_curr_record.TabIndex = 16;
            this.lb_curr_record.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(355, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "Current row :";
            // 
            // lb_task
            // 
            this.lb_task.AutoSize = true;
            this.lb_task.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_task.Location = new System.Drawing.Point(232, 26);
            this.lb_task.Name = "lb_task";
            this.lb_task.Size = new System.Drawing.Size(0, 24);
            this.lb_task.TabIndex = 3;
            // 
            // fProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 220);
            this.Controls.Add(this.lb_curr_record);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lb_record_count);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.prg_bar);
            this.Controls.Add(this.lb_curr_page);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmd_cancel);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fProgress";
            this.Text = "fProgress";
            this.Load += new System.EventHandler(this.fProgress_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmd_cancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lb_curr_page;
        private System.Windows.Forms.ProgressBar prg_bar;
        private System.Windows.Forms.Label lb_record_count;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lb_curr_record;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lb_task;
    }
}