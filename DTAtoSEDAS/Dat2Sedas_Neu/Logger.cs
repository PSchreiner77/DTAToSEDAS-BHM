using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Dat2Sedas_Neu
{
    public class Logger
    {
        #region SINGLETON 
        private static Logger logger;
        public static Logger GetInstance()
        {
            if (logger == null)
            {
                logger = new Logger();
                logger.HaltOnCriticalErrors = true;
                logger.MaxLogfileLines = 500;
                logger.LogfileName = "Logfile.txt";
                //string loggerStart = $"Logger gestartet: {DateTime.Now.ToString()}\n{logger.GetLoggerSettings()}";
                //logger.WriteToLogfile(loggerStart, "", MsgType.Message);
            }

            return logger;
        }
        #endregion

        #region ENUMS
        public enum MsgType
        {
            Message,
            Warning,
            Critical,
        }

        public enum Output
        {
            LogOnly = 0,
            Console = 1,
            Window = 2
        }
        #endregion

        #region FIELDs
        #endregion

        #region Properties
        public bool HaltOnCriticalErrors { get; set; }
        public Output OutputMedium { get; set; }
        private int _maxLogfileLines;
        public int MaxLogfileLines
        {
            get { return _maxLogfileLines; }
            set { if (value < 50) { _maxLogfileLines = 50; } else { _maxLogfileLines = value; } }
        }
        public int LinesToDelete { get => MaxLogfileLines - 100 < 50 ? 50 : MaxLogfileLines - 100; }
        public string LogfileName { get; set; }
        public string GetLogfilePath
        {
            get
            {
                string path = Directory.GetCurrentDirectory() + "\\" + this.LogfileName;
                return path;
            }
        }
        #endregion

        #region METHODS
        #region Privat
        private void WriteToLogfile(string msg, string prefix, MsgType type)
        {
            string message = $"{DateTime.Now.ToString()} {prefix} - {msg}\n";
            IEnumerable<string> logMessageText = message.Split('\n');

            try
            {
                CheckLogFileLength(logMessageText.Count());
                File.AppendAllLines(GetLogfilePath, logMessageText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DisplayMessage(string prefix, string msg, string title, MsgType type, MessageBoxIcon messageIcon)
        {
            switch (OutputMedium)
            {
                case Output.LogOnly:
                    break;
                case Output.Console:
                    ShowConsoleMessage(prefix, msg);
                    if (type == MsgType.Critical & HaltOnCriticalErrors)
                    {
                        Console.WriteLine("(weiter mit Taste)");
                        Console.ReadKey();
                    }
                    break;
                case Output.Window:
                    if (type == MsgType.Critical & HaltOnCriticalErrors)
                    {
                        ShowWindowMessage(msg, title, messageIcon);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ShowWindowMessage(string msg, string title, MessageBoxIcon messageIcon)
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, messageIcon);
        }

        private void ShowConsoleMessage(string prefix, string msg)
        {
            Console.WriteLine("{0:-10} - {1}", prefix, msg);
        }

        private void CheckLogFileLength(int linesToAdd)
        {
            try
            {
                List<string> actualLogfileLines = File.ReadAllLines(GetLogfilePath).ToList<string>();

                List<string> newlLogfileLines = new List<string>();
                if (actualLogfileLines.Count >= MaxLogfileLines)
                {
                    newlLogfileLines.Add($"---===### Logdatei um {LinesToDelete} Zeilen gekürzt ###===---");
                    newlLogfileLines.AddRange(actualLogfileLines.GetRange(actualLogfileLines.Count - LinesToDelete, LinesToDelete));
                    File.WriteAllLines(GetLogfilePath, newlLogfileLines);
                }

            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Public
        public void Log(string msg)
        { Log(msg, "", MsgType.Message, Output.LogOnly); }

        public void Log(string msg, MsgType type)
        { Log(msg, "", type, OutputMedium); }

        public void Log(string msg, Output OutputMedium)
        { Log(msg, "", MsgType.Message, OutputMedium); }

        public void Log(string msg, MsgType type, Output OutputMedium)
        { Log(msg, "", type, OutputMedium); }

        public void Log(string msg, string title)
        { Log(msg, title, MsgType.Message, this.OutputMedium); }

        public void Log(string msg, string title, MsgType type)
        { Log(msg, title, type, this.OutputMedium); }

        public void Log(string msg, string title, Output OutputMedium)
        { Log(msg, title, MsgType.Message, OutputMedium); }

        public void Log(string msg, string title, MsgType type, Output OutputMedium)
        {
            MessageBoxIcon icon = new MessageBoxIcon();
            string prefix = "";

            switch (type)
            {
                case MsgType.Message:
                    title = "Nachricht";
                    prefix = "[Meldung]";
                    icon = MessageBoxIcon.Error;
                    break;
                case MsgType.Warning:
                    title = "Warnung";
                    prefix = "[WARNUNG]";
                    break;
                case MsgType.Critical:
                    title = "Kritischer Fehler!";
                    prefix = "[**FEHLER * *]";
                    break;
            }

            WriteToLogfile(msg, title, type);
            DisplayMessage(prefix, msg, title, type, icon);
        }

        public string GetLoggerSettings()
        {
            string returnString = $"Logger-Settings:\n" +
                                  $"Speicherort Logdatei: {GetLogfilePath}\n" +
                                  $"Standardausgabe: {OutputMedium}\n" +
                                  $"HaltOnAllErrors: {HaltOnCriticalErrors}\n" +
                                  $"Maximale Zeilen: {MaxLogfileLines}";
            return returnString;
        }
        #endregion
        #endregion
    }
}

