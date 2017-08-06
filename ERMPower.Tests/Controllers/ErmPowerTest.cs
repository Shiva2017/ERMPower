using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ERMPower;
using ERMPower.Controllers;
using ERMPower.Orchestration;
using Moq;
using Microsoft.Practices.Unity;
using FizzWare.NBuilder.Implementation;
using System.IO;

namespace ERMPower.Tests.Controllers
{
    [TestClass]
    public class ErmPowerTest
    {
        IProcessErmData ermData;
        public ErmPowerTest()
        {
            var repMock = new Mock<IProcessErmData>();
            var container = new UnityContainer();
            container.RegisterInstance<IProcessErmData>(repMock.Object);
            ermData = container.Resolve<ProcessErmData>();
        }

        [TestMethod]
        public void ErmPower_Check_LP_Data()
        {
            ProcessErmData erm = new ProcessErmData();
            erm.ListLpCsv = new List<string>();
            erm.ListLpCsv.Add(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"\CsvData\LP_210095893_20150901T011608049.csv"));
            erm.ListLpCsv.Add(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"\CsvData\LP_214612534_20150907T084333712.csv"));
            erm.ListLpCsv.Add( string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"\CsvData\LP_214612653_20150907T220027915.csv"));

            var csvData=  erm.ProcessLPData();
            Assert.IsNotNull(csvData);
            Assert.AreEqual(csvData.Count(), 2009);

            var median =Convert.ToDouble(csvData.FirstOrDefault().Median);
            foreach (var itm in csvData.Where(p => p.DataValue < median).Take(10).ToList())
            {
                var IsLessThanMedian = (itm.DataValue < median);
                Assert.IsTrue(IsLessThanMedian);
            }
        }

        [TestMethod]
        public void ErmPower_Check_TOU_Data()
        {
            ProcessErmData erm = new ProcessErmData();
            erm.ListTOUCsv = new List<string>();
            erm.ListTOUCsv.Add(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"\CsvData\TOU_212621145_20150911T022358.csv"));
            erm.ListTOUCsv.Add(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"\CsvData\TOU_212621147_20150911T022240.csv"));
            erm.ListTOUCsv.Add(string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"\CsvData\TOU_214667141_20150901T040057.csv"));

            var csvData = erm.ProcessTOUData();
            Assert.IsNotNull(csvData);
            Assert.AreEqual(csvData.Count(), 2);

            var median = csvData.FirstOrDefault().Median;
            foreach (var itm in csvData)
            {
                var IsLessThanMedian = (itm.Energy < Convert.ToDouble(median));
                Assert.IsTrue(IsLessThanMedian);
            }
        } 
        
    }
}
