using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PseudorangesBaseline
{
    class Baseline
    {
        public MasterReceiverPosition masterReceiverPosition = new MasterReceiverPosition();
        public RoverReceiverPosition roverReceiverPosition = new RoverReceiverPosition();
        public BaseOneEpo baseOneEpo = new BaseOneEpo();

        /// <summary>
        /// 通用时转换为GPS时
        /// </summary>
        /// <param name="time">通用时</param>
        /// <returns>GPS时</returns>
        public GPSTime TimeToGPSTime(Time time)
        {
            GPSTime gpsTime = new GPSTime();
            double JD, UT;
            int y, m;
            UT = time.Hour + (time.Minute / 60.0) + (time.Second / 3600.0);
            if (time.Month <= 2)
            {
                y = time.Year - 1;
                m = time.Month + 12;
            }
            else
            {
                y = time.Year;
                m = time.Month;
            }
            JD = (int)(365.25 * y) + (int)(30.6001 * (m + 1)) + time.Day + (UT / 24) + 1720981.5;
            gpsTime.GPSWeek = (int)((JD - 2444244.5) / 7);
            gpsTime.GPSSecond = (JD - 2444244.5) * 3600 * 24 - gpsTime.GPSWeek * 3600 * 24 * 7;
            return gpsTime;
        }

        /// <summary>
        /// 根据O_FileDataHead中的卫星PRN和TOC找出N_FileData中对应的星历序号
        /// </summary>
        /// <param name="sat_PRN">O_FileDataHead中的卫星PRN</param>
        /// <param name="epoch">O_FileDataHead中的TOC</param>
        /// <returns>N_FileData中对应的星历序号</returns>
        public int FindBestEph(int sat_PRN, Time epoch)
        {
            GPSTime oTime = TimeToGPSTime(epoch);
            for (int i = 0; i < N_FileSum.n_FileDataSum.Count; i++)
            {
                if (sat_PRN == N_FileSum.n_FileDataSum[i].PRN)
                {
                    GPSTime nTime = TimeToGPSTime(N_FileSum.n_FileDataSum[i].TOC);
                    if (Math.Abs(oTime.GPSSecond - nTime.GPSSecond) < 3600.0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 计算卫星在信号发射时的位置
        /// </summary>
        /// <param name="n_FileData">星历数据</param>
        /// <param name="satSendTime">卫星信号发射的时间</param>
        /// <returns>卫星在地心坐标系中的空间直角坐标</returns>
        public Position SatellitePosition(N_FileData n_FileData, GPSTime satSendTime)
        {
            Position satPosition = new Position();
            double A, n0, tk, n, Mk, Ek, E0, f, uu, omega_uk, omega_rk, omega_ik, uk, rk, ik, xk, yk, Lk;

            A = Math.Pow(n_FileData.SqrtA, 2);//轨道长半轴
            n0 = Math.Sqrt((Constant.GM / Math.Pow(A, 3)));
            tk = satSendTime.GPSWeek * 604800 + satSendTime.GPSSecond - n_FileData.GPSWeek * 604800 - n_FileData.TOE;
            if (tk > 302400)
            {
                tk = tk - 604800;
            }
            else if (tk < -302400)
            {
                tk = tk + 604800;
            }
            n = n0 + n_FileData.DetlaN;
            Mk = n_FileData.M0 + n * tk;
            E0 = Mk;
            Ek = Mk + n_FileData.E * Math.Sin(E0);
            while (Math.Abs(Ek - E0) > Math.Pow(10, -12))
            {
                E0 = Ek;
                Ek = Mk + n_FileData.E * Math.Sin(E0);
            }
            f = Math.Atan2((Math.Sqrt(1 - Math.Pow(n_FileData.E, 2))) * Math.Sin(Ek), Math.Cos(Ek) - n_FileData.E);
            uu = f + n_FileData.OmegaLow;
            omega_uk = n_FileData.Cus * Math.Sin(2 * uu) + n_FileData.Cuc * Math.Cos(2 * uu);
            omega_rk = n_FileData.Crs * Math.Sin(2 * uu) + n_FileData.Crc * Math.Cos(2 * uu);
            omega_ik = n_FileData.Cis * Math.Sin(2 * uu) + n_FileData.Cic * Math.Cos(2 * uu);
            uk = uu + omega_uk;
            rk = A * (1 - n_FileData.E * Math.Cos(Ek)) + omega_rk;
            ik = n_FileData.I0 + omega_ik + n_FileData.IDot * tk;
            xk = rk * Math.Cos(uk);
            yk = rk * Math.Sin(uk);
            Lk = n_FileData.Omega + (n_FileData.OmegaDot - Constant.OmegaDotE) * tk - Constant.OmegaDotE * n_FileData.TOE;
            satPosition.XX = xk * Math.Cos(Lk) - yk * Math.Cos(ik) * Math.Sin(Lk);
            satPosition.YY = xk * Math.Sin(Lk) + yk * Math.Cos(ik) * Math.Cos(Lk);
            satPosition.ZZ = yk * Math.Sin(ik);
            return satPosition;
        }

        /// <summary>
        /// 地球自转改正
        /// </summary>
        /// <param name="deltaT">信号传播时间</param>
        /// <param name="satPosition">卫星在信号发射时刻的位置</param>
        /// <returns>改正后的卫星位置</returns>
        public Position EarthRotationCorrection(double deltaT, Position satPosition)
        {
            double[,] R = new double[3, 3];
            double[] XYZ = new double[3];
            double[] XXYYZZ = new double[3];
            Position satPosCorrection = new Position();
            R[0, 0] = Math.Cos(Constant.OmegaDotE * deltaT);
            R[0, 1] = Math.Sin(Constant.OmegaDotE * deltaT);
            R[0, 2] = 0.0;
            R[1, 0] = -Math.Sin(Constant.OmegaDotE * deltaT);
            R[1, 1] = Math.Cos(Constant.OmegaDotE * deltaT);
            R[1, 2] = 0.0;
            R[2, 0] = 0.0;
            R[2, 1] = 0.0;
            R[2, 2] = 1.0;
            XYZ[0] = satPosition.XX;
            XYZ[1] = satPosition.YY;
            XYZ[2] = satPosition.ZZ;
            for (int i = 0; i < 3; i++)
            {
                XXYYZZ[i] = 0;
                for (int j = 0; j < 3; j++)
                {
                    XXYYZZ[i] = XXYYZZ[i] + R[i, j] * XYZ[j];
                }
            }
            satPosCorrection.XX = XXYYZZ[0];
            satPosCorrection.YY = XXYYZZ[1];
            satPosCorrection.ZZ = XXYYZZ[2];
            return satPosCorrection;
        }

        public bool SinglePointPositioning()
        {
            MasterReceiverPositionSum.receiverPositionSum.Clear();
            OneEpochData oneEpochData = new OneEpochData();
            List<OneEpochData> oneEpochDataSum = new List<OneEpochData>();

            GPSTime TR = new GPSTime();//历元时刻
            double x, y, z, cdtr;//测站位置和钟差
            double xx = 0;
            double yy = 0;
            double zz = 0;//用于储存每个历元的定位结果，用于赋值给下一历元的初值
            double deltaT0Si;//卫星信号传播时间
            double dtSi;//卫星钟差
            double deltaT1Si;
            double rho;//伪距
            GPSTime TSi = new GPSTime();//卫星信号发射概略时刻
            Position XSi = new Position();//卫星在TSi时刻的位置
            Position XSiW = new Position();//卫星自转改正后的位置
            GPSTime T1Si = new GPSTime();//卫星信号发射时刻
            double RSi;//测站和卫星的几何距离

            double b0Si, b1Si, b2Si, b3Si;//卫星方向余弦
            double ISi;//卫星在观测方程中的余数项

            double[,] A;
            double[,] AT;
            double[,] ATA = new double[4, 4];
            double[,] revATA = new double[4, 4];
            double[,] L;
            double[,] ATL = new double[4, 1];
            double[,] xi;

            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            double sumCdtr = 0;

            if (N_FileSum.n_FileDataSum.Count == 0)
            {
                MessageBox.Show("没有打开导航电文文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (O_FlieSum.master_O_FileDataSum.Count == 0)
            {
                MessageBox.Show("没有打开基准站观测数据文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            for (int i = 0; i < O_FlieSum.master_O_FileDataSum.Count; i++)
            {
                x = xx;
                y = yy;
                z = zz;//每个历元初值等于上个历元的定位结果，第一个历元为0
                cdtr = 0;//第一次循环为0
                xi = new double[4, 1];

                do//解算一个历元
                {
                    x = x + xi[0, 0];//第一次改正数为0，以后每次循环将改正数累加。
                    y = y + xi[1, 0];
                    z = z + xi[2, 0];
                    cdtr = cdtr + xi[3, 0];

                    for (int j = 0; j < O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num; j++)//循环一个历元中的全部卫星
                    {
                        TR = TimeToGPSTime(O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Epoch);//O_FileDataHead中的历元时刻转换为GPS时

                        int prnNum = FindBestEph(O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_PRN[j], O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Epoch);
                        if (prnNum == -1)
                        {
                            MessageBox.Show("卫星{0}没有对应的星历文件", O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_PRN[j].ToString());
                        }

                        rho = O_FlieSum.master_O_FileDataSum[i].o_FileDataObs[j].C1;


                        GPSTime gpsTimeTOC = TimeToGPSTime(N_FileSum.n_FileDataSum[prnNum].TOC);
                        dtSi = N_FileSum.n_FileDataSum[prnNum].ClkBias + N_FileSum.n_FileDataSum[prnNum].ClkDrift * (TR.GPSSecond - gpsTimeTOC.GPSSecond) + N_FileSum.n_FileDataSum[prnNum].ClkDriftRate * Math.Pow((TR.GPSSecond - gpsTimeTOC.GPSSecond), 2);
                        deltaT1Si = (rho / Constant.SpeedOfLight) + N_FileSum.n_FileDataSum[prnNum].ClkBias + N_FileSum.n_FileDataSum[prnNum].ClkDrift * (TR.GPSSecond - gpsTimeTOC.GPSSecond) + N_FileSum.n_FileDataSum[prnNum].ClkDriftRate * Math.Pow((TR.GPSSecond - gpsTimeTOC.GPSSecond), 2) - cdtr / Constant.SpeedOfLight;
                        do
                        {

                            deltaT0Si = deltaT1Si;
                            TSi.GPSSecond = TR.GPSSecond - deltaT0Si;
                            TSi.GPSWeek = TR.GPSWeek;
                            XSi = SatellitePosition(N_FileSum.n_FileDataSum[prnNum], TSi);
                            XSiW = EarthRotationCorrection(deltaT0Si, XSi);
                            RSi = Math.Sqrt(Math.Pow(XSiW.XX - (O_FlieSum.master_O_FileHeadSum.Approx_Position.XX + x), 2) + Math.Pow(XSiW.YY - (O_FlieSum.master_O_FileHeadSum.Approx_Position.YY + y), 2) + Math.Pow(XSiW.ZZ - (O_FlieSum.master_O_FileHeadSum.Approx_Position.ZZ + z), 2));
                            deltaT1Si = RSi / Constant.SpeedOfLight;
                        } while (Math.Abs(deltaT1Si - deltaT0Si) > Math.Pow(10, -7));
                        T1Si.GPSSecond = TR.GPSSecond - deltaT1Si;
                        T1Si.GPSWeek = TR.GPSWeek;

                        b0Si = (O_FlieSum.master_O_FileHeadSum.Approx_Position.XX + x - XSiW.XX) / rho;
                        b1Si = (O_FlieSum.master_O_FileHeadSum.Approx_Position.YY + y - XSiW.YY) / rho;
                        b2Si = (O_FlieSum.master_O_FileHeadSum.Approx_Position.ZZ + z - XSiW.ZZ) / rho;
                        b3Si = 1.0;
                        ISi = rho - RSi + dtSi * Constant.SpeedOfLight - cdtr;

                        oneEpochData.b0S = b0Si;
                        oneEpochData.b1S = b1Si;
                        oneEpochData.b2S = b2Si;
                        oneEpochData.b3S = b3Si;
                        oneEpochData.IS = ISi;
                        oneEpochDataSum.Add(oneEpochData);
                        oneEpochData = new OneEpochData();
                    }//for j

                    A = new double[oneEpochDataSum.Count, 4];
                    L = new double[oneEpochDataSum.Count, 1];
                    AT = new double[4, oneEpochDataSum.Count];
                    for (int k = 0; k < oneEpochDataSum.Count; k++)
                    {
                        A[k, 0] = oneEpochDataSum[k].b0S;
                        A[k, 1] = oneEpochDataSum[k].b1S;
                        A[k, 2] = oneEpochDataSum[k].b2S;
                        A[k, 3] = oneEpochDataSum[k].b3S;
                        L[k, 0] = oneEpochDataSum[k].IS;
                    }
                    AT = Matrix.Transfer(A, oneEpochDataSum.Count, 4);
                    ATA = Matrix.Multiply(AT, A, 4, oneEpochDataSum.Count, 4);
                    revATA = Matrix.MatrixOpp(ATA);
                    ATL = Matrix.Multiply(AT, L, 4, oneEpochDataSum.Count, 1);
                    xi = Matrix.Multiply(revATA, ATL, 4, 4, 1);
                    oneEpochDataSum.Clear();
                } while (Math.Abs(xi[0, 0]) > 0.001 && Math.Abs(xi[1, 0]) > 0.001 && Math.Abs(xi[2, 0]) > 0.001);
                xx = x + xi[0, 0];//之前全部的改正加最后一次循环的改正数，这个历元的最终结果
                yy = y + xi[1, 0];
                zz = z + xi[2, 0];

                masterReceiverPosition.X = O_FlieSum.master_O_FileHeadSum.Approx_Position.XX + xx;
                masterReceiverPosition.Y = O_FlieSum.master_O_FileHeadSum.Approx_Position.YY + yy;
                masterReceiverPosition.Z = O_FlieSum.master_O_FileHeadSum.Approx_Position.ZZ + zz;
                masterReceiverPosition.Cdtr = (cdtr + xi[3, 0]) / Constant.SpeedOfLight;

                MasterReceiverPositionSum.receiverPositionSum.Add(masterReceiverPosition);
                masterReceiverPosition = new MasterReceiverPosition();
                oneEpochDataSum.Clear();
            }//for i

            for (int i = 0; i < MasterReceiverPositionSum.receiverPositionSum.Count; i++)
            {
                sumX = sumX + MasterReceiverPositionSum.receiverPositionSum[i].X;
                sumY = sumY + MasterReceiverPositionSum.receiverPositionSum[i].Y;
                sumZ = sumZ + MasterReceiverPositionSum.receiverPositionSum[i].Z;
                sumCdtr = sumCdtr + MasterReceiverPositionSum.receiverPositionSum[i].Cdtr;
            }
            SPP_Result.X = sumX / MasterReceiverPositionSum.receiverPositionSum.Count;
            SPP_Result.Y = sumY / MasterReceiverPositionSum.receiverPositionSum.Count;
            SPP_Result.Z = sumZ / MasterReceiverPositionSum.receiverPositionSum.Count;
            SPP_Result.Cdtr = sumCdtr / MasterReceiverPositionSum.receiverPositionSum.Count;



            return true;
        }

        public bool RelativePositioning()
        {
            BaselineResult.baselineResult.Clear();

            BaselineOneEpo baselineOneEpo = new BaselineOneEpo();
            List<BaselineOneEpo> baselineOneEpoSum = new List<BaselineOneEpo>();

            GPSTime time_Master = new GPSTime();//基准站历元时刻
            GPSTime time_Rover = new GPSTime();//移动站历元时刻



            double deltaT0Si_Master;//卫星信号传播时间
            double deltaT1Si_Master;
            double deltaT0Si_Rover;//卫星信号传播时间
            double deltaT1Si_Rover;
            GPSTime TSi_Master = new GPSTime();//卫星信号发射概略时刻
            Position XSi_Master = new Position();//卫星在TSi时刻的位置
            Position XSiW_Master1 = new Position();//卫星自转改正后的位置
            Position XSiW_Master = new Position();
            GPSTime TSi_Rover = new GPSTime();//卫星信号发射概略时刻
            Position XSi_Rover = new Position();//卫星在TSi时刻的位置
            Position XSiW_Rover = new Position();//卫星自转改正后的位置
            double RSi_Master;//测站和卫星的几何距离
            double RSi_Rover;//测站和卫星的几何距离
            double rho_Master1;//参考卫星伪距
            double rho_Rover1;
            double rho_Master;//历元内剩余卫星伪距
            double rho_Rover;

            double li, mi, ni, omci;

            double[,] A;
            double[,] AT;
            double[,] D;
            double[,] DT;
            double[,] DDT;
            double[,] C;
            double[,] omc;
            double[,] ATC;
            double[,] ATCA;
            double[,] ATComc;
            double[,] invATCA;
            double[,] x;

            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;


            
            if (N_FileSum.n_FileDataSum.Count == 0)
            {
                MessageBox.Show("没有打开导航电文文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (O_FlieSum.master_O_FileDataSum.Count == 0)
            {
                MessageBox.Show("没有打开基准站观测数据文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (O_FlieSum.rover_O_FileDataSum.Count == 0)
            {
                MessageBox.Show("没有打开移动站观测数据文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //****************如果发现没有进行基准站单点定位，则首先进行单点定位****************//
            if (SPP_Result.X == 0 && SPP_Result.Y == 0 && SPP_Result.Z == 0 && SPP_Result.Cdtr == 0)
            {
                SinglePointPositioning();
            }
            //****************如果发现没有进行基准站单点定位，则首先进行单点定位****************//


            double xx = 0;
            double yy = 0;
            double zz = 0;

            for (int i = 0; i < O_FlieSum.master_O_FileDataSum.Count; i++)
            {
                time_Master = TimeToGPSTime(O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Epoch);
                time_Rover = TimeToGPSTime(O_FlieSum.rover_O_FileDataSum[i].o_FileDataHead.Epoch);
                if (time_Master.GPSSecond != time_Rover.GPSSecond)
                {
                    MessageBox.Show("观测历元不匹配", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //以基准站的卫星序列为顺序基准，查找找共视卫星，并将流动站卫星重新排序
                int[] rover_PRN = new int[O_FlieSum.rover_O_FileDataSum[i].o_FileDataHead.Sat_Num];
                double[] rover_C1 = new double[O_FlieSum.rover_O_FileDataSum[i].o_FileDataHead.Sat_Num];
                for (int sat_Num = 0; sat_Num < O_FlieSum.rover_O_FileDataSum[i].o_FileDataHead.Sat_Num; sat_Num++)
                {
                    for (int prn = 0; prn < O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num; prn++)
                    {
                        if (O_FlieSum.rover_O_FileDataSum[i].o_FileDataObs[sat_Num].PRN == O_FlieSum.master_O_FileDataSum[i].o_FileDataObs[prn].PRN)
                        {
                            rover_PRN[prn] = O_FlieSum.rover_O_FileDataSum[i].o_FileDataObs[sat_Num].PRN;
                            rover_C1[prn] = O_FlieSum.rover_O_FileDataSum[i].o_FileDataObs[sat_Num].C1;
                            break;
                        }
                    }
                }
                for (int ii = 0; ii < rover_PRN.Length; ii++)
                {
                    O_FlieSum.rover_O_FileDataSum[i].o_FileDataObs[ii].PRN = rover_PRN[ii];
                    O_FlieSum.rover_O_FileDataSum[i].o_FileDataObs[ii].C1 = rover_C1[ii];
                }

                //************下一历元初值等于上一历元计算结果************//
                roverReceiverPosition.X = SPP_Result.X + xx;
                roverReceiverPosition.Y = SPP_Result.Y + yy;
                roverReceiverPosition.Z = SPP_Result.Z + zz;
                //************下一历元初值等于上一历元计算结果************//

                for (int iteration = 0; iteration < 8; iteration++)//迭代8次
                {
                    //第一颗卫星为参考卫星
                    rho_Master1 = O_FlieSum.master_O_FileDataSum[i].o_FileDataObs[0].C1;
                    rho_Rover1 = O_FlieSum.rover_O_FileDataSum[i].o_FileDataObs[0].C1;
                    int prnNum1 = FindBestEph(O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_PRN[0], O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Epoch);
                    GPSTime gpsTimeTOC = TimeToGPSTime(N_FileSum.n_FileDataSum[prnNum1].TOC);
                    deltaT1Si_Master = (rho_Master1 / Constant.SpeedOfLight) + N_FileSum.n_FileDataSum[prnNum1].ClkBias + N_FileSum.n_FileDataSum[prnNum1].ClkDrift * (time_Master.GPSSecond - gpsTimeTOC.GPSSecond) + N_FileSum.n_FileDataSum[prnNum1].ClkDriftRate * Math.Pow((time_Master.GPSSecond - gpsTimeTOC.GPSSecond), 2);
                    do
                    {
                        deltaT0Si_Master = deltaT1Si_Master;
                        TSi_Master.GPSSecond = time_Master.GPSSecond - deltaT0Si_Master;
                        TSi_Master.GPSWeek = time_Master.GPSWeek;
                        XSi_Master = SatellitePosition(N_FileSum.n_FileDataSum[prnNum1], TSi_Master);
                        XSiW_Master1 = EarthRotationCorrection(deltaT0Si_Master, XSi_Master);
                        RSi_Master = Math.Sqrt(Math.Pow(XSiW_Master1.XX - SPP_Result.X, 2) + Math.Pow(XSiW_Master1.YY - SPP_Result.Y, 2) + Math.Pow(XSiW_Master1.ZZ - SPP_Result.Z, 2));
                        deltaT1Si_Master = RSi_Master / Constant.SpeedOfLight;
                    } while (Math.Abs(deltaT1Si_Master - deltaT0Si_Master) > Math.Pow(10, -7));
                    deltaT1Si_Rover = (rho_Rover1 / Constant.SpeedOfLight) + N_FileSum.n_FileDataSum[prnNum1].ClkBias + N_FileSum.n_FileDataSum[prnNum1].ClkDrift * (time_Master.GPSSecond - gpsTimeTOC.GPSSecond) + N_FileSum.n_FileDataSum[prnNum1].ClkDriftRate * Math.Pow((time_Master.GPSSecond - gpsTimeTOC.GPSSecond), 2);
                    do
                    {
                        deltaT0Si_Rover = deltaT1Si_Rover;
                        TSi_Rover.GPSSecond = time_Master.GPSSecond - deltaT0Si_Rover;
                        TSi_Rover.GPSWeek = time_Master.GPSWeek;
                        XSi_Rover = SatellitePosition(N_FileSum.n_FileDataSum[prnNum1], TSi_Rover);
                        XSiW_Rover = EarthRotationCorrection(deltaT0Si_Rover, XSi_Rover);
                        RSi_Rover = Math.Sqrt(Math.Pow(XSiW_Rover.XX - roverReceiverPosition.X, 2) + Math.Pow(XSiW_Rover.YY - roverReceiverPosition.Y, 2) + Math.Pow(XSiW_Rover.ZZ - roverReceiverPosition.Z, 2));
                        deltaT1Si_Rover = RSi_Rover / Constant.SpeedOfLight;
                    } while (Math.Abs(deltaT1Si_Rover - deltaT0Si_Rover) > Math.Pow(10, -7));
                    rho_Master1 = RSi_Master;
                    rho_Rover1 = RSi_Rover;


                    //历元内剩余卫星
                    for (int sat = 1; sat < O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num; sat++)//循环历元内剩余卫星
                    {
                        rho_Master = O_FlieSum.master_O_FileDataSum[i].o_FileDataObs[sat].C1;
                        rho_Rover = O_FlieSum.rover_O_FileDataSum[i].o_FileDataObs[sat].C1;
                        int prnNum = FindBestEph(O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_PRN[sat], O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Epoch);
                        gpsTimeTOC = TimeToGPSTime(N_FileSum.n_FileDataSum[prnNum].TOC);
                        deltaT1Si_Master = (rho_Master / Constant.SpeedOfLight) + N_FileSum.n_FileDataSum[prnNum].ClkBias + N_FileSum.n_FileDataSum[prnNum].ClkDrift * (time_Master.GPSSecond - gpsTimeTOC.GPSSecond) + N_FileSum.n_FileDataSum[prnNum].ClkDriftRate * Math.Pow((time_Master.GPSSecond - gpsTimeTOC.GPSSecond), 2);
                        do
                        {
                            deltaT0Si_Master = deltaT1Si_Master;
                            TSi_Master.GPSSecond = time_Master.GPSSecond - deltaT0Si_Master;
                            TSi_Master.GPSWeek = time_Master.GPSWeek;
                            XSi_Master = SatellitePosition(N_FileSum.n_FileDataSum[prnNum], TSi_Master);
                            XSiW_Master = EarthRotationCorrection(deltaT0Si_Master, XSi_Master);
                            RSi_Master = Math.Sqrt(Math.Pow(XSiW_Master.XX - SPP_Result.X, 2) + Math.Pow(XSiW_Master.YY - SPP_Result.Y, 2) + Math.Pow(XSiW_Master.ZZ - SPP_Result.Z, 2));
                            deltaT1Si_Master = RSi_Master / Constant.SpeedOfLight;
                        } while (Math.Abs(deltaT1Si_Master - deltaT0Si_Master) > Math.Pow(10, -7));
                        deltaT1Si_Rover = (rho_Rover / Constant.SpeedOfLight) + N_FileSum.n_FileDataSum[prnNum].ClkBias + N_FileSum.n_FileDataSum[prnNum].ClkDrift * (time_Master.GPSSecond - gpsTimeTOC.GPSSecond) + N_FileSum.n_FileDataSum[prnNum].ClkDriftRate * Math.Pow((time_Master.GPSSecond - gpsTimeTOC.GPSSecond), 2);
                        do
                        {
                            deltaT0Si_Rover = deltaT1Si_Rover;
                            TSi_Rover.GPSSecond = time_Master.GPSSecond - deltaT0Si_Rover;
                            TSi_Rover.GPSWeek = time_Master.GPSWeek;
                            XSi_Rover = SatellitePosition(N_FileSum.n_FileDataSum[prnNum], TSi_Rover);
                            XSiW_Rover = EarthRotationCorrection(deltaT0Si_Rover, XSi_Rover);
                            RSi_Rover = Math.Sqrt(Math.Pow(XSiW_Rover.XX - roverReceiverPosition.X, 2) + Math.Pow(XSiW_Rover.YY - roverReceiverPosition.Y, 2) + Math.Pow(XSiW_Rover.ZZ - roverReceiverPosition.Z, 2));
                            deltaT1Si_Rover = RSi_Rover / Constant.SpeedOfLight;
                        } while (Math.Abs(deltaT1Si_Rover - deltaT0Si_Rover) > Math.Pow(10, -7));
                        rho_Master = RSi_Master;
                        rho_Rover = RSi_Rover;


                        li = ((XSiW_Master1.XX - roverReceiverPosition.X) / rho_Rover1) - ((XSiW_Master.XX - roverReceiverPosition.X) / rho_Rover);
                        mi = ((XSiW_Master1.YY - roverReceiverPosition.Y) / rho_Rover1) - ((XSiW_Master.YY - roverReceiverPosition.Y) / rho_Rover);
                        ni = ((XSiW_Master1.ZZ - roverReceiverPosition.Z) / rho_Rover1) - ((XSiW_Master.ZZ - roverReceiverPosition.Z) / rho_Rover);
                        double observed = O_FlieSum.master_O_FileDataSum[i].o_FileDataObs[0].C1 - O_FlieSum.rover_O_FileDataSum[i].o_FileDataObs[0].C1 - O_FlieSum.master_O_FileDataSum[i].o_FileDataObs[sat].C1 + O_FlieSum.rover_O_FileDataSum[i].o_FileDataObs[sat].C1;
                        double calculated = rho_Master1 - rho_Rover1 - rho_Master + rho_Rover;
                        omci = observed - calculated;

                        baselineOneEpo.l = li;
                        baselineOneEpo.m = mi;
                        baselineOneEpo.n = ni;
                        baselineOneEpo.omc = omci;
                        baselineOneEpoSum.Add(baselineOneEpo);
                        baselineOneEpo = new BaselineOneEpo();
                    }//for sat
                    A = new double[baselineOneEpoSum.Count, 3];
                    AT = new double[3, baselineOneEpoSum.Count];
                    omc = new double[baselineOneEpoSum.Count, 1];
                    D = new double[O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1, 2 * (O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1) + 2];
                    for (int Di = 0; Di < O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1; Di++)
                    {
                        D[Di, 0] = 1;
                        D[Di, O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num] = -1;
                        D[Di, Di + 1] = -1;
                        D[Di, Di + O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num + 1] = 1;
                    }
                    DT = new double[2 * (O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1) + 2, O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1];
                    DT = Matrix.Transfer(D, O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1, 2 * (O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1) + 2);
                    DDT = new double[O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1, O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1];
                    DDT = Matrix.Multiply(D, DT, O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1, 2 * (O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1) + 2, O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1);
                    C = new double[O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1, O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1];
                    C = Matrix.MatrixOpp(DDT);

                    for (int k = 0; k < baselineOneEpoSum.Count; k++)
                    {
                        A[k, 0] = baselineOneEpoSum[k].l;
                        A[k, 1] = baselineOneEpoSum[k].m;
                        A[k, 2] = baselineOneEpoSum[k].n;
                        omc[k, 0] = baselineOneEpoSum[k].omc;
                    }
                    AT = Matrix.Transfer(A, baselineOneEpoSum.Count, 3);
                    ATC = new double[3, O_FlieSum.master_O_FileDataSum[i].o_FileDataHead.Sat_Num - 1];
                    ATC = Matrix.Multiply(AT, C, 3, baselineOneEpoSum.Count, baselineOneEpoSum.Count);
                    ATCA = new double[3, 3];
                    ATCA = Matrix.Multiply(ATC, A, 3, baselineOneEpoSum.Count, 3);
                    ATComc = new double[3, 1];
                    ATComc = Matrix.Multiply(ATC, omc, 3, baselineOneEpoSum.Count, 1);
                    invATCA = new double[3, 3];
                    invATCA = Matrix.MatrixOpp(ATCA);
                    x = new double[3, 1];
                    x = Matrix.Multiply(invATCA, ATComc, 3, 3, 1);
                    roverReceiverPosition.X = roverReceiverPosition.X + x[0, 0];
                    roverReceiverPosition.Y = roverReceiverPosition.Y + x[1, 0];
                    roverReceiverPosition.Z = roverReceiverPosition.Z + x[2, 0];

                    baselineOneEpoSum.Clear();
                }//for iteration
                baseOneEpo.X = roverReceiverPosition.X - SPP_Result.X;
                baseOneEpo.Y = roverReceiverPosition.Y - SPP_Result.Y;
                baseOneEpo.Z = roverReceiverPosition.Z - SPP_Result.Z;

                xx =xx+ baseOneEpo.X;
                yy =yy+ baseOneEpo.Y;
                zz =zz+ baseOneEpo.Z;

                BaselineResult.baselineResult.Add(baseOneEpo);
                baseOneEpo = new BaseOneEpo();
                baselineOneEpoSum.Clear();
            }//for i



            for (int p = 0; p < BaselineResult.baselineResult.Count; p++)
            {
                sumX = sumX + BaselineResult.baselineResult[p].X;
                sumY = sumY + BaselineResult.baselineResult[p].Y;
                sumZ = sumZ + BaselineResult.baselineResult[p].Z;
            }
            RelativePositioning_Result.X = sumX / BaselineResult.baselineResult.Count;
            RelativePositioning_Result.Y = sumY / BaselineResult.baselineResult.Count;
            RelativePositioning_Result.Z = sumZ / BaselineResult.baselineResult.Count;


            return true;
        }
    }
}
