using System;
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
    public class CreditsController : ControllerBase
    {
        private readonly ModelsContext _context;

        public CreditsController(ModelsContext context)
        {
            _context = context;
        }

        // GET: api/Credits/GetCredits
        //get all credits in stored in all safes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Credit>>> GetCredits()
        {
            return await _context.Credits.ToListAsync();
        }

        // GET: api/Credits/GetSafeCredits?serialNumer=safeserialnumber
        //get all credits stored in the safe with the specified serial number
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Credit>>> GetSafeCredits(string serialNumber)
        {
            if (serialNumber == null || serialNumber.Length == 0)
            {
                return NotFound();
            }
            try
            {
                var creditsQuery = from Safe in _context.Safe
                                   join SafeCredit in _context.SafeCredit on Safe.Id equals SafeCredit.Safe.Id
                                   join Credit in _context.Credits on SafeCredit.Credit.Id equals Credit.Id
                                   where (Safe.SerialNumber.ToUpper().Equals(serialNumber.ToUpper()))
                                   select new
                                   {
                                       Credit.Id,
                                       Credit.Value,
                                       Credit.DateTime
                                   };
                List<Credit> credits = new List<Credit>();
                foreach (var item in creditsQuery)
                {
                    Credit credit = new Credit()
                    {
                        Id = item.Id,
                        Value = item.Value,
                        DateTime = item.DateTime
                    };
                    credits.Add(credit);
                }
                return new ActionResult<IEnumerable<Credit>>(credits);
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
        //GET: api/Credits/GetSafeTotalCredit?serialNumer=safeserialnumber
        //get total credits stored in the safe with the specified serial number
        [HttpGet]
        public async Task<ActionResult<double>> GetSafeTotalCredit(string serialNumber)
        {
            if (serialNumber == null || serialNumber.Length == 0)
            {
                return NotFound();
            }
            double total = GetSafeCredits(serialNumber).Result.Value.ToList<Credit>().Sum(s => s.Value);
            return new ActionResult<double>(total);
        }

        // GET: api/Credits/GetCredit?id=creditId
        [HttpGet("{id}")]
        public async Task<ActionResult<Credit>> GetCredit(int id)
        {
            var credit = await _context.Credits.FindAsync(id);

            if (credit == null)
            {
                return NotFound();
            }

            return credit;
        }
        // GET: api/Credits/GetSafeBalance?serialnumber=postmanTestSafe
        [HttpGet]
        public async Task<ActionResult<int>> GetSafeBalance(string serialnumber)
        {
            int withdrawals = (int)new WithdrawalsController(_context).GetSafeTotalWithdrawal(serialnumber).Result.Value;
            int deposits = (int)GetSafeTotalCredit(serialnumber).Result.Value;
            int balance = deposits - withdrawals;
            return balance;
        }

        // PUT: api/Credits/UpdateCredit
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCredit(int id, Credit credit)
        {
            if (id != credit.Id)
            {
                return BadRequest();
            }

            _context.Entry(credit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CreditExists(id))
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
        // POST: api/Credits/AddCredit
        [HttpPost]
        public async Task<ActionResult<int>> AddCredit(string serialnumber, [FromBody] Credit credit)
        {
            if (ModelState.IsValid && credit != null && serialnumber.Length > 0)
            {
                Safe safe = _context.Safe.Where(s => s.SerialNumber.ToUpper().Equals(serialnumber.ToUpper())).FirstOrDefault<Safe>();
                if(safe == null)
                {
                    return NotFound();
                }

                credit.DateTime = DateTime.Now; // get current date and time;

                SafeCredit safeCredit = new SafeCredit()
                {
                    Credit = credit,
                    Safe = safe
                };
                _context.Credits.Add(credit);
                _context.SafeCredit.Add(safeCredit);
                await _context.SaveChangesAsync();

                int withdrawals = (int)new WithdrawalsController(_context).GetSafeTotalWithdrawal(serialnumber).Result.Value;
                int deposits = (int)GetSafeTotalCredit(serialnumber).Result.Value;
                int balance = deposits - withdrawals;
                return balance;
            }
            else
            {
                return BadRequest();
            }
            
        }
        // DELETE: api/Credits/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Credit>> DeleteCredit(int id)
        {
            var credit = await _context.Credits.FindAsync(id);
            if (credit == null)
            {
                return NotFound();
            }

            _context.Credits.Remove(credit);
            await _context.SaveChangesAsync();

            return credit;
        }

        private bool CreditExists(int id)
        {
            return _context.Credits.Any(e => e.Id == id);
        }
    }
}
