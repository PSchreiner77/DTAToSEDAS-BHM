using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatToSedas_CSharp
{
    class Parameter
    {
        private string _Arguments;
        private string _SourceFileName;
        private string _SourceFilePath;
        private string _SourceFullPath;
        private string _DestinationFileName;
        private string _DestinationFilePath;
        private string _DestinationFullPath;
        private bool _DeleteSourceFile;
        private bool _IgnoreMessages;
        private bool _Help;
        private bool _AppendToSedas;
        private int _Counter;

        public string[] Arguments { get; set; }
        public string SourceFileName { get; set; }
        public string SourceFilePath { get; set; }
        public string SourceFullPath { get; set; }
        public string DestinationFileName { get; set; }
        public string DestinationFilePath { get; set; }
        public string DestinationFullPath { get; set; }
        public bool DeleteSourceFile { get; set; }
        public bool IgnoreMessages { get; set; }
        public bool Help { get; set; }
        public bool AppendToSedas { get; set; }
        public int Counter { get; set; }

        public void New()
        {
            Arguments = new string[] { };
            SourceFullPath = "";
            DestinationFullPath = "";
            DeleteSourceFile = false;
            IgnoreMessages = false;
            AppendToSedas = false;
        }

        public void SetSourceFullPath(string SrcFullPath)
        {
            if (SrcFullPath != "")
            {
                SourceFullPath = SrcFullPath;
            }
        }

        public void SetSourceFullPath(string SrcPath, string SrcName)
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

        public void SetDestinationFullPath(string DestFullPath)
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

        public void SetDestinationFullPath(string DestPath, string DestName)
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