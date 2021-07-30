using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu
{
    //Delegate zum Senden einer Nachricht einrichten
    public Action<string> MessageEventHandler;

    class ProgramInit
    {

        public static bool Init()
        {   
            
            #region Initialisierung
            
            LogMessage.LogOnly("Initialisierung des Programms erfolgreich.");

            //in INIT verschieben
            if (!CheckParmeters())
            {  // flag2 Then
                ShowErrorMessage("Prüfung der Parameter fehlgeschlagen.");
                return false;
            }
            LogMessage.LogOnly("Prüfung der Parameter erfolgreich.");

            //in INIT verschieben
            if (!CheckSource())
            {  // flag3 Then
                ShowErrorMessage("Prüfung des Quellpfades fehlgeschlagen.");
                return false;
            }
            LogMessage.LogOnly("Prüfung des Quellpfades erfolgreich.");


            Param.SetDestinationFullPath(Param.DestinationFilePath, Param.DestinationFileName);
            // The following expression was wrapped in a checked-expression
            #endregion

            return true;
        }

        //IN INIT verschieben
        public static bool ReadIniValues()
        {
            bool result = true;
            try
            {
                Param.SourceFileName = INI.Read("Setup", "Quelldateiname");
                Param.SourceFilePath = INI.Read("Setup", "Quelldateipfad");
                Dim flag As Boolean = Operators.CompareString(Param.SourceFilePath, "", false) <> 0;
                if (Param.SourceFilePath != "")
                {  // flag Then
                    bool flag2 = Operators.CompareString(Strings.Mid(Param.SourceFilePath, Strings.Len(Param.SourceFilePath), 1), "\\", false) <> 0;
                    if (flag2)
                    {
                        Param.SourceFilePath = Param.SourceFilePath + "\\";
                    }
                    else
                    {
                        Param.SourceFilePath = Directory.GetCurrentDirectory() + "\\";
                    }
                    Param.DestinationFileName = INI.Read("Setup", "Zieldateiname");
                    Param.DestinationFilePath = INI.Read("Setup", "Zieldateipfad");
                    bool flag3 = Operators.CompareString(Param.DestinationFilePath, "", false) <> 0;
                    if (Param.DestinationFilePath != "")
                    {
                        bool flag4 = Operators.CompareString(Strings.Mid(Param.DestinationFilePath, Strings.Len(Param.DestinationFilePath), 1), "\\", false) <> 0;
                        if (flag4)
                        {
                            Param.DestinationFilePath = Param.DestinationFilePath + "\\";
                        }
                        else
                        {
                            Param.DestinationFilePath = Directory.GetCurrentDirectory() + "\\";
                        }

                        bool flag5 = Operators.CompareString(Param.DestinationFileName, "", false) = 0 And Operators.CompareString(Param.DestinationFilePath, "", false) <> 0;
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
                LogMessage.LogOnly("Fehler beim Einlesen der Config.ini: " + "\n\r" + ex.ToString());
                result = false;
            }

            return result;
        }

        public static bool CreateNewConfigIni()
        {
            bool flag = true;
            LogMessage.LogOnly("Erstellen einer neuen leeren Config.ini...");
            string value = "-----------------------" + "\n\r" + "DATtoSEDAS Config-Datei" + "\n\r" + "-----------------------" + "\n\r" + "Quell- und Zielpfad müssen mit Laufwerksbuchstabe angegeben werden (vollständig), jedoch ohne Dateiname." + "\n\r" + "Der Dateiname der Quell- und Zieldatei wird separat eingetragen." + "\n\r" + "Werden Quell- und Zieldateiname beim Programmstart per Schalter übergeben (/Q=, /Z=), werden die Einträge" + "\n\r" + "in der Config.ini übergangen." + "\n\r" + "Dies gilt auch für alle weiteren Schalter (z.B. QuelleLöschen, /D)" + "\n\r" + "\n\r" + "[Setup]" + "\n\r" + "Counter=" + "\n\r" + "Quelldateipfad=" + "\n\r" + "Quelldateiname=1.txt" + "\n\r" + "Zieldateipfad=C:\\Temp" + "\n\r" + "Zieldateiname=Sedas.dat" + "\n\r" + "\n\r" + "QuelleLöschen=0" + "\n\r" + "IgnoriereMeldungen=0" + "\n\r" + "DatenAnhängen=0";
            try
            {
                using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\Config.ini"))
                {
                    sw.WriteLine(value);
                }
            }
            catch (Exception ex)
            {
                LogMessage.LogOnly("Erstellen einer neuen leeren Config.ini fehlgeschlagen: " + "\n\r" + ex.ToString());
                flag = false;
            }
            return flag;
        }

        public static bool CheckParameters()
        {
            CheckParameters = true;


            //-----------------------------------------------------------------------------------------
            //Keine Parameter übergeben. Funktion Ende.
            if (Arguments.GetUpperBound(0) < 1)
            {
                //Keine Parameter übergeben
                return CheckParameters;
            }

            //-----------------------------------------------------------------------------------------
            //Wenn nur ein Element übergeben wurde, prüfen, ob es /? ist.
            if (Arguments.GetUpperBound(0) = 1)
            {
                if (InStr(Arguments(1), "/?") > 0)
                {
                    Param.Help = true;
                    ShowHelp();
                    CheckParameters = false;
                    return CheckParameters;
                }
            }

            //-----------------------------------------------------------------------------------------
            //'Prüfen, ob mehr als 1 Argument übergeben und im Array /? vorkommt
            //'Wenn ja, dann Parameter falsch.
            if ()
            {  // Arguments.GetUpperBound(0) > 1 And Array.Exists(Arguments, Function(element)
                Return element.Contains("/?");
                End Function) Then
                ShowMessage("FEHLER! Falsche Startparameter angegeben. '/?' darf nur alleine verwendet werden.", false);
                CheckParameters = false;
                return CheckParameters();
            }

            //'-----------------------------------------------------------------------------------------
            //'## Parameter auswerten.
            int erg = 0;
            for (int i = 0; i < Arguments.GetUpperBound(0); i++) // i = 1 To Arguments.GetUpperBound(0)
            {
                string Switch = Arguments(i);

                //'Parameter ermitteln
                if (InStr(Arguments(i), "=") > 0)
                {
                    'Prüfen, ob der Parameter ein = enthält
                    Switch = Mid(Arguments(i), 1, InStr(Arguments(i), "="));
                }
                else
                {
                    Switch = Arguments(i);
                }

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
                                Param.DestinationFilePath = Mid(Arguments(i), InStr(Arguments(i), "=") + 1);
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
                        Param.DeleteSourceFile = true;
                        //'erg = erg + 1
                        break;

                    case "/I":
                        //'Fehlermeldungen unterdrücken
                        Param.IgnoreMessages = true;
                        break;

                    case "/A":
                        //'Fehlermeldungen unterdrücken
                        Param.Append = true;
                        break;

                    default:
                        ShowMessage("FEHLER! Falsche Startparameter angegeben.", false);
                        CheckParameters = false;
                        return CheckParameters;

                        break;
                }
            }
            return CheckParameters;
        }

        public static bool CheckSource()
        {
            bool flag = true;
            if (Param.SourceFullPath == "")
            {
                Param.SetSourceFullPath(Param.SourceFilePath, Param.SourceFileName);

                LogMessage.LogOnly("Prüfen des Quellpfades: " + Param.SourceFullPath);
                if (Param.SourceFullPath != "")
                {

                    flag = false;
                    LogMessage.Show("Es wurde keine Quelldatei angegeben.", LogMessage.MsgType.Critical);
                }
            }
            else
            {
                bool flag3 = Not File.Exists(Param.SourceFullPath);
                if (flag3)
                {
                    flag = false;
                    LogMessage.Show("Die Quelldatei existiert nicht oder ist nicht erreichbar.", LogMessage.MsgType.Critical);
                }
                else
                {
                    flag = flag;
                }
                return flag;
            }
            return true;
        }

        private static void ShowErrorMessage(string message)
        {
            string msgText = "Fehler bei der Initialisierung: " + message;
            LogMessage.LogOnly(message);
        }

    }


}
