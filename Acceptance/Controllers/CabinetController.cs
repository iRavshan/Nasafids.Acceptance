using Acceptance.ContractClasses;
using Acceptance.Domain;
using Acceptance.Service.Services;
using Acceptance.ViewModels.Cabinet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Acceptance.Controllers
{
    public class CabinetController : Controller
    {
        private readonly IApplicantService applicantService;
        private readonly IMoverService moverService;
        private readonly IWebHostEnvironment webHost;

        public CabinetController(IApplicantService applicantService,
                                 IMoverService moverService,
                                 IWebHostEnvironment webHost)
        {
            this.applicantService = applicantService;
            this.moverService = moverService;
            this.webHost = webHost;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (ModelState.IsValid)
            {
                InfoViewModel infoModel = new InfoViewModel();

                Applicant student = await applicantService.GetByPassport(model.FullPassportSeriesNumber.ToUpper());

                if (student.FullName is null)
                {
                    Mover estudent = await moverService.GetByPassport(model.FullPassportSeriesNumber.ToUpper());

                    if(estudent.FullName is null)
                    {
                        ModelState.AddModelError(string.Empty, "Ma'lumot topilmadi. Qayta urinib ko'ring");
                        return View();
                    }

                    infoModel.Id = estudent.Id;

                    infoModel.Name = estudent.FullName;

                    infoModel.Payments = (await moverService.GetAll()).FirstOrDefault(w => w.Id.Equals(estudent.Id)).Payments;

                    return View("Info", infoModel);
                }

                infoModel.Id = student.Id;
                infoModel.Name = student.FullName;
                infoModel.Payments = student.Payments;
                return View("Info", infoModel);
            }

            return View();
        }

        [HttpPost]
        public async Task<FileResult> Receipt(Guid Id)
        {
            Applicant applicant = (await applicantService.GetAll())
                                    .Where(w => w.Id.Equals(Id)).FirstOrDefault();

            if (applicant is not null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
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
                    passport.SpacingAfter = 30;
                    document.Add(passport);

                    Paragraph author = new Paragraph("Nasafiylar Merosi Xalqaro Universiteti\n", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                    author.Add("qabul komissiyasi mas'ul kotibi:                                                   Saidov F.");
                    author.IndentationLeft = 50;
                    author.SpacingAfter = 30;
                    document.Add(author);

                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        using (Bitmap bitmap = NewQrCode().GetGraphic(5))
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

            Mover eapplicant = (await moverService.GetAll())
                                    .Where(w => w.Id.Equals(Id)).FirstOrDefault();

            using (MemoryStream ms = new MemoryStream())
            {
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
                title.Add(new Chunk("                                                      N " + eapplicant.JShIR, new iTextSharp.text.Font(FontFactory.GetFont("VERDANA", 12))));
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
                faculty.Add(new Chunk(eapplicant.Faculty.Name, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                document.Add(faculty);

                Paragraph fullname = new Paragraph("2. Talabgorning F.I.SH: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                fullname.Alignment = Element.ALIGN_LEFT;
                fullname.Font = fuente;
                fullname.IndentationLeft = 50;
                fullname.Add(new Chunk(eapplicant.FullName, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
                document.Add(fullname);

                Paragraph TypeOfEducation = new Paragraph("3. Ta'lim turi: ", FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.BOLD));
                TypeOfEducation.Alignment = Element.ALIGN_LEFT;
                TypeOfEducation.Font = fuente;
                TypeOfEducation.IndentationLeft = 50;
                TypeOfEducation.SpacingAfter = 30;
                TypeOfEducation.Add(new Chunk(eapplicant.TypeOfEducation, FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL)));
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

                using (MemoryStream ms2 = new MemoryStream())
                {
                    using (Bitmap bitmap = NewQrCode().GetGraphic(5))
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

        [HttpPost]
        public async Task<FileResult> Contract(Guid Id)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 30, 30, 20, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                
                document.Add(NewDefaultTitle("To'lov-kontrakt (ikki tomonlama) asosida mutaxassis tayyorlashga"));

                Applicant applicant = (await applicantService.GetAll())
                                    .Where(w => w.Id.Equals(Id)).FirstOrDefault();

                if (applicant is not null)
                {
                    document.Add(NewDefaultTitle("KONTRAKT N: " + applicant.JShIR));

                    document.Add(NewDefaultTitle("Qarshi shahri                                                                                               " + DateTime.Now.ToString("dd/MM/yyyy")));

                    Paragraph mainTitle = new Paragraph("    NASAFIYLAR MEROSI XALQARO UNIVERSITETI (keyingi o'rinlarda “Ta’lim muassasasi”) nomidan rektor (direktor) KURBONOV AKMAL SHERALIYEVICH bir tomondan, " + applicant.FullName.ToUpper() + " (keyingi o'rinlarda “Ta’lim oluvchi”) ikkinchi tomondan, birgalikda “Tomonlar” deb ataladigan shaxslar mazkur kontraktni quyidagicha tuzdilar:");
                    mainTitle.Alignment = Element.ALIGN_LEFT;
                    mainTitle.SpacingBefore = 30;
                    mainTitle.SpacingAfter = 15;
                    mainTitle.Font = new iTextSharp.text.Font(FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL));
                    document.Add(mainTitle);

                    document.Add(NewHeader(Subject.header));

                    document.Add(NewParagraph(Subject.OneOne));
                    document.Add(NewParagraph("Ta'lim bosqichi: Bakalavr"));
                    document.Add(NewParagraph("Ta'lim shakli: " + applicant.TypeOfEducation));

                    if (applicant.TypeOfEducation.ToLower().Equals("kunduzgi"))
                    {
                        document.Add(NewParagraph("O'qish muddati: " + applicant.Faculty.PeriodOfDay + " yil"));
                    }
                    else
                        document.Add(NewParagraph("O'qish muddati: " + applicant.Faculty.PeriodOfNight + " yil"));

                    document.Add(NewParagraph("O'quv kursi: 1-kurs 1-semestr"));
                    document.Add(NewParagraph("Ta'lim yo'nalishi: " + applicant.Faculty.Number + " - " + applicant.Faculty.Name));
                    
                    document.Add(NewParagraph(Subject.OneTwo));

                    document.Add(NewHeader(Price.header));

                    document.Add(NewParagraph(Price.TwoOne));

                    if (applicant.TypeOfEducation.ToLower().Equals("kunduzgi"))
                    {
                        document.Add(NewParagraph("2.2. Ushbu kontrakt bo‘yicha ta’lim oluvchini bir yillik o‘qitish uchun to‘lov bazaviy hisoblash miqdorining " + applicant.Faculty.PriceOfDay + " barobari miqdorida " + applicant.Faculty.PriceOfDay * 300000 + " so'mni (Stipendiyasiz) tashkil etadi va quyidagi muddatda amalga oshiriladi:"));
                    }
                    else
                        document.Add(NewParagraph("2.2. Ushbu kontrakt bo‘yicha ta’lim oluvchini bir yillik o‘qitish uchun to‘lov bazaviy hisoblash miqdorining " + applicant.Faculty.PriceOfNight + " barobari miqdorida " + applicant.Faculty.PriceOfNight * 300000 + " so'mni (Stipendiyasiz) tashkil etadi va quyidagi muddatda amalga oshiriladi:"));

                }

                else
                {
                    Mover mover = (await moverService.GetAll())
                                    .Where(w => w.Id.Equals(Id)).FirstOrDefault();

                    document.Add(NewDefaultTitle("KONTRAKT N: " + mover.JShIR));

                    document.Add(NewDefaultTitle("Qarshi shahri                                                                                                  " + DateTime.Now.ToString("dd/MM/yyyy")));

                    Paragraph mainTitle = new Paragraph("    NASAFIYLAR MEROSI XALQARO UNIVERSITETI (keyingi o'rinlarda “Ta’lim muassasasi”) nomidan rektor (direktor) KURBONOV AKMAL SHERALIYEVICH bir tomondan, " + mover.FullName.ToUpper() + " (keyingi o'rinlarda “Ta’lim oluvchi”) ikkinchi tomondan, birgalikda “Tomonlar” deb ataladigan shaxslar mazkur kontraktni quyidagicha tuzdilar:");
                    mainTitle.Alignment = Element.ALIGN_LEFT;
                    mainTitle.SpacingBefore = 30;
                    mainTitle.SpacingAfter = 15;
                    mainTitle.Font = new iTextSharp.text.Font(FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL));
                    document.Add(mainTitle);

                    document.Add(NewHeader(Subject.header));

                    document.Add(NewParagraph(Subject.OneOne));
                    document.Add(NewParagraph("Ta'lim bosqichi: Bakalavr"));
                    document.Add(NewParagraph("Ta'lim shakli: " + mover.TypeOfEducation));

                    if (mover.TypeOfEducation.ToLower().Equals("kunduzgi"))
                    {
                        document.Add(NewParagraph("O'qish muddati: " + (mover.Faculty.PeriodOfDay - 1) + " yil"));
                    }
                    else
                        document.Add(NewParagraph("O'qish muddati: " + (mover.Faculty.PeriodOfNight - 1) + " yil"));

                    document.Add(NewParagraph("O'quv kursi: 2-kurs 1-semestr"));
                    document.Add(NewParagraph("Ta'lim yo'nalishi: " + mover.Faculty.Number + " - " + mover.Faculty.Name));
                    document.Add(NewParagraph(Subject.OneTwo));

                    document.Add(NewHeader(Price.header));

                    document.Add(NewParagraph(Price.TwoOne));

                    if (mover.TypeOfEducation.ToLower().Equals("kunduzgi"))
                    {
                        document.Add(NewParagraph("2.2. Ushbu kontrakt bo‘yicha ta’lim oluvchini bir yillik o‘qitish uchun to‘lov bazaviy hisoblash miqdorining " + mover.Faculty.PriceOfDay + " barobari miqdorida "+ mover.Faculty.PriceOfDay * 300000 + " so'mni (Stipendiyasiz) tashkil etadi va quyidagi muddatda amalga oshiriladi:"));
                    }
                    else
                        document.Add(NewParagraph("2.2. Ushbu kontrakt bo‘yicha ta’lim oluvchini bir yillik o‘qitish uchun to‘lov bazaviy hisoblash miqdorining " + mover.Faculty.PriceOfNight + " barobari miqdorida " + mover.Faculty.PriceOfNight * 300000 + " so'mni (Stipendiyasiz) tashkil etadi va quyidagi muddatda amalga oshiriladi:"));

                }

                document.Add(NewLiParagraph(Price.LiOne));
                document.Add(NewParagraph(Price.TwoThree));

                document.Add(NewHeader(CommitmentOfUniversity.header));

                document.Add(NewTitle(CommitmentOfUniversity.title));

                document.Add(NewLiParagraph(CommitmentOfUniversity.LiOne));
                document.Add(NewLiParagraph(CommitmentOfUniversity.LiTwo));
                document.Add(NewLiParagraph(CommitmentOfUniversity.LiThree));
                document.Add(NewLiParagraph(CommitmentOfUniversity.LiFour));
                document.Add(NewLiParagraph(CommitmentOfUniversity.LiFive));
                document.Add(NewLiParagraph(CommitmentOfUniversity.LiSix));

                document.Add(NewTitle(CommitmentOfStudent.title));

                document.Add(NewLiParagraph(CommitmentOfStudent.LiOne));
                document.Add(NewLiParagraph(CommitmentOfStudent.LiTwo));
                document.Add(NewLiParagraph(CommitmentOfStudent.LiThree));
                document.Add(NewLiParagraph(CommitmentOfStudent.LiFour));
                document.Add(NewLiParagraph(CommitmentOfStudent.LiFive));

                document.Add(NewHeader(RightOfUniversity.header));

                document.Add(NewTitle(RightOfUniversity.title));

                document.Add(NewLiParagraph(RightOfUniversity.LiOne));
                document.Add(NewLiParagraph(RightOfUniversity.LiTwo));
                document.Add(NewLiParagraph(RightOfUniversity.LiThree));
                document.Add(NewLiParagraph(RightOfUniversity.LiFour));

                document.Add(NewTitle(RightOfStudent.title));

                document.Add(NewLiParagraph(RightOfStudent.LiOne));
                document.Add(NewLiParagraph(RightOfStudent.LiTwo));
                document.Add(NewLiParagraph(RightOfStudent.LiThree));

                document.Add(NewHeader(Period.header));

                document.Add(NewParagraph(Period.FiveOne));
                document.Add(NewParagraph(Period.FiveTwo));
                document.Add(NewParagraph(Period.FiveThree));
                document.Add(NewParagraph(Period.FiveFour));
                document.Add(NewLiParagraph(Period.LiOne));
                document.Add(NewLiParagraph(Period.LiTwo));
                document.Add(NewLiParagraph(Period.LiThree));
                document.Add(NewLiParagraph(Period.LiFour));
                document.Add(NewLiParagraph(Period.LiFive));

                document.Add(NewHeader(Rules.header));

                document.Add(NewParagraph(Rules.SixOne));
                document.Add(NewParagraph(Rules.SixTwo));
                document.Add(NewParagraph(Rules.SixThree));
                document.Add(NewParagraph(Rules.SixFour));
                document.Add(NewParagraph(Rules.SixFive));

                document.Add(NewHeader("VII. TOMONLARNING REKVIZITLARI VA IMZOLARI"));

                PdfPTable table = new PdfPTable(2);
                PdfPCell cell = new PdfPCell(new Phrase("7.1. Ta'lim muassasasi:\n ", FontFactory.GetFont("VERDANA", 11, iTextSharp.text.Font.NORMAL)));
                cell.Phrase.Add(new Phrase("", FontFactory.GetFont("VERDANA", 11, iTextSharp.text.Font.NORMAL)));
                cell.Phrase.Add("NASAFIYLAR MEROSI XALQARO UNIVERSITETI\n\n\nPochta manzili:\n180100 Qashqadaryo viloyati, Qarshi shahri,\nBog'ishamol mahallasi, Chorbog' ko'chasi 2-uy\n\n\nBank rekvizitlari:\nX/R 20208000505556017001 MFO 00192\nSTIR 309769293 JSCB Turon Bank Qarshi filiali\n\n\nTa'lim muassasasi rahbari:\nKURBONOV AKMAL SHERALIYEVICH");
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Border = 0;
                table.AddCell(cell);

                PdfPCell cell2 = new PdfPCell(new Phrase("Ta'lim oluvchi:\n ", FontFactory.GetFont("VERDANA", 11, iTextSharp.text.Font.NORMAL)));
                cell2.Phrase.Add(new Phrase("", FontFactory.GetFont("VERDANA", 11, iTextSharp.text.Font.NORMAL)));

                Applicant napplicant = (await applicantService.GetAll())
                                    .Where(w => w.Id.Equals(Id)).FirstOrDefault();

                if(applicant is not null)
                {
                    cell2.Phrase.Add("F.I.SH.:"+napplicant.FullName.ToUpper()+"\nYashash manzili: "+ napplicant.Location+"\nPassport ma'lumotlari: " +napplicant.PassportSeries + napplicant.PassportNumber+"\nTalaba kodi: "+napplicant.JShIR+"\nTelefon raqami: "+napplicant.PhoneNumber+"\nTa'lim oluvching imzosi:\n");
                }

                else
                {
                    Mover eapplicant = (await moverService.GetAll())
                                    .Where(w => w.Id.Equals(Id)).FirstOrDefault();

                    cell2.Phrase.Add("F.I.SH.:" + eapplicant.FullName.ToUpper() + "\nYashash manzili: " + eapplicant.Location + "\nPassport ma'lumotlari: " + eapplicant.PassportSeries + eapplicant.PassportNumber + "\nTalaba kodi: " + eapplicant.JShIR + "\nTelefon raqami: " + eapplicant.PhoneNumber + "\nTa'lim oluvching imzosi:\n");

                }


                cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell2.Border = 0;
                table.AddCell(cell2);

                document.Add(table);

                using (MemoryStream ms2 = new MemoryStream())
                {
                    using (Bitmap bitmap = NewQrCode().GetGraphic(5))
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
                return File(constant, "application/vnd", "Shartnoma.pdf");
            }
        }

        private Paragraph NewParagraph(string text)
        {
            FontFactory.RegisterDirectories();
            Paragraph p = new Paragraph(text);
            p.Font = new iTextSharp.text.Font(FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL));
            p.Alignment = Element.ALIGN_LEFT;
            return p;
        }

        private Paragraph NewLiParagraph(string text)
        {
            FontFactory.RegisterDirectories();
            Paragraph p = new Paragraph("•  " + text);
            p.Font = new iTextSharp.text.Font(FontFactory.GetFont("VERDANA", 12, iTextSharp.text.Font.NORMAL));
            p.Alignment = Element.ALIGN_LEFT;
            p.IndentationLeft = 35;
            return p;
        }

        private PdfPTable NewHeader(string text)
        {
            FontFactory.RegisterDirectories();
            PdfPTable table = new PdfPTable(1);
            PdfPCell cell;
            cell= new PdfPCell(new Phrase(text, FontFactory.GetFont("VERDANA", 10, iTextSharp.text.Font.BOLD)));
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = 0;
            table.SpacingBefore = 15;
            table.SpacingAfter = 10;
            table.AddCell(cell);
            return table;
        }

        private Paragraph NewDefaultTitle(string text)
        {
            FontFactory.RegisterDirectories();
            Paragraph DocSubTitle = new Paragraph(text);
            DocSubTitle.Alignment = Element.ALIGN_CENTER;
            DocSubTitle.Font = new iTextSharp.text.Font(FontFactory.GetFont("VERDANA", 18, iTextSharp.text.Font.NORMAL));
            return DocSubTitle;
        }

        private PdfPTable NewTitle(string text)
        {
            FontFactory.RegisterDirectories();
            PdfPTable table = new PdfPTable(1);
            PdfPCell cell;
            cell = new PdfPCell(new Phrase(text, FontFactory.GetFont("VERDANA", 11, iTextSharp.text.Font.BOLD)));
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 0;
            cell.Padding = 0;
            table.SpacingBefore = 20;
            table.AddCell(cell);
            return table;
        }

        private QRCode NewQrCode()
        {
            QRCodeGenerator qrcode = new QRCodeGenerator();
            QRCodeData data = qrcode.CreateQrCode("http://qabul.nmxu.uz/cabinet", QRCodeGenerator.ECCLevel.L);
            QRCode code = new QRCode(data);
            return code;
        }
    }
}
