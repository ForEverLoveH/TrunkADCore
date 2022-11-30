using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCoreDataCommon.GameModel
{
    public  class ProjectSettingBack
    {
        /// <summary>
        /// 
        /// </summary>
         public  string projectID { get; set; }
        /// <summary>
        ///  项目id
        /// </summary>
         public  string Name { get; set; }  
        /// <summary>
        /// 项目类型
        /// </summary>
         public   int type { get; set; } 

         // 轮次
         public int roundCount { get; set; }
         
         public int bestScoreMode { get; set; }
        public   int TestMethod { get; set; }   
         public int FloatType { get; set; }

    }
}
