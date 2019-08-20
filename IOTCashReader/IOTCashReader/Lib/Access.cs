using IOTCashReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCashReader.Lib
{
    public class Access
    {
        public string Username{ get; set; }
        public string SafeSerial { get; set; }
        public bool Granted { get; set; }
    }
}
