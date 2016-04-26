using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PseudorangesBaseline
{
    class Rover_O_FileReader
    {
        //Read_O_File
        public O_FileHead o_FileHead = new O_FileHead();
        public O_FileDataHead o_FileDataHead = new O_FileDataHead();
        public O_FileDataObs o_FileDataObs = new O_FileDataObs();
        public O_FileData o_FileData = new O_FileData();

        /// <summary>
        /// 读取流动站观测值文件
        /// </summary>
        public bool Read_O_File()
        {
            string path;//打开文件路径
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择流动站观测数据文件";
            ofd.Filter = "O文件|*.*o|所有文件|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
            }
            else
            {
                return false;
            }
            //清除
            O_FlieSum.rover_O_FileHeadSum = null;
            O_FlieSum.rover_O_FileDataSum.Clear();

            StreamReader o_FileReader = new StreamReader(path);
            string o_FileLine;//储存读到的每一行
            string o_FileLineJumpLine;//用于观测值类型多于5个时跳过一行
            o_FileLine = o_FileReader.ReadLine();
            /**************************************读取头文件********************************************/
            while (o_FileLine.Trim() != "END OF HEADER")
            {
                string tempStr = o_FileLine.Substring(60, o_FileLine.Length - 60);
                switch (tempStr.Trim())
                {
                    case "APPROX POSITION XYZ":
                        o_FileHead.Approx_Position.XX = double.Parse(o_FileLine.Substring(0, 14));
                        o_FileHead.Approx_Position.YY = double.Parse(o_FileLine.Substring(14, 14));
                        o_FileHead.Approx_Position.ZZ = double.Parse(o_FileLine.Substring(28, 14));
                        break;
                    case "ANTENNA: DELTA H/E/N":
                        o_FileHead.AntennaDeltaH = double.Parse(o_FileLine.Substring(0, 14));
                        o_FileHead.AntennaDeltaE = double.Parse(o_FileLine.Substring(14, 14));
                        o_FileHead.AntennaDeltaN = double.Parse(o_FileLine.Substring(28, 14));
                        break;
                    case "WAVELENGTH FACT L1/2":
                        o_FileHead.L1WaveLength = int.Parse(o_FileLine.Substring(0, 6));
                        o_FileHead.L2WaveLength = int.Parse(o_FileLine.Substring(6, 6));
                        break;
                    case "# / TYPES OF OBSERV":
                        o_FileHead.ObservDataTypeSum = int.Parse(o_FileLine.Substring(0, 6));
                        o_FileHead.TypeOfObserv = new string[o_FileHead.ObservDataTypeSum];
                        for (int i = 0; i < o_FileHead.ObservDataTypeSum; i++)
                        {
                            o_FileHead.TypeOfObserv[i] = o_FileLine.Substring(10 + i * 6, 2);
                        }//for
                        break;
                    case "INTERVAL":
                        o_FileHead.Interval = double.Parse(o_FileLine.Substring(0, 10));
                        break;
                    case "TIME OF FIRST OBS":
                        o_FileHead.TimeOfFirstObs.Year = int.Parse(o_FileLine.Substring(0, 6));
                        o_FileHead.TimeOfFirstObs.Month = int.Parse(o_FileLine.Substring(6, 6));
                        o_FileHead.TimeOfFirstObs.Day = int.Parse(o_FileLine.Substring(12, 6));
                        o_FileHead.TimeOfFirstObs.Hour = int.Parse(o_FileLine.Substring(18, 6));
                        o_FileHead.TimeOfFirstObs.Minute = int.Parse(o_FileLine.Substring(24, 6));
                        o_FileHead.TimeOfFirstObs.Second = double.Parse(o_FileLine.Substring(30, 13));
                        break;
                    case "TIME OF LAST OBS":
                        o_FileHead.TimeOfLastObs.Year = int.Parse(o_FileLine.Substring(0, 6));
                        o_FileHead.TimeOfLastObs.Month = int.Parse(o_FileLine.Substring(6, 6));
                        o_FileHead.TimeOfLastObs.Day = int.Parse(o_FileLine.Substring(12, 6));
                        o_FileHead.TimeOfLastObs.Hour = int.Parse(o_FileLine.Substring(18, 6));
                        o_FileHead.TimeOfLastObs.Minute = int.Parse(o_FileLine.Substring(24, 6));
                        o_FileHead.TimeOfLastObs.Second = double.Parse(o_FileLine.Substring(30, 13));
                        break;
                }//switch (tempStr)
                o_FileLine = o_FileReader.ReadLine();
            }//while (o_FileLine.Trim()!= "END OF HEADER")
            O_FlieSum.rover_O_FileHeadSum = o_FileHead;//读出的值存入O_FileSum

            /**************************************读取数据文件********************************************/
            o_FileLine = o_FileReader.ReadLine();
            while (o_FileLine != null)
            {
                //数据第一行
                o_FileDataHead.Epoch.Year = int.Parse(o_FileLine.Substring(1, 2)) + 2000;
                o_FileDataHead.Epoch.Month = int.Parse(o_FileLine.Substring(3, 3));
                o_FileDataHead.Epoch.Day = int.Parse(o_FileLine.Substring(6, 3));
                o_FileDataHead.Epoch.Hour = int.Parse(o_FileLine.Substring(9, 3));
                o_FileDataHead.Epoch.Minute = int.Parse(o_FileLine.Substring(12, 3));
                o_FileDataHead.Epoch.Second = double.Parse(o_FileLine.Substring(15, 11));
                o_FileDataHead.Epoch_Flag = int.Parse(o_FileLine.Substring(26, 3));
                o_FileDataHead.Sat_Num = int.Parse(o_FileLine.Substring(29, 3));
                o_FileDataHead.Sat_PRN = new int[o_FileDataHead.Sat_Num];
                for (int i = 0; i < o_FileDataHead.Sat_Num; i++)
                {
                    o_FileDataHead.Sat_PRN[i] = int.Parse(o_FileLine.Substring(33 + i * 3, 2));
                }
                o_FileData.o_FileDataHead = o_FileDataHead;//读出的值存入O_FileData

                //各颗卫星的数据
                o_FileData.o_FileDataObs = new O_FileDataObs[o_FileDataHead.Sat_Num];
                for (int i = 0; i < o_FileDataHead.Sat_Num; i++)
                {
                    o_FileDataObs.PRN = o_FileDataHead.Sat_PRN[i];
                    o_FileLine = o_FileReader.ReadLine();
                    if (o_FileHead.ObservDataTypeSum > 5)
                    {
                        o_FileLineJumpLine = o_FileReader.ReadLine();
                    }//观测值类型多于5个时，跳过一行
                    for (int j = 0; j < o_FileHead.ObservDataTypeSum; j++)
                    {
                        if (o_FileHead.TypeOfObserv[j] == "C1")
                        {
                            o_FileDataObs.C1 = double.Parse(o_FileLine.Substring(16 * j, 14));
                        }
                        //if (o_FileHead.TypeOfObserv[j] == "P1")
                        //{
                        //    o_FileDataObs.P1 = double.Parse(o_FileLine.Substring(16 * j, 14));
                        //}
                        //if (o_FileHead.TypeOfObserv[j] == "P2")
                        //{
                        //    o_FileDataObs.P2 = double.Parse(o_FileLine.Substring(16 * j, 14));
                        //}
                        //if (o_FileHead.TypeOfObserv[j] == "L1")
                        //{
                        //    o_FileDataObs.L1 = double.Parse(o_FileLine.Substring(16 * j, 14));
                        //}
                        //if (o_FileHead.TypeOfObserv[j] == "L2")
                        //{
                        //    o_FileDataObs.L2 = double.Parse(o_FileLine.Substring(16 * j, 14));
                        //}
                    }//for j
                    o_FileData.o_FileDataObs[i] = o_FileDataObs;//读出的值存入O_FileData
                    o_FileDataObs = new O_FileDataObs();
                }//for i
                O_FlieSum.rover_O_FileDataSum.Add(o_FileData);//全部读出的值存入O_FileSum
                o_FileDataHead = new O_FileDataHead();
                o_FileData = new O_FileData();
                o_FileLine = o_FileReader.ReadLine();
            }//while (o_FileLine != null)
            return true;
        }
    }
}
