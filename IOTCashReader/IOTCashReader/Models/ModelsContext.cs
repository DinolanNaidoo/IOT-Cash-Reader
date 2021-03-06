﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCashReader.Models
{
    public class ModelsContext : DbContext
    {
        public ModelsContext(DbContextOptions<ModelsContext> options)
        : base(options){
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Safe>().HasIndex(s => s.SerialNumber).IsUnique();
        }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Safe> Safe { get; set; }
        public virtual DbSet<UserCredit> UserCredit { get; set; }
        public virtual DbSet<Credit> Credits { get; set; }
        public virtual DbSet<UserWithdrawal> UserWithdrawal { get; set; }
        public virtual DbSet<Withdrawal> Withdrawal { get; set; }
        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<ActivityLog> ActivityLog { get; set; }
        public virtual DbSet<Cashout> Cashout { get; set; }
    }
}
