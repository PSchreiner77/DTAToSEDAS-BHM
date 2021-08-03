using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu
{
    static class Parameters                 //TODO Prüfen der Parameter auf Notwendigkeit
    {
        //private string _Arguments;
        //private string _SourceFileName;
        //private string _SourceFilePath;
        //private string _SourceFullPath;
        //private string _DestinationFileName;
        //private string _DestinationFilePath;
        //private string _DestinationFullPath;
        //private bool _DeleteSourceFile;
        //private bool _IgnoreMessages;
        //private bool _Help;
        //private bool _AppendToSedas;
        //private int _Counter;

        public static string[] Arguments { get; set; }
        public static string SourceFileName { get; set; }
        public static string SourceFilePath { get; set; }
        public static string SourceFullPath { get; set; }
        public static string DestinationFileName { get; set; }
        public static string DestinationFilePath { get; set; }
        public static string DestinationFullPath { get; set; }
        public static bool DeleteSourceFile { get; set; }
        public static bool IgnoreMessages { get; set; }
        public static bool Help { get; set; }
        public static bool AppendToSedas { get; set; }
        public static int Counter { get; set; }

        public static void New()
        {
            Arguments = new string[] { };
            SourceFullPath = "";
            DestinationFullPath = "";
            DeleteSourceFile = false;
            IgnoreMessages = false;
            AppendToSedas = false;
        }

        public static void SetSourceFullPath(string SrcFullPath)
        {
            if (SrcFullPath != "")
            {
                SourceFullPath = SrcFullPath;
            }
        }

        public static void SetSourceFullPath(string SrcPath, string SrcName)
        {
            if (SrcPath != "")
            {
                if (SrcPath.Substring(SrcPath.Length, 1) != "\\")
                {
                    SrcPath += "\\";
                }
            }
            SourceFullPath = SourceFilePath + SourceFileName;
        }

        public static void SetDestinationFullPath(string DestFullPath)
        {
            if (DestFullPath != "")
            {

                if (DestFullPath.Substring(DestFullPath.Length, 1) != "\\")
                {
                    DestFullPath += "\\";
                }
            }
            DestinationFullPath = DestFullPath;
        }

        public static void SetDestinationFullPath(string DestPath, string DestName)
        {
            if (DestPath != "")
            {
                if (DestPath.Substring(DestPath.Length, 1) != "\\")
                {
                    DestPath += "\\";
                }
            }
            DestinationFullPath = DestinationFilePath + DestinationFileName;
        }
    }
}
