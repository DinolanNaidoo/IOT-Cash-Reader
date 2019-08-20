using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IOTCashReader.Models;
using IOTCashReader.Lib;

namespace IOTCashReader.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ModelsContext _context;

        public ServiceController(ModelsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<bool>> Login(string username, string password)
        {
            User user = _context.User.Where(u => u.Username.Equals(username)).FirstOrDefault<User>();
            if (user == null)
            {
                return NotFound("user not found");
            }
            if (Security.CompareHashedData(user.Password, password)) {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<Safe>>> GetUserSafes(string username)
        {
            User user = _context.User.Where(u => u.Username.Equals(username)).FirstOrDefault<User>();
            if (user == null)
            {
                return NotFound("user not found");
            }
            List<Safe> safes = _context.Safe.Where(u => u.User.Id == user.Id || u.User.Username.Equals(username)).ToList<Safe>();
            return new ActionResult<List<Safe>>(safes);
        }

        // POST: api/Users/AddUserSafe
        [HttpPost]
        public async Task<ActionResult<User>> AddUserSafe(string username, Safe safe)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (username == null || username.Length == 0 || safe == null)
                    {
                        return BadRequest("invalid request parameters");
                    }
                    User user = _context.User.Where(u => u.Username.Equals(username)).FirstOrDefault<User>();
                    if (user == null)
                    {
                        return NotFound("username does not exist");
                    }
                    safe.User = user;
                    _context.Safe.Add(safe);
                    _context.SaveChanges();
                    return Ok("Successfully added " + safe.SafeName + " to " + user.Username);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<Access>> CheckAccessCode([FromBody] AccessCode code)
        {
            Access access = new Access() {
                Username = "N/A",
                SafeSerial = "N/A",
                Granted = false
            };
            if (code.Code != null || code.Code.Length >0)
            {
                //logic
                User user = _context.User.Where(u => u.Code.Equals(code.Code)).FirstOrDefault<User>();
                if(user != null)
                {
                    access.Username = user.Username;
                    Safe safe = _context.Safe.Where(s => s.User.Username.Equals(user.Username)).FirstOrDefault<Safe>();
                    if(safe != null)
                    {
                        access.SafeSerial = safe.SerialNumber;
                    }
                    if (!access.SafeSerial.Equals("N/A"))
                    {
                        access.Granted = true;
                    }
                }
              return Ok(access);
            }
            else
            {
                return BadRequest(access);
            }
        }
        

    }
}
