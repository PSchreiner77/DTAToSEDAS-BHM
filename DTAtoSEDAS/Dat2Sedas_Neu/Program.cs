using INIManager;
using System;
using ConvertDatToSedas;
using System.IO;

namespace Dat2Sedas_Neu
{
    class Program
    {
        public Parameters Param;
        private Logger log;

        static void Main()
        {
            Program prog = new Program();
            prog.ProgramLoop();
            Console.WriteLine("Testhalt");
            Console.ReadKey();
        }

        private int SetCounter(int counter)
        {
            if (counter >= 990)
            {
                Console.WriteLine("Counter von {0} zurückgesetzt.");
                counter = 0;
            }
            return ++counter;
        }

        private void RewriteSettingsToConfig()
        {
            log.Log("Zurückschreiben der Einstellungen...", "Speichern", Logger.MsgType.Message);
            log.Log("...Counter aktualisieren...", "Speichern", Logger.MsgType.Message);
            IniManager INI = new IniManager(Param.INIFilePath);
            INI.UpdateParameterValue("Setup", "Counter", Param.Counter.ToString());
            INI.WriteIniFile();
            log.Log("...zurückschreiben beendet.", "Speichern", Logger.MsgType.Message);
        }

        private void ExitProgram()
        {
            log.Log("Programm wird nach Fehler beendet.", "Programmabbruch", Logger.MsgType.Critical);
            Console.ReadKey();
            Environment.Exit(0);
        }

        public void ProgramLoop()
        {
            log = Logger.GetInstance();
            log.HaltOnCriticalErrors = true;
            log.MaxLogfileLines = 500;
            log.OutputMedium = Logger.Output.Console;

            log.Log("**********************************");
            log.Log("--------- PROGRAMMSTART ----------");
            log.Log(log.GetLoggerSettings());

            //Parameter abrufen
            Param = Parameters.GetInstance;

            if (!ProgramInit.Init())
            { ExitProgram(); }
            log.HaltOnCriticalErrors = Param.IgnoreCriticalMessages; //TODO Widersprüchliche Angabe von True/False



            //Counter setzen
            Param.Counter = SetCounter(Param.Counter);
            string _pathDeleteCustomer = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
            string _pathDeleteArticle = Directory.GetCurrentDirectory() + @"\loescheArtikel.txt";
            string _pathChangeArticle = Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt";


            ConvertToSedas newSedas = new ConvertToSedas(Param.Counter,
                                                         DataProcessing.GetChangeArticlesList(_pathChangeArticle),
                                                         DataProcessing.GetDeleteArticlesList(_pathDeleteArticle),
                                                         DataProcessing.GetDeleteCustomersList(_pathDeleteCustomer));

            newSedas.ConvertDatFile(Param.SourceFullPath);


            //## Daten konvertieren und schreiben
            //Sedas-Objekt erstellen mit aktuellem Datum
            ConvertToSedas DatToSedas = new ConvertToSedas(Param.SourceFullPath, Param.DestinationFullPath, Param.Counter);

            //Daten zusammenstellen
            DatSource sdf = DatToSedas.ImportSourceDatFile(Param.SourceFullPath);
            DatToSedas.CreateSedasFile(sdf);

            //Daten in Datei schreiben
            DatToSedas.WriteSedasData();

            //Settings zurückschreiben
            RewriteSettingsToConfig();
            //##

            log.Log("--- Programm normal beendet. ---");
            log.Log("********************************");
            log.Log("");
        }
    }
}
