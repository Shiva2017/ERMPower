using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERMPower.Models
{
    public class LPMeter
    {
        public string MeterPointCode { get; set; }
        public string SerialNumber { get; set; }
        public string PlantCode { get; set; }
        public string DateTime { get; set; }
        public string DataType { get; set; }
        public double DataValue { get; set; }
        public string Units { get; set; }
        public string Status { get; set; }
        public string FileName { get; set; }
        public string Median { get; set; }
        public double DataPercentageValue { get; set; }
        public bool Is20PercentAboveMedian { get; set; }
        public bool IsBelowMedian { get; set; }
    }
}