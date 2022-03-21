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
    public class DatOrderTests
    {
        [TestMethod()]
        public void AddTest_AddOrderLine()
        {
            //Arrange
            DatOrder datTestOrder = new DatOrder("1050");
            DatOrderLine newDatOrderLine = new DatOrderLine("NF;1050;1050;200924;200925;;4000;;444;;10");
            int expected = datTestOrder.Count() + 1;

            //Act
            datTestOrder.Add(newDatOrderLine);
            int actual = datTestOrder.Count();

            //Assert
            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public void AddListTest()
        {
            //Arrange
            DatOrder datTestOrder = new DatOrder("1050");
            List<DatOrderLine> newDatOrderLines = new List<DatOrderLine>();

            newDatOrderLines.Add(new DatOrderLine("NF;1050;1050;200924;200925;;4000;;444;;10"));
            newDatOrderLines.Add(new DatOrderLine("NF;1050;1050;200924;200925;;4000;;444;;10"));
            int expected = datTestOrder.Count() + 2;

            //Act
            datTestOrder.AddList(newDatOrderLines);
            int actual = datTestOrder.Count();

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }

    internal static class TestDatOrder
    {
        internal static DatOrder GetDatOrder()
        {
            DatOrder testOrder = new DatOrder("1050");

            testOrder.Add(new DatOrderLine("NF;1050;1050;200924;200925;;20000;;209;;10"));
            testOrder.Add(new DatOrderLine("NF;1050;1050;200924;200925;;10000;;212;;10"));
            testOrder.Add(new DatOrderLine("NF;1050;1050;200924;200925;;15000;;203;;10"));
            testOrder.Add(new DatOrderLine("NF;1060;1060;200924;200925;;18000;;206;;10"));
            testOrder.Add(new DatOrderLine("NF;1060;1060;200924;200925;;15000;;219;;10"));
            testOrder.Add(new DatOrderLine("NF;1060;1060;200924;200925;;20000;;235;;10"));
            testOrder.Add(new DatOrderLine("NF;1070;1070;200924;200925;;4000;;27;;10"));
            testOrder.Add(new DatOrderLine("NF;1070;1070;200924;200925;;4000;;28;;10"));
            testOrder.Add(new DatOrderLine("NF;1080;1080;200924;200925;;3000;;170;;10"));
            testOrder.Add(new DatOrderLine("NF;1080;1080;200924;200925;;7000;;33;;10"));

            return testOrder;
        }
    }
}