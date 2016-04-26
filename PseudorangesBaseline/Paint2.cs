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
    public partial class Paint2 : Form
    {
        public Paint2()
        {
            InitializeComponent();
        }

        private void Paint2_Load(object sender, EventArgs e)
        {
            double[] x = new double[BaselineResult.baselineResult.Count];
            double[] y = new double[BaselineResult.baselineResult.Count];
            double[] z = new double[BaselineResult.baselineResult.Count];

            for (int i = 0; i < BaselineResult.baselineResult.Count; i++)
            {
                x[i] = BaselineResult.baselineResult[i].X - BaselineResult.baselineResult[0].X;
                y[i] = BaselineResult.baselineResult[i].Y - BaselineResult.baselineResult[0].Y;
                z[i] = BaselineResult.baselineResult[i].Z - BaselineResult.baselineResult[0].Z;
            }

            chart1.Series.Clear();
            Series series1 = new Series("X");
            Series series2 = new Series("Y");
            Series series3 = new Series("Z");
            series1.ChartType = SeriesChartType.FastLine;
            series2.ChartType = SeriesChartType.FastLine;
            series3.ChartType = SeriesChartType.FastLine;

            series1.IsValueShownAsLabel = true;


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
