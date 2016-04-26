using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PseudorangesBaseline
{
    class N_FileReader
    {
        //Read_N_File
        public N_FlieHead n_FileHead = new N_FlieHead();
        public N_FileData n_FileData = new N_FileData();

        /// <summary>
        /// 读取N文件
        /// </summary>
        public bool Read_N_File()
        {
            string path;//打开文件路径
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择导航电文文件";
            ofd.Filter = "N文件|*.*n|所有文件|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
            }
            else
            {
                return false;
            }
            //清除
            N_FileSum.n_FileHeadSum = null;
            N_FileSum.n_FileDataSum.Clear();


            StreamReader n_FileReader = new StreamReader(path);
            string n_FileLine;//储存读到的每一行
            n_FileLine = n_FileReader.ReadLine();
            while (n_FileLine != null)
            {
                /**************************************读取头文件********************************************/
                while (n_FileLine.Trim() != "END OF HEADER")
                {
                    string tempStr = n_FileLine.Substring(60, n_FileLine.Length - 60);//用于判断内容的部分
                    switch (tempStr.Trim())
                    {
                        case "ION ALPHA":
                            n_FileHead.Ion_Alpha = new double[4];
                            n_FileHead.Ion_Alpha[0] = double.Parse(n_FileLine.Substring(2, 8)) * Math.Pow(10, double.Parse(n_FileLine.Substring(11, 3)));
                            n_FileHead.Ion_Alpha[1] = double.Parse(n_FileLine.Substring(14, 8)) * Math.Pow(10, double.Parse(n_FileLine.Substring(23, 3)));
                            n_FileHead.Ion_Alpha[2] = double.Parse(n_FileLine.Substring(26, 8)) * Math.Pow(10, double.Parse(n_FileLine.Substring(35, 3)));
                            n_FileHead.Ion_Alpha[3] = double.Parse(n_FileLine.Substring(38, 8)) * Math.Pow(10, double.Parse(n_FileLine.Substring(47, 3)));
                            break;
                        case "ION BETA":
                            n_FileHead.Ion_Beta = new double[4];
                            n_FileHead.Ion_Beta[0] = double.Parse(n_FileLine.Substring(2, 8)) * Math.Pow(10, double.Parse(n_FileLine.Substring(11, 3)));
                            n_FileHead.Ion_Beta[1] = double.Parse(n_FileLine.Substring(14, 8)) * Math.Pow(10, double.Parse(n_FileLine.Substring(23, 3)));
                            n_FileHead.Ion_Beta[2] = double.Parse(n_FileLine.Substring(26, 8)) * Math.Pow(10, double.Parse(n_FileLine.Substring(35, 3)));
                            n_FileHead.Ion_Beta[3] = double.Parse(n_FileLine.Substring(38, 8)) * Math.Pow(10, double.Parse(n_FileLine.Substring(47, 3)));
                            break;
                        case "DELTA-UTC: A0,A1,T,W":
                            n_FileHead.A0 = double.Parse(n_FileLine.Substring(3, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(19, 3)));
                            n_FileHead.A1 = double.Parse(n_FileLine.Substring(22, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(38, 3)));
                            n_FileHead.T = int.Parse(n_FileLine.Substring(41, 9));
                            n_FileHead.W = int.Parse(n_FileLine.Substring(50, 9));
                            break;
                        case "LEAP SECONDS":
                            n_FileHead.Leap_Seconds = int.Parse(n_FileLine.Substring(0, 6));
                            break;
                    }//swich
                    n_FileLine = n_FileReader.ReadLine();
                } //while (n_FileLine != "END OF HEADER")
                N_FileSum.n_FileHeadSum = n_FileHead;//读出的值存入N_FileSum

                /**************************************读取数据文件********************************************/
                n_FileLine = n_FileReader.ReadLine();
                int i = 0;
                while (n_FileLine != null)
                {
                    switch (i)
                    {
                        case 0:
                            n_FileData.PRN = int.Parse(n_FileLine.Substring(0, 2).Trim());
                            n_FileData.TOC.Year = int.Parse(n_FileLine.Substring(3, 2)) + 2000;
                            n_FileData.TOC.Month = int.Parse(n_FileLine.Substring(5, 3));
                            n_FileData.TOC.Day = int.Parse(n_FileLine.Substring(8, 3));
                            n_FileData.TOC.Hour = int.Parse(n_FileLine.Substring(11, 3));
                            n_FileData.TOC.Minute = int.Parse(n_FileLine.Substring(14, 3));
                            n_FileData.TOC.Second = double.Parse(n_FileLine.Substring(17, 5));
                            n_FileData.ClkBias = double.Parse(n_FileLine.Substring(22, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(38, 3)));
                            n_FileData.ClkDrift = double.Parse(n_FileLine.Substring(41, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(57, 3)));
                            n_FileData.ClkDriftRate = double.Parse(n_FileLine.Substring(60, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(76, 3)));
                            break;
                        case 1:
                            n_FileData.IODE = double.Parse(n_FileLine.Substring(3, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(19, 3)));
                            n_FileData.Crs = double.Parse(n_FileLine.Substring(22, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(38, 3)));
                            n_FileData.DetlaN = double.Parse(n_FileLine.Substring(41, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(57, 3)));
                            n_FileData.M0 = double.Parse(n_FileLine.Substring(60, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(76, 3)));
                            break;
                        case 2:
                            n_FileData.Cuc = double.Parse(n_FileLine.Substring(3, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(19, 3)));
                            n_FileData.E = double.Parse(n_FileLine.Substring(22, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(38, 3)));
                            n_FileData.Cus = double.Parse(n_FileLine.Substring(41, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(57, 3)));
                            n_FileData.SqrtA = double.Parse(n_FileLine.Substring(60, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(76, 3)));
                            break;
                        case 3:
                            n_FileData.TOE = double.Parse(n_FileLine.Substring(3, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(19, 3)));
                            n_FileData.Cic = double.Parse(n_FileLine.Substring(22, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(38, 3)));
                            n_FileData.Omega = double.Parse(n_FileLine.Substring(41, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(57, 3)));
                            n_FileData.Cis = double.Parse(n_FileLine.Substring(60, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(76, 3)));
                            break;
                        case 4:
                            n_FileData.I0 = double.Parse(n_FileLine.Substring(3, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(19, 3)));
                            n_FileData.Crc = double.Parse(n_FileLine.Substring(22, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(38, 3)));
                            n_FileData.OmegaLow = double.Parse(n_FileLine.Substring(41, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(57, 3)));
                            n_FileData.OmegaDot = double.Parse(n_FileLine.Substring(60, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(76, 3)));
                            break;
                        case 5:
                            n_FileData.IDot = double.Parse(n_FileLine.Substring(3, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(19, 3)));
                            n_FileData.CodesOnL2Channel = double.Parse(n_FileLine.Substring(22, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(38, 3)));
                            n_FileData.GPSWeek = double.Parse(n_FileLine.Substring(41, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(57, 3)));
                            n_FileData.L2PDataFlag = double.Parse(n_FileLine.Substring(60, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(76, 3)));
                            break;
                        case 6:
                            n_FileData.SVAccuracy = double.Parse(n_FileLine.Substring(3, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(19, 3)));
                            n_FileData.SVHealth = double.Parse(n_FileLine.Substring(22, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(38, 3)));
                            n_FileData.TGD = double.Parse(n_FileLine.Substring(41, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(57, 3)));
                            n_FileData.IODC = double.Parse(n_FileLine.Substring(60, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(76, 3)));
                            break;
                        case 7:
                            n_FileData.TransTimeOfMsg = double.Parse(n_FileLine.Substring(3, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(19, 3)));
                            //n_FileData.Spare1 = double.Parse(n_FileLine.Substring(22, 15)) * Math.Pow(10, double.Parse(n_FileLine.Substring(38, 3)));//有的文件没有这个数据，会报错。
                            break;
                    }//switch (i)
                    i = i + 1;
                    i = i % 8;
                    if (i == 0)
                    {
                        N_FileSum.n_FileDataSum.Add(n_FileData);//读出的值存入N_FileSum
                        n_FileData = new N_FileData();
                    }
                    n_FileLine = n_FileReader.ReadLine();
                }//while (n_FileLine != null)
            }//while (n_FileLine != null)
            return true;
        }
    }
}
