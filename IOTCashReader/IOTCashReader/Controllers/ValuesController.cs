﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IOTCashReader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IOTCashReader.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ModelsContext _context;
        private IConfiguration _config;
        public ValuesController(ModelsContext db, IConfiguration config)
        {
            _context = db;
            _config = config;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<Credit> GetCredit()
        {
            try
            {
                return (_context.Credits.ToArray<Credit>()).ToArray<Credit>();
            }
            catch (Exception)
            {
                return new List<Credit>();
            }
        }
        [HttpGet]
        public string AddCredit(double credit)
        {
            try
            {
                Credit creditObj = new Credit();
                creditObj.CoinValue = credit;
                _context.Credits.Add(creditObj);
                _context.SaveChanges();
                return "successfully added R"+credit;
            }
            catch (Exception e)
            {
               return "error : "+e.Message; 
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> AddCoin([FromBody] Credit coin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.AddAsync(coin);
                    _context.SaveChanges();

                    return new OkObjectResult(coin);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e.Message);
                }
            }
            else
            {
                return new BadRequestObjectResult("coin entry cannot be null");
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
