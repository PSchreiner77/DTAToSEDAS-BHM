using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConvertDatToSedas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas.Tests
{
    [TestClass]
    public class ConvertToSedasTests
    {
        [TestMethod]
        public void ImportDatFileContent_GenerateSeparateOrderFromGroupedOrderLines()
        {
            //Arrange            
            List<string> orders = new List<string>();
            orders.Add("NF;1050;1050;200924;200925;;20000;;209;;10");
            orders.Add("NF;1050;1050;200924;200925;;10000;;212;;10");
            orders.Add("NF;1050;1050;200924;200925;;15000;;203;;10");
            orders.Add("NF;1060;1060;200924;200925;;18000;;206;;10");
            orders.Add("NF;1060;1060;200924;200925;;15000;;219;;10");
            orders.Add("NF;1060;1060;200924;200925;;20000;;235;;10");
            orders.Add("NF;1070;1070;200924;200925;;4000;;27;;10");
            orders.Add("NF;1070;1070;200924;200925;;4000;;28;;10");
            orders.Add("NF;1080;1080;200924;200925;;3000;;170;;10");
            orders.Add("NF;1080;1080;200924;200925;;7000;;33;;10");

            ConvertToSedas C2S = new ConvertToSedas();
            SourceFile newSourceFile = new SourceFile();

            //Act
            newSourceFile = C2S.ImportDatFileContent(orders);

            //Assert
            Assert.IsTrue(newSourceFile.SourceOrders.Count() == 4);
            Assert.IsTrue(newSourceFile.ElementAt(0).ElementAt(0).BHMArtikelNummer == "209");
        }

        [TestMethod]
        public void ImportDatFileContent_ReturnNullValue()
        {
            //Arrange
            List<string> orders = new List<string>();
            ConvertToSedas C2S = new ConvertToSedas();
            SourceFile newSourceFile = new SourceFile();

            //Act
            newSourceFile = C2S.ImportDatFileContent(orders);

            //Assert
            Assert.IsTrue(newSourceFile == null);


        }
    }
}