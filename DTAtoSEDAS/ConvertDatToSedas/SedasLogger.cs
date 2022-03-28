using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public delegate string SedasLogHandler(string Message);

    public static class SedasLogger
    {
        public static event SedasLogHandler SedasLogHandler;


        public static void Log(string Message)
        {
            SedasLogHandler("test");
        }
    }
}
