namespace CSDataBase
{
    partial class fCancelQuery
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
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.lbTask = new System.Windows.Forms.Label();
            this.lbTime = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picIcon
            // 
            this.picIcon.Location = new System.Drawing.Point(21, 15);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(58, 50);
            this.picIcon.TabIndex = 0;
            this.picIcon.TabStop = false;
            // 
            // lbTask
            // 
            this.lbTask.AutoSize = true;
            this.lbTask.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTask.Location = new System.Drawing.Point(105, 29);
            this.lbTask.Name = "lbTask";
            this.lbTask.Size = new System.Drawing.Size(92, 16);
            this.lbTask.TabIndex = 1;
            this.lbTask.Text = "Getting data ...";
            // 
            // lbTime
            // 
            this.lbTime.AutoSize = true;
            this.lbTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTime.Location = new System.Drawing.Point(202, 52);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(39, 16);
            this.lbTime.TabIndex = 2;
            this.lbTime.Text = "00:00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(105, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Time passed:";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(94, 119);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(88, 27);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.picIcon);
            this.panel1.Controls.Add(this.lbTask);
            this.panel1.Controls.Add(this.lbTime);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(292, 100);
            this.panel1.TabIndex = 5;
            // 
            // fCancelQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 158);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cmdCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fCancelQuery";
            this.Text = "Open Query";
            this.Load += new System.EventHandler(this.fCancelQuery_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.Label lbTask;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Panel panel1;
    }
}