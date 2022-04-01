using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConvertDatToSedas
{
   public enum LogMessageLevel
    {
        Information,
        Warning,
        Critical,
        Error
    }

    

    //public static class Logger
    //{
    //    internal static void Log(object sender, string message, LogMessageLevel level)
    //    {
    //        SedasLogger.LogMessage(sender,message, level);
    //    }
    //} 

    internal interface ISedasLogger
    {
        event LogMessageEventHandler SedasLogMessage;     
    }
}
