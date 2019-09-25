using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProyectoWFDB;
using ProyectoWFDB.Entidades;

namespace WFDB_Test
{
    [TestClass]
    public class WfdbTest
    {
        [TestMethod]
        public void WFDB_Open_whenFileExist_returnFile()
        {
            //Arrange

            //Act
            var result=ManagerWFDB.Wfdb_Open("100s", "hea", ProyectoWFDB.Enumeraciones.Stat.Read);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReadHeader_whenFileHeaderExist_returnSignals()
        {
            //Arrange
            
            //Act
            Record result = ManagerWFDB.ReadHeader("p10143");
            //Assert
            Assert.IsTrue(result.Signals.Count() == 2 );

            //Act
            result = ManagerWFDB.ReadHeader("7001");
            //Assert
            Assert.IsTrue(result.Signals.Count() == 2);

            //Act
            result = ManagerWFDB.ReadHeader("100");
            //Assert
            Assert.IsTrue(result.Signals.Count() == 2);
        }

        [TestMethod]
        public void ReadSignals_whenFileSignalExist_returnSignalsWithSamples()
        {
            //Arrange
            Record result;


            //Act
            result = ManagerWFDB.ReadSignals("01911");

            //Assert
            Assert.AreEqual(30, result.Signals.ElementAt(0).Samples[184]);
            Assert.AreEqual(-13, result.Signals.ElementAt(1).Samples[128]);
        

            //Act
            result = ManagerWFDB.ReadSignals("S0088_V1");

            //Assert
            Assert.AreEqual(-2047, result.Signals.ElementAt(0).Samples[0]);
            Assert.AreEqual(1592, result.Signals.ElementAt(1).Samples[1]);


            //Act
            result = ManagerWFDB.ReadSignals("N1_evoked_ave100_F1_R1");

            //Assert
            Assert.AreEqual(-536690716, result.Signals.ElementAt(0).Samples[0]);
            Assert.AreEqual(6941587, result.Signals.ElementAt(1).Samples[1]);

            //Act
            result = ManagerWFDB.ReadSignals("p10143");

            //Assert
            Assert.AreEqual(12175, result.Signals.ElementAt(1).Samples[0]);


            //Act
            result = ManagerWFDB.ReadSignals("charis1");

            //Assert
            Assert.AreEqual(-1333, result.Signals.ElementAt(0).Samples[0]);

            //Act
            result = ManagerWFDB.ReadSignals("aami3a");

            //Assert
            Assert.AreEqual(2072,result.Signals.ElementAt(0).Samples[0]);

            //Act
            result = ManagerWFDB.ReadSignals("100s");
            
            //Assert
            Assert.AreEqual(result.Signals.ElementAt(0).Samples[0] ,995);
        }

        [TestMethod]
        public void ExportSamplesCSV_whenRecordExiste_writeCSV()
        {
            string csvPath;
            csvPath = ManagerWFDB.ExportSamplesCSV("n16",0,60,true);
            csvPath = ManagerWFDB.ExportSamplesCSV("100s");

            Assert.IsTrue(File.Exists(csvPath));
        }

        [TestMethod]
        public void ExportRecordJson_whenRecordExiste_writeJSON()
        {
            string path;
            path = ManagerWFDB.ExportRecordJSON("n16",true);
            path = ManagerWFDB.ExportRecordJSON("a01");
            Assert.IsTrue(File.Exists(path));
        }

        [TestMethod]
        public void ExportAnnotations_whenAnnotatorExiste_writeCSV()
        {
            string csvPath;
            csvPath = ManagerWFDB.ExportAnnotationsCSV("0001", "atr",true);
            csvPath = ManagerWFDB.ExportAnnotationsCSV("0201", "atr",true);
            csvPath = ManagerWFDB.ExportAnnotationsCSV("rec_1", "atr",true);
            csvPath = ManagerWFDB.ExportAnnotationsCSV("a01", "apn", true);
            Assert.IsTrue(File.Exists(csvPath));
        }

        [TestMethod]
        public void ReadAnnotator_WhenAnotationFileExist_ReturnListAnnotatnions()
        {
            Annotator a;

            a = ManagerWFDB.ReadAnnotator("0201", "atr");
            Assert.AreEqual(2699891, a.Annotations.Last().Time);


            a = ManagerWFDB.ReadAnnotator("0001", "atr");
            Assert.AreEqual(2699891, a.Annotations.Last().Time);


            a = ManagerWFDB.ReadAnnotator("rec_1", "atr");

            Assert.AreEqual(4407, a.Annotations.Last().Time);
            Assert.AreEqual(22, a.Annotations.Count);

            a = ManagerWFDB.ReadAnnotator("a01", "apn");

            Assert.AreEqual(2928000, a.Annotations.Last().Time);
            Assert.AreEqual(489, a.Annotations.Count);

            a = ManagerWFDB.ReadAnnotator("a01", "qrs");

            Assert.AreEqual(2956445, a.Annotations.Last().Time);
            Assert.AreEqual(29938, a.Annotations.Count);
        }

        [TestMethod]
        public void ReadEDF_whenEdfExist_ReturnRecord()
        {
            Record r;
            r = ManagerWFDB.ReadEDF("n16", true);
            r = ManagerWFDB.ReadEDF("SC4001E0-PSG", true);      

            Assert.IsNotNull(r);
        }


        [TestMethod]
        public void ReadHeader_ECGID()
        {
            string jsonPath;
            jsonPath = ManagerWFDB.ExportRecordJSON("rec_1");
            Assert.IsTrue(File.Exists(jsonPath));
        }

        [TestMethod]
        public void ReadSignals_ECGID()
        {           
            string csvPath;
            csvPath = ManagerWFDB.ExportSamplesCSV("rec_1");
            Assert.IsTrue(File.Exists(csvPath));
        }

        [TestMethod]
        public void ReadAnnotation_ECGID()
        {
            string csvPath;
            csvPath = ManagerWFDB.ExportAnnotationsCSV("rec_1","atr",true);
            Assert.IsTrue(File.Exists(csvPath));
        }


        [TestMethod]
        public void ReadHeader_SLEEPEDF()
        {
            string jsonPath;
            jsonPath = ManagerWFDB.ExportRecordJSON("SC4001E0-PSG", isEDF: true);
            Assert.IsTrue(File.Exists(jsonPath));
        }

        [TestMethod]
        public void ReadSignals_SLEEPEDF()
        {
            string csvPath;
            csvPath = ManagerWFDB.ExportSamplesCSV("SC4001E0-PSG", isEDF: true);
            Assert.IsTrue(File.Exists(csvPath));
        }

        [TestMethod]
        public void ReadAnnotation_AHADB()
        {
            string csvPath;
            csvPath = ManagerWFDB.ExportAnnotationsCSV("0001", "atr", true);
            Assert.IsTrue(File.Exists(csvPath));
        }
    }
}   
