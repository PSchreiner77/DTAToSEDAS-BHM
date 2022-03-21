using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConvertDatToSedas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas.Tests
{
    [TestClass()]
    public class DatOrderLineTests
    {
        [TestMethod()]
        public void DatOrderLineTest_ImportBhmOrderLine()
        {
            //Arrange
            /* NF; 180;3785;091121;101121;1111; 9000;0;  2;1.000;1     (BHM-Datei Beispiel) 
              0  NF               Kennzeichen neues Format
              1  FilNr            BHM Filialnummer der Filiale / Aldi-Filial/Kundennummer
              2  KdNr             BHM Kundennummer der Filiale / Aldi-Filial/Kundennummer
              3  Bestelldatum     Bestelldatum = Tagesdatum beim Einlesen = Erstelldatum
              4  Lieferdatum      Lieferdatum
              5  Artikelkey       BHM ArtikelKey
              6  Menge            Menge*1000, echte Menge=Menge/1000> 20000=20
              7  Preis            Preis (Bsp. 2.000), Dezimal = .
              8  ArtNummer        BHM ArtikelNummer
              9  VPE              Verpackungseinheit
              10 10               Anzahl Bestellpositionen in Datei
            */

            string BhmDatOrderLine = "NF; 180;3785;091121;101121;1111; 9000;0;  2;1.000;1";

            //Act
            DatOrderLine newDatOrderLine = new DatOrderLine(BhmDatOrderLine);

            //Assert

            Assert.AreEqual("NF", newDatOrderLine.NFKennzeichen);
            Assert.AreEqual("180", newDatOrderLine.BHMFilialNummer);
            Assert.AreEqual("3785", newDatOrderLine.BHMKundenNummer);
            Assert.AreEqual("091121", newDatOrderLine.BestellDatumTTMMJJ);
            Assert.AreEqual("101121", newDatOrderLine.LieferDatumTTMMJJ);
            Assert.AreEqual("1111", newDatOrderLine.BHMArtikelKey);
            Assert.AreEqual("9000", newDatOrderLine.BestellMenge);
            Assert.AreEqual("0", newDatOrderLine.Preis);
            Assert.AreEqual("2", newDatOrderLine.BHMArtikelNummer);
            Assert.AreEqual("1.000", newDatOrderLine.Verpackungseinheit);
            Assert.AreEqual("1", newDatOrderLine.AnzahlBestellPositionen);
        }

        [TestMethod()]
        public void DatOrderLineTest_ImportAldiOrderLine()
        {
            //Arrange
            /* NF;1050;1050;200924;200925;    ;20000; ;209;     ;10    (Aldi-Datei Beispiel)
              0  NF               Kennzeichen neues Format
              1  FilNr            BHM Filialnummer der Filiale / Aldi-Filial/Kundennummer
              2  KdNr             BHM Kundennummer der Filiale / Aldi-Filial/Kundennummer
              3  Bestelldatum     Bestelldatum = Tagesdatum beim Einlesen = Erstelldatum
              4  Lieferdatum      Lieferdatum
              5  Artikelkey       BHM ArtikelKey
              6  Menge            Menge*1000, echte Menge=Menge/1000> 20000=20
              7  Preis            Preis (Bsp. 2.000), Dezimal = .
              8  ArtNummer        BHM ArtikelNummer
              9  VPE              Verpackungseinheit
              10 10               Anzahl Bestellpositionen in Datei
            */

            string AldiDatOrderLine = "NF;1050;1050;200924;200925;    ;20000; ;209;     ;10";

            //Act
            DatOrderLine newDatOrderLine = new DatOrderLine(AldiDatOrderLine);

            //Assert
            Assert.AreEqual("NF", newDatOrderLine.NFKennzeichen);
            Assert.AreEqual("1050", newDatOrderLine.BHMFilialNummer);
            Assert.AreEqual("1050", newDatOrderLine.BHMKundenNummer);
            Assert.AreEqual("200924", newDatOrderLine.BestellDatumTTMMJJ);
            Assert.AreEqual("200925", newDatOrderLine.LieferDatumTTMMJJ);
            Assert.AreEqual("", newDatOrderLine.BHMArtikelKey);
            Assert.AreEqual("20000", newDatOrderLine.BestellMenge);
            Assert.AreEqual("", newDatOrderLine.Preis);
            Assert.AreEqual("209", newDatOrderLine.BHMArtikelNummer);
            Assert.AreEqual("", newDatOrderLine.Verpackungseinheit);
            Assert.AreEqual("10", newDatOrderLine.AnzahlBestellPositionen);
        }
    }
}