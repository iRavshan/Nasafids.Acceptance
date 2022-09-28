using Acceptance.Domain;
using Acceptance.Service.Services;
using Acceptance.ViewModels.Invoice;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using OfficeOpenXml;
using System.Collections.Generic;

namespace Acceptance.Controllers
{
    
    public class InvoiceController : Controller
    {
        private readonly IMoverService moverService;
        private readonly IApplicantService applicantService;
        private readonly IPaymentService paymentService;

        public InvoiceController(IMoverService moverService,
                                 IApplicantService applicantService,
                                 IPaymentService paymentService)
        {
            this.moverService = moverService;
            this.applicantService = applicantService;
            this.paymentService = paymentService;
        }

        [HttpGet]
        [Authorize(Roles = "owner")]
        public IActionResult Applicant()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "owner")]
        public IActionResult Mover()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "owner")]
        public IActionResult PaymentForApplicant()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "owner")]
        public IActionResult PaymentForMover()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Applicant(SearchViewModel model)
        {
            Applicant applicant = (await applicantService.GetAll()).FirstOrDefault(w => w.JShIR.Equals(model.SearchText));

            if(applicant == null)
            {
                return View();
            }

            PaymentForApplicant newModel = new PaymentForApplicant
            {
                Applicant = applicant
            };

            return View("paymentforapplicant", newModel);
        }

        [HttpPost]
        public async Task<IActionResult> Mover(SearchViewModel model)
        {
            Mover mover = (await moverService.GetAll()).FirstOrDefault(w => w.JShIR.Equals(model.SearchText));
            
            if(mover == null)
            {
                return View();
            }

            PaymentForMover newModel = new PaymentForMover
            {
                Mover = mover
            };

            return View("paymentformover", newModel);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentForApplicant(PaymentForApplicant model)
        {

            if (ModelState.IsValid)
            {
                Applicant applicant = (await applicantService.GetAll()).FirstOrDefault(w => w.Id.Equals(model.Applicant.Id));
                
                Payment payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    Sum = model.Sum,
                    DateTime = DateTime.UtcNow.AddHours(5)
                };

                await paymentService.Create(payment);

                applicant.Payments.Add(payment);

                applicantService.Update(applicant);

                await applicantService.CompleteAsync();

                return RedirectToAction("payments", "invoice");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PaymentForMover(PaymentForMover model)
        {

            if (ModelState.IsValid)
            {
                Mover mover = (await moverService.GetAll()).FirstOrDefault(w => w.Id.Equals(model.Mover.Id));

                Payment payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    Sum = model.Sum,
                    DateTime = DateTime.UtcNow.AddHours(5)
                };

                await paymentService.Create(payment);

                mover.Payments.Add(payment);

                moverService.Update(mover);

                await moverService.CompleteAsync();

                return RedirectToAction("payments", "invoice");
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> Payments()
        {
            PaymentsViewModel model = new PaymentsViewModel();

            model.Payments = (await paymentService.GetAll()).OrderByDescending(w => w.DateTime);

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> Details(Guid Id)
        {
            DetailsViewModel model = new DetailsViewModel();

            model.Payment = (await paymentService.GetAll()).FirstOrDefault(w => w.Id.Equals(Id));

            return View(model);
        }

        [HttpGet]
        public async Task<FileResult> GetPayment(Guid Id)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Payment payment = (await paymentService.GetAll()).FirstOrDefault(w => w.Id.Equals(Id));

                    Document document = new Document(PageSize.A4, 25, 25, 20, 30);
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);

                    document.Open();
                    FontFactory.RegisterDirectories();

                    iTextSharp.text.Font fuente = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 18);

                    Paragraph title2 = new Paragraph("TO'LOV", FontFactory.GetFont("VERDANA", 22, iTextSharp.text.Font.BOLD));
                    title2.Alignment = Element.ALIGN_CENTER;
                    title2.SpacingAfter = 20;
                    document.Add(title2);

                    Paragraph datetime = new Paragraph("Sana va vaqt: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                    datetime.Alignment = Element.ALIGN_LEFT;
                    datetime.Font = fuente;
                    datetime.IndentationLeft = 50;
                    datetime.Add(new Chunk(payment.DateTime.ToString("dd.MM.yyyy HH:mm:ss"), FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                    document.Add(datetime);

                    Paragraph guid = new Paragraph("To'lov ID raqami: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                    guid.Alignment = Element.ALIGN_LEFT;
                    guid.Font = fuente;
                    guid.IndentationLeft = 50;
                    guid.Add(new Chunk(payment.Id.ToString(), FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                    document.Add(guid);

                    Paragraph fullname = new Paragraph("Talaba: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                    fullname.Alignment = Element.ALIGN_LEFT;
                    fullname.Font = fuente;
                    fullname.IndentationLeft = 50;

                    if(payment.Mover is null)
                    {
                        Faculty fakultet = (await applicantService.GetAll()).FirstOrDefault(w => w.Id.Equals(payment.Applicant.Id)).Faculty;

                        fullname.Add(new Chunk(payment.Applicant.FullName.ToUpper(), FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(fullname);

                        Paragraph passport = new Paragraph("Pasport seriya va raqami: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        passport.Alignment = Element.ALIGN_LEFT;
                        passport.Font = fuente;
                        passport.IndentationLeft = 50;
                        passport.Add(new Chunk(payment.Applicant.PassportSeries + payment.Applicant.PassportNumber, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(passport);

                        Paragraph id = new Paragraph("Shartnoma raqami: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        id.Alignment = Element.ALIGN_LEFT;
                        id.Font = fuente;
                        id.IndentationLeft = 50;
                        id.Add(new Chunk(payment.Applicant.JShIR, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(id);

                        Paragraph faculty = new Paragraph("Fakulteti: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        faculty.Alignment = Element.ALIGN_LEFT;
                        faculty.Font = fuente;
                        faculty.IndentationLeft = 50;
                        faculty.Add(new Chunk(fakultet.Number + " - " + fakultet.Name, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(faculty);

                        Paragraph grade = new Paragraph("Kursi: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        grade.Alignment = Element.ALIGN_LEFT;
                        grade.Font = fuente;
                        grade.IndentationLeft = 50;
                        grade.Add(new Chunk("1 - kurs", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(grade);

                        Paragraph typedu = new Paragraph("Ta'lim shakli: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        typedu.Alignment = Element.ALIGN_LEFT;
                        typedu.Font = fuente;
                        typedu.IndentationLeft = 50;
                        typedu.Add(new Chunk(payment.Applicant.TypeOfEducation, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(typedu);
                    }

                    else
                    {
                        Faculty fakultet = (await moverService.GetAll()).FirstOrDefault(w => w.Id.Equals(payment.Mover.Id)).Faculty;

                        fullname.Add(new Chunk(payment.Mover.FullName.ToUpper(), FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(fullname);

                        Paragraph passport = new Paragraph("Pasport seriya va raqami: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        passport.Alignment = Element.ALIGN_LEFT;
                        passport.Font = fuente;
                        passport.IndentationLeft = 50;
                        passport.Add(new Chunk(payment.Mover.PassportSeries + payment.Mover.PassportNumber, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(passport);

                        Paragraph id = new Paragraph("Shartnoma raqami: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        id.Alignment = Element.ALIGN_LEFT;
                        id.Font = fuente;
                        id.IndentationLeft = 50;
                        id.Add(new Chunk(payment.Mover.JShIR, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(id);

                        Paragraph faculty = new Paragraph("Fakulteti: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        faculty.Alignment = Element.ALIGN_LEFT;
                        faculty.Font = fuente;
                        faculty.IndentationLeft = 50;
                        faculty.Add(new Chunk(fakultet.Number + " - " + fakultet.Name, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(faculty);

                        Paragraph grade = new Paragraph("Kursi: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        grade.Alignment = Element.ALIGN_LEFT;
                        grade.Font = fuente;
                        grade.IndentationLeft = 50;
                        grade.Add(new Chunk("2 - kurs", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(grade);

                        Paragraph typedu = new Paragraph("Ta'lim shakli: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                        typedu.Alignment = Element.ALIGN_LEFT;
                        typedu.Font = fuente;
                        typedu.IndentationLeft = 50;
                        typedu.Add(new Chunk(payment.Mover.TypeOfEducation, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                        document.Add(typedu);
                    }

                    Paragraph sum = new Paragraph("To'lov miqdori: " + payment.Sum.ToString() + " so'm", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                    sum.Alignment = Element.ALIGN_LEFT;
                    sum.Font = fuente;
                    sum.IndentationLeft = 50;
                    document.Add(sum);

                    QRCodeGenerator qrcode = new QRCodeGenerator();
                    QRCodeData data = qrcode.CreateQrCode("http://qabul.nmxu.uz/invoice/getpayment/" + Id, QRCodeGenerator.ECCLevel.L);
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

                    return File(constant, "application/vnd", "To'lov.pdf");
            }
        }

        [HttpGet]
        public async Task<FileResult> GetPayments()
        {
            byte[] fileContent;

            IEnumerable<Payment> payments = await paymentService.GetAll();

            using (var package = new ExcelPackage())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var worksheet = package.Workbook.Worksheets.Add("sheet1");

                for (int i = 1; i <= payments.Count(); i++)
                {
                    worksheet.Cells[i, 1].Value = i;

                    if (payments.ElementAt(i - 1).Applicant is not null)
                        worksheet.Cells[i, 2].Value = payments.ElementAt(i - 1).Applicant.FullName;
                    else worksheet.Cells[i, 2].Value = payments.ElementAt(i - 1).Mover.FullName;

                    worksheet.Cells[i, 3].Value = payments.ElementAt(i - 1).Sum;
                    worksheet.Cells[i, 4].Value = payments.ElementAt(i - 1).DateTime.ToString("dd/MM/yyyy HH:mm:ss");
                }

                fileContent = await package.GetAsByteArrayAsync();
            }

            return File(
                fileContents: fileContent,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "To'lovlar.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var p = (await paymentService.GetAll()).FirstOrDefault(w => w.Id.Equals(Id));
            paymentService.Delete(p);
            await paymentService.CompleteAsync();
            return RedirectToAction("payments");
        }
    }
}
