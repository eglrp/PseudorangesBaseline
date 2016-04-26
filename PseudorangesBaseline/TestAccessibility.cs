using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudorangesBaseline
{
    class TestAccessibility
    {
        /// <summary>
        /// 查看能否正常访问读取到的星历数据
        /// </summary>
        /// <returns></returns>
        public string Test_N()
        {
            string prn = N_FileSum.n_FileDataSum[0].PRN.ToString();
            string leap_Seconds = N_FileSum.n_FileHeadSum.Leap_Seconds.ToString();
            string output = "Leap Seconds:" + leap_Seconds + "\n" + "PRN:" + prn;
            return output;
        }

        /// <summary>
        /// 查看能否正常访问读取到的基准站观测值数据
        /// </summary>
        /// <returns></returns>
        public string Test_Master_O()
        {
            string type = O_FlieSum.master_O_FileHeadSum.TypeOfObserv[0];
            string first_Epo_Sat_Num = O_FlieSum.master_O_FileDataSum[0].o_FileDataHead.Sat_Num.ToString();
            string fifth_Epo_PRN = O_FlieSum.master_O_FileDataSum[4].o_FileDataHead.Sat_PRN[2].ToString();
            string c1 = O_FlieSum.master_O_FileDataSum[0].o_FileDataObs[0].C1.ToString();
            string p1 = O_FlieSum.master_O_FileDataSum[1].o_FileDataObs[1].P1.ToString();
            string output = "Type:" + type + "\n" + "1st Epoch Sat_Num:" + first_Epo_Sat_Num + "\n" + 
                            "5th Epoch PRN:" + fifth_Epo_PRN + "\n" + "1st Epoch,1st Sat C1:" 
                            + c1 + "\n" + "2nd Epoch,2nd Sat P1:" + p1;
            return output;
        }
    }
}
