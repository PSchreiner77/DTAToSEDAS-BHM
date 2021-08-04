using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu
{
    //Delegate zum Senden einer Nachricht einrichten
    public delegate void InitMessageHandler(string message);

    class ProgramInit
    {
        public static event InitMessageHandler InitNotification;
        public static event InitMessageHandler InitError;
        private static Parameters Param = Parameters.GetInstance;

        //METHODEN
        private static void InitErrorMessage(string message)
        {
            InitError?.Invoke($"Initialisierung: FEHLER  {message}");
        }
        private static void InitMessage(string message)
        {
            InitNotification?.Invoke($"Initialisierung: {message}");
        }

        private static bool CheckStartArguments()
        {
            bool CheckParameters = true;

            string[] Arguments = Environment.GetCommandLineArgs();
            //-----------------------------------------------------------------------------------------
            //Keine Parameter übergeben. Funktion Ende.
            if (Arguments.GetUpperBound(0) < 1)
            {
                InitError("Es wurden keine Startparameter übergeben. Bitte Hilfe aufrufen mit Parametr /?");
                return false;
            }

            //-----------------------------------------------------------------------------------------
            //Wenn /? enthalten ist.
            if (Arguments.Contains("/?"))
            {
                Param.Help = true;
                Help.Show();
                return false;
            }

            //'-----------------------------------------------------------------------------------------
            //'## Parameter auswerten.
            foreach (string arg in Arguments)
            {
                string S = arg.Split('=')[0];

                switch (S.ToUpper())
                {
                    case "/Q":
                        try
                        {
                            Param.SourceFileFolder = Path.GetDirectoryName(S[1].ToString());
                            Param.SourceFileFolder = Path.GetFileName(S[1].ToString());
                        }
                        catch (Exception ex)
                        { }
                        break;

                    case "/Z":
                        break;

                    case "/D":
                        break;

                    case "/I":
                        break;

                    case "/A":
                        break;

                    default:
                        InitError("Mindestens ein falscher Startparameter angegeben!");
                        break;
                }
            }

            for (int i = 0; i < Arguments.GetUpperBound(0); i++) // i = 1 To Arguments.GetUpperBound(0)
            {

                //'Parameter prüfen und Parameterwert setzen

                switch (Switch.ToUpper())
                {
                    case "/Q=":
                        //'Prüfen, ob Eintrag aus Pfad und/oder Dateiname besteht
                        if (InStr(Mid(Arguments(i), InStr(Arguments(i), "=") + 1), "\\") > 0)
                        {
                            //'Pfad enthalten.
                            if (InStr(Mid(Arguments(i), InStrRev(Arguments(i), "\\") + 1), ".") > 0)
                            {
                                //'Dateiname enthalten: Eintrag als kompletten Pfad übernehmen.
                                Param.SetSourceFullPath(Mid(Arguments(i), InStr(Arguments(i), "=") + 1));
                            }
                            else
                            {
                                //'Kein Dateiname enthalten: Eintrag als nur Pfad übernehmen.
                                Param.SourceFilePath = Mid(Arguments(i), InStr(Arguments(i), "=") + 1);
                            }
                        }
                        else
                        {
                            //'Kein Pfad enthalten
                            if (InStr(Mid(Arguments(i), InStrRev(Arguments(i), "\\") + 1), ".") > 0)
                            {
                                //'Dateiname enthalten: Eintrag als Dateiname übernehmen.
                                Param.SourceFileName = Mid(Arguments(i), InStr(Arguments(i), "=") + 1);
                            }
                        }
                        break;

                    case "/Z=":
                        if (InStr(Mid(Arguments(i), InStr(Arguments(i), "=") + 1), "\\") > 0)
                        {
                            //'Pfad enthalten.
                            if (InStr(Mid(Arguments(i), InStrRev(Arguments(i), "\\") + 1), ".") > 0)
                            {
                                //'Dateiname enthalten: Eintrag als kompletten Pfad übernehmen.
                                Param.SetDestinationFullPath(Mid(Arguments(i), InStr(Arguments(i), "=") + 1));
                            }
                            else
                            {
                                //'Kein Dateiname enthalten: Eintrag als nur Pfad übernehmen.
                                Param.DestinationFileFolder = Mid(Arguments(i), InStr(Arguments(i), "=") + 1);
                            }
                        }
                        else
                        {
                            //'Kein Pfad enthalten
                            if (InStr(Mid(Arguments(i), InStrRev(Arguments(i), "\\") + 1), ".") > 0)
                            {
                                //'Dateiname enthalten: Eintrag als Dateiname übernehmen.
                                Param.DestinationFileName = Mid(Arguments(i), InStr(Arguments(i), "=") + 1);
                            }
                        }
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

                        return false; ;
                        break;
                }
            }
            return true;
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
            bool result = true;
            try
            {
                Param.SourceFileName = INI.Read("Setup", "Quelldateiname");
                Param.SourceFileFolder = INI.Read("Setup", "Quelldateipfad");
                Dim flag As Boolean = Operators.CompareString(Param.SourceFileFolder, "", false) <> 0;
                if (Param.SourceFileFolder != "")
                {  // flag Then
                    bool flag2 = Operators.CompareString(Strings.Mid(Param.SourceFileFolder, Strings.Len(Param.SourceFileFolder), 1), "\\", false) <> 0;
                    if (flag2)
                    {
                        Param.SourceFilePath = Param.SourceFileFolder + "\\";
                    }
                    else
                    {
                        Param.SourceFilePath = Directory.GetCurrentDirectory() + "\\";
                    }
                    Param.DestinationFileName = INI.Read("Setup", "Zieldateiname");
                    Param.DestinationFileFolder = INI.Read("Setup", "Zieldateipfad");
                    bool flag3 = Operators.CompareString(Param.DestinationFileFolder, "", false) <> 0;
                    if (Param.DestinationFileFolder != "")
                    {
                        bool flag4 = Operators.CompareString(Strings.Mid(Param.DestinationFileFolder, Strings.Len(Param.DestinationFileFolder), 1), "\\", false) <> 0;
                        if (flag4)
                        {
                            Param.DestinationFileFolder = Param.DestinationFileFolder + "\\";
                        }
                        else
                        {
                            Param.DestinationFileFolder = Directory.GetCurrentDirectory() + "\\";
                        }

                        bool flag5 = Operators.CompareString(Param.DestinationFileName, "", false) = 0 And Operators.CompareString(Param.DestinationFileFolder, "", false) <> 0;
                        if (flag5)
                        {
                            Param.DestinationFileName = "SEDAS.DAT";
                        }

                        Param.DeleteSourceFile = Conversions.ToBoolean(INI.Read("Setup", "QuelleLöschen"));
                        Param.IgnoreMessages = Conversions.ToBoolean(INI.Read("Setup", "IgnoriereMeldungen"));
                        Param.Counter = Conversions.ToInteger(INI.Read("Setup", "Counter"));
                    }
                }
            }
            catch (Exception ex)
            {
                //LogMessage.LogOnly("Fehler beim Einlesen der Config.ini: " + "\n\r" + ex.ToString());
                result = false;
            }

            return result;
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

        private static void SetDestinationPath()
        {
            Param.SetDestinationFullPath(Param.DestinationFileFolder, Param.DestinationFileName);
        }

        public static bool Init()
        {
            #region Initialisierung

            InitMessage("Initialisieren der Programmparameter...");

            if (!CheckStartArguments()) return false;
            if (!ReadIniValues()) return false;
            if (!CheckSource()) return false;
            SetDestinationPath();

            // The following expression was wrapped in a checked-expression
            #endregion

            InitMessage("Initialisierung der Parameter OK.");
            return true;
        }
    }
}
