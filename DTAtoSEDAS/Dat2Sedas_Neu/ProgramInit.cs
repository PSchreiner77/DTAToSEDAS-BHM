using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INIManager;

namespace Dat2Sedas_Neu
{
    class ProgramInit
    {
        private static Parameters Param;
        private static Logger log = Logger.GetInstance();
        private static string messageTitle = "";
        private static string message = "";

        private static bool CheckIniFile()
        {
            if (!File.Exists(Param.INIFilePath))
            {
                string title = "Initialisierungsfehler";
                string message = $"Config.ini Datei ist nicht vorhanden oder nicht erreichbar. {Param.INIFilePath}\n" +
                                  "Es wird eine neue Config.ini Datei angelegt. Bitte prüfen sie die Einstellungen.";
                log.Log(message, title, Logger.MsgType.Critical);
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
            messageTitle = "Initialisierungsfehler";
            message = "Fehler beim Einlesen der Config.ini: " + "\n\r" + ex.ToString();
            log.Log(message, messageTitle, Logger.MsgType.Critical);
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
                messageTitle = "Programminitialisierung";
                message = "Erstellen einer neuen Config.ini Datei...";
                log.Log(message, messageTitle, Logger.MsgType.Message);
                using (StreamWriter sw = new StreamWriter(Param.INIFilePath))
                {
                    sw.WriteLine(iniContent);
                }
            }
            catch (Exception ex)
            {
                messageTitle = "Initialisierungsfehler";
                message = "Fehler beim Erstellen einer neuen leeren Config.ini:\n\r" + ex.ToString();
                log.Log(message, messageTitle, Logger.MsgType.Critical);

                return false;
            }

            messageTitle = "Programminitialisierung";
            message = "Erstellen einer neuen Config.ini Datei OK.\nBitte prüfen Sie die Einstellungen in der Config.ini!";
            log.Log(message, messageTitle, Logger.MsgType.Critical);
            return true;
        }

        private static bool CheckStartArguments()
        {
            bool CheckParameters = true;
            string[] Arguments = Environment.GetCommandLineArgs();

            if (Arguments.Count() < 2)
            {
                messageTitle = "Initialisierungsfehler";
                message = "Es wurden keine Startparameter übergeben. Bitte Hilfe aufrufen mit Parametr /?";
                log.Log(message, messageTitle, Logger.MsgType.Critical);
                return false;
            }

            if (Arguments.Contains("/?"))
            {
                Param.Help = true;
                Help.Show();
                return false;
            }

            for (int i = 1; i < Arguments.Count(); i++)
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
                        messageTitle = "Initialisierungsfehler";
                        message = "Mindestens ein falscher Startparameter angegeben!";
                        log.Log(message, messageTitle, Logger.MsgType.Critical);
                        return false;
                }
            }
            return true;
        }

        private static bool CheckSource()
        {
            messageTitle = "Programminitialisierung";
            message = "Prüfen des Quelldateipfades...";
            log.Log(message, messageTitle, Logger.MsgType.Message);
            string source = Param.SourceFullPath;     

            if (source == "")
            {
                messageTitle = "Initialisierungsfehler";
                message = "Es wurde keine Quelldatei angegeben.";
                log.Log(message, messageTitle, Logger.MsgType.Critical);                 
                return false;
            }

            if (!File.Exists(source))
            {
                messageTitle = "Programminitialisierung";
                message = $"Die Quelldatei {source} existiert nicht oder ist nicht erreichbar.";
                log.Log(message, messageTitle, Logger.MsgType.Critical); 
                return false;
            }

            messageTitle = "Initialisierungsfehler";
            message = "Quelldateipfad OK.";
            log.Log(message, messageTitle, Logger.MsgType.Message);
            return true;
        }

        private static void SetDestinationPath()
        {
            Param.SetDestinationFullPath(Param.DestinationFileFolder, Param.DestinationFileName);
        }

        public static bool Init()
        {
            messageTitle = "Programminitialisierung";
            message = "Initialisieren der Programmparameter...";
            log.Log(message, messageTitle, Logger.MsgType.Message);
            Param = Parameters.GetInstance;

            if (!CheckIniFile()) return false;
            if (!ReadIniValues()) return false;
            if (!CheckStartArguments()) return false;
            if (!CheckSource()) return false;
            SetDestinationPath();

            messageTitle = "Programminitialisierung";
            message = "Initialisierung der Parameter OK.";
            log.Log(message, messageTitle, Logger.MsgType.Message);
            return true;
        }
    }
}
