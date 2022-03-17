﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dat2Sedas_Neu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu.Tests
{
    [TestClass()]
    public class ParametersTests
    {

        Parameters parameters;

        [TestMethod()]
        public void DestinationFullPathProperty_PathWithBackslashEnding_WithFilename()
        {
            //Arrange
            parameters = Parameters.GetInstance;
            parameters.DestinationFileFolder = @"D:\Testfolder\";
            parameters.DestinationFileName = "TestFilename.txt";
            string expected = @"D:\Testfolder\TestFilename.txt";

            //Act
            string actual = parameters.DestinationFullPath;

            //Assert
            Assert.AreEqual(expected, actual);

            parameters = null;
        }

        public void DestinationFullPathProperty_PathWithoutBackslashEnding_WithFilename()
        {
            //Arrange
            parameters = Parameters.GetInstance;
            parameters.DestinationFileFolder = @"D:\Testfolder";
            parameters.DestinationFileName = "TestFilenam2.txt";
            string expected = @"D:\Testfolder\TestFilenam2.txt";

            string actual = parameters.DestinationFullPath;

            //Assert
            Assert.AreEqual(expected, actual);

            parameters = null;
        }

        public void DestinationFullPathProperty_PathWithBackslashEnding_WithoutFilename()
        {
            //Arrange
            parameters = Parameters.GetInstance;
            parameters.DestinationFileFolder = @"D:\Testfolder\";
            //parameters.DestinationFileName = "TestFilenam2.txt";
            string expected = @"D:\Testfolder\Sedas.dat";

            string actual = parameters.DestinationFullPath;

            //Assert
            Assert.AreEqual(expected, actual);

            parameters = null;
        }

        public void DestinationFullPathProperty_PathWithoutBackslashEnding_WithoutFilename()
        {
            //Arrange
            parameters = Parameters.GetInstance;
            parameters.DestinationFileFolder = @"D:\Testfolder";
            //parameters.DestinationFileName = "TestFilenam2.txt";
            string expected = @"D:\Testfolder\Sedas.dat";

            string actual = parameters.DestinationFullPath;

            //Assert
            Assert.AreEqual(expected, actual);

            parameters = null;
        }
    }
}