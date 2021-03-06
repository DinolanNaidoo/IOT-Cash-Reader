﻿using System;
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
            if (Security.CompareHashedData(user.Password, password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        public async Task<ActionResult<Access>> CheckAccessCode(int reqId, [FromBody] AccessCode code)
        {
            try
            {
                Access access = new Access()
                {
                    UserId = 0,
                    Granted = false
                };
                Request request = null;
                if (code != null || reqId > 0)
                {
                    //logic
                    if (reqId > 0)
                    {
                        request = _context.Request.Where(r => r.Id == reqId).FirstOrDefault<Request>();
                        if (request.isCompleted)
                        {
                            return access;
                        }
                        User user = _context.User.Where(u => u.Id == (_context.Request.Where(r => r.Id == request.Id).FirstOrDefault<Request>().User.Id)).FirstOrDefault<User>();
                        access.UserId = user.Id;
                        access.Granted = true;
                        request.isCompleted = true;
                        request.Response = "Activattion Successful";
                        _context.Request.Update(request);
                    }
                    else
                    {
                        User user = _context.User.Where(u => u.Code.Equals(code.Code)).FirstOrDefault<User>();
                        if (user != null)
                        {
                            access.UserId = user.Id;
                            access.Granted = true;
                            request = new Request()
                            {
                                User = user,
                                Type = "Acitvation",
                                isCompleted = true,
                                Response = "Activattion successful"
                            };
                            _context.Request.Add(request);
                        }
                    }
                    _context.SaveChanges();
                    return Ok(access);
                }
                else
                {
                    return BadRequest(access);
                }
            }
            catch(Exception e)
            {
                e.GetBaseException();
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public string getHashedPass(string password)
        {
            return Security.HashSensitiveData(password);
        }
        [HttpGet]
        public async Task<ActionResult<HomeInfo>> GetHomeInfo(string username)
        {
            HomeInfo homeInfo = new HomeInfo();
            try
            {
                User user = _context.User.Where(u => u.Username.Equals(username)).FirstOrDefault<User>();
                if (user == null)
                {
                    return null;
                }
                //get all withdrawal details
                homeInfo.Withdrawals = new WithdrawalsController(_context).GetSafeWithdrawals(user.Id).Result.Value.ToList<Withdrawal>();
                homeInfo.WithdrawalMonth = homeInfo.Withdrawals.Where(ws => ws.DateTime.Month == DateTime.Now.Month).ToList<Withdrawal>().Sum(w => w.Value);
                homeInfo.WithdrawalDay = homeInfo.Withdrawals.Where(ws => ws.DateTime.Day == DateTime.Now.Day).ToList<Withdrawal>().Sum(w => w.Value);
                homeInfo.WithdrawalTotal = homeInfo.Withdrawals.Sum(w => w.Value);
                //ge all deposits details
                homeInfo.Deposits = new CreditsController(_context).GetSafeCredits(user.Id).Result.Value.ToList<Credit>();
                homeInfo.DepositsMonth = homeInfo.Deposits.Where(ws => ws.DateTime.Month == DateTime.Now.Month).ToList<Credit>().Sum(s => s.Value);
                homeInfo.DepositsDay = homeInfo.Deposits.Where(ws => ws.DateTime.Day == DateTime.Now.Day).ToList<Credit>().Sum(s => s.Value);
                homeInfo.DepositsTotal = homeInfo.Deposits.Sum(c => c.Value);
                //calculate balance
                homeInfo.Balance = homeInfo.WithdrawalTotal - homeInfo.DepositsTotal;

                //get Dispensor remaining bills
                Safe dispensorDetails = _context.Safe.FirstOrDefault<Safe>();
                if (dispensorDetails != null)
                {
                    homeInfo.Bill100 = dispensorDetails.Bill100;
                    homeInfo.Bill50 = dispensorDetails.Bill50;
                    homeInfo.Bill20 = dispensorDetails.Bill20;
                    homeInfo.Bill10 = dispensorDetails.Bill10;
                }

                return homeInfo;
            }
            catch (Exception e)
            {
                return homeInfo;
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<string>> UpdateBills(string type, int count)
        {
            try
            {
                Safe safe = _context.Safe.FirstOrDefault<Safe>();
                if(safe == null)
                {
                    return "no safe exists";
                }
                if (type.Equals("s100"))
                {
                    safe.Bill100 = count;
                }else if (type.Equals("s50"))
                {
                    safe.Bill50 = count;
                }
                else if (type.Equals("s20"))
                {
                    safe.Bill20 = count;
                }
                else if (type.Equals("s10"))
                {
                    safe.Bill10 = count;
                }
                _context.Safe.Update(safe);
                _context.SaveChanges();
                return "update successful";
            }
            catch(Exception e)
            {
                e.GetBaseException();
                return "an unexpected error occured, " + e.Message;
            }
        }
        [HttpPost]
        public async Task<ActionResult<string>> UpdateRequest([FromBody] Request request)
        {
            try
            {
                if(request == null)
                {
                    return "failed to update request";
                }
                Request dbRequest = _context.Request.Where(r => r.Id == request.Id).FirstOrDefault<Request>();
                if (dbRequest == null)
                {
                    return "request not found";//still busy with another request
                }
                if (dbRequest.Type == "PrepareBag" || dbRequest.Type == "SealBag" )
                {
                    ActivityLog activityLog = new ActivityLog()
                    {
                        Type = request.Type,
                        Status = "failed, GSM server unreachable.",
                        Date = DateTime.Now
                    };
                    _context.ActivityLog.Add(activityLog);
                }
                dbRequest.Response = request.Response;
                _context.Request.Update(dbRequest);
                _context.SaveChanges();
                return "request updated";

            }
            catch (Exception e)
            {
                return "failed to update request, db error"; ;//an error occured
            }
        }
        [HttpPost]
        public async Task<ActionResult<int>> MakeRequest(string username, [FromBody] Request request)
        {
            try
            {
                User user = _context.User.Where(u => u.Username.Equals(username)).FirstOrDefault<User>();
                if (user == null)
                {
                    return -1;
                }
                Request tmpReq = _context.Request.Where(r => r.isCompleted == false && (r.Response.Length == 0 || r.Response == "busy2")).FirstOrDefault<Request>();
                if (tmpReq != null)
                {
                    return 0;//still busy with another request
                }
                Request newRequest = new Request()
                {
                    User = user,
                    Type = request.Type,
                    Amount = request.Amount,
                    isCompleted = false,
                    Response = "",
                    Counts = request.Counts
                };
                _context.Request.Add(newRequest);
                _context.SaveChanges();
                int Id = _context.Request.Where(r => r.User.Id == user.Id).ToList<Request>().Max(r => r.Id);
                return Id;

            }
            catch (Exception e)
            {
                return -1;//an error occured
            }
        }
        public async Task<ActionResult<int>> GetRequestID(int reqId)
        {
            try
            {
                Request request = _context.Request.Where(r => r.isCompleted == false).FirstOrDefault<Request>();
                if (request == null)
                {
                    return 0;
                }
                else
                {
                    return request.Id;
                }

            }
            catch (Exception e)
            {
                return -1;
            }
        }
        public async Task<ActionResult<string>> EndRequest(int reqId)
        {
            try
            {
                Request request = _context.Request.Where(r => r.Id == reqId).FirstOrDefault<Request>();
                if(request == null)
                {
                    return "request not fount";
                }
                if (request.Type == "PrepareBag" || request.Type == "SealBag" || request.Type == "ClearPath" || request.Type == "UnlockSafe" || request.Type == "LockSafe")
                {
                    ActivityLog activityLog = new ActivityLog()
                    {
                        Type = request.Type,
                        Status = "failed, GSM server unreachable.",
                        Date = DateTime.Now
                    };
                    _context.ActivityLog.Add(activityLog);
                }
                request.isCompleted = true;
                request.Response = "Could not reach GSM";
                _context.Request.Update(request);
                _context.SaveChanges();
                return "Request ended";

            }catch(Exception e)
            {
                e.GetBaseException();
                return "failed";
            }
        }
        public async Task<ActionResult<Request>> getRequest()
        {
            try
            {
                Request request = _context.Request.Where(r => r.isCompleted == false).FirstOrDefault<Request>();
                if(request != null)
                {
                    User user = _context.User.Where(u => u.Id == (_context.Request.Where(r => r.Id == request.Id).FirstOrDefault<Request>().User.Id)).FirstOrDefault<User>();
                    if (user == null)
                    {
                        request.isCompleted = true;
                        request.Response = "User not found";
                        _context.Request.Update(request);
                        _context.SaveChanges();
                        request.Type = "N/A";// make sure dispenser nor bill acceptor do not execute this request
                    }
                    else
                    {
                        if (request.Type == "Deactivation")
                        {
                            request.isCompleted = true;
                            request.Response = "Deactivation successful";
                            _context.Request.Update(request);
                            _context.SaveChanges();
                        } else if (request.Type == "ClearPath" || request.Type == "UnlockSafe" || request.Type == "LockSafe")
                        {
                            request.isCompleted = true;
                            request.Response = "Request recieved succesffuly";
                            ActivityLog activityLog = new ActivityLog() {
                                Type = request.Type,
                                Status = "Success",
                                Date = DateTime.Now
                            };
                            _context.ActivityLog.Add(activityLog);
                            _context.Request.Update(request);
                            _context.SaveChanges();
                        }
                        else if(request.Type == "PrepareBag" || request.Type == "SealBag" || request.Type == "Withdrawal")
                        {
                            request.isCompleted = true;
                            request.Response = "busy2";
                            _context.Request.Update(request);
                            _context.SaveChanges();
                        }
                        request.User = user;
                    }
                    return request;
                }
                else
                {
                    request = new Request()
                    {
                        Type = "N/A"
                    };
                    return request;
                }
            }catch(Exception e)
            {
                e.GetBaseException();
                Request request = new Request()
                {
                    Type = "N/A"
                };
                return request;
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<ActivityLog>>> GetActivityLog()
        {
            try
            {
                return _context.ActivityLog.ToList<ActivityLog>();
            }catch(Exception e)
            {
                e.GetBaseException();
                return new List<ActivityLog>();
            }
        }
        [HttpGet]
        public async Task<ActionResult<string>> checkRequestStatus(int reqId)
        {
            if (reqId > 1)
            {
                try
                {
                    Request request = _context.Request.Where(r => r.Id == reqId).FirstOrDefault<Request>();
                    if(request != null)
                    {
                        if (!request.isCompleted)
                        {
                            return "busy";
                        }
                        else if(request.isCompleted && request.Response == "busy2")
                        {
                            return "busy2";
                        }
                        else
                        {
                            return request.Response;
                        }
                    }
                    else
                    {
                        return "error, invalid request";
                    }
                }catch(Exception e)
                {
                    e.GetBaseException();
                    return "an unexpected error occured";
                }
            }
            else
            {
                return "invalid request";
            }
        }
    }
}
