using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PseudorangesBaseline
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = null;
            label2.Text = null;
            label3.Text = null;

            SPPButton.Enabled = false;
            SPPButton.Text = "请打开导航电文文件";
            recalculateButton.Enabled = false;
            paintSPP_ResultButton.Enabled = false;
            label4.Visible = false;

            RPButton.Enabled = false;
            RPButton.Text = "请打开导航电文文件";
            recalculateButton2.Enabled = false;
            paintRP_ResultButton.Enabled = false;
            label5.Visible = false;

            toolTip1.InitialDelay = 50;
            toolTip1.ReshowDelay = 0;
            toolTip1.AutoPopDelay = 10000;
            toolTip1.SetToolTip(n_FileReaderButton, "单击左键选择打开导航电文文件");
            toolTip1.SetToolTip(master_O_FileReaderButton, "单击左键选择打开基准站观测值文件");
            toolTip1.SetToolTip(rover_O_FileReaderButton, "单击左键选择打开流动站站观测值文件");
            toolTip1.SetToolTip(SPPButton, "单击左键开始解算基准站坐标");
            toolTip1.SetToolTip(RPButton, "单击左键开始伪距相对定位");
            toolTip1.SetToolTip(paintSPP_ResultButton, "单击左键查看伪距单点定位结果分析");
            toolTip1.SetToolTip(paintRP_ResultButton, "单击左键查看伪距相对定位结果分析");
            toolTip1.SetToolTip(richTextBox1, "结果显示窗口");
        }



        bool is_N_FileReadComplete, is_M_OFileReadComplete, is_R_OFileReadComplete;
        private void n_FileReaderButton_Click(object sender, EventArgs e)
        {
            label1.Text = null;

            N_FileReader n_FileReader = new N_FileReader();
            bool flag = n_FileReader.Read_N_File();

            is_N_FileReadComplete = flag;
            if (is_N_FileReadComplete == true)
            {
                label1.Text = "√";
            }
            if (is_N_FileReadComplete == true && is_M_OFileReadComplete == false)
            {
                SPPButton.Enabled = false;
                SPPButton.ForeColor = Color.Black;
                SPPButton.Text = "请打开基准站观测文件";
                RPButton.Enabled = false;
                RPButton.ForeColor = Color.Black;
                RPButton.Text = "请打开基准站观测文件";

                //***************用于查看能否正常访问读取到的数据***************//
                TestAccessibility test = new TestAccessibility();
                richTextBox1.Text = test.Test_N();
                //***************用于查看能否正常访问读取到的数据***************//
            }
            if (is_N_FileReadComplete == true && is_M_OFileReadComplete == true)
            {
                SPPButton.Enabled = true;
                SPPButton.ForeColor = Color.Black;
                SPPButton.Text = "基准站单点定位";
                RPButton.Enabled = false;
                RPButton.ForeColor = Color.Black;
                RPButton.Text = "请打开流动站观测文件";
            }
            if (is_N_FileReadComplete == true && is_M_OFileReadComplete == true && is_R_OFileReadComplete == true)
            {
                RPButton.Enabled = true;
                RPButton.ForeColor = Color.Black;
                RPButton.Text = "相对定位";
            }
        }



        private void master_O_FileReaderButton_Click(object sender, EventArgs e)
        {
            label2.Text = null;

            Master_O_FileReader master_O_FileReader = new Master_O_FileReader();
            bool flag = master_O_FileReader.Read_O_File();

            is_M_OFileReadComplete = flag;
            if (is_M_OFileReadComplete == true)
            {
                label2.Text = "√";
            }
            if (is_N_FileReadComplete == true && is_M_OFileReadComplete == true)
            {
                SPPButton.Enabled = true;
                SPPButton.ForeColor = Color.Black;
                SPPButton.Text = "基准站单点定位";
                RPButton.Enabled = false;
                RPButton.ForeColor = Color.Black;
                RPButton.Text = "请打开流动站观测文件";


                //***************用于查看能否正常访问读取到的数据***************//
                TestAccessibility test = new TestAccessibility();
                richTextBox1.Text = test.Test_Master_O();
                //***************用于查看能否正常访问读取到的数据***************//
            }
            if (is_N_FileReadComplete == true && is_M_OFileReadComplete == true && is_R_OFileReadComplete == true)
            {
                RPButton.Enabled = true;
                RPButton.ForeColor = Color.Black;
                RPButton.Text = "相对定位";
            }
        }

        private void rover_O_FileReaderButton_Click(object sender, EventArgs e)
        {
            label3.Text = null;

            Rover_O_FileReader rover_O_FileReader = new Rover_O_FileReader();
            bool flag = rover_O_FileReader.Read_O_File();

            is_R_OFileReadComplete = flag;
            if (is_R_OFileReadComplete == true)
            {
                label3.Text = "√";
            }
            if (is_N_FileReadComplete == true && is_M_OFileReadComplete == true && is_R_OFileReadComplete == true)
            {
                RPButton.Enabled = true;
                RPButton.ForeColor = Color.Black;
                RPButton.Text = "相对定位";
            }
        }

        private void SPPButton_Click(object sender, EventArgs e)
        {
            SPPButton.ForeColor = Color.Red;
            SPPButton.Text = "计算中...";
            Baseline baseline = new Baseline();
            bool flag = baseline.SinglePointPositioning();
            if (flag == true)
            {
                SPPButton.Text = "计算完成";
                SPPButton.Enabled = false;
                label4.Visible = true;
                paintSPP_ResultButton.Enabled = true;
                recalculateButton.Enabled = true;

                if (richTextBox1.Text.Contains("相对定位结果:") == false)
                {
                    richTextBox1.Text = "基准站单点定位结果:" + "\n" + "X: " + SPP_Result.X.ToString() + " m" + "\n"
                        + "Y: " + SPP_Result.Y.ToString() + " m" + "\n" + "Z: " + SPP_Result.Z.ToString() + " m";
                }
            }
            else
            {
                SPPButton.Text = "计算失败";
                SPPButton.Enabled = false;
                recalculateButton.Enabled = true;
            }
        }

        private void paintSPP_ResultButton_Click(object sender, EventArgs e)
        {
            Paint1 paint1 = new Paint1();
            paint1.Show();
        }

        private void recalculateButton_Click(object sender, EventArgs e)
        {
            SPP_Result.X = 0;
            SPP_Result.Y = 0;
            SPP_Result.Z = 0;
            SPP_Result.Cdtr = 0;
            SPPButton.ForeColor = Color.Black;
            SPPButton.Text = "基准站单点定位";
            SPPButton.Enabled = true;
            recalculateButton.Enabled = false;
            paintSPP_ResultButton.Enabled = false;
            label4.Visible = false;

            if (richTextBox1.Text.Contains("相对定位结果:") == false)
            {
                richTextBox1.Text = null;
            }
        }

        private void RPButton_Click(object sender, EventArgs e)
        {
            RPButton.ForeColor = Color.Red;
            RPButton.Text = "计算中...";
            Baseline baseline = new Baseline();
            bool flag = baseline.RelativePositioning();
            if (flag == true)
            {
                RPButton.Text = "计算完成";
                RPButton.Enabled = false;
                paintRP_ResultButton.Enabled = true;
                label5.Visible = true;
                recalculateButton2.Enabled = true;

                SPPButton.Text = "计算完成";
                SPPButton.Enabled = false;
                label4.Visible = true;
                paintSPP_ResultButton.Enabled = true;
                recalculateButton.Enabled = true;

                string a = "基准站单点定位结果:" + "\n" + "X: " + SPP_Result.X.ToString() + " m" + "\n"
                    + "Y: " + SPP_Result.Y.ToString() + " m" + "\n" + "Z: " + SPP_Result.Z.ToString() + " m";
                richTextBox1.Text = a + "\n" + "相对定位结果:" + "\n" + "x~: " + RelativePositioning_Result.X.ToString() + " m" + "\n"
                    + "y~: " + RelativePositioning_Result.Y.ToString() + " m" + "\n" + "z~: " + RelativePositioning_Result.Z.ToString() + " m";
            }
            else
            {
                RPButton.Text = "计算失败";
                RPButton.Enabled = false;
                recalculateButton2.Enabled = true;
            }
        }

        private void recalculateButton2_Click(object sender, EventArgs e)
        {
            RelativePositioning_Result.X = 0;
            RelativePositioning_Result.Y = 0;
            RelativePositioning_Result.Z = 0;

            RPButton.ForeColor = Color.Black;
            RPButton.Text = "相对定位";
            RPButton.Enabled = true;
            recalculateButton2.Enabled = false;
            paintRP_ResultButton.Enabled = false;
            label5.Visible = false;

            richTextBox1.Text = null;
        }

        private void paintRP_ResultButton_Click(object sender, EventArgs e)
        {
            Paint2 paint2 = new Paint2();
            paint2.Show();
        }
    }
}
