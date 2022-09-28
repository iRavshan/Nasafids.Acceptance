using Acceptance.Domain;
using Acceptance.Service;
using Acceptance.Service.Services;
using Acceptance.ViewModels.Mover;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Acceptance.Controllers
{
    [Authorize]
    public class MoverController : Controller
    {
        private readonly IWebHostEnvironment webHost;
        private readonly IMoverService moverService;
        private readonly IUserService userService;
        private readonly IFacultyService facultyService;
        private readonly IPaymentService paymentService;

        public MoverController(IWebHostEnvironment webHost,
                               IMoverService moverService,
                               IUserService userService,
                               IFacultyService facultyService,
                               IPaymentService paymentService)
        {
            this.webHost = webHost;
            this.moverService = moverService;
            this.userService = userService;
            this.facultyService = facultyService;
            this.paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult> Acceptance()
        {
            AcceptanceViewModel model = new AcceptanceViewModel
            {
                Faculties = await facultyService.GetAll()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Acceptance(AcceptanceViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!await moverService.HasMoverByPassport(model.PassportSeries + model.PassportNumber))
                {
                    AcceptanceViewModel exmodel = new AcceptanceViewModel
                    {
                        Faculties = await facultyService.GetAll()
                    };

                    ModelState.AddModelError("", "Ushbu passport seriya va raqam ro'yxatga olingan");
                    return View(exmodel);
                }

                if (!await moverService.HasMoverByJshir(model.JShIR))
                {
                    AcceptanceViewModel exmodel = new AcceptanceViewModel
                    {
                        Faculties = await facultyService.GetAll()
                    };

                    ModelState.AddModelError("", "Ushbu JShIR ro'yxatga olingan");
                    return View(exmodel);
                }

                Mover applicant = new Mover()
                {
                    Id = Guid.NewGuid(),
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    RegistrationDateTime = DateTime.UtcNow.AddHours(5),
                    Faculty = await facultyService.GetById(Guid.Parse(model.Faculty)),
                    TypeOfEducation = model.TypeOfEducation,
                    JShIR = model.JShIR,
                    Location = model.Location,
                    PassportNumber = model.PassportNumber,
                    PassportSeries = model.PassportSeries.ToUpper(),
                    SecondPhoneNumber = model.SecondPhoneNumber,
                    State = 0,
                    Payments = null,
                    User = await userService.GetById(Guid.Parse(User.Claims.FirstOrDefault(w => w.Type.Equals(ClaimTypes.NameIdentifier)).Value))
                };

                await moverService.Create(applicant);

                await moverService.CompleteAsync();

                return RedirectToAction("index", "home");
            }

            AcceptanceViewModel emodel = new AcceptanceViewModel
            {
                Faculties = await facultyService.GetAll()
            };

            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public async Task<IActionResult> Search(MoversViewModel model)
        {
            MoversViewModel newmodel = new MoversViewModel();

            if (User.IsInRole("guest") || User.IsInRole("host"))
            {
                if (model.SearchText is not null)
                {
                    newmodel.Movers = (await moverService.GetByFullname(model.SearchText))
                             .Where(w => w.User.Username.Equals(User.Identity.Name));
                }

                else newmodel.Movers = (await moverService.GetAll())
                             .Where(w => w.User.Username.Equals(User.Identity.Name));
            }

            else
            {
                if (model.SearchText is not null)
                {
                    newmodel.Movers = (await moverService.GetByFullname(model.SearchText));
                }

                else newmodel.Movers = (await moverService.GetAll());
            }

            newmodel.SearchText = model.SearchText;

            return View("movers", newmodel);
        }

        [HttpGet]
        public async Task<IActionResult> Movers()
        {
            MoversViewModel model = new MoversViewModel();

            if (User.IsInRole("guest") || User.IsInRole("host"))
            {
                model.Movers = (await moverService.GetAll())
                             .Where(w => w.User.Username.Equals(User.Identity.Name));
            }

            else
            {
                model.Movers = await moverService.GetAll();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var applicant = (await moverService.GetAll()).
                Where(w => w.Id.Equals(Id)).FirstOrDefault();

            AcceptanceViewModel model = new AcceptanceViewModel
            {
                Id = applicant.Id,
                FullName = applicant.FullName,
                PassportNumber = applicant.PassportNumber,
                PassportSeries = applicant.PassportSeries,
                JShIR = applicant.JShIR,
                Location = applicant.Location,
                Faculties = await facultyService.GetAll(),
                Faculty = applicant.Faculty.Name,
                TypeOfEducation = applicant.TypeOfEducation,
                PhoneNumber = applicant.PhoneNumber,
                SecondPhoneNumber = applicant.SecondPhoneNumber
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid Id)
        {
            DetailsViewModel model = new DetailsViewModel()
            {
                Mover = (await moverService.GetAll()).Where(w => w.Id.Equals(Id)).FirstOrDefault()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AcceptanceViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!await moverService.MoverExistByNumber(model.PhoneNumber))
                {
                    AcceptanceViewModel exmodel = new AcceptanceViewModel
                    {
                        Faculties = await facultyService.GetAll()
                    };

                    ModelState.AddModelError("", "Ushbu telefon raqam ro'yxatga olingan");
                    return View(exmodel);
                }

                var applicant = (await moverService.GetAll()).
                    Where(w => w.Id.Equals(model.Id)).FirstOrDefault();

                if (model.Faculty is not null)
                {
                    applicant.Faculty = await facultyService.GetById(Guid.Parse(model.Faculty));
                }

                applicant.FullName = model.FullName;
                applicant.JShIR = model.JShIR;
                applicant.Location = model.Location;
                applicant.PassportNumber = model.PassportNumber;
                applicant.PassportSeries = model.PassportSeries;
                applicant.SecondPhoneNumber = model.SecondPhoneNumber;
                applicant.TypeOfEducation = model.TypeOfEducation;
                applicant.PhoneNumber = model.PhoneNumber;
                if(applicant.State != 2)
                {
                    applicant.State = 0;
                }
                moverService.Update(applicant);

                await moverService.CompleteAsync();

                DetailsViewModel viewmodel = new DetailsViewModel
                {
                    Mover = applicant
                };

                return View("details", viewmodel);
            }

            AcceptanceViewModel emodel = new AcceptanceViewModel
            {
                Faculties = await facultyService.GetAll()
            };

            return View(emodel);
        }

        [HttpPost]
        public async Task<IActionResult> MoverToAccept(Guid Id)
        {
            var mover = (await moverService.GetAll()).Where(w => w.Id.Equals(Id)).FirstOrDefault();

            mover.State = 2;

            moverService.Update(mover);

            await moverService.CompleteAsync();

            return View("details", new DetailsViewModel { Mover = mover });
        }

        [HttpPost]
        public async Task<IActionResult> MoverToRepetition(Guid Id)
        {
            var mover = (await moverService.GetAll()).Where(w => w.Id.Equals(Id)).FirstOrDefault();

            mover.State = 1;

            moverService.Update(mover);

            await moverService.CompleteAsync();

            return View("details", new DetailsViewModel { Mover = mover });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMover(Guid Id)
        {
            var mover = (await moverService.GetAll()).FirstOrDefault(w => w.Id.Equals(Id));

            foreach (Payment payment in mover.Payments)
            {
                if (payment is not null)
                {
                    paymentService.Delete(payment);
                }
            }

            moverService.Delete(mover);

            await moverService.CompleteAsync();

            return RedirectToAction("movers");
        }

        [HttpPost]
        public async Task<FileResult> Receipt(Guid Id)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var applicant = (await moverService.GetAll())
                        .Where(w => w.Id.Equals(Id)).FirstOrDefault();

                Document document = new Document(PageSize.A4, 25, 25, 40, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();
                FontFactory.RegisterDirectories();

                string logoPath = Path.Combine(webHost.WebRootPath, "images\\logo", "little-logo.png");

                var logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.Alignment = Element.ALIGN_CENTER;
                document.Add(logo);

                iTextSharp.text.Font fuente = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 18);


                PdfPTable pt = new PdfPTable(1);
                PdfPCell _cell;


                _cell = new PdfPCell(new Phrase("NASAFIYLAR MEROSI XALQARO UNIVERSITETI", FontFactory.GetFont("VERDANA", 14, iTextSharp.text.Font.BOLD)));
                _cell.VerticalAlignment = Element.ALIGN_CENTER;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = 0;
                pt.AddCell(_cell);

                _cell = new PdfPCell(new Phrase("NODAVLAT OLIY TA’LIM MUASSASASI", FontFactory.GetFont("VERDANA", 14, iTextSharp.text.Font.BOLD)));
                _cell.VerticalAlignment = Element.ALIGN_CENTER;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = 0;
                pt.AddCell(_cell);

                pt.SpacingBefore = 30;
                pt.SpacingAfter = 50;

                document.Add(pt);

                Paragraph title = new Paragraph(DateTime.UtcNow.AddHours(5).ToString("dd/MM/yyyy HH:mm:ss"));
                title.Alignment = Element.ALIGN_CENTER;
                title.Font = fuente;
                title.SpacingAfter = 40;
                title.Add(new Chunk("                                                      N " + applicant.JShIR, new iTextSharp.text.Font(FontFactory.GetFont("VERDANA", 12))));
                document.Add(title);

                Paragraph title2 = new Paragraph("T I L X A T");
                title2.Alignment = Element.ALIGN_CENTER;
                title2.Font = fuente;
                title2.SpacingAfter = 20;
                document.Add(title2);

                Paragraph faculty = new Paragraph("1. Yo'nalish nomi: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                faculty.Alignment = Element.ALIGN_LEFT;
                faculty.Font = fuente;
                faculty.IndentationLeft = 50;
                faculty.Add(new Chunk(applicant.Faculty.Name, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                document.Add(faculty);

                Paragraph fullname = new Paragraph("2. Talabgorning F.I.SH: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                fullname.Alignment = Element.ALIGN_LEFT;
                fullname.Font = fuente;
                fullname.IndentationLeft = 50;
                fullname.Add(new Chunk(applicant.FullName, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                document.Add(fullname);

                Paragraph TypeOfEducation = new Paragraph("3. Ta'lim turi: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                TypeOfEducation.Alignment = Element.ALIGN_LEFT;
                TypeOfEducation.Font = fuente;
                TypeOfEducation.IndentationLeft = 50;
                TypeOfEducation.SpacingAfter = 30;
                TypeOfEducation.Add(new Chunk(applicant.TypeOfEducation, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                document.Add(TypeOfEducation);

                Paragraph subtitle = new Paragraph("Nasafiylar Merosi Xalqaro Universiteti talabgorning quyidagi hujjatlarini qabul qildi:");
                subtitle.Alignment = Element.ALIGN_CENTER;
                subtitle.Font = fuente;
                subtitle.SpacingAfter = 30;
                document.Add(subtitle);

                Paragraph diploma = new Paragraph("1. Attestat yoki diplom (asl nusxa)", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL));
                diploma.Alignment = Element.ALIGN_LEFT;
                diploma.IndentationLeft = 50;
                document.Add(diploma);

                Paragraph photo = new Paragraph("2. Rasm 3x4 (6 dona) ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL));
                photo.Alignment = Element.ALIGN_LEFT;
                photo.IndentationLeft = 50;
                document.Add(photo);
                
                Paragraph passport = new Paragraph("3. Pasport nusxasi ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL));
                passport.Alignment = Element.ALIGN_LEFT;
                passport.IndentationLeft = 50;
                document.Add(passport);

                Paragraph transcript = new Paragraph("4.  Akademik ma'lumotnoma (o'qishni ko'chiruvchilar uchun kursni tugatganligi to'g'risida transkript asli) ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL));
                transcript.Alignment = Element.ALIGN_LEFT;
                transcript.IndentationLeft = 50;
                transcript.SpacingAfter = 30;
                document.Add(transcript);

                Paragraph author = new Paragraph("Nasafiylar Merosi Xalqaro Universiteti\n", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                author.Add("qabul komissiyasi mas'ul kotibi:                                                   Saidov F.");
                author.IndentationLeft = 50;
                author.SpacingAfter = 30;
                document.Add(author);

                QRCodeGenerator qrcode = new QRCodeGenerator();
                QRCodeData data = qrcode.CreateQrCode("http://qabul.nmxu.uz/cabinet", QRCodeGenerator.ECCLevel.L);
                QRCode code = new QRCode(data);

                using (MemoryStream ms2 = new MemoryStream())
                {
                    using (Bitmap bitmap = code.GetGraphic(5))
                    {
                        bitmap.Save(ms2, ImageFormat.Png);

                        var image = Convert.ToBase64String(ms2.ToArray());

                        Byte[] bytes = Convert.FromBase64String(image);
                        iTextSharp.text.Image sigimage = iTextSharp.text.Image.GetInstance(bytes);
                        sigimage.ScalePercent(40f);

                        sigimage.Alignment = Element.ALIGN_RIGHT;
                        sigimage.IndentationRight = 50;

                        document.Add(sigimage);
                    }
                }

                document.Close();
                writer.Close();
                ms.Close();

                var constant = ms.ToArray();

                return File(constant, "application/vnd", "Tilxat.pdf");
            }
        }

        [HttpGet]
        public async Task<FileResult> GetExcelFile()
        {
            byte[] fileContent;

            IEnumerable<Mover> applicants = await moverService.GetAll();

            using (var package = new ExcelPackage())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var worksheet = package.Workbook.Worksheets.Add("sheet1");

                for (int i = 1; i <= applicants.Count(); i++)
                {
                    worksheet.Cells[i, 1].Value = "998" + applicants.ElementAt(i - 1).PhoneNumber.Replace("-", string.Empty).Replace(")", string.Empty).Replace("(", string.Empty).Replace(" ", string.Empty);
                    worksheet.Cells[i, 2].Value = "998" + applicants.ElementAt(i - 1).SecondPhoneNumber.Replace("-", string.Empty).Replace(")", string.Empty).Replace("(", string.Empty).Replace(" ", string.Empty);
                    worksheet.Cells[i, 1].Style.Font.Size = 14;
                    worksheet.Cells[i, 2].Style.Font.Size = 14;
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
