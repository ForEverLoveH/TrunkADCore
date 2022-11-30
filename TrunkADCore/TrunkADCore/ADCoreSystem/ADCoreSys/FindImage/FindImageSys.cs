using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrunkADCore.ADCoreWindow;

namespace TrunkADCore.ADCoreSystem.ADCoreSys 
{
    public class FindImageSys
    {
        FindImage  findImage=null;
        public  string fileName = null;
        public static bool isSucess = false;
        public  void ShowFindImage(List<imgMsS> imgMs)
        {
            findImage = new FindImage();
            findImage.imgMs = imgMs;
            findImage.ShowDialog();
        }

        public  void SetFindImageBack( string fileName)
        {
            fileName = fileName.Trim();
        }
        public  string GetFindImageBack()
        {
            return   fileName;   
        }
    }
}
