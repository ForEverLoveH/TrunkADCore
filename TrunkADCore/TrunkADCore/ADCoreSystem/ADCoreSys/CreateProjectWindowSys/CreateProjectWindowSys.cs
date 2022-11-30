using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrunkADCore.ADCoreWindow;

namespace TrunkADCore.ADCoreSystem.ADCoreSys
{
    public class CreateProjectWindowSys
    {
        public static string projectname = null;

        public static bool IsSucess = false;
        CreateProjectWindow createProjectWindow = null;

        public  void ShowCreateProjectWindow()
        {
            createProjectWindow = new CreateProjectWindow();
            createProjectWindow.ShowDialog();   
        }

        public  string  ShowCreateProjectWindowBack()
        {
            if (!string.IsNullOrEmpty(projectname) && IsSucess)
            {
                return projectname;
            }
            else
                 return  null;
        }
    }

}
