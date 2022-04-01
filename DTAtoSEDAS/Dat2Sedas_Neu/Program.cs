using INIManager;
using System;
using ConvertDatToSedas;
using System.IO;
using System.Collections.Generic;

namespace Dat2Sedas_Neu
{
    class Program
    {
        public ArticleChangeList sedasArticleChangeList { get; set; }
        public ArticleDeletionList sedasArticleDeletionList { get; set; }
        public CustomerDeletionList sedasCustomerDeletionList { get; set; }

        public Parameters Param;
        private Logger log;


        static void Main()
        {
            Program prog = new Program();
            prog.ProgramLoop();

            // ### TEST
            Console.WriteLine("Testhalt");
            Console.ReadKey();
            // ###
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
            {
                ExitProgram();
            }

            log.HaltOnCriticalErrors = Param.IgnoreCriticalMessages; //TODO Widersprüchliche Angabe von True/False

            Param.Counter = SetCounter(Param.Counter);

            ConvertToSedas SedasConverter = new ConvertToSedas();
            SedasConverter.LogEventHandler += NewSedas_SedasLogEventHandler; // Sedas LogEventHandler abonnieren

            List<string> newOrders = DataProcessing.LoadInputFile(Param.SourceFullPath);
            DatFile newSourceOrders = SedasConverter.ImportDatFileContent(newOrders);
            SedasFile newSedasFile = SedasConverter.ToSedas(newSourceOrders, Param.Counter);

            CorrectionLists correctionLists = new CorrectionLists();
            correctionLists.Generate();
            newSedasFile.RemoveCustomers(correctionLists.sedasCustomerDeletionList);
            newSedasFile.RemoveArticles(correctionLists.sedasArticleDeletionList);
            newSedasFile.ChangeArticles(correctionLists.sedasArticleChangeList);

            DataProcessing.WriteToFile(newSedasFile.ToString(), Param.DestinationFullPath);
            RewriteSettingsToConfig();

            log.Log("--- Programm normal beendet. ---");
            log.Log("********************************");
            log.Log("");
        }

        private void NewSedas_SedasLogEventHandler(object sender, string message, LogMessageLevel level)
        {
            Console.WriteLine($"{level.ToString().ToUpper()}: {message}");
            Logger.MsgType type = Logger.MsgType.Message;
            string logMessage = $"[{sender}] [{level}] - {message}";

            switch (level)
            {
                case LogMessageLevel.Information:
                    type = Logger.MsgType.Message;
                    break;
                case LogMessageLevel.Warning:
                    type = Logger.MsgType.Warning;
                    break;
                case LogMessageLevel.Critical:
                    break;
                case LogMessageLevel.Error:
                    type = Logger.MsgType.Critical;
                    break;
                default:
                    break;
            }
            this.log.Log(logMessage, type);
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
