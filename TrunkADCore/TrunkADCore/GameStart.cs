using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrunkADCore.ADCoreSystem.ADCoreSys;

namespace TrunkADCore
{
     public  class GameStart
    {
         static  Mainsys Mainsys = new Mainsys();
        public   static  void StartGame()
        {
            Awake();
            Start();


        }

        private static void Start()
        {
             Mainsys.Init();
        }

        private static void Awake()
        {
             Mainsys.Awake();
        }
    }
}
