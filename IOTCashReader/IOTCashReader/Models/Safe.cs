using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCashReader.Models
{
    public class Safe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string  SerialNumber { get; set; }
        public string SafeName { get; set; }
        public virtual User User { get; set; }
    }
}
