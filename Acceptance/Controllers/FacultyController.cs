using Acceptance.Domain;
using Acceptance.Service.Services;
using Acceptance.ViewModels.Faculty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.Controllers
{
    [Authorize(Roles = "owner")]
    public class FacultyController : Controller
    {
        private readonly IFacultyService facultyService;

        public FacultyController(IFacultyService facultyService)
        {
            this.facultyService = facultyService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Faculties()
        {
            FacultiesViewModel faculties = new FacultiesViewModel
            {
                Faculties = await facultyService.GetAll()
            };

            return View(faculties);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            Faculty faculty = new Faculty
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Number = model.Number,
                PriceOfDay = model.PriceOfDay,
                PriceOfNight = model.PriceOfNight,
                PeriodOfDay= model.PeriodOfDay,
                PeriodOfNight = model.PeriodOfNight
            };

            await facultyService.Create(faculty);

            await facultyService.CompleteAsync();

            FacultiesViewModel faculties = new FacultiesViewModel
            {
                Faculties = await facultyService.GetAll()
            };

            return View("faculties", faculties);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var faculty = await facultyService.GetById(Id);
            facultyService.Delete(faculty);
            await facultyService.CompleteAsync();
            return RedirectToAction("faculties");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var faculty = await facultyService.GetById(Id);

            return View(faculty);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Faculty faculty)
        {
            facultyService.Update(faculty);
            await facultyService.CompleteAsync();
            return RedirectToAction("faculties");
        }
    }
}
