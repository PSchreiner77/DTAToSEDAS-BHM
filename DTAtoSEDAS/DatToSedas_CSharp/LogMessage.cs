using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatToSedas_CSharp
{
    public enum MsgType
    {
        Message,
        Warning,
        Critical,
    }

    enum Output
    {
        Console = 1
    }

    class LogMessage
    {


        private static string MessagePrefix = "Meldung";
        private static string WarningPrefix = "[Warnung]";
        private static string CriticalPrefix = "[**FEHLER**]";
        private static string _Path = Directory.GetCurrentDirectory() + "\\DatToSedas.log";

        public static bool GlobalLog { get; set; } = true;
        public static bool GlobalOutputToConsole { get; set; } = false;


        public static void Show(string msg)
        { LogMessage.ShowMessage(msg, "", MsgType.Message, 1); }

        public static void Show(string msg, MsgType type)
        { LogMessage.ShowMessage(msg, "", type, 0); }

        public static void Show(string msg, Output output)
        { LogMessage.ShowMessage(msg, "", MsgType.Message, 1); }

        public static void Show(string msg, MsgType type, Output output)
        { LogMessage.ShowMessage(msg, "", type, 1); }

        public static void Show(string msg, string title)
        { LogMessage.ShowMessage(msg, title, MsgType.Message, 1); }

        public static void Show(string msg, string title, MsgType type)
        { LogMessage.ShowMessage(msg, title, type, 1); }

        public static void Show(string msg, string title, Output output)
        { LogMessage.ShowMessage(msg, title, MsgType.Message, 1); }

        public static void Show(string msg, string title, MsgType type, Output output)
        { LogMessage.ShowMessage(msg, title, type, 1); }

        public static void LogOnly(string msg)
        { LogMessage.WriteToLogfile(msg, MsgType.Message); }

        public static void LogOnly(string msg, MsgType type)
        { LogMessage.LogOnly(msg, type); }



        private static void ShowMessage(string msg, string title, MsgType type, int output)
        {
            string prefix = "";
            switch (type)
            {
                case MsgType.Message:
                    title = "Nachricht";
                    prefix = LogMessage.MessagePrefix;
                    break;
                case MsgType.Warning:
                    title = "Warnung";
                    prefix = LogMessage.WarningPrefix;
                    break;
                case MsgType.Critical:
                    title = "Kritischer Fehler!";
                    prefix = LogMessage.CriticalPrefix;
                    break;
            }


            bool flag4 = !LogMessage.GlobalOutputToConsole & output == 0;
            if (!flag4)
            {
                if (type == MsgType.Critical)
                {
                    LogMessage.WriteToConsole(prefix, msg);
                    Console.Write("Bitte ENTER drücken...");
                    Console.ReadLine();
                }
                else
                {
                    LogMessage.WriteToConsole(prefix, msg);
                }
            }

            if (LogMessage.GlobalLog)
            {
                LogMessage.WriteToLogfile(msg, type);
            }
        }


        private static void WriteToConsole(string prefix, string msg)
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

        private static void WriteToLogfile(string msg, MsgType type)
        {
            string[] array = Strings.Split(msg, vbCrLf, -1, CompareMethod.Binary);
            List<string> list = new List<string>();
            string arg = "";
            int num = Strings.Len(DateAndTime.Now.ToString());

            switch (type)
            {
                case MsgType.Message:
                    arg = LogMessage.MessagePrefix;
                    break;
                case MsgType.Warning:
                    arg = LogMessage.WarningPrefix;
                    break;
                case MsgType.Critical:
                    arg = LogMessage.CriticalPrefix;
                    break;
            }

            msg = msg.Replace("\n\r", " ");
            try
            {
                File.AppendAllText(LogMessage._Path, String.Format("{0};{1,15} ;{2}", DateAndTime.Now.ToString(), arg, msg) + vbCrLf);
            }
            catch (Exception expr_AC)
            {
                Console.WriteLine(expr_AC.ToString());
            }
        }

        public bool CheckLogFile(int maxSizeKB)
        {
            string text = "";
            List<string> list = new List<string>();
            string text2 = "";
            bool flag = Not File.Exists(LogMessage._Path);
            bool flag2;
            if (flag)
            {
                flag2 = flag2;
            }
            else
            {
                long num = MyProject.Computer.FileSystem.GetFileInfo(LogMessage._Path).Length;
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
                    LogMessage.LogOnly("Verkleinern der Logdatei von ", num.ToString(), text2, " auf ca. ", (num / 2L).ToString() + text2);
                    using (StreamReader sr = new StreamReader(LogMessage._Path))
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

                    File.Delete(LogMessage._Path);
                    text = "";
                    LogMessage.LogOnly("######## Logdatei verkleinert ##########");

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
                    File.AppendAllText(LogMessage._Path, text)
                }
            }
            return flag2;
        }
    }
}
