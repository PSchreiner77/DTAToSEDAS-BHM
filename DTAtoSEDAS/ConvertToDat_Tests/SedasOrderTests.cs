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
    public class SedasOrderTests
    {
        [TestMethod()]
        public void SedasOrderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetOrderedArticleQuantityTest()
        {
            //Arrange
            SedasOrder testOrder = TestSedasOrder.GetSedasOrder();
            int expected = 10;
            //Act
            int actual = testOrder.Count();

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void AddTest_CheckIfNewOrderLineIsAdded()
        {
            //Arrange
            SedasOrder testOrder = TestSedasOrder.GetSedasOrder();
            SedasOrderLine newOrderLine = new SedasOrderLine("547", "2000");
            int startCount = testOrder.Count();

            //Act
            testOrder.Add(newOrderLine);
            int endCount = testOrder.Count();

            //Assert
            Assert.IsTrue(endCount == startCount + 1);
        }


        [TestMethod()]
        public void AddListTest()
        {
            //Arrange
            SedasOrder testOrder = TestSedasOrder.GetSedasOrder();
            int startCount = testOrder.Count();

            List<SedasOrderLine> orderLineList = new List<SedasOrderLine>();
            orderLineList.Add(new SedasOrderLine("547", "2000"));
            orderLineList.Add(new SedasOrderLine("712", "12000"));

            //Act
            testOrder.AddList(orderLineList);
            int endCount = testOrder.Count();

            //Assert
            Assert.IsTrue(endCount == startCount + 2);
        }

        [TestMethod()]
        public void RemoveArticlesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveArticleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveOrderPositionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangeArticleTest()
        {
            //Changes the article of the first OrderLine of the TestOrder.
            //Arrange
            SedasOrder order = TestSedasOrder.GetSedasOrder();
            ArticleChangePair articleChangePair = new ArticleChangePair("209", "1209", "Testaustausch");
            SedasOrderLine expectedOrderLine = new SedasOrderLine("1209", "20000");

            //Act
            order.ChangeArticle(articleChangePair);
            SedasOrderLine actual = order.ElementAt(0);

            //Assert
            Assert.AreEqual(expectedOrderLine.ToString(),actual.ToString());
        }

        [TestMethod()]
        public void HeaderTest()
        {
            /* ;030,14,00000000000000000,180705,180706,,,,3789         ,,
                                 ;030,        Kennung Header-Zeile
                                   14,        ???
                    00000000000000000, 
                               180705,        Bestelldatum JJMMTT
                               180706,        Lieferdatum JJMMTT
                                   ,,,
                        3789         ,        Kundennummer (inkl.Platzhalter).
                                     ,
             */

            //Arrange
            string headerExpected = ";030,14,00000000000000000,221013,221014,,,,1050         ,,";
            SedasOrder testOrder = TestSedasOrder.GetSedasOrder();

            //Act
            string headerActual = testOrder.Header();

            //Assert
            Assert.AreEqual(headerExpected, headerActual);
        }

        [TestMethod()]
        public void FooterTest()
        {
            //;05000000039000
            //;05               Kennung Footer
            //   000000039      neun Stellen für Summe bestellter Artikelmengen
            //            0000  1000er Stelle für Artikelmenge (fix)

            //Arrange
            string footerExpected = ";05000000116000";
            SedasOrder testOrder = TestSedasOrder.GetSedasOrder();

            //Act
            string footerActual = testOrder.Footer();

            //Assert
            Assert.AreEqual(footerExpected, footerActual);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            /* ;0400000000000317,40002000,,,,02 000000,,
            *      ;040000          = Kennung Zeile BestellPosition
            *      0000000317       = zehn Stellen Artikelnummer
            *      ,4               = fix
            *      00002000         = Bestellenge (Wert/1000)
            *      ,,,,02 000000,,  = fix
            */

            //Arrange
            string headerExpected = ";030,14,00000000000000000,221013,221014,,,,1050         ,,";
            string orderLinesExpected = ";0400000000000209,40020000,,,,02 000000,," + "\r\n" +
                                        ";0400000000000212,40010000,,,,02 000000,," + "\r\n" +
                                        ";0400000000000203,40015000,,,,02 000000,," + "\r\n" +
                                        ";0400000000000206,40018000,,,,02 000000,," + "\r\n" +
                                        ";0400000000000219,40015000,,,,02 000000,," + "\r\n" +
                                        ";0400000000000235,40020000,,,,02 000000,," + "\r\n" +
                                        ";0400000000000027,40004000,,,,02 000000,," + "\r\n" +
                                        ";0400000000000028,40004000,,,,02 000000,," + "\r\n" +
                                        ";0400000000000170,40003000,,,,02 000000,," + "\r\n" +
                                        ";0400000000000033,40007000,,,,02 000000,," + "\r\n";
            string footerExpected = ";05000000116000";
            string expected = headerExpected + "\r\n" + orderLinesExpected + footerExpected + "\r\n";
            SedasOrder testOrder = TestSedasOrder.GetSedasOrder();

            //Act
            string actual = testOrder.ToString();

            //Assert
            Assert.AreEqual(expected, actual);
        }

    }

    internal static class TestSedasOrder
    {
        internal static SedasOrder GetSedasOrder()
        {
               SedasOrder order = new SedasOrder("221013", "221014", "1050");

                order.AddList(new List<SedasOrderLine>()
                {
                new SedasOrderLine("209", "20000"),
                new SedasOrderLine("212", "10000"),
                new SedasOrderLine("203", "15000"),
                new SedasOrderLine("206", "18000"),
                new SedasOrderLine("219", "15000"),
                new SedasOrderLine("235", "20000"),
                new SedasOrderLine("27", "4000"),
                new SedasOrderLine("28", "4000"),
                new SedasOrderLine("170", "3000"),
                new SedasOrderLine("33", "7000")
                });

            return order;
        }
    }
}