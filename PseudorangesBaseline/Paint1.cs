using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PseudorangesBaseline
{
    public partial class Paint1 : Form
    {
        public Paint1()
        {
            InitializeComponent();
        }

        private void Paint1_Load(object sender, EventArgs e)
        {
            double[] x = new double[MasterReceiverPositionSum.receiverPositionSum.Count];
            double[] y = new double[MasterReceiverPositionSum.receiverPositionSum.Count];
            double[] z = new double[MasterReceiverPositionSum.receiverPositionSum.Count];


            for (int i = 0; i < MasterReceiverPositionSum.receiverPositionSum.Count; i++)
            {
                x[i] = MasterReceiverPositionSum.receiverPositionSum[i].X - MasterReceiverPositionSum.receiverPositionSum[0].X;
                y[i] = MasterReceiverPositionSum.receiverPositionSum[i].Y - MasterReceiverPositionSum.receiverPositionSum[0].Y;
                z[i] = MasterReceiverPositionSum.receiverPositionSum[i].Z - MasterReceiverPositionSum.receiverPositionSum[0].Z;
            }
            chart1.Series.Clear();
            Series series1 = new Series("X");
            Series series2 = new Series("Y");
            Series series3 = new Series("Z");
            //series1.Color = Color.Blue;
            //series2.Color = Color.Red;
            //series3.Color = Color.Green;
            series1.ChartType = SeriesChartType.FastLine;
            series2.ChartType = SeriesChartType.FastLine;
            series3.ChartType = SeriesChartType.FastLine;
            for (int i = 0; i < x.Length; i++)
            {
                series1.Points.AddY(x[i]);
                series2.Points.AddY(y[i]);
                series3.Points.AddY(z[i]);
            }
            chart1.Series.Add(series1);
            chart1.Series.Add(series2);
            chart1.Series.Add(series3);
        }
    }
}
