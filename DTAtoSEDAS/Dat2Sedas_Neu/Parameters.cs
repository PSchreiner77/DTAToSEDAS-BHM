using System.IO;

namespace Dat2Sedas_Neu
{
   public class Parameters
    {
        #region SINGLETON
        //SINGLETON Definition
        private static Parameters instance;

        private Parameters()
        {
        }

        public static Parameters GetInstance
        {
            get
            {
                return instance == null ? instance = new Parameters() : instance;
            }
        }
        
        public static void DestroyInstance()
        {
            instance = null;
        }
        #endregion


        //EIGENSCHAFTEN
        public string[] Arguments = new string[] { };
        
        public string SourceFileName { get; set; }
        public string SourceFileFolder { get; set; }
        public string SourceFullPath { get { return SourceFileFolder + SourceFileName; } }
       
        public string DestinationFileName { get; set; }
        public string DestinationFileFolder { get; set; }
        public string DestinationFullPath { get { return Path.Combine(DestinationFileFolder, DestinationFileName); } }
       
        public string INIFilePath { get; } = Directory.GetCurrentDirectory() + @"\config.ini";
       
        public string PathDeleteCustomerList { get => Directory.GetCurrentDirectory() + @"\loescheKunde.txt"; }
        public string PathDeleteArticleList { get => Directory.GetCurrentDirectory() + @"\loescheArtikel.txt"; }
        public string PathChangeArticlesList { get => Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt"; }

        public bool DeleteSourceFile { get; set; } = false;
        public bool IgnoreCriticalMessages { get; set; } = false;
        public bool Help { get; set; } = false;
        public bool AppendToSedas { get; set; } = false;

        public int Counter { get; set; }

        //METHODEN
        public void SetSourceFullPath(string SourceFilePath)
        {
            SetSourceFullPath(Path.GetDirectoryName(SourceFilePath), Path.GetDirectoryName(SourceFileName));
        }

        public void SetSourceFullPath(string SourceFileFolder, string SourceFileName)
        {
            this.SourceFileFolder = FolderPathCorrection(SourceFileFolder);
            this.SourceFileName = SourceFileName;
        }

        public void SetDestinationFullPath(string DestinationFilePath)
        {
            SetDestinationFullPath(Path.GetDirectoryName(DestinationFilePath), Path.GetDirectoryName(DestinationFilePath));
        }

        public void SetDestinationFullPath(string DestinationFileFolder, string DestinationFileName)
        {
            this.DestinationFileFolder = FolderPathCorrection(DestinationFileFolder);
            if (DestinationFileName == "")
                DestinationFileName = "Sedas.dat";
            this.DestinationFileName = DestinationFileName;
        }
        

        private string FolderPathCorrection(string folderPath)
        {
            if (folderPath != "")
            {
                if (folderPath.Substring(folderPath.Length - 1, 1) != "\\")
                {
                    folderPath += "\\";
                }
            }
            return folderPath;
        }
    }
}
