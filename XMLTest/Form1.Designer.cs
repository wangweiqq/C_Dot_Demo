namespace XMLTest
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lbl_addr = new System.Windows.Forms.Label();
            this.chklockaddree = new System.Windows.Forms.CheckBox();
            this.txtcellval = new System.Windows.Forms.TextBox();
            this.cbocelltype = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.cbotableids = new System.Windows.Forms.ComboBox();
            this.btnReadExcel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1009, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "ReadXML";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1007, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "linq测试";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1090, 20);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "创建XML";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 63);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1153, 568);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(373, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(628, 57);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "修改";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.lbl_addr);
            this.flowLayoutPanel1.Controls.Add(this.chklockaddree);
            this.flowLayoutPanel1.Controls.Add(this.txtcellval);
            this.flowLayoutPanel1.Controls.Add(this.cbocelltype);
            this.flowLayoutPanel1.Controls.Add(this.button4);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 20);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(611, 33);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // lbl_addr
            // 
            this.lbl_addr.AutoSize = true;
            this.lbl_addr.Location = new System.Drawing.Point(3, 0);
            this.lbl_addr.MinimumSize = new System.Drawing.Size(55, 0);
            this.lbl_addr.Name = "lbl_addr";
            this.lbl_addr.Size = new System.Drawing.Size(55, 15);
            this.lbl_addr.TabIndex = 0;
            // 
            // chklockaddree
            // 
            this.chklockaddree.AutoSize = true;
            this.chklockaddree.Location = new System.Drawing.Point(64, 3);
            this.chklockaddree.Name = "chklockaddree";
            this.chklockaddree.Size = new System.Drawing.Size(59, 19);
            this.chklockaddree.TabIndex = 1;
            this.chklockaddree.Text = "锁定";
            this.chklockaddree.UseVisualStyleBackColor = true;
            // 
            // txtcellval
            // 
            this.txtcellval.Location = new System.Drawing.Point(129, 3);
            this.txtcellval.Name = "txtcellval";
            this.txtcellval.Size = new System.Drawing.Size(250, 25);
            this.txtcellval.TabIndex = 2;
            // 
            // cbocelltype
            // 
            this.cbocelltype.FormattingEnabled = true;
            this.cbocelltype.Items.AddRange(new object[] {
            "文本",
            "公式"});
            this.cbocelltype.Location = new System.Drawing.Point(385, 3);
            this.cbocelltype.Name = "cbocelltype";
            this.cbocelltype.Size = new System.Drawing.Size(121, 23);
            this.cbocelltype.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(512, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 30);
            this.button4.TabIndex = 4;
            this.button4.Text = "修改";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cbotableids
            // 
            this.cbotableids.FormattingEnabled = true;
            this.cbotableids.Location = new System.Drawing.Point(145, 17);
            this.cbotableids.Name = "cbotableids";
            this.cbotableids.Size = new System.Drawing.Size(158, 23);
            this.cbotableids.TabIndex = 5;
            this.cbotableids.SelectedIndexChanged += new System.EventHandler(this.cbotableids_SelectedIndexChanged);
            // 
            // btnReadExcel
            // 
            this.btnReadExcel.Location = new System.Drawing.Point(22, 15);
            this.btnReadExcel.Name = "btnReadExcel";
            this.btnReadExcel.Size = new System.Drawing.Size(117, 46);
            this.btnReadExcel.TabIndex = 6;
            this.btnReadExcel.Text = "读取模板";
            this.btnReadExcel.UseVisualStyleBackColor = true;
            this.btnReadExcel.Click += new System.EventHandler(this.btnReadExcel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1177, 643);
            this.Controls.Add(this.btnReadExcel);
            this.Controls.Add(this.cbotableids);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Location = new System.Drawing.Point(0, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbocelltype;
        private System.Windows.Forms.TextBox txtcellval;
        private System.Windows.Forms.CheckBox chklockaddree;
        private System.Windows.Forms.Label lbl_addr;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ComboBox cbotableids;
        private System.Windows.Forms.Button btnReadExcel;
    }
}

