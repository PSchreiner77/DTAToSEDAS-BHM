using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConvertDatToSedas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO;

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
            DatFile newSourceFile = new DatFile();

            //Act
            newSourceFile = C2S.ImportDatFileContent(orders);

            //Assert
            Assert.IsTrue(newSourceFile.Count() == 4);
            Assert.IsTrue(newSourceFile.ElementAt(0).ElementAt(0).BHMArtikelNummer == "209");
        }

        [TestMethod]
        public void ImportDatFileContent_ReturnNullValue()
        {
            //Arrange
            List<string> orders = new List<string>();
            ConvertToSedas C2S = new ConvertToSedas();

            //Act
            DatFile newSourceFile = C2S.ImportDatFileContent(orders);

            //Assert
            Assert.IsTrue(newSourceFile == null);


        }

        [TestMethod()]
        public void ToSedas_Test_OutputCompareTest()
        {
            //Arrange
            string inputFilePath = @"..\\TestFiles\20201022_TestInputNFDatFile.dat";
            ConvertToSedas C2S = new ConvertToSedas();
            List<string> importFileLines = File.ReadAllLines(inputFilePath).ToList<string>();

            string expectedFilePath = @".\TestFiles\20201022_TestOutputExpected.dat";
            string expected = File.ReadAllText(expectedFilePath);

            //Act
            DatFile inputFile = C2S.ImportDatFileContent(importFileLines);
            SedasFile outputSedasFile = C2S.ToSedas(inputFile,255);
            string actual = outputSedasFile.ToString();
            

            //Assert
            Assert.AreEqual(expected,actual);
        }
    }
}