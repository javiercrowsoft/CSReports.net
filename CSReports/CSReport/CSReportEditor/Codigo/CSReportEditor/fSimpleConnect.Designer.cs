namespace CSReportEditor
{
    partial class fSimpleConnect
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmd_cancel = new System.Windows.Forms.Button();
            this.cmd_apply = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tx_password = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tx_user = new System.Windows.Forms.TextBox();
            this.op_sqlConnection = new System.Windows.Forms.RadioButton();
            this.op_trustedConnection = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.tx_database = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tx_server = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(438, 358);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lbTitle);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(432, 67);
            this.panel1.TabIndex = 2;
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.Location = new System.Drawing.Point(74, 19);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(216, 26);
            this.lbTitle.TabIndex = 2;
            this.lbTitle.Text = "Database connection";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CSReportEditor.Properties.Resources.config_page;
            this.pictureBox1.Location = new System.Drawing.Point(22, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 31);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cmd_cancel);
            this.panel2.Controls.Add(this.cmd_apply);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 312);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(432, 43);
            this.panel2.TabIndex = 4;
            // 
            // cmd_cancel
            // 
            this.cmd_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmd_cancel.Location = new System.Drawing.Point(348, 11);
            this.cmd_cancel.Name = "cmd_cancel";
            this.cmd_cancel.Size = new System.Drawing.Size(75, 23);
            this.cmd_cancel.TabIndex = 1;
            this.cmd_cancel.Text = "Cancel";
            this.cmd_cancel.UseVisualStyleBackColor = true;
            this.cmd_cancel.Click += new System.EventHandler(this.cmd_cancel_Click);
            // 
            // cmd_apply
            // 
            this.cmd_apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmd_apply.Location = new System.Drawing.Point(267, 11);
            this.cmd_apply.Name = "cmd_apply";
            this.cmd_apply.Size = new System.Drawing.Size(75, 23);
            this.cmd_apply.TabIndex = 0;
            this.cmd_apply.Text = "Apply";
            this.cmd_apply.UseVisualStyleBackColor = true;
            this.cmd_apply.Click += new System.EventHandler(this.cmd_apply_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.tx_password);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.tx_user);
            this.panel3.Controls.Add(this.op_sqlConnection);
            this.panel3.Controls.Add(this.op_trustedConnection);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.tx_database);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.tx_server);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 76);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(432, 230);
            this.panel3.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 173);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Password";
            // 
            // tx_password
            // 
            this.tx_password.Location = new System.Drawing.Point(109, 170);
            this.tx_password.Name = "tx_password";
            this.tx_password.PasswordChar = '*';
            this.tx_password.Size = new System.Drawing.Size(243, 20);
            this.tx_password.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "User";
            // 
            // tx_user
            // 
            this.tx_user.Location = new System.Drawing.Point(109, 144);
            this.tx_user.Name = "tx_user";
            this.tx_user.Size = new System.Drawing.Size(243, 20);
            this.tx_user.TabIndex = 11;
            // 
            // op_sqlConnection
            // 
            this.op_sqlConnection.AutoSize = true;
            this.op_sqlConnection.Location = new System.Drawing.Point(22, 108);
            this.op_sqlConnection.Name = "op_sqlConnection";
            this.op_sqlConnection.Size = new System.Drawing.Size(102, 17);
            this.op_sqlConnection.TabIndex = 10;
            this.op_sqlConnection.TabStop = true;
            this.op_sqlConnection.Text = "SQL connection";
            this.op_sqlConnection.UseVisualStyleBackColor = true;
            this.op_sqlConnection.CheckedChanged += new System.EventHandler(this.op_sqlConnection_CheckedChanged);
            // 
            // op_trustedConnection
            // 
            this.op_trustedConnection.AutoSize = true;
            this.op_trustedConnection.Location = new System.Drawing.Point(22, 85);
            this.op_trustedConnection.Name = "op_trustedConnection";
            this.op_trustedConnection.Size = new System.Drawing.Size(117, 17);
            this.op_trustedConnection.TabIndex = 9;
            this.op_trustedConnection.TabStop = true;
            this.op_trustedConnection.Text = "Trusted connection";
            this.op_trustedConnection.UseVisualStyleBackColor = true;
            this.op_trustedConnection.CheckedChanged += new System.EventHandler(this.op_trustedConnection_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Database";
            // 
            // tx_database
            // 
            this.tx_database.Location = new System.Drawing.Point(79, 43);
            this.tx_database.Name = "tx_database";
            this.tx_database.Size = new System.Drawing.Size(335, 20);
            this.tx_database.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Server";
            // 
            // tx_server
            // 
            this.tx_server.Location = new System.Drawing.Point(79, 17);
            this.tx_server.Name = "tx_server";
            this.tx_server.Size = new System.Drawing.Size(335, 20);
            this.tx_server.TabIndex = 5;
            // 
            // fSimpleConnect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 358);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fSimpleConnect";
            this.Text = "Connection Settings";
            this.Load += new System.EventHandler(this.fSimpleConnect_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button cmd_cancel;
        private System.Windows.Forms.Button cmd_apply;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tx_database;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tx_server;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tx_password;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tx_user;
        private System.Windows.Forms.RadioButton op_sqlConnection;
        private System.Windows.Forms.RadioButton op_trustedConnection;
    }
}