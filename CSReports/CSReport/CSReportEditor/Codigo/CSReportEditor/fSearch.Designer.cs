namespace CSReportEditor
{
    partial class fSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fSearch));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmd_edit = new System.Windows.Forms.Button();
            this.cmd_close = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lv_controls = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmd_search = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tx_toSearch = new System.Windows.Forms.TextBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lv_controls, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(523, 405);
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
            this.panel1.Size = new System.Drawing.Size(517, 67);
            this.panel1.TabIndex = 2;
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.Location = new System.Drawing.Point(74, 19);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(170, 26);
            this.lbTitle.TabIndex = 2;
            this.lbTitle.Text = "Report definition";
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
            // cmd_edit
            // 
            this.cmd_edit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmd_edit.Location = new System.Drawing.Point(352, 12);
            this.cmd_edit.Name = "cmd_edit";
            this.cmd_edit.Size = new System.Drawing.Size(75, 23);
            this.cmd_edit.TabIndex = 0;
            this.cmd_edit.Text = "Edit";
            this.cmd_edit.UseVisualStyleBackColor = true;
            this.cmd_edit.Click += new System.EventHandler(this.cmd_edit_Click);
            // 
            // cmd_close
            // 
            this.cmd_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmd_close.Location = new System.Drawing.Point(433, 12);
            this.cmd_close.Name = "cmd_close";
            this.cmd_close.Size = new System.Drawing.Size(75, 23);
            this.cmd_close.TabIndex = 1;
            this.cmd_close.Text = "Close";
            this.cmd_close.UseVisualStyleBackColor = true;
            this.cmd_close.Click += new System.EventHandler(this.cmd_close_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.cmd_close);
            this.panel3.Controls.Add(this.cmd_edit);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 359);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(517, 43);
            this.panel3.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.tx_toSearch);
            this.panel2.Controls.Add(this.cmd_search);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 76);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(517, 53);
            this.panel2.TabIndex = 10;
            // 
            // lv_controls
            // 
            this.lv_controls.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4});
            this.lv_controls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_controls.FullRowSelect = true;
            this.lv_controls.Location = new System.Drawing.Point(3, 135);
            this.lv_controls.MultiSelect = false;
            this.lv_controls.Name = "lv_controls";
            this.lv_controls.Size = new System.Drawing.Size(517, 218);
            this.lv_controls.SmallImageList = this.imageList;
            this.lv_controls.TabIndex = 11;
            this.lv_controls.UseCompatibleStateImageBehavior = false;
            this.lv_controls.View = System.Windows.Forms.View.Details;
            this.lv_controls.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lv_controls_KeyUp);
            this.lv_controls.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lv_controls_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Found in";
            this.columnHeader4.Width = 250;
            // 
            // cmd_search
            // 
            this.cmd_search.Location = new System.Drawing.Point(407, 15);
            this.cmd_search.Name = "cmd_search";
            this.cmd_search.Size = new System.Drawing.Size(75, 23);
            this.cmd_search.TabIndex = 1;
            this.cmd_search.Text = "Search";
            this.cmd_search.UseVisualStyleBackColor = true;
            this.cmd_search.Click += new System.EventHandler(this.cmd_search_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Search";
            // 
            // tx_toSearch
            // 
            this.tx_toSearch.Location = new System.Drawing.Point(66, 17);
            this.tx_toSearch.Name = "tx_toSearch";
            this.tx_toSearch.Size = new System.Drawing.Size(335, 20);
            this.tx_toSearch.TabIndex = 2;
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "formula2.ico");
            this.imageList.Images.SetKeyName(1, "formula.ico");
            this.imageList.Images.SetKeyName(2, "property.ico");
            this.imageList.Images.SetKeyName(3, "base002.ico");
            this.imageList.Images.SetKeyName(4, "sectionblue.ico");
            this.imageList.Images.SetKeyName(5, "sectionorange.ico");
            this.imageList.Images.SetKeyName(6, "sectionred.ico");
            this.imageList.Images.SetKeyName(7, "textedit.gif");
            // 
            // fSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 405);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "fSearch";
            this.Text = "fSearch";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListView lv_controls;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button cmd_close;
        private System.Windows.Forms.Button cmd_edit;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button cmd_search;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tx_toSearch;
        private System.Windows.Forms.ImageList imageList;
    }
}