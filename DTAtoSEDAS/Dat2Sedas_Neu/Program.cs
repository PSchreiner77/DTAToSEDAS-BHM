using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu
{
    class Program
    {
        //TODO ShowMessage Einträge gegen LogMessage austauschen

        //private string[] Arguments = Environment.GetCommandLineArgs(); //TODO CommandlineArgs besorgen
        private INIFile INI;  //TODO INI-DLL implementieren

        public Parameters Param = Parameters.GetInstance;

        static void Main()
        {
            Program prog = new Program();
            prog.ProgramLoop();
        }

        public void ProgramLoop()
        {
            //Log-Messages ausgeben, DLL implementieren
            //LogMessage.GlobalLog = true;    
            //LogMessage.GlobalOutputToConsole = true;
            //LogMessage.LogOnly("**********************************");
            //LogMessage.LogOnly("--------- PROGRAMMSTART ----------");
            //LogMessage.CheckLogFile(100);

            ProgramInit.InitError += ProgramInit_InitFailed;
            ProgramInit.InitNotification += ProgramInit_InitNotification;

            if (!ProgramInit.Init()) { ExitProgram(); }

            Param.Counter = SetCounter(Param.Counter);

            ConvertDatToSedas D2S = new ConvertDatToSedas(Param.SourceFileFolder, Param.DestinationFileFolder);
            D2S.CreateSedasData();
            D2S.WriteSedasData();

            RewriteSettings();

            //LogMessage.LogOnly("--- Programm normal beendet. ---");
            //LogMessage.LogOnly("********************************");
            //LogMessage.LogOnly(""); ;
        }

        private void ProgramInit_InitNotification(string message)
        {
            ShowMessage(message, Pause: false);
        }

        private void ProgramInit_InitFailed(string message)
        {
            ShowMessage(message, Pause: true);
        }

        public int SetCounter(int counter)
        {
            if (counter >= 999)
            {
                counter = 0;
            }
            return ++counter;
        }

        public void RewriteSettings()
        {
            //TODO Save settings back to INI-File

        }

        public void ShowMessage(string Message, bool Pause = false)
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

        public static void ExitProgram()
        {
            //LogMessage.Show("Programm wird nach Fehler beendet.")
            Environment.Exit(0);
        }
    }
}
