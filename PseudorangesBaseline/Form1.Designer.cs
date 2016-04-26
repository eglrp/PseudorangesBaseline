namespace PseudorangesBaseline
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.n_FileReaderButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.master_O_FileReaderButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.rover_O_FileReaderButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.paintSPP_ResultButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SPPButton = new System.Windows.Forms.Button();
            this.recalculateButton = new System.Windows.Forms.Button();
            this.RPButton = new System.Windows.Forms.Button();
            this.recalculateButton2 = new System.Windows.Forms.Button();
            this.paintRP_ResultButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // n_FileReaderButton
            // 
            this.n_FileReaderButton.Location = new System.Drawing.Point(12, 69);
            this.n_FileReaderButton.Name = "n_FileReaderButton";
            this.n_FileReaderButton.Size = new System.Drawing.Size(297, 107);
            this.n_FileReaderButton.TabIndex = 0;
            this.n_FileReaderButton.Text = "读取导航电文文件";
            this.n_FileReaderButton.UseVisualStyleBackColor = true;
            this.n_FileReaderButton.Click += new System.EventHandler(this.n_FileReaderButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(315, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Flag";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 709);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(473, 172);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            // 
            // master_O_FileReaderButton
            // 
            this.master_O_FileReaderButton.Location = new System.Drawing.Point(413, 69);
            this.master_O_FileReaderButton.Name = "master_O_FileReaderButton";
            this.master_O_FileReaderButton.Size = new System.Drawing.Size(297, 107);
            this.master_O_FileReaderButton.TabIndex = 3;
            this.master_O_FileReaderButton.Text = "读取基准站观测值文件";
            this.master_O_FileReaderButton.UseVisualStyleBackColor = true;
            this.master_O_FileReaderButton.Click += new System.EventHandler(this.master_O_FileReaderButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(716, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Flag";
            // 
            // rover_O_FileReaderButton
            // 
            this.rover_O_FileReaderButton.Location = new System.Drawing.Point(809, 69);
            this.rover_O_FileReaderButton.Name = "rover_O_FileReaderButton";
            this.rover_O_FileReaderButton.Size = new System.Drawing.Size(297, 107);
            this.rover_O_FileReaderButton.TabIndex = 5;
            this.rover_O_FileReaderButton.Text = "读取流动站站观测值文件";
            this.rover_O_FileReaderButton.UseVisualStyleBackColor = true;
            this.rover_O_FileReaderButton.Click += new System.EventHandler(this.rover_O_FileReaderButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(1112, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "Flag";
            // 
            // paintSPP_ResultButton
            // 
            this.paintSPP_ResultButton.Location = new System.Drawing.Point(715, 709);
            this.paintSPP_ResultButton.Name = "paintSPP_ResultButton";
            this.paintSPP_ResultButton.Size = new System.Drawing.Size(149, 88);
            this.paintSPP_ResultButton.TabIndex = 7;
            this.paintSPP_ResultButton.Text = "查看";
            this.paintSPP_ResultButton.UseVisualStyleBackColor = true;
            this.paintSPP_ResultButton.Click += new System.EventHandler(this.paintSPP_ResultButton_Click);
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(690, 800);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(204, 84);
            this.label4.TabIndex = 8;
            this.label4.Text = "点击查看基准站单点定位结果分析";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SPPButton
            // 
            this.SPPButton.Location = new System.Drawing.Point(73, 561);
            this.SPPButton.Name = "SPPButton";
            this.SPPButton.Size = new System.Drawing.Size(284, 99);
            this.SPPButton.TabIndex = 9;
            this.SPPButton.Text = "基准站单点定位";
            this.SPPButton.UseVisualStyleBackColor = true;
            this.SPPButton.Click += new System.EventHandler(this.SPPButton_Click);
            // 
            // recalculateButton
            // 
            this.recalculateButton.Location = new System.Drawing.Point(363, 577);
            this.recalculateButton.Name = "recalculateButton";
            this.recalculateButton.Size = new System.Drawing.Size(122, 67);
            this.recalculateButton.TabIndex = 10;
            this.recalculateButton.Text = "重新计算";
            this.recalculateButton.UseVisualStyleBackColor = true;
            this.recalculateButton.Click += new System.EventHandler(this.recalculateButton_Click);
            // 
            // RPButton
            // 
            this.RPButton.Location = new System.Drawing.Point(678, 561);
            this.RPButton.Name = "RPButton";
            this.RPButton.Size = new System.Drawing.Size(284, 99);
            this.RPButton.TabIndex = 11;
            this.RPButton.Text = "相对定位";
            this.RPButton.UseVisualStyleBackColor = true;
            this.RPButton.Click += new System.EventHandler(this.RPButton_Click);
            // 
            // recalculateButton2
            // 
            this.recalculateButton2.Location = new System.Drawing.Point(968, 577);
            this.recalculateButton2.Name = "recalculateButton2";
            this.recalculateButton2.Size = new System.Drawing.Size(122, 67);
            this.recalculateButton2.TabIndex = 12;
            this.recalculateButton2.Text = "重新计算";
            this.recalculateButton2.UseVisualStyleBackColor = true;
            this.recalculateButton2.Click += new System.EventHandler(this.recalculateButton2_Click);
            // 
            // paintRP_ResultButton
            // 
            this.paintRP_ResultButton.Location = new System.Drawing.Point(941, 709);
            this.paintRP_ResultButton.Name = "paintRP_ResultButton";
            this.paintRP_ResultButton.Size = new System.Drawing.Size(149, 88);
            this.paintRP_ResultButton.TabIndex = 13;
            this.paintRP_ResultButton.Text = "查看";
            this.paintRP_ResultButton.UseVisualStyleBackColor = true;
            this.paintRP_ResultButton.Click += new System.EventHandler(this.paintRP_ResultButton_Click);
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(913, 800);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(209, 84);
            this.label5.TabIndex = 14;
            this.label5.Text = "点击查看伪距相对定位定位结果分析";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1182, 893);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.paintRP_ResultButton);
            this.Controls.Add(this.recalculateButton2);
            this.Controls.Add(this.RPButton);
            this.Controls.Add(this.recalculateButton);
            this.Controls.Add(this.SPPButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.paintSPP_ResultButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rover_O_FileReaderButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.master_O_FileReaderButton);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.n_FileReaderButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GPS伪距相对定位";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button n_FileReaderButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button master_O_FileReaderButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button rover_O_FileReaderButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button paintSPP_ResultButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button SPPButton;
        private System.Windows.Forms.Button recalculateButton;
        private System.Windows.Forms.Button RPButton;
        private System.Windows.Forms.Button recalculateButton2;
        private System.Windows.Forms.Button paintRP_ResultButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

