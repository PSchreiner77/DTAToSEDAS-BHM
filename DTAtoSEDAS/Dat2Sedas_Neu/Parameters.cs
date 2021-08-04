using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu
{
    class Parameters                 //TODO Prüfen der Parameter auf Notwendigkeit
    {
        //SINGLETON Definition
        private static Parameters instance;
        private Parameters() { }
        public static Parameters GetInstance
        {
            get
            {
                if (instance != null)
                {
                    instance = new Parameters();
                }
                return instance;
            }
        }

        //EIGENSCHAFTEN
        public string[] Arguments = new string[] { };
        public string SourceFileName { get;  set; }
        public string SourceFileFolder { get;  set; }
        public string SourceFullPath { get { return SourceFileFolder + SourceFileName; } }
        public string DestinationFileName { get;  set; }
        public string DestinationFileFolder { get; private set; }
        public string DestinationFullPath { get { return DestinationFileFolder + DestinationFileFolder; } }
        public string INIFilePath { get; } = Directory.GetCurrentDirectory() + @"\config.ini";

        public bool DeleteSourceFile { get; set; } = false;
        public bool IgnoreMessages { get; set; } = false;
        public bool Help { get; set; } = false;
        public bool AppendToSedas { get; set; } = false;

        public int Counter { get; set; }


        //METHODEN
        public void SetSourceFullPath(string SourceFileFolder, string SourceFileName)
        {
            if (SourceFileFolder != "")
            {
                if (SourceFileFolder.Substring(SourceFileFolder.Length, 1) != "\\")
                {
                    SourceFileFolder += "\\";
                }
            }
            this.SourceFileFolder = SourceFileFolder;
            this.SourceFileName = SourceFileName;
        }

        public void SetDestinationFullPath(string DestinationFileFolder, string DestinationFileName)
        {
            if (DestinationFileName == "") DestinationFileName = "Sedas.dat";

            if (DestinationFileFolder == "")
            {
                DestinationFileFolder = Directory.GetCurrentDirectory() + "\\";    
            }
            else
            {
                if (DestinationFileFolder.Substring(SourceFileFolder.Length, 1) != "\\")
                {
                    DestinationFileFolder += "\\";
                }
            }
            this.DestinationFileFolder = DestinationFileFolder;
            this.DestinationFileName = DestinationFileName;
        }

        //DELETE
        //public void SetSourceFullPath(string SrcFullPath)
        //{
        //    if (SrcFullPath != "")
        //    {
        //        SourceFullPath = SrcFullPath;
        //    }
        //}

        //DELETE
        //public void SetDestinationFullPath(string DestFullPath)
        //{
        //    if (DestFullPath != "")
        //    {

        //        if (DestFullPath.Substring(DestFullPath.Length, 1) != "\\")
        //        {
        //            DestFullPath += "\\";
        //        }
        //    }
        //    DestinationFullPath = DestFullPath;
        //
        //}
    }
}
