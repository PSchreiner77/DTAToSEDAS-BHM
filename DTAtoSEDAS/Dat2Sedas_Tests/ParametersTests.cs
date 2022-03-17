using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Parameters.DestroyInstance();
            parameters = Parameters.GetInstance;
            parameters.DestinationFileFolder = @"D:\Testfolder\";
            parameters.DestinationFileName = "TestFilename.txt";
            string expected = @"D:\Testfolder\TestFilename.txt";

            //Act
            string actual = parameters.DestinationFullPath;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DestinationFullPathProperty_PathWithoutBackslashEnding_WithFilename()
        {
            //Arrange
            Parameters.DestroyInstance();
            parameters = Parameters.GetInstance;
            parameters.DestinationFileFolder = @"D:\Testfolder";
            parameters.DestinationFileName = "TestFilenam2.txt";
            string expected = @"D:\Testfolder\TestFilenam2.txt";

            //Act
            string actual = parameters.DestinationFullPath;

            //Assert
            Assert.AreEqual(expected, actual);
        }
               

        [TestMethod()]
        public void DestinationFullPathProperty_NoFilenameSet()
        {
            //Arrange
            Parameters.DestroyInstance();
            parameters = Parameters.GetInstance;
            parameters.DestinationFileFolder = @"D:\Testfolder";

            //Assert
            Assert.ThrowsException<ArgumentNullException>(() => parameters.DestinationFullPath);
        }

        [TestMethod()]
        public void DestinationFullPathProperty_NoFolderpathSet()
        {
            //Arrange
            Parameters.DestroyInstance();
            parameters = Parameters.GetInstance;
            parameters.DestinationFileName = "TestFile.txt";

            //Assert
            Assert.ThrowsException<ArgumentNullException>(() => parameters.DestinationFullPath);
        }




        [TestMethod()]
        public void SourceFullPathProperty_PathWithBackslashEnding_WithFilename()
        {
            //Arrange
            Parameters.DestroyInstance();
            parameters = Parameters.GetInstance;
            parameters.SourceFileFolder = @"D:\Testfolder\";
            parameters.SourceFileName = "TestFilename.txt";
            string expected = @"D:\Testfolder\TestFilename.txt";

            //Act
            string actual = parameters.SourceFullPath;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SourceFullPathProperty_PathWithoutBackslashEnding_WithFilename()
        {
            //Arrange
            Parameters.DestroyInstance();
            parameters = Parameters.GetInstance;
            parameters.SourceFileFolder = @"D:\Testfolder";
            parameters.SourceFileName = "TestFilenam2.txt";
            string expected = @"D:\Testfolder\TestFilenam2.txt";

            //Act
            string actual = parameters.SourceFullPath;

            //Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void SourceFullPathProperty_NoFilenameSet()
        {
            //Arrange
            Parameters.DestroyInstance();
            parameters = Parameters.GetInstance;
            parameters.SourceFileFolder = @"D:\Testfolder";

            //Assert
            Assert.ThrowsException<ArgumentNullException>(() => parameters.SourceFullPath);
        }

        [TestMethod()]
        public void SourceFullPathProperty_NoFolderpathSet()
        {
            //Arrange
            Parameters.DestroyInstance();
            parameters = Parameters.GetInstance;
            parameters.SourceFileName = "TestFile.txt";

            //Assert
            Assert.ThrowsException<ArgumentNullException>(() => parameters.SourceFullPath);
        }
    }
}