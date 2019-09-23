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
        public async Task<ActionResult<IEnumerable<Credit>>> GetSafeCredits(int UserId)
        {
            if ( UserId <= 0)
            {
                return NotFound("User not found");
            }
            try
            {
                var creditsQuery = from User in _context.User
                                   join UserCredit in _context.UserCredit on User.Id equals UserCredit.User.Id
                                   join Credit in _context.Credits on UserCredit.Credit.Id equals Credit.Id
                                   where (User.Id == UserId)
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
        public async Task<ActionResult<double>> GetUserTotalCredit(int UserId)
        {
            if ( UserId <= 0)
            {
                return NotFound();
            }
            double total = GetSafeCredits(UserId).Result.Value.ToList<Credit>().Sum(s => s.Value);
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
        public async Task<ActionResult<int>> GetSafeBalance(int UserId)
        {
            int withdrawals = (int)new WithdrawalsController(_context).GetUserTotalWithdrawal(UserId).Result.Value;
            int deposits = (int)GetUserTotalCredit(UserId).Result.Value;
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
        public async Task<ActionResult<int>> AddCredit(int UserId,[FromBody] Credit credit)
        {
            if (ModelState.IsValid && credit != null && UserId > 0 )
            {
                User user = _context.User.Where(u => u.Id == UserId).FirstOrDefault<User>();
                Safe safe = _context.Safe.FirstOrDefault<Safe>();
                if (user == null)
                {
                    return NotFound("User not foound");
                }

                credit.DateTime = DateTime.Now; // get current date and time;

                UserCredit userCredit = new UserCredit()
                {
                    Credit = credit,
                    User = user
                };
                _context.Credits.Add(credit);
                _context.UserCredit.Add(userCredit);

                //update requestsTable
                Request request = new Request()
                {
                    User = user,
                    Type = "Deposit",
                    isCompleted = true,
                    Amount = credit.Value,
                    Response = "Deposit successful"
                };
                _context.Request.Add(request);
                await _context.SaveChangesAsync();

                int withdrawals = (int)new WithdrawalsController(_context).GetUserTotalWithdrawal(UserId).Result.Value;
                int deposits = (int)GetUserTotalCredit(UserId).Result.Value;
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
