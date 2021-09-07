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
            prog.log.HaltOnAllErrors = false;
            prog.log.MaxLogfileLines = 100;
            prog.log.OutputMedium = Logger.Output.Console;

            prog.ProgramLoop();
            Console.ReadKey();
        }

        private void ProgramInit_InitNotification(string message)
        {
            log.Log(message, Logger.MsgType.Message);
        }

        private void ProgramInit_InitFailed(string message)
        {
            log.Log(message, Logger.MsgType.Message);
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
            ShowMessage("Zurückschreiben der Einstellungen...", false);
            ShowMessage("...Counter aktualisieren...");
            IniManager INI = new IniManager(Param.INIFilePath);
            INI.UpdateParameterValue("Setup", "Counter", Param.Counter.ToString());
            INI.WriteIniFile();
            ShowMessage("...zurückschreiben beendet.");
        }

        private void ShowMessage(string Message, bool Pause = false)
        {
            if (!Param.IgnoreMessages)
            {
                Console.WriteLine(Message);
                if (Pause)
                {
                    Console.Write("<Enter> drücken...");
                    Console.ReadLine();
                }
            }
        }

        private static void ExitProgram()
        {
            //LogMessage.Show("Programm wird nach Fehler beendet.")
            Environment.Exit(0);
        }

        public void ProgramLoop()
        {
            log = Logger.GetInstance();
            log.HaltOnAllErrors = false;
            log.MaxLogfileLines = 100;
            log.OutputMedium = Logger.Output.Console;

            log.Log("**********************************");
            log.Log("--------- PROGRAMMSTART ----------");
            log.Log(log.GetLoggerSettings());

            Param = Parameters.GetInstance;
            ProgramInit.InitError += ProgramInit_InitFailed;
            ProgramInit.InitNotification += ProgramInit_InitNotification;

            if (!ProgramInit.Init()) { ExitProgram(); }

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
