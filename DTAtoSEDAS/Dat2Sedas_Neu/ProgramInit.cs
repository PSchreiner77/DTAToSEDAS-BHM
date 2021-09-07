using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INIManager;

namespace Dat2Sedas_Neu
{
    //Delegate zum Senden einer Nachricht einrichten
    public delegate void InitMessageHandler(string message);

    class ProgramInit
    {
        private static Parameters Param;
        private Logger log = Logger.GetInstance();
        
        public static event InitMessageHandler InitNotification;
        public static event InitMessageHandler InitError;

        //METHODEN
        private static void InitErrorMessage(string message)
        {
            InitError?.Invoke($"Initialisierung: FEHLER  {message}");
        }
        
        private static void InitMessage(string message)
        {
            InitNotification?.Invoke($"Initialisierung: {message}");
        }


        private static bool CheckIniFile()
        {
            if (!File.Exists(Param.INIFilePath))
            {
                string message = $"Config.ini Datei ist nicht vorhanden oder nicht erreichbar. {Param.INIFilePath}\n" +
                                  "Es wird eine neue Config.ini Datei angelegt. Bitte prüfen sie die Einstellungen.";
                InitErrorMessage(message);
                CreateNewConfigIni();
                return false;
            }
            return true;
        }

        private static bool ReadIniValues()
        {
            IniManager INI = new IniManager(Param.INIFilePath);
            
            try
            {
                Param.SourceFileName = INI.GetParameterValue("Setup", "Quelldateiname");
                Param.SourceFileFolder = INI.GetParameterValue("Setup", "Quelldateipfad");
                Param.DestinationFileName = INI.GetParameterValue("Setup", "Zieldateiname");
                Param.DestinationFileFolder = INI.GetParameterValue("Setup", "Zieldateipfad");
                var test = INI.GetParameterValue("Setup", "QuelleLöschen");
                Param.DeleteSourceFile = Convert.ToBoolean(Convert.ToInt32(INI.GetParameterValue("Setup", "QuelleLöschen")));
                Param.IgnoreMessages = Convert.ToBoolean(Convert.ToInt32(INI.GetParameterValue("Setup", "IgnoriereMeldungen")));
                Param.Counter = Convert.ToInt32(INI.GetParameterValue("Setup", "Counter"));
            }
            catch (Exception ex)
            {
                InitError("Fehler beim Einlesen der Config.ini: " + "\n\r" + ex.ToString());
                return false;
            }
            return true;
        }

        private static bool CreateNewConfigIni()
        {
            bool flag = true;
            //LogMessage.LogOnly("Erstellen einer neuen leeren Config.ini...");
            string iniContent = "-----------------------\n\r" +
                                "DATtoSEDAS Config-Datei\n\r" +
                                "-----------------------\n\r" +
                                "Quell- und Zielpfad müssen mit Laufwerksbuchstabe angegeben werden (vollständig), jedoch ohne Dateiname.\n\r" +
                                "Der Dateiname der Quell- und Zieldatei wird separat eingetragen.\n\r" +
                                "Werden Quell- und Zieldateiname beim Programmstart per Schalter übergeben (/Q=, /Z=), werden die Einträge\n\r" +
                                "in der Config.ini übergangen.\n\r" +
                                "Dies gilt auch für alle weiteren Schalter (z.B. QuelleLöschen, /D)\n\r" +
                                "\n\r" +
                                "[Setup]\n\r" +
                                "Counter=\n\r" +
                                "Quelldateipfad=\n\r" +
                                "Quelldateiname=1.txt\n\r" +
                                "Zieldateipfad=C:\\Temp\n\r" +
                                "Zieldateiname=Sedas.dat\n\r" +
                                "\n\r" +
                                "QuelleLöschen=0\n\r" +
                                "IgnoriereMeldungen=0\n\r" +
                                "DatenAnhängen=0\n\r";

            try
            {
                InitMessage("Erstellen einer neuen Config.ini Datei...");
                using (StreamWriter sw = new StreamWriter(Param.INIFilePath))
                {
                    sw.WriteLine(iniContent);
                }
            }
            catch (Exception ex)
            {
                InitError("Fehler beim Erstellen einer neuen leeren Config.ini: " + "\n\r" + ex.ToString());
                return false;
            }
            InitMessage("Erstellen einer neuen Config.ini Datei OK.\nBitte prüfen Sie die Einstellungen in der Config.ini!");
            return true;
        }

        private static bool CheckStartArguments()
        {
            bool CheckParameters = true;
            string[] Arguments = Environment.GetCommandLineArgs();

            //if (Arguments.Count() < 2)
            //{
            //    InitError("Es wurden keine Startparameter übergeben. Bitte Hilfe aufrufen mit Parametr /?");
            //    return false;
            //}

            if (Arguments.Contains("/?"))
            {
                Param.Help = true;
                Help.Show();
                return false;
            }

            for(int i=1;i<Arguments.Count();i++)
            {
                string[] S = Arguments[i].Split('=');

                switch (S[0].ToUpper())
                {
                    case "/Q":
                        try
                        {
                            Param.SourceFileFolder = Path.GetDirectoryName(S[1].ToString());
                            Param.SourceFileName = Path.GetFileName(S[1].ToString());
                        }
                        catch (Exception ex)
                        { }
                        break;

                    case "/Z":
                        try
                        {
                            Param.DestinationFileFolder = Path.GetDirectoryName(S[1].ToString());
                            Param.DestinationFileName = Path.GetFileName(S[1].ToString());
                        }
                        catch (Exception ex)
                        { }
                        break;

                    case "/D":
                        Param.DeleteSourceFile = true;  //Quelldatei nach Programmende löschen.        
                        break;

                    case "/I":
                        Param.IgnoreMessages = true;    //'Fehlermeldungen unterdrücken
                        break;

                    case "/A":
                        Param.AppendToSedas = true;     //Existiernde Zieldatei fortschreiben
                        break;

                    default:
                        InitError("Mindestens ein falscher Startparameter angegeben!");
                        return false;                        
                }
            }                
            return true;
        }
        
        private static bool CheckSource()
        {
            InitMessage("Prüfen des Quelldateipfades...");
            string source = Param.SourceFullPath;

            if (source == "")
            {
                InitErrorMessage("Es wurde keine Quelldatei angegeben.");
                return false;
            }

            if (!File.Exists(source))
            {
                InitErrorMessage($"Die Quelldatei {source} existiert nicht oder ist nicht erreichbar.");
                return false;
            }

            InitMessage("Quelldateipfad OK.");
            return true;
        }

        private static void SetDestinationPath()
        {
            Param.SetDestinationFullPath(Param.DestinationFileFolder, Param.DestinationFileName);
        }
                
        public static bool Init()
        {
            InitMessage("Initialisieren der Programmparameter...");
            Param = Parameters.GetInstance;

            if (!CheckIniFile()) return false;
            if (!ReadIniValues()) return false;
            if (!CheckStartArguments()) return false;
            if (!CheckSource()) return false;
            SetDestinationPath();

            InitMessage("Initialisierung der Parameter OK.");
            return true;
        }
    }
}
