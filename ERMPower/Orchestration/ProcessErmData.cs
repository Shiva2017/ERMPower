using ERMPower.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ERMPower.Orchestration
{
    public class ProcessErmData : IProcessErmData
    {
        #region Csv Data
       
        string Lp_Csv1 = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"Orchestration\CsvData\LP_210095893_20150901T011608049.csv");
        string Lp_Csv2 = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"Orchestration\CsvData\LP_214612534_20150907T084333712.csv");
        string Lp_Csv3 = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"Orchestration\CsvData\LP_214612653_20150907T220027915.csv");

        string TOU_Csv1 = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"Orchestration\CsvData\TOU_212621145_20150911T022358.csv");
        string TOU_Csv2 = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"Orchestration\CsvData\TOU_212621147_20150911T022240.csv");
        string TOU_Csv3 = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, @"Orchestration\CsvData\TOU_214667141_20150901T040057.csv");
        public List<string> ListLpCsv;
        public List<string> ListTOUCsv;
        const char rowDelimiter = '\n';
        const char columnDelimiter = ',';
        #endregion

        public ProcessErmData()
        {
            ListLpCsv = new List<string> { Lp_Csv1,Lp_Csv2,Lp_Csv3};
            ListTOUCsv = new List<string> { TOU_Csv1, TOU_Csv2, TOU_Csv3 };
        }
        public List<LPMeter> ProcessLPData()
        {            
            List<LPMeter> lpFilteredData = new List<LPMeter>();
            foreach (var csvFile in ListLpCsv)
            {
                List<LPMeter> lpData = new List<LPMeter>();
                var contents = File.ReadAllText(csvFile).Split(rowDelimiter);
                var csv =( from line in contents
                           select line.Split(columnDelimiter)).ToArray().Skip(1);
                
                foreach (var item in csv)
                {
                    if(item.Length > 1)
                        lpData.Add(new LPMeter
                        {
                            MeterPointCode = item[0],
                            SerialNumber = item[1],
                            PlantCode = item[2],
                            DateTime = item[3],
                            DataType = item[4],
                            DataValue = Convert.ToDouble(item[5]),
                            Units = item[6],
                            Status = item[7],
                            FileName = csvFile.Split('\\')[csvFile.Split('\\').Length-1]
                        });
                }
                lpFilteredData.AddRange(calculateLPMedianValues(lpData.OrderBy(p=>p.DataValue).ToList()));
            }
            return lpFilteredData.OrderByDescending(p=>p.DataValue).ToList();
        }
        public List<TOUMeter> ProcessTOUData()
        {            
            List<TOUMeter> TouFilteredData = new List<TOUMeter>();
            foreach (var TouFile in ListTOUCsv)
            {
                List<TOUMeter> TouData = new List<TOUMeter>();
                var contents = File.ReadAllText(TouFile).Split(rowDelimiter);
                var csv = (from line in contents
                           select line.Split(columnDelimiter)).ToArray().Skip(1);

                foreach (var item in csv)
                {
                    if (item.Length > 1)
                        TouData.Add(new TOUMeter
                        {
                            MeterPointCode = item[0],
                            SerialNumber = item[1],
                            PlantCode = item[2],
                            DateTime = item[3],
                            DataType = item[4],
                            Energy = Convert.ToDouble(item[5]),
                            MaximumDemand = item[6],
                            TimeofMaxDemand = item[7],
                            Units = item[8],
                            Status = item[9],
                            Period = item[10],
                            DLSActive =  item[11],
                            BillingResetCount = item[12],
                            BillingResetDate =  item[13],
                            Rate =item[14] ,
                            FileName = TouFile.Split('\\')[TouFile.Split('\\').Length - 1]
                        });
                }
                TouFilteredData.AddRange(calculateTOUMedianValues(TouData.OrderBy(p=>p.Energy).ToList()));
            }
            return TouFilteredData;
        }
        public Meter ProcessMeterData()
        {
            Meter meter = new Meter();
            meter.LpMeter = ProcessLPData();
            meter.TouMeter = ProcessTOUData();            
            return meter;
        }

        private List<LPMeter> calculateLPMedianValues(List<LPMeter> LpData)
        {
            List<LPMeter> lp = new List<LPMeter>();
            var m1 = LpData[LpData.Count() / 2].DataValue;
            var m2 = LpData[(LpData.Count() / 2)-1].DataValue;
            double medianValue = (m1+ m2)/2; 

            lp = LpData.Select(p => new LPMeter
            {
                MeterPointCode = p.MeterPointCode,
                SerialNumber = p.SerialNumber,
                PlantCode = p.PlantCode,
                DateTime = p.DateTime,
                DataType = p.DataType,
                DataValue = p.DataValue,
                Units = p.Units,
                Status = p.Status,
                DataPercentageValue = GetPercentage(p.DataValue),
                Is20PercentAboveMedian = Is20PercentAboveMedian(p.DataValue, medianValue),
                IsBelowMedian =( p.DataValue < medianValue),
                Median =Convert.ToString(medianValue),
                FileName=p.FileName
            }).ToList();
            return lp.Where(p=>p.Is20PercentAboveMedian==true || p.IsBelowMedian == true).ToList();
        }
        private List<TOUMeter> calculateTOUMedianValues(List<TOUMeter> TouData)
        {
            List<TOUMeter> tou = new List<TOUMeter>();
            var m1 = TouData[TouData.Count() / 2].Energy;
            var m2 = TouData[(TouData.Count() / 2) - 1].Energy;
            double medianValue = (m1 + m2) / 2; 

            tou = TouData.Select(p => new TOUMeter
            {
                MeterPointCode = p.MeterPointCode,
                SerialNumber = p.SerialNumber,
                PlantCode = p.PlantCode,
                DateTime = p.DateTime,
                DataType = p.DataType,
                Energy = p.Energy,
                MaximumDemand = p.MaximumDemand,
                TimeofMaxDemand = p.TimeofMaxDemand,
                Units = p.Units,
                Status = p.Status,
                Period = p.Period,
                DLSActive = p.DLSActive,
                BillingResetCount = p.BillingResetCount,
                BillingResetDate = p.BillingResetDate,
                Rate = p.Rate,
                DataPercentageValue = GetPercentage(p.Energy),
                Is20PercentAboveMedian = Is20PercentAboveMedian(p.Energy, medianValue),
                IsBelowMedian = (p.Energy < medianValue),
                Median = Convert.ToString(medianValue),
                FileName = p.FileName
            }).ToList();
            return tou.Where(p => p.Is20PercentAboveMedian == true || p.IsBelowMedian == true).ToList();
        }

        private double GetPercentage(double data)
        {
            if (data > 0)
                return ((data * 20)) / 100;
            else return 0;
        }

        private bool Is20PercentAboveMedian(double data, double median)
        {
            bool Is20PercentAbove = false;
            double valuePercent = GetPercentage(data);
            Is20PercentAbove = valuePercent > median;
            return Is20PercentAbove;
        }
      
    }    
}