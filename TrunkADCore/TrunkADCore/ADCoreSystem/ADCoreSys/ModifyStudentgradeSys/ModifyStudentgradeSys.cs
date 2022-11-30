using ADCoreDataCommon.GameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrunkADCore.ADCoreWindow;

namespace TrunkADCore.ADCoreSystem.ADCoreSys 
{
    public class ModifyStudentgradeSys
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public  static  string _projectNames ;
        /// <summary>
        /// 群组信息
        /// </summary>
        public  static string _groupName ;
        /// <summary>
        /// 学生姓名
        /// </summary>
        public static string _name ;
        /// <summary>
        /// 
        /// </summary>
        public  static string _idNumber ;
        public  static  bool  IsSucess =false ;
        /// <summary>
        /// 
        /// </summary>
        public  static string _status ;
        /// <summary>
        /// 轮次
        /// </summary>
        public  static int _roundid ;
        public  static   double updatagrade; 
        ModifyStudentgradeWindow modifyStudentGradeWindow = null;
        /// <summary>
        /// 设置初始时候的页面信息
        /// </summary>
        /// <param name="projectNames"></param>
        /// <param name="groupName"></param>
        /// <param name="name"></param>
        /// <param name="idNumber"></param>
        /// <param name="status"></param>
        /// <param name="rountid"></param>

        public  void SetModifyStudentgradeData(string projectNames, string groupName, string name, string idNumber, string status, int rountid)
        {
             _projectNames = projectNames;
            _groupName = groupName;
            _name = name;
            _idNumber = idNumber;
            _status = status;
            _roundid = rountid;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  bool ShowModifyStudentgrade()
        {
            modifyStudentGradeWindow = new ModifyStudentgradeWindow();
            modifyStudentGradeWindow.ShowDialog();
            if (IsSucess)
            { 
                return true;
            }
            else
            {
                return false;
            }


        }
         static ModifyDataback modifyDataback = null   ;
        /// <summary>
        ///  修改学生信息的回调
        /// </summary>
        /// <returns></returns>
         
        public  ModifyDataback ModifyStudentgradeBackData()
        {
            if( modifyDataback == null )
                return null ;
            return modifyDataback;
        }

        public void SetModifyBackdata(int updaterountId, double updatescore, string status)
        {
            modifyDataback = new ModifyDataback()
            {
                roundid = updaterountId,
                updateScore = updatescore,
                updatestatus = status,

            };
        }
    }
}
