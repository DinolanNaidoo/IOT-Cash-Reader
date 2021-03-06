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
        public async Task<ActionResult<IEnumerable<Withdrawal>>> GetSafeWithdrawals(int UserId)
        {
            if ( UserId <= 0)
            {
                return NotFound();
            }
            try
            {
                var creditsQuery = from User in _context.User
                                   join UserWithdrawal in _context.UserWithdrawal on User.Id equals UserWithdrawal.User.Id
                                   join Withdrawal in _context.Withdrawal on UserWithdrawal.Withdrawal.Id equals Withdrawal.Id
                                   where (UserId == UserWithdrawal.User.Id)
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
        public async Task<ActionResult<double>> GetUserTotalWithdrawal(int UserId)
        {
            if ( UserId <= 0)
            {
                return NotFound("User not found");
            }
            double total = GetSafeWithdrawals(UserId).Result.Value.ToList<Withdrawal>().Sum(s => s.Value);
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
        public async Task<ActionResult<string>> MakeWithdrawal(int reqId,string status,string counts, [FromBody] Withdrawal withdrawal)
        {
            if (ModelState.IsValid && withdrawal != null && reqId > 0 )
            {
                Request request = _context.Request.Where(r => r.Id == reqId).FirstOrDefault<Request>();
                if(request == null)
                {
                    return NotFound("User not found");
                }
                User user = _context.User.Where(u => u.Id == _context.Request.Where(r => r.Id == request.Id).FirstOrDefault<Request>().User.Id).FirstOrDefault<User>();
                if (user == null)
                {
                    return NotFound("User not found");
                }
                
                withdrawal.DateTime = DateTime.Now; // get current date and time;

                UserWithdrawal userWithdrawal = new UserWithdrawal()
                {
                    Withdrawal = withdrawal,
                    User = user
                };
                if (status.Contains("E0"))
                {
                    request.Response = "Withdrawal successful";
                }
                else
                {
                    request.Response = "Withdrawal status: " + status;
                }

                string strV = "" + counts[0] + "" + counts[1]; 
                int c100 = int.Parse(strV);
                strV = "" + counts[2] + "" + counts[3];
                int c50 = int.Parse(strV);
                if(c100 + c50 <= 0)
                {
                    request.isCompleted = true;
                    request.Counts = counts;
                    _context.Request.Update(request);
                    await _context.SaveChangesAsync();
                    return request.Response;
                }
                Safe safe = _context.Safe.FirstOrDefault<Safe>();
                safe.Bill100 = safe.Bill100 - c100;
                safe.Bill50 = safe.Bill50 - c50;
                withdrawal.Value = c100 * 100 + c50 * 50;
                request.isCompleted = true;
                request.Counts = counts;
                _context.Request.Update(request);
                _context.Withdrawal.Add(withdrawal);
                _context.UserWithdrawal.Add(userWithdrawal);
                _context.Safe.Update(safe);
                await _context.SaveChangesAsync();

                int deposits = (int)new CreditsController(_context).GetUserTotalCredit(user.Id).Result.Value;
                int withdrawals = (int)GetUserTotalWithdrawal(user.Id).Result.Value;
                int balance = deposits - withdrawals;
                return request.Response;
            }
            else
            {
                return BadRequest("inavlid withdrawal request details");
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
