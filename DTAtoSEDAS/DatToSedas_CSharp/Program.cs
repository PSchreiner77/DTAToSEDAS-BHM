using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatToSedas_CSharp
{
    class Program
    {

        private string[] Arguments = new Environment.GetCommandLineArgs(); //CommandlineArgs besorgen

        public string FileDate = "";

        private INIFile INI;

        public Parameter Param = new Parameter();

        public List<string> ListDelCustomer;

        public List<string> ListDelArticle;

        public List<string> ListChangeArticle;

        //TODO ??
        //< STAThread() >
        public void Main()
        {
            LogMessage.GlobalLog = true;
            LogMessage.GlobalOutputToConsole = true;
            LogMessage.LogOnly("**********************************");
            LogMessage.LogOnly("--------- PROGRAMMSTART ----------");
            LogMessage.CheckLogFile(100);


            if (!InitProgram())
            {  // flag Then
                LogMessage.LogOnly("Initialisierung des Programms fehlgeschlagen.");
                //ExitProgram();
            }
            LogMessage.LogOnly("Initialisierung des Programms erfolgreich.");

            if (!CheckParmeters())
            {  // flag2 Then
                LogMessage.LogOnly("Prüfung der Parameter fehlgeschlagen.");
                //ExitProgram();
            }
            LogMessage.LogOnly("Prüfung der Parameter erfolgreich.");

            if (!CheckSource())
            {  // flag3 Then
                LogMessage.LogOnly("Prüfung des Quellpfades fehlgeschlagen.");
                //ExitProgram();
            }
            LogMessage.LogOnly("Prüfung des Quellpfades erfolgreich.");

            Param.SetDestinationFullPath(Param.DestinationFilePath, Param.DestinationFileName);
            // The following expression was wrapped in a checked-expression

            Param.Counter = Param.Counter + 1;
            if (Param.Counter > 999)
            {
                Param.Counter = 1;
            }
            LogMessage.LogOnly("Start der Konvertierung der Bestelldatei in eine Sedas.dat...");
            ConvertDATtoSEDAS convertDATtoSEDAS = New ConvertDATtoSEDAS(Param.SourceFullPath, Param.DestinationFullPath, Param.Counter, ListDelCustomer, ListDelArticle);
            bool flag4 = convertDATtoSEDAS.ConvertFile();
            if (flag4)
            {
                LogMessage.Show("Datei " + Param.SourceFullPath + " erstellt.");
            }
            else
            {
                LogMessage.Show("Fehler beim Konvertieren/Erstellen der Sedas.dat.", LogMessage.MsgType.Critical);
                ExitProgram();
            }
            LogMessage.LogOnly("Counter in Config.ini zurückschreiben...");

            bool flag5 = Not INI.Write("Setup", "Counter", Conversions.ToString(Param.Counter));
            if (flag5)
            {
                LogMessage.Show("Zähler konnte nicht in Config.ini zurückgeschrieben werden!" + "\n\r" + "Weitere Programmausführung nicht möglich!", LogMessage.MsgType.Critical);
                ExitProgram();
            }

            bool deleteSourceFile = Param.DeleteSourceFile;
            if (deleteSourceFile)
            {
                LogMessage.LogOnly("Quelldatei wird gelöscht.");
                File.Delete(Param.SourceFullPath);
            }

            LogMessage.LogOnly("--- Programm normal beendet. ---");
            LogMessage.LogOnly("********************************");
            LogMessage.LogOnly(""); ;
        }


        public bool InitProgram()
        {
            bool flag = true;
            LogMessage.LogOnly("Initialisierung des Programms...");
            LogMessage.LogOnly("Einlesen der Config.ini Datei...");

            bool flag2 = File.Exists(Directory.GetCurrentDirectory() + "\\config.ini");
            if (flag2)
            {
                INI = New INIFile(Directory.GetCurrentDirectory() + "\\Config.ini");

                if (!ReadIniValues())
                {
                    LogMessage.Show("Fehler beim Auslesen der Config.ini. Bitte Einstellungen prüfen. Programm wird beendet.", LogMessage.MsgType.Critical);
                    flag = !flag;
                }
                else
                {
                    LogMessage.LogOnly("Einlesen der Config.ini Datei erfolgreich.");
                    LogMessage.LogOnly("Einlesen der Datei loeschKunde.txt...");
                    bool flag4 = File.Exists(Directory.GetCurrentDirectory() + "\\loeschKunde.txt");
                    if (flag4)
                    {
                        ListDelCustomer = New List(Of String)();
                        try
                        {
                            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\loeschKunde.txt"))
                            {
                                if (sr.EndOfStream)
                                {
                                    //Exit While //
                                }
                                string text = sr.ReadLine();
                                if (text != "")
                                {
                                    ListDelCustomer.Add(text);
                                }
                            }

                        }
                        catch (Exception ex)
                        { // expr_157 As Exception


                            LogMessage.LogOnly(ex.ToString());

                            return flag;
                        }
                    }
                    else
                    {
                        try
                        {
                            File.CreateText(Directory.GetCurrentDirectory() + "\\loeschKunde.txt");
                        }
                        catch (Exception ex)
                        {
                            LogMessage.Show("Die Datei 'loeschKunde.txt' hat nicht existiert und eine Neuanlage ist fehlgeschlagen." + "\n\r" + "Das Programm wird beendet", LogMessage.MsgType.Critical);
                            LogMessage.LogOnly(ex.ToString());

                            return flag;
                        }

                    }
                    LogMessage.LogOnly("Einlesen der Datei loeschKunde.txt erfolgreich.");

                    LogMessage.LogOnly("Einlesen der Datei loeschArtikel.txt...");
                    bool flag6 = File.Exists(Directory.GetCurrentDirectory() + "\\loeschArtikel.txt");
                    if (flag6)
                    {
                        ListDelArticle = New List(Of String)();
                        try
                        {
                            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\loeschArtikel.txt"))
                            {

                                if (sr.EndOfStream)
                                {  // endOfStream2 Then
                                   //Exit WHILE(){ //
                                }
                                string text2 = sr.ReadLine();
                                if (text2 != "")
                                {
                                    ListDelArticle.Add(text2);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage.LogOnly(ex.ToString());
                            return flag;
                        }
                    }
                    else
                    {
                        try
                        {
                            File.CreateText(Directory.GetCurrentDirectory() + "\\loeschArtikel.txt");
                        }
                        catch (Exception ex)
                        {
                            LogMessage.Show("Die Datei 'loeschArtikel.txt' hat nicht existiert und eine Neuanlage ist fehlgeschlagen." + "\n\r" + "Das Programm wird beendet", LogMessage.MsgType.Critical);
                            LogMessage.LogOnly(ex.ToString());
                            return flag;
                        }
                        LogMessage.LogOnly("Einlesen der Datei loeschArtikel.txt erfolgreich.");
                        LogMessage.LogOnly("Einlesen der Datei tauscheArtikel.txt...");

                        bool flag8 = File.Exists(Directory.GetCurrentDirectory() + "\\tauscheArtikel.txt");
                        if (flag8)
                        {
                            ListChangeArticle = New List(Of String)();
                            try
                            {
                                using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\tauscheArtikel.txt"))
                                {
                                    if (sr.EndOfStream)
                                    {
                                        // Exit(){ //;
                                    }
                                    string text3 = sr.ReadLine();
                                    if (text3 != "")
                                    {  // flag9 Then
                                        ListChangeArticle.Add(text3);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogMessage.Show("Fehler beim Einlesen der tauscheArtikel.txt." + "\n\r" + "Programm wird beendet.", LogMessage.MsgType.Critical);
                                LogMessage.LogOnly(ex.ToString());
                                return flag;
                            }
                        }
                        else
                        {
                            try
                            {
                                File.CreateText(Directory.GetCurrentDirectory() + "\\tauscheArtikel.txt");
                            }
                            catch (Exception ex)
                            {
                                LogMessage.Show("Die Datei 'tauscheArtikel.txt' hat nicht existiert und eine Neuanlage ist fehlgeschlagen." + "\n\r" + "Das Programm wird beendet", LogMessage.MsgType.Critical);
                                LogMessage.LogOnly(ex.ToString());
                                return flag;

                            }

                            LogMessage.LogOnly("Einlesen der Datei tauscheArtikel.txt erfolgreich.");
                        }
                    }
                }
            }
            else
            {
                LogMessage.Show("! Es wurde keine Config.ini Datei gefunden. Es wird eine neue Config.ini mit " + "\n\r" + "  Standardeinstellungen erstellt.", LogMessage.MsgType.Warning);
                if (!CreateNewConfigIni())
                {
                    LogMessage.Show("Eine neue Config.ini konnte nicht erstellt werden. Programm wird beendet.", LogMessage.MsgType.Critical);
                }
                else
                {
                    LogMessage.Show("Die Datei 'Config.ini' hat nicht existiert und wurde neu angelegt." + "\n\r" + "  Bitte die Einstellungen kontrollieren, bevor das Programm erneut ausgeführt wird.", LogMessage.MsgType.Critical);
                }
                flag = !flag;
            }
            return flag;
        }


        public bool ReadIniValues()
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

        public bool CreateNewConfigIni()
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

        public bool CheckParameters()
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

        public bool CheckSource()
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
        }

        public void ShowMessage(string Message, bool Ignorable = true, bool Pause = false)
        {
            bool flag = Not Param.IgnoreMessages Or Ignorable;
            if (flag)
            {  // (flag)

                Console.WriteLine(Message);
                if (Pause)
                {

                    Console.Write("<Enter> drücken...");
                    Console.ReadLine();

                }
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("DATtoSEDAS-Converter, Version " + MyProject.Application.Info.Version.ToString());
            Console.WriteLine("Wandelt eine Bestell.dat in eine SEDAS.dat um, für den Import in CSB.");
            Console.WriteLine("Wird das Programm ohne Parameter gestartet, werden Pfad und Dateiinformationen");
            Console.WriteLine("aus der Datei Config.ini übernommen.");
            Console.WriteLine("Parameter, die mit Schaltern übergeben werden überschreiben Einstellungen ");
            Console.WriteLine("der Config.ini");
            Console.WriteLine();
            Console.WriteLine("DATtoSEDAS.exe [/Q=][Laufwerk:][Pfad][Dateiname] [/Z=][Laufwerk:][Pfad][Dateiname] [/D] [/I]");
            Console.WriteLine("               [/A] [/?]");
            Console.WriteLine("[Laufwerk:]  Laufwerksbuchstabe, z.B. C:");
            Console.WriteLine("[Pfad]       Verzeichnispfad");
            Console.WriteLine("[Dateiname]  Dateiname");
            Console.WriteLine();
            Console.WriteLine(" /Q=         Laufwerk/Pfad/Dateiname der Quelldatei.");
            Console.WriteLine("                Wird nur ein Dateiname angegeben, wird dieser im Programm-");
            Console.WriteLine("                Verzeichnis gesucht.");
            Console.WriteLine(" /Z=         Laufwerk/Pfad/Dateiname der Zieldatei." + "\n\r" + "                Wird kein Zielpfad angegeben,wird die " + "\n\r" + "                Zieldatei im Programmverzeichnis abgelegt." + "\n\r" + "              ! Existiert eine gleichnamige Zieldatei bereits, wird");
            Console.WriteLine("                sie ohne Rückfrage überschrieben!");
            Console.WriteLine(" /D          Quelldatei wird nach der Konvertierung gelöscht.");
            Console.WriteLine(" /I          Statusmeldungen werden nicht angezeigt (jedoch Fehlermeldungen!).");
            Console.WriteLine(" /?          Ruft diese Hilfeseite auf.");
            Console.WriteLine();
            Console.WriteLine("Beispiel:");
            Console.WriteLine(">> DATtoSEDAS.exe /Q=C:\Daten\Bestell.dat /Z=D:\Import\Sedas.dat /D /I");
            Console.WriteLine(" - Quelldatei wird in Zieldatei eingelesen, die Quelldatei wird anschließend gelöscht.");
            Console.WriteLine("   Statusmeldungen werden nicht ausgegeben.");
            Console.WriteLine();
            Console.WriteLine(">> DATtoSEDAS.exe /Q=C:\Daten\Bestell.dat /I");
            Console.WriteLine(" - Quelldatei wird in Zieldatei eingelesen. Statusmeldungen werden nicht ausgegeben.");
            Console.WriteLine();
            Console.Write("<Enter> drücken...");
            Console.ReadLine();
        }

        public void ExitProgram()
        {
            LogMessage.Show("Programm wird nach Fehler beendet.")
                                                                                                    Environment.[Exit](0);
        }
    }
}
