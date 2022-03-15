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
    public class SedasToolsTests
    {
        [TestMethod()]
        public void CutLeftStringSideTest()
        {
            //Kürzt einen String vorne auf die angegebene Länge
            //Arrange
            string testString = "DeleteChars-KeepChars";
            int charsToKeep = 9;
            string expected = "KeepChars";

            //Act
            string actual = SedasTools.CutLeftStringSide(testString, charsToKeep);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ExpandLeftStringSideTest()
        {
            //Erweitert einen String links um "0" bis zur angegebenen Länge
            //Arrange
            string inputString = "OriginalChars";
            int newTextLenght = 20;
            string expected = "0000000OriginalChars";

            //Act
            string actual = SedasTools.ExpandLeftStringSide(inputString, newTextLenght);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ConvertToSedasDateJJMMTTTest()
        {
            //Arrange

            //Act

            //Assert
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertToSedasDateJJMMTTTest1()
        {
            //Arrange

            //Act

            //Assert
            Assert.Fail();
        }
    }
}