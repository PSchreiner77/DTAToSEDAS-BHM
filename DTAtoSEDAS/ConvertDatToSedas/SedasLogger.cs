using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public delegate string SedasLogHandler(object sender, string message);

    public static class SedasLogger
    {
        public static event SedasLogHandler LogHandler;

        internal static void Log(object sender ,string Message)
        {
            LogHandler(sender, Message);
        }
    }
}
