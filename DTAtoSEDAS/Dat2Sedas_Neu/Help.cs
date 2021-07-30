using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu
{
    static class Help
    {
        public static void handlertest()
        {
            ProgramInit.InitFailed += ProgramInit_InitFailed;

        }

        private static void ProgramInit_InitFailed(string message)
        {
            Console.WriteLine(message);
        }

        public static void Show()
        {
            Console.WriteLine();
            Console.WriteLine("DATtoSEDAS-Converter, Version " + MyProject.Application.Info.Version.ToString());
            Console.WriteLine("Wandelt eine Bestell.dat in eine SEDAS.dat um, für den Import in CSB.");
            Console.WriteLine("Wird das Programm ohne Parameter gestartet, werden Pfad und Dateiinformationen");
            Console.WriteLine("aus der Datei Config.ini übernommen.");
            Console.WriteLine("Parameter, die mit Schaltern übergeben werden überschreiben Einstellungen ");
            Console.WriteLine("der Config.ini");
            Console.WriteLine();
            Console.WriteLine("DATtoSEDAS.exe [/Q=][Laufwerk:][Pfad][Dateiname] [/Z=][Laufwerk:][Pfad][Dateiname] [/D] [/I]");
            Console.WriteLine("               [/A] [/?]");
            Console.WriteLine("[Laufwerk:]  Laufwerksbuchstabe, z.B. C:");
            Console.WriteLine("[Pfad]       Verzeichnispfad");
            Console.WriteLine("[Dateiname]  Dateiname");
            Console.WriteLine();
            Console.WriteLine(" /Q=         Laufwerk/Pfad/Dateiname der Quelldatei.");
            Console.WriteLine("                Wird nur ein Dateiname angegeben, wird dieser im Programm-");
            Console.WriteLine("                Verzeichnis gesucht.");
            Console.WriteLine(" /Z=         Laufwerk/Pfad/Dateiname der Zieldatei." + "\n\r" + "                Wird kein Zielpfad angegeben,wird die " + "\n\r" + "                Zieldatei im Programmverzeichnis abgelegt." + "\n\r" + "              ! Existiert eine gleichnamige Zieldatei bereits, wird");
            Console.WriteLine("                sie ohne Rückfrage überschrieben!");
            Console.WriteLine(" /D          Quelldatei wird nach der Konvertierung gelöscht.");
            Console.WriteLine(" /I          Statusmeldungen werden nicht angezeigt (jedoch Fehlermeldungen!).");
            Console.WriteLine(" /?          Ruft diese Hilfeseite auf.");
            Console.WriteLine();
            Console.WriteLine("Beispiel:");
            Console.WriteLine(">> DATtoSEDAS.exe /Q=C:\Daten\Bestell.dat /Z=D:\Import\Sedas.dat /D /I");
            Console.WriteLine(" - Quelldatei wird in Zieldatei eingelesen, die Quelldatei wird anschließend gelöscht.");
            Console.WriteLine("   Statusmeldungen werden nicht ausgegeben.");
            Console.WriteLine();
            Console.WriteLine(">> DATtoSEDAS.exe /Q=C:\Daten\Bestell.dat /I");
            Console.WriteLine(" - Quelldatei wird in Zieldatei eingelesen. Statusmeldungen werden nicht ausgegeben.");
            Console.WriteLine();
            Console.Write("<Enter> drücken...");
            Console.ReadLine();
        }
    }
}
