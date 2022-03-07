using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program prog = new Program();
            List<string> delList = new List<string>() { "Schreiner", "Müller"};

            Personen Personen = new Personen()
            {
                new Person(){Vorname="Patrick",Nachname= "Schreiner" },
                new Person(){Vorname="Anja",Nachname= "Schreiner" },
                new Person(){Vorname="Stefan",Nachname= "Rach" },
                new Person(){Vorname="Thomas",Nachname= "Stüber" },
                new Person(){Vorname="Klaus",Nachname= "Kleber" },
                new Person(){Vorname="Gundula",Nachname= "Gause" },
                new Person(){Vorname="Hubert",Nachname= "Stüber" }
            };

            Console.WriteLine(Personen.ToString());
            Console.WriteLine(Personen.GetHashCode());

            foreach(string item in delList)
            {
                var result = Personen.personList.FirstOrDefault(p => p.Nachname == item);
                if ( result != null)
                {
                    List<Person> newList = Personen.personList.Where(p => p.Nachname != item).ToList();
                    Personen.personList = newList;
                }
            }

            Console.WriteLine(Personen.ToString());
            Console.WriteLine(Personen.GetHashCode());
            Console.ReadKey();
        }
    }


    public class Personen : IEnumerable<Person>
    {
       public List<Person> personList;

        public Personen()
        {
            personList = new List<Person>();
        }

        public Personen(List<Person> PersonenListe)
        {
            personList.AddRange(PersonenListe);
        }

        public void Add(Person newPerson)
        {
            personList.Add(newPerson);

        }

        public override string ToString()
        {
            string output="";
            foreach(Person p in personList)
            {
                output += p.Vorname + ", " + p.Nachname + "\n";
            }
            return output;
        }

        public IEnumerator<Person> GetEnumerator()
        {
            return personList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }

    public class Person
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }
    }
}
