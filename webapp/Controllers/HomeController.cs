#region Using

using eSPP.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static eSPP.Controllers.AccountController;

#endregion

namespace eSPP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: home/index
        public ActionResult Index(string getuserid, ManageMessageId? message)
        {
			ViewBag.StatusMessage =
			message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
			: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
			: message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
			: message == ManageMessageId.Error ? "An error has occurred."
			: message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
			: message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
			: message == ManageMessageId.ResetPassword ? "Katalaluan Baru Telah Dihantar Ke Emel"
			: "";

			ApplicationDbContext db = new ApplicationDbContext();
            var user = User.Identity.GetUserId();
            var userList = db.Users.SingleOrDefault(s => s.Id == user);

            if (userList == null)
            {
                userList = new ApplicationUser();
            }

            getuserid = user;
            ViewBag.id = getuserid;
            DateTime currentDate = DateTime.Now;
            DateTime futureDate = Convert.ToDateTime(userList.PasswordUpdate).AddMonths(3); 
            DateTime futureDate1 = currentDate.AddMonths(-3);
            DateTime startDate = Convert.ToDateTime(userList.PasswordUpdate);
            DateTime endDate = Convert.ToDateTime(userList.PasswordUpdate).AddDays(3);
            int date = ((futureDate - Convert.ToDateTime(userList.PasswordUpdate)).Days);
            if(futureDate <= DateTime.Now)
			{
				return RedirectToAction("ChangePassword", "Manage", new { getuserid = getuserid, Message = ManageMessageId.UpdatePassword });
                //ViewBag.message = "Kata Laluan Anda Sudah Menghampiri 3 Bulan. Sila Tukar Kata Laluan";
            }
            return View();
        }

        public ActionResult Social()
        {
            return View();
        }

        // GET: home/inbox
        public ActionResult Inbox()
        {
            return View();
        }

        // GET: home/widgets
        public ActionResult Widgets()
        {
            return View();
        }

        // GET: home/chat
        public ActionResult Chat()
        {
            return View();
        }
    }
}