using ADCoreDataCommon.SQLiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrunkADCore.ADCoreWindow;

namespace TrunkADCore.ADCoreSystem.ADCoreSys 
{
    public class ExportGradeIntoSys
    {
        string _treeGroupTxt=string.Empty;
        string _projectId = string.Empty;
        string _projectName =string.Empty;
        ADCoreSqlite  ADCoreSqlite=     null;
         ExportGradeIntoWindow ExportGradeIntoWindow= null; 
        /// <summary>
        /// 展示导入成绩页面
        /// </summary>
        public  void ShowExportGrade()
        {
             ExportGradeIntoWindow = new ExportGradeIntoWindow();
             ExportGradeIntoWindow.ShowDialog();   
        }
        /// <summary>
        /// 设置导入成绩信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="treeGroupTxt"></param>
        /// <param name="projectName"></param>
        /// <param name="aDCoreSqlite"></param>
        public void SetExportGradeData(string projectId, string treeGroupTxt, string projectName, ADCoreSqlite aDCoreSqlite)
        {
            _projectId= projectId;
            _treeGroupTxt= treeGroupTxt;
            _projectName= projectName;
            ADCoreSqlite= aDCoreSqlite;
        }
    }
}
