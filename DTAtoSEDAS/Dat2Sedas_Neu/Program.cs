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

        static void Main()
        {
            Program prog = new Program();
            prog.ProgramLoop();
            Console.ReadKey();
        }

        private void ProgramInit_InitNotification(string message)
        {
            ShowMessage(message, Pause: false);
        }

        private void ProgramInit_InitFailed(string message)
        {
            ShowMessage(message, Pause: true);
        }

        private int SetCounter(int counter)
        {
            if (counter >= 999)
            {
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
            //Log-Messages ausgeben, DLL implementieren
            //LogMessage.GlobalLog = true;    
            //LogMessage.GlobalOutputToConsole = true;
            //LogMessage.LogOnly("**********************************");
            //LogMessage.LogOnly("--------- PROGRAMMSTART ----------");
            //LogMessage.CheckLogFile(100);

            Param = Parameters.GetInstance;
            ProgramInit.InitError += ProgramInit_InitFailed;
            ProgramInit.InitNotification += ProgramInit_InitNotification;

            if (!ProgramInit.Init()) { ExitProgram(); }

            Param.Counter = SetCounter(Param.Counter);

            ConvertDatToSedas D2S = new ConvertDatToSedas(Param.SourceFullPath, Param.DestinationFullPath);
            D2S.CreateSedasData();
            D2S.WriteSedasData();

            RewriteSettings();

            //LogMessage.LogOnly("--- Programm normal beendet. ---");
            //LogMessage.LogOnly("********************************");
            //LogMessage.LogOnly(""); ;
        }
    }
}
