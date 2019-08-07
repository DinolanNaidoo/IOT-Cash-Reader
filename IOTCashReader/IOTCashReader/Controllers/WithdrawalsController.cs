﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IOTCashReader.Models;

namespace IOTCashReader.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WithdrawalsController : ControllerBase
    {
        private readonly ModelsContext _context;

        public WithdrawalsController(ModelsContext context)
        {
            _context = context;
        }

        // GET: api/Withdrawals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Withdrawal>>> GetWithdrawal()
        {
            return await _context.Withdrawal.ToListAsync();
        }
        // GET: api/Withdrawals/GetSafeWithdrawals?serialNumber=postmanTestSafe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Withdrawal>>> GetSafeWithdrawals(string serialNumber)
        {
            if (serialNumber == null || serialNumber.Length == 0)
            {
                return NotFound();
            }
            try
            {
                var creditsQuery = from Safe in _context.Safe
                                   join SafeWithdrawal in _context.SafeWithdrawal on Safe.Id equals SafeWithdrawal.Safe.Id
                                   join Withdrawal in _context.Withdrawal on SafeWithdrawal.Withdrawal.Id equals Withdrawal.Id
                                   where (Safe.SerialNumber.ToUpper().Equals(serialNumber.ToUpper()))
                                   select new
                                   {
                                       Withdrawal.Id,
                                       Withdrawal.Value,
                                       Withdrawal.DateTime
                                   };
                List<Withdrawal> withdrawals = new List<Withdrawal>();
                foreach (var item in creditsQuery)
                {
                    Withdrawal withdrawal = new Withdrawal()
                    {
                        Id = item.Id,
                        Value = item.Value,
                        DateTime = item.DateTime
                    };
                    withdrawals.Add(withdrawal);
                }
                return new ActionResult<IEnumerable<Withdrawal>>(withdrawals);
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
        // GET: api/Withdrawals/GetSafeTotalWithdrawal?serialNumber=postmanTestSafe
        public async Task<ActionResult<double>> GetSafeTotalWithdrawal(string serialNumber)
        {
            if (serialNumber == null || serialNumber.Length == 0)
            {
                return NotFound();
            }
            double total = GetSafeWithdrawals(serialNumber).Result.Value.ToList<Withdrawal>().Sum(s => s.Value);
            return new ActionResult<double>(total);
        }
        // GET: api/Credits/GetWithdrawal?id=withdrawalId
        [HttpGet("{id}")]
        public async Task<ActionResult<Withdrawal>> GetWithdrawal(int id)
        {
            var withdrawal = await _context.Withdrawal.FindAsync(id);

            if (withdrawal == null)
            {
                return NotFound();
            }

            return withdrawal;
        }

        // PUT: api/Withdrawals/PutWithdrawal?id=withdrawalId
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWithdrawal(int id, Withdrawal withdrawal)
        {
            if (id != withdrawal.Id)
            {
                return BadRequest();
            }

            _context.Entry(withdrawal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WithdrawalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Withdrawals/MakeWithdrawal?serialnumber=serialnumber
        [HttpPost]
        public async Task<ActionResult<int>> MakeWithdrawal(string serialnumber, [FromBody] Withdrawal withdrawal)
        {
            if (ModelState.IsValid && withdrawal != null && serialnumber.Length > 0)
            {
                Safe safe = _context.Safe.Where(s => s.SerialNumber.ToUpper().Equals(serialnumber.ToUpper())).FirstOrDefault<Safe>();
                if (safe == null)
                {
                    return NotFound();
                }

                withdrawal.DateTime = DateTime.Now; // get current date and time;

                SafeWithdrawal safeWithdrawal = new SafeWithdrawal()
                {
                    Withdrawal = withdrawal,
                    Safe = safe
                };
                _context.Withdrawal.Add(withdrawal);
                _context.SafeWithdrawal.Add(safeWithdrawal);
                await _context.SaveChangesAsync();

                int deposits = (int)new CreditsController(_context).GetSafeTotalCredit(serialnumber).Result.Value;
                int withdrawals = (int)GetSafeTotalWithdrawal(serialnumber).Result.Value;
                int balance = deposits - withdrawals;
                return balance;
            }
            else
            {
                return BadRequest();
            }
        }

        // DELETE: api/Withdrawals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Withdrawal>> DeleteWithdrawal(int id)
        {
            var withdrawal = await _context.Withdrawal.FindAsync(id);
            if (withdrawal == null)
            {
                return NotFound();
            }

            _context.Withdrawal.Remove(withdrawal);
            await _context.SaveChangesAsync();

            return withdrawal;
        }

        private bool WithdrawalExists(int id)
        {
            return _context.Withdrawal.Any(e => e.Id == id);
        }
    }
}
