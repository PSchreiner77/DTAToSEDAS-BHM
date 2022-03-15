using INIManager;
using System;
using ConvertDatToSedas;
using System.IO;
using System.Collections.Generic;

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

            //Korrekturlisten erstellen
            string _pathDeleteCustomer = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
            string _pathDeleteArticle = Directory.GetCurrentDirectory() + @"\loescheArtikel.txt";
            string _pathChangeArticle = Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt";
            ArticleChangeList SedasArticleChangeList = DataProcessing.GetChangeArticlesList(_pathChangeArticle);
            ArticleDeletionList SedasArticleDeletionList = DataProcessing.GetDeleteArticlesList(_pathDeleteArticle);
            CustomerDeletionList SedasCustomerDeletionList = DataProcessing.GetDeleteCustomersList(_pathDeleteCustomer);


            ConvertToSedas newSedas = new ConvertToSedas(Param.Counter,
                                                         SedasArticleChangeList,
                                                         SedasArticleDeletionList,
                                                         SedasCustomerDeletionList);

            List<string> newOrders = DataProcessing.LoadInputFile(Param.SourceFullPath);
            SourceFile newSourceOrders = newSedas.ImportDatFileContent(newOrders);
            SedasFile newSedasFile = newSedas.ToSedas(newSourceOrders, Param.Counter);

            //Daten in Datei schreiben
            DataProcessing.WriteToFile(newSedas.ToString(), Param.DestinationFullPath);

            //Settings zurückschreiben
            RewriteSettingsToConfig();
            //##

            log.Log("--- Programm normal beendet. ---");
            log.Log("********************************");
            log.Log("");
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
    }
}
