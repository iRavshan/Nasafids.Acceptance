using Acceptance.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acceptance.Domain;
using System.Security.Claims;
using Acceptance.ViewModels.Home;
using Humanizer;

namespace Acceptance.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IApplicantService applicantService;
        private readonly IMoverService moverService;
        private readonly IUserService userService;
        public HomeController(IApplicantService applicantService,
                              IMoverService moverService,
                              IUserService userService)
        {
            this.applicantService = applicantService;
            this.moverService = moverService;
            this.userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            IndexViewModel model = new IndexViewModel()
            {
                ApplicantsCount = (await applicantService.GetAll()).Count(),
                ApplicantsCountOnMonts = (await applicantService.GetAll()).Where(w => w.RegistrationDateTime.Month == DateTime.UtcNow.AddHours(5).Month).Count(),
                ApplicantsCountOnToday = (await applicantService.GetAll()).Where(w => w.RegistrationDateTime.Year == DateTime.UtcNow.AddHours(5).Year && w.RegistrationDateTime.Month == DateTime.UtcNow.AddHours(5).Month && w.RegistrationDateTime.Day == DateTime.UtcNow.AddHours(5).Day).Count(),
                ApplicantsCountOnWeek = (await applicantService.GetAll()).Where(w => w.RegistrationDateTime.Year == DateTime.UtcNow.AddHours(5).Year && w.RegistrationDateTime.Month == DateTime.UtcNow.AddHours(5).Month && w.RegistrationDateTime.Day == DateTime.UtcNow.AddHours(5).Day - 1).Count(),
                MoversCount = (await moverService.GetAll()).Count(),
                MoversCountOnMonth = (await moverService.GetAll()).Where(w => w.RegistrationDateTime.Month == DateTime.UtcNow.AddHours(5).Month).Count(),
                MoversCountOnToday = (await moverService.GetAll()).Where(w => w.RegistrationDateTime.Year == DateTime.UtcNow.AddHours(5).Year && w.RegistrationDateTime.Month == DateTime.UtcNow.AddHours(5).Month && w.RegistrationDateTime.Day == DateTime.UtcNow.AddHours(5).Day).Count(),
                MoversCountOnWeek = (await moverService.GetAll()).Where(w => w.RegistrationDateTime.Year == DateTime.UtcNow.AddHours(5).Year && w.RegistrationDateTime.Month == DateTime.UtcNow.AddHours(5).Month && w.RegistrationDateTime.Day== DateTime.UtcNow.AddHours(5).Day - 1).Count(),
                Users = await userService.GetAll()
            };

            return View(model); 
        }
        public IActionResult Success()
        {
            return View();
        }
    }
}
