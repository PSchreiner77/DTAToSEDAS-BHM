using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            Console = 1
        }
        #endregion

        #region FIELDs
        private int _MaxLogfileLines;
        #endregion

        #region Properties

        public bool GlobalLog { get; set; } = true;
        public bool GlobalOutputToConsole { get; set; } = false;

        public int MaxLogfileLines { get; set; }
        public string LogfileName { get; set; }
        public string GetLogfilePath
        {
            get
            {
                if (LogfileName == "") LogfileName = "Logfile.txt"; return Directory.GetCurrentDirectory() + "\\" + LogfileName;
            }
        }
        #endregion

        #region Methods
        public void Show(string msg)
        { ShowMessage(msg, "", MsgType.Message, 1); }

        public void Show(string msg, MsgType type)
        { ShowMessage(msg, "", type, 0); }

        public void Show(string msg, Output output)
        { ShowMessage(msg, "", MsgType.Message, 1); }

        public void Show(string msg, MsgType type, Output output)
        { ShowMessage(msg, "", type, 1); }

        public void Show(string msg, string title)
        { ShowMessage(msg, title, MsgType.Message, 1); }

        public void Show(string msg, string title, MsgType type)
        { ShowMessage(msg, title, type, 1); }

        public void Show(string msg, string title, Output output)
        { ShowMessage(msg, title, MsgType.Message, 1); }

        public void Show(string msg, string title, MsgType type, Output output)
        { ShowMessage(msg, title, type, 1); }




        public void LogOnly(string msg)
        { WriteToLogfile(msg, MsgType.Message); }

        public void LogOnly(string msg, MsgType type)
        { LogOnly(msg, type); }




        private void ShowMessage(string msg, string title, MsgType type, int output)
        {
            string prefix = "";
            switch (type)
            {
                case MsgType.Message:
                    title = "Nachricht";
                    prefix = "Meldung";
                    break;
                case MsgType.Warning:
                    title = "Warnung";
                    prefix = "[Warnung]";
                    break;
                case MsgType.Critical:
                    title = "Kritischer Fehler!";
                    prefix = "[**FEHLER * *]";
                    break;
            }


            bool flag4 = !GlobalOutputToConsole & output == 0;
            if (!flag4)
            {
                if (type == MsgType.Critical)
                {
                    WriteToConsole(prefix, msg);
                    Console.Write("Bitte ENTER drücken...");
                    Console.ReadLine();
                }
                else
                {
                    WriteToConsole(prefix, msg);
                }
            }

            if (GlobalLog)
            {
                WriteToLogfile(msg, type);
            }
        }

        private void WriteToConsole(string prefix, string msg)
        {
            string array = Strings.Split(msg, vbCrLf, -1, CompareMethod.Binary);
            List<string> list = new List<string>();
            int num = 80;
            string[] array2 = new string[] { };

            // The following expression was wrapped in a checked-statement
            for (int i = 0; i < array2.Length; i++)
            {
                string text = array2[i];
                while (text.Length > num)
                {
                    list.Add(text.Substring(1, num));
                    text = text.Substring(num + 1);
                }
                list.Add(text);
            }

            int num2 = list.Count - 1;
            for (int j = 0; j <= num2; j++)
            {
                if (j == 0)
                {
                    Console.WriteLine(String.Format("{0,-12}: {1}", prefix, list(j)));
                }
                else
                {
                    Console.WriteLine("{0,-12}: {1}", "", list(j));
                }
            }
        }



        private void WriteToLogfile(string msg, MsgType type)
        {
            string[] array = Strings.Split(msg, vbCrLf, -1, CompareMethod.Binary);
            List<string> list = new List<string>();
            string arg = "";
            int num = Strings.Len(DateAndTime.Now.ToString());

            switch (type)
            {
                case MsgType.Message:
                    arg = MessagePrefix;
                    break;
                case MsgType.Warning:
                    arg = WarningPrefix;
                    break;
                case MsgType.Critical:
                    arg = CriticalPrefix;
                    break;
            }

            msg = msg.Replace("\n\r", " ");
            try
            {
                File.AppendAllText(_Path, String.Format("{0};{1,15} ;{2}", DateAndTime.Now.ToString(), arg, msg) + vbCrLf);
            }
            catch (Exception expr_AC)
            {
                Console.WriteLine(expr_AC.ToString());
            }
        }

        private void CheckLogFileLength()
        {
            
            try
            {
                List<string> actualLogfileLines = File.ReadAllLines(GetLogfilePath);
                
                if(actualLogfileLines.Length >= MaxLogfileLines)
                {
                    
                }


            }
            catch (Exception ex)
            {

            }

            string text = "";
            List<string> list = new List<string>();
            string text2 = "";
            bool flag = Not File.Exists(_Path);
            bool flag2;
            if (flag)
            {
                flag2 = flag2;
            }
            else
            {
                long num = MyProject.Computer.FileSystem.GetFileInfo(_Path).Length;
                long num2 = num;
                bool flag3 = num2 > 1000L;
                if (flag3)
                {
                    num /= 1000L;
                    text2 = "KB";
                }

                bool flag4 = num > CLng(maxSizeKB);

                ' The following expression was wrapped in a checked-statement
                    if (flag4)
                {
                    LogOnly("Verkleinern der Logdatei von ", num.ToString(), text2, " auf ca. ", (num / 2L).ToString() + text2);
                    using (StreamReader sr = new StreamReader(_Path))
                    {
                        text = sr.ReadToEnd();
                    }

                    string[] array = Strings.Split(text, vbCrLf, -1, CompareMethod.Binary);
                    int num3 = array.Count() / 2;
                    int upperBound = array.GetUpperBound(0);
                    for (int i = num3; i < upperBound; i++)
                    {
                        list.Add(array(i));
                    }

                    File.Delete(_Path);
                    text = "";
                    LogOnly("######## Logdatei verkleinert ##########");

                    try
                    {
                        List<string> enumerator = new List<string>().Enumerator;
                        enumerator = list.GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            text = text + current + "\n\r";
                        }
                    }
                    catch (Exception ex)
                    { }
                    finally
                    {
                        Dim enumerator As List(Of String).Enumerator
                            CType(enumerator, IDisposable).Dispose()
                        }
                    File.AppendAllText(_Path, text)
                }
            }
            return flag2;
        } 
    
        #endregion
    }

}
