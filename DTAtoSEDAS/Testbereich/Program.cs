using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testbereich
{
    class Program
    {
        private List<string> Disney = new List<string>();
        private int MaxLogfileLines = 5;
        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.Disney = new List<string> { "Tick", "Trick", "Track", "Donald", "Daisy", "Goofy", "MickyMaus", "Pluto", "MiniMaus" };

            prog.CheckLogFileLength(3);
            Console.Clear();
            foreach (string element in prog.Disney)
            {
                Console.WriteLine(element);
            }
            Console.ReadKey();
        }


        private void CheckLogFileLength(int LinesToCut)
        {
            try
            {
                List<string> newlist = new List<string>();
                if (Disney.Count >= MaxLogfileLines)
                {
                    Disney = Disney.GetRange(LinesToCut, Disney.Count - LinesToCut);
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
