using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADCoreDataCommon.GameModel 
{
     public  class ProjectModel
    {
        public  string projectName { get;set;}  
        public  List<GroupModel> groupModels { get; set; }
    }

    public class GroupModel
    {
        public  string name { get; set; }
        public  int IsAllTested { get; set; }   
    }
}
