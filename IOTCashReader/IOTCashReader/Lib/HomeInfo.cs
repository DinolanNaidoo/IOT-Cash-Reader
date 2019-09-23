using IOTCashReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCashReader.Lib
{
    public class HomeInfo
    {
        public double Balance { get; set; }
        public List<Withdrawal> Withdrawals  { get; set; }
        public double WithdrawalDay { get; set; }
        public double WithdrawalMonth { get; set; }
        public double WithdrawalTotal { get; set; }
        public List<Credit> Deposits { get; set; }
        public double DepositsDay { get; set; }
        public double DepositsMonth { get; set; }
        public double DepositsTotal { get; set; }
        public int Bill100 { get; set; }
        public int Bill50 { get; set; }
        public int Bill20 { get; set; }
        public int Bill10 { get; set; }
    }
}
