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

            Program prog = new Program();

            List<Tier> neueTiere = new List<Tier>();
            neueTiere.Add(new Tier() { Art="Hase",Name="Helmuth"});
            neueTiere.Add(new Tier() { Art="Hase",Name="Klaus"});
            neueTiere.Add(new Tier() { Art="Katze",Name="Kitty"});
            neueTiere.Add(new Tier() { Art="Katze",Name="Sheba"});
            neueTiere.Add(new Tier() { Art="Hund",Name="Bello"});
            neueTiere.Add(new Tier() { Art="Hund",Name="Hasso"});
            neueTiere.Add(new Tier() { Art="Pferd",Name="Renner"});
            neueTiere.Add(new Tier() { Art="Pferd",Name="Blitz"});



            TierArten arten = new TierArten();

            var tierartenGruppiert = neueTiere.GroupBy(t => t.Art);
            foreach (var tierArt in tierartenGruppiert)
            {
                Tiere tiereEinerArt = new Tiere();
                tiereEinerArt.Add(tierArt.ToList());
                Console.ReadKey();
            }

        }
    }



    public class TierArten :IEnumerable<Tiere>
    {
        List<Tiere> tierArten = new List<Tiere>();

        public IEnumerator<Tiere> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tierArten.GetEnumerator();
        }
    }

    public class Tiere : IEnumerable<Tier>

    {
        List<Tier> tiere = new List<Tier>();


        public void Add(Tier tier)
        {
            tiere.Add(tier);
        }
        public void Add(List<Tier> tiere)
        {
            foreach (Tier tier in tiere)
            {
                this.Add(tier);
            }
        }

        public IEnumerator<Tier> GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tiere.GetEnumerator();
        }
    }


    public class Tier
    {
        public string Art { get; set; }
        public string Name { get; set; }

    }
}
