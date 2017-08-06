using ERMPower.Models;
using ERMPower.Orchestration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ERMPower.Controllers
{
    public class ErmPowerController : Controller
    {
        IProcessErmData _processAglData;
        public ErmPowerController(IProcessErmData processAglData)
        {
            _processAglData = processAglData;
        }
        public ActionResult CsvData()
        {
            var meter = _processAglData.ProcessMeterData();
            return View("ErmPower", meter);
        }      
    }
}