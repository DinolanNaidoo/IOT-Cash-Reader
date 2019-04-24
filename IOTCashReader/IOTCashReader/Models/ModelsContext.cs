using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCashReader.Models
{
    public class ModelsContext : DbContext
    {
        public ModelsContext(DbContextOptions<ModelsContext> options)
        : base(options)
        {
        }
        public virtual DbSet<Credit> Credits { get; set; }
    }
}
