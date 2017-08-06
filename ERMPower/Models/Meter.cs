using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERMPower.Models
{
    public class Meter
    {
        public List<LPMeter> LpMeter { get; set; }
        public List<TOUMeter> TouMeter { get; set; }
    }
}