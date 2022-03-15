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
    public class SedasOrderLineTests
    {
        [TestMethod()]
        public void ToStringTest_OutputStringIsCorrect()
        {
            /* ;0400000000000317,40002000,,,,02 000000,,
            *      ;040000          = Kennung Zeile BestellPosition
            *      0000000317       = zehn Stellen Artikelnummer
            *      ,4               = fix
            *      00002000         = Bestellenge (Wert/1000)
            *      ,,,,02 000000,,  = fix
            */
            //Arrange
            string testArticle = "317";
            string testAmmount = "12";
            string compare = $";0400000000000317,40012000,,,,02 000000,,";
            SedasOrderLine Sol = new SedasOrderLine("317", "12000");

            //Act
            string output = Sol.ToString();

            //Assert
            Assert.AreEqual(compare, output);
        }
    }
}