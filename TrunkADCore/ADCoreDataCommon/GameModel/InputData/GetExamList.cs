using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCoreDataCommon.GameModel
{    
    /// <summary>
    /// 体测学校列表
    /// </summary>
    public class GetExamList
    {
        public List<GetExamListResults> Results { get; set; }

        public String Error { get; set; }

    }
    /// <summary>
    ///机器码列表
    /// </summary>
    public class GetExamListResults
    {
        public String exam_id;

        public String title;
    }

    /// <summary>
    /// 机器码列表
    /// </summary>
    public class GetMachineCodeList
    {
        public List<GetMachineCodeListResults> Results;

        public String Error;
    }
    public class GetMachineCodeListResults
    {
        public String title;

        public String MachineCode;
    }

    /// <summary>
    /// 按组数获取学生
    /// </summary>
    public class GetGroupStudent
    {

        /// <summary>
        /// 
        /// </summary>
        public Results Results { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Error { get; set; }

    }
    public class Results
    {
        /// <summary>
        /// 
        /// </summary>
        public List<GroupsItem> groups { get; set; }


    }

    public class GroupsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<StudentInfosItem> StudentInfos { get; set; }
    }

    public class StudentInfosItem
    {
        /// <summary>
        /// 高小雅
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 女
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 淮安市文通中学
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 初三
        /// </summary>
        public string GradeName { get; set; }

        /// <summary>
        /// 1班
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClassNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GroupIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Projects { get; set; }
    }
}
