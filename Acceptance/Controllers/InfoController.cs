using Acceptance.Domain;
using Acceptance.Service;
using Acceptance.Service.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.Controllers
{
    public class InfoController : Controller
    {
        private readonly IApplicantService applicantService;
        private readonly IMoverService moverService;

        public InfoController(IApplicantService applicantService,
                              IMoverService moverService)
        {
            this.applicantService = applicantService;
            this.moverService = moverService;
        }

        [HttpGet]
        public IActionResult Applicants()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Movers()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AllApplicants()
        {
            byte[] fileContent;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            IEnumerable<Applicant> applicants = await applicantService.GetAll();

            using (var package = new ExcelPackage())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var worksheet = package.Workbook.Worksheets.Add("sheet1");

                for (int i = 1; i <= applicants.Count(); i++)
                {
                    worksheet.Cells[i, 1].Value = i;
                    worksheet.Cells[i, 2].Value = applicants.ElementAt(i - 1).FullName;
                    worksheet.Cells[i, 3].Value = applicants.ElementAt(i - 1).Faculty.Name;
                    worksheet.Cells[i, 4].Value = applicants.ElementAt(i - 1).TypeOfEducation;
                    if(applicants.ElementAt(i - 1).TypeOfEducation.ToLower() == "kunduzgi")
                    {
                        worksheet.Cells[i, 5].Value = applicants.ElementAt(i - 1).Faculty.PriceOfDay * 300000;
                    }
                    else
                    {
                        worksheet.Cells[i, 5].Value = applicants.ElementAt(i - 1).Faculty.PriceOfNight * 300000;
                    }
                    worksheet.Cells[i, 6].Value = applicants.ElementAt(i - 1).PassportSeries + applicants.ElementAt(i - 1).PassportNumber;
                    worksheet.Cells[i, 7].Value = applicants.ElementAt(i - 1).PhoneNumber;
                    worksheet.Cells[i, 8].Value = applicants.ElementAt(i - 1).SecondPhoneNumber;
                    worksheet.Cells[i, 9].Value = applicants.ElementAt(i - 1).User.Username;
                }

                fileContent = await package.GetAsByteArrayAsync();
            }

            return File(
                fileContents: fileContent,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "Talabgorlar.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> AllMovers()
        {
            byte[] fileContent;

            IEnumerable<Mover> applicants = await moverService.GetAll();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var worksheet = package.Workbook.Worksheets.Add("sheet1");

                for (int i = 1; i <= applicants.Count(); i++)
                {
                    worksheet.Cells[i, 1].Value = i;
                    worksheet.Cells[i, 2].Value = applicants.ElementAt(i - 1).FullName;
                    worksheet.Cells[i, 3].Value = applicants.ElementAt(i - 1).Faculty.Name;
                    worksheet.Cells[i, 4].Value = applicants.ElementAt(i - 1).TypeOfEducation;
                    if (applicants.ElementAt(i - 1).TypeOfEducation.ToLower() == "kunduzgi")
                    {
                        worksheet.Cells[i, 5].Value = applicants.ElementAt(i - 1).Faculty.PriceOfDay * 300000;
                    }
                    else
                    {
                        worksheet.Cells[i, 5].Value = applicants.ElementAt(i - 1).Faculty.PriceOfNight * 300000;
                    }
                    worksheet.Cells[i, 6].Value = applicants.ElementAt(i - 1).PassportSeries + applicants.ElementAt(i - 1).PassportNumber;
                    worksheet.Cells[i, 7].Value = applicants.ElementAt(i - 1).PhoneNumber;
                    worksheet.Cells[i, 8].Value = applicants.ElementAt(i - 1).SecondPhoneNumber;
                    worksheet.Cells[i, 9].Value = applicants.ElementAt(i - 1).User.Username;
                }

                fileContent = await package.GetAsByteArrayAsync();
            }

            return File(
                fileContents: fileContent,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "O'qishni ko'chiruvchilar.xlsx");
        }
    }
}
