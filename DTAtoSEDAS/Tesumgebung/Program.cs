using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesumgebung
{
    internal class Program
    {
        static void Main(string[] args)
        {

           

        }


    }

    public sealed class Logger
    {
        private static Logger instance = new Logger();

        public static void Log(string Typ, string Message)
        {
            instance.NeueMeldung(Typ, Message);
        }

        private ILogger _logger;
        public static void SetLogger(ILogger newLogger)
        {
            instance._logger = newLogger;
        }

        private void NeueMeldung(string Typ, string Message)
        {
            
        }
    }


    interface ILogger
    {
        void NeueMeldung(string Typ, string Message);
        List<string> GetMeldungen() ;
    }

    public class ConsoleLogger : ILogger
    {
        public List<string> GetMeldungen()
        {
            throw new NotImplementedException();
        }

        public void NeueMeldung(string Typ, string Message)
        {
            throw new NotImplementedException();
        }
    }



    ////Der Logger
    ///// <summary> 
    /////  Ein global verfügbarer Logger. 
    /////  </summary> 
    //public sealed class Logger
    //{
    //    #region Statische Member 
    //    private static readonly Logger _instanz = new Logger();
    //    /// <summary> 
    //    /// Loggt die übergebene Meldung. 
    //    /// </summary> 
    //    /// <param name="typ">Typ der Meldung.</param>
    //    /// <param name="meldung">Text der Meldung.</param>
    //    public static void Log(LogEintragTyp typ, string meldung)
    //    {
    //        _instanz.NeueMeldung(typ, meldung);
    //    }

    //    /// <summary>
    //    /// Setzt den Logger, der zur Ausgabe der Meldungen verwendet werden soll. 
    //    /// </summary>
    //    /// <param name="logger"></param> 
    //    public static void SetLogger(ILogger logger)
    //    {
    //        _instanz._logger = logger;
    //    }
    //    #endregion

    //    #region Instanzmember 
    //    private ILogger _logger = null;
    //    private List<LogEintrag> _eintraege = new List<LogEintrag>();
    //    private void NeueMeldung(LogEintragTyp typ, string meldung)
    //    {
    //        LogEintrag le = new LogEintrag(typ, meldung);
    //        this._eintraege.Add(le);
    //        if (this._logger != null)
    //            this._logger.NeueMeldung(le);
    //    }
    //    #endregion
    //}

    ////Logger-Implementierung
    //public interface ILogger
    //{
    //    /// <summary> 
    //    /// Wird aufgerufen, sobald eine neue Meldung geloggt wird. 
    //    /// </summary> 
    //    /// <param name="meldung">Die neue Meldung.</param> 
    //    void NeueMeldung(LogEintrag meldung);

    //    /// <summary> 
    //    /// Gibt die Liste aller geloggten Meldungen zurück.
    //    /// </summary> 
    //    /// <returns>Die Liste aller geloggten Meldungen.</returns> 
    //    List<LogEintrag> GetMeldungen();
    //}


    ////Zur farbigen Ausgabe auf der Konsole habe ich die hier beschriebene Klasse verwendet: Putting colour/color to work on the console.
    ///// <summary> 
    ///// Ein Logger, der die Meldungen lediglich (farbig) auf der Konsole ausgibt. 
    ///// </summary> 
    //public class ConsoleLogger : ILogger
    //{
    //    #region ConsoleColor 
    //    /// <summary> 
    //    /// Static class for console colour manipulation. 
    //    /// (http://www.codeproject.com/csharp/console_apps__colour_text.asp) 
    //    /// </summary> 
    //    private class ConsoleColor
    //    {
    //        // source code of ConsoleColour from http://www.codeproject.com/csharp/console_apps__colour_text.asp
    //    }
    //    #endregion

    //    #region ILogger Member 
    //    /// <summary> 
    //    /// Gibt eine neue Meldung auf der Konsole aus. 
    //    /// </summary> 
    //    /// <param name="meldung">Die neue Meldung.</param> 
    //    public void NeueMeldung(LogEintrag meldung)
    //    {
    //        switch (meldung.Typ)
    //        {
    //            case LogEintragTyp.Erfolg:
    //                ConsoleColor.SetForeGroundColour(ConsoleColor.ForeGroundColour.Green);
    //                break;
    //            case LogEintragTyp.Fehler:
    //                ConsoleColor.SetForeGroundColour(ConsoleColor.ForeGroundColour.Red);
    //                break;
    //            default:
    //                break;
    //        }
    //        Console.WriteLine(meldung.Zeitpunkt.ToString("HH:mm:ss.ffffff") + " " + meldung.Typ.ToString() + ": " + meldung.Text);
    //        ConsoleColor.SetForeGroundColour();
    //    }

    //    /// <summary> 
    //    /// Nicht implementiert. 
    //    /// </summary> 
    //    /// <returns>-</returns>
    //    public List<LogEintrag> GetMeldungen()
    //    {
    //        throw new Exception("The method or operation is not implemented.");
    //    }
    //    #endregion 
    //}

    //// Log-Einträge
    ///// <summary> 
    ///// Mögliche Typen eines Log-Eintrags. 
    ///// </summary> 
    //public enum LogEintragTyp
    //{
    //    /// <summary>
    //    /// Erfolgsmeldung.
    //    /// </summary> 
    //    Erfolg,
    //    /// <summary> 
    //    /// Fehlermeldung. 
    //    /// </summary>
    //    Fehler,
    //    /// <summary> 
    //    /// Hinweis. 
    //    /// </summary> 
    //    Hinweis
    //}

    ///// <summary> 
    ///// Eine Meldung für das Log.
    ///// </summary> 
    //public struct LogEintrag
    //{
    //    /// <summary> 
    //    /// Der Typ der aktuellen Meldung.
    //    /// </summary> 
    //    public LogEintragTyp Typ;

    //    /// <summary> 
    //    ///Der Text der aktuellen Meldung.
    //    ///</summary> 
    //    public string Text;

    //    /// <summary> 
    //    /// Zeitpunkt zu dem die Meldung erstellt wurde.
    //    /// </summary> 
    //    public DateTime Zeitpunkt;


    //    ///<summary> 
    //    /// Konstruktor. 
    //    /// </summary> 
    //    /// <param name="typ">Der Typ der neuen Meldung.</param> 
    //    /// <param name="text">Der Text der neuen Meldung.</param> 
    //    public LogEintrag(LogEintragTyp typ, string text)
    //    {
    //        this.Typ = typ;
    //        this.Text = text;
    //        this.Zeitpunkt = DateTime.Now;
    //    }

    //    /// <summary> 
    //    /// Gibt Typ und Text der aktuellen Meldung aus. 
    //    /// </summary> 
    //    /// <returns>Typ und Text der aktuellen Meldung.</returns> 
    //    public override string ToString()
    //    {
    //        return this.Typ.ToString() + ": " + this.Text;
    //    }
    //}
}
