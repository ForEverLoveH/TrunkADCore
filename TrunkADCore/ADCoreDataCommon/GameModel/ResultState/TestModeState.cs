using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCoreDataCommon.GameModel {
    public class TestModeState
    {
        public static int TestMode1 = 0;//自动下一位
        public static int TestMode2 = 1;//自动下一轮
        /// <summary>
        /// 测试模式
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static int TestModeStateType2Int(string state)
        {
            switch (state)
            {
                case "自动下一位":
                    return TestMode1;
                case "自动下一轮":
                    return TestMode2;
                default:
                    return -1;
            }
        }
    }
        public class BestScoreModeState
        {
            public static int BestScoreMode1 = 0;//自动下一位
            public static int BestScoreMode2 = 1;//自动下一轮
            /// <summary>
            /// 最好成绩取值
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public static int BestScoreModeStateType2Int(string state)
            {
                switch (state)
                {
                    case "数值最大最优":
                        return BestScoreMode1;
                    case "数值最小最优":
                        return BestScoreMode2;

                    default:
                        return -1;
                }
            }
        }
    
}
