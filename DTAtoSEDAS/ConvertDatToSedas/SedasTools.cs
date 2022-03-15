using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public static class SedasTools
    {
        public static string CutLeftStringSide(string Input, int MaxLength)
        {
            //Kürzt einen String vorne auf die angegebene Länge            
            string returnString = "";
            if (Input.Length > MaxLength)
            {
                returnString = Input.Substring(Input.Length - MaxLength);
            }
            return returnString;
        }

        public static string ExpandLeftStringSide(string InputString, int MaxLength)
        {
            //Erweitert einen String links um "0" bis zur angegebenen Länge

            while (InputString.Length < MaxLength)
            {
                InputString = "0" + InputString;
            }

            return InputString;
        }

        /// <summary>
        /// Gibt ein Datum als String zurück in der Form: 'JJMMTT'
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>String: 'JJMMTT'</returns>
        public static string ConvertToSedasDateJJMMTT(DateTime date)
        {
            string JJ = date.Year.ToString().Substring(2, 2);
            string MM = date.Month.ToString();
            string TT = date.Day.ToString();
            if (MM.Length < 2)
                MM = "0" + MM;
            if (TT.Length < 2)
                TT = "0" + TT;
            return JJ + MM + TT;
        }

        /// <summary>
        /// Dreht das Quelldatei-Datumsformat um in das Sedas-Datumsformat: TTMMJJ => JJMMTT
        /// </summary>
        /// <param name="DateTTMMJJ">Quelldatei-Datumsformat: TTMMJJ</param>
        /// <returns></returns>
        public static string ConvertToSedasDateJJMMTT(string DateTTMMJJ)
        {
            string returnString = "";
            for (int i = DateTTMMJJ.Length - 2; i >= 0; i -= 2)
            {
                returnString += DateTTMMJJ.Substring(i, 2);
            }
            return returnString;
        }
    }
}
