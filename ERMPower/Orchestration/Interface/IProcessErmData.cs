using ERMPower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERMPower.Orchestration
{
  public interface IProcessErmData
    {
       List<LPMeter> ProcessLPData();
      List<TOUMeter> ProcessTOUData();
      Meter ProcessMeterData();
    }
}
