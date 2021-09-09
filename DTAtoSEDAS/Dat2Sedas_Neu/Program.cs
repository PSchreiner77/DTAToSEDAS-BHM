using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INIManager;

namespace Dat2Sedas_Neu
{
    class Program
    {
        //TODO ShowMessage Einträge gegen LogMessage austauschen

        //private string[] Arguments = Environment.GetCommandLineArgs(); //TODO CommandlineArgs besorgen       

        public Parameters Param;
        private Logger log;

        static void Main()
        {
            Program prog = new Program();              
            prog.ProgramLoop();
            Console.WriteLine("Testhalt");Console.ReadKey();
        }
       
        private int SetCounter(int counter)
        {
            if (counter >= 990)
            {
                //TODO Meldung: Counter zurückgesetzt von Counter auf 1
                Console.WriteLine("Counter von {0} zurückgesetzt.");
                counter = 0;

            }
            return ++counter;
        }

        private void RewriteSettings()
        {
            //TODO Rückschreiben der Config.Datei prüfen.
            log.Log("Zurückschreiben der Einstellungen...", "Speichern", Logger.MsgType.Message) ;
            log.Log("...Counter aktualisieren...","Speichern",Logger.MsgType.Message);
            IniManager INI = new IniManager(Param.INIFilePath);
            INI.UpdateParameterValue("Setup", "Counter", Param.Counter.ToString());
            INI.WriteIniFile();
            log.Log("...zurückschreiben beendet.","Speichern",Logger.MsgType.Message);
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
            log.MaxLogfileLines = 100;
            log.OutputMedium = Logger.Output.Console;

            log.Log("**********************************");
            log.Log("--------- PROGRAMMSTART ----------");
            log.Log(log.GetLoggerSettings());

            Param = Parameters.GetInstance;

            if (!ProgramInit.Init()) { ExitProgram(); }
            log.HaltOnCriticalErrors = Param.IgnoreCriticalMessages;
            Param.Counter = SetCounter(Param.Counter);

            ConvertDatToSedas D2S = new ConvertDatToSedas(Param.SourceFullPath, Param.DestinationFullPath, Param.Counter);
            D2S.CreateSedasData();
            D2S.WriteSedasData();

            RewriteSettings();

            log.Log("--- Programm normal beendet. ---");
            log.Log("********************************");
            log.Log("");
        }
    }
}
