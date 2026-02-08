using Microsoft.AspNetCore.Mvc;
using MVC.implement;
using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MVC.Models;
using MVC.interfaces;

namespace MVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _repo;

        public ReportController(IReportRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            string userJson = HttpContext.Session.GetString("UserData");

            if (!string.IsNullOrEmpty(userJson))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public JsonResult GetAggregatedReport(string? slicer, string? start, string? end)
        {
            string userJson = HttpContext.Session.GetString("UserData");

            if (string.IsNullOrEmpty(userJson))
            {
                return Json(new { Redirect = "/Home/Login" });
            }

            try
            {
                if (string.IsNullOrEmpty(slicer) && (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end)))
                    return Json(new List<object>());

                DateTime startDate, endDate;
                var today = DateTime.Today;

                if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end)
                    && DateTime.TryParse(start, out var s) && DateTime.TryParse(end, out var e))
                {
                    startDate = s.Date;
                    endDate = e.Date;

                    // Prevent future dates
                    if (startDate > today) startDate = today;
                    if (endDate > today) endDate = today;


                    if (startDate > endDate)
                    {
                        return Json(new { Error = "Start Date cannot be greater than End Date." });
                    }
                }
                else if (!string.IsNullOrEmpty(slicer))
                {
                    slicer = slicer.ToLower().Trim();
                    switch (slicer)
                    {
                        case "lastmonth":
                            var lastMonth = today.AddMonths(-1);
                            startDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                            endDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
                            break;

                        case "lastquarter":
                            int currentQuarter = ((today.Month - 1) / 3) + 1;
                            int lastQuarter = currentQuarter - 1;
                            int year = today.Year;
                            if (lastQuarter == 0) { lastQuarter = 4; year -= 1; }
                            startDate = new DateTime(year, (lastQuarter - 1) * 3 + 1, 1);
                            endDate = startDate.AddMonths(3).AddDays(-1);
                            break;

                        case "lastyear":
                            startDate = new DateTime(today.Year - 1, 1, 1);
                            endDate = new DateTime(today.Year - 1, 12, 31);
                            break;

                        default:
                            return Json(new List<object>());
                    }
                }
                else
                {
                    return Json(new List<object>());
                }

                var reportData = _repo.FetchAggregatedReport(startDate, endDate);
                return Json(reportData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetAggregatedReport: " + ex);
                return Json(new { Error = "Server error occurred. Check logs." });
            }
        }

        [HttpGet]
        public IActionResult PrintReport(string slicer, string? start, string? end)
        {
            string userJson = HttpContext.Session.GetString("UserData");

            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                DateTime startDate, endDate;
                var today = DateTime.Today;

                if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end)
                    && DateTime.TryParse(start, out var s) && DateTime.TryParse(end, out var e))
                {
                    startDate = s.Date;
                    endDate = e.Date;

                    // Prevent future dates
                    if (startDate > today) startDate = today;
                    if (endDate > today) endDate = today;

                    // Ensure start <= end
                    if (startDate > endDate)
                        return Content("Start Date cannot be greater than End Date.");
                }
                else if (!string.IsNullOrEmpty(slicer))
                {
                    slicer = slicer.ToLower().Trim();
                    switch (slicer)
                    {
                        case "lastmonth":
                            var lastMonth = today.AddMonths(-1);
                            startDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                            endDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
                            break;

                        case "lastquarter":
                            int currentQuarter = ((today.Month - 1) / 3) + 1;
                            int lastQuarter = currentQuarter - 1;
                            int year = today.Year;
                            if (lastQuarter == 0) { lastQuarter = 4; year -= 1; }
                            startDate = new DateTime(year, (lastQuarter - 1) * 3 + 1, 1);
                            endDate = startDate.AddMonths(3).AddDays(-1);
                            break;

                        case "lastyear":
                            startDate = new DateTime(today.Year - 1, 1, 1);
                            endDate = new DateTime(today.Year - 1, 12, 31);
                            break;

                        default:
                            return Content("Invalid slicer selected.");
                    }
                }
                else
                {
                    return Content("Please select a Quick Filter or a valid date range to print the report.");
                }

                var (transactions, grandTotal) = _repo.FetchTransactionsForPdf(startDate, endDate);

                // PDF generation logic
                using var ms = new MemoryStream();
                Document doc = new Document(PageSize.A4, 20, 20, 20, 20);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                Paragraph title = new Paragraph("Expense Report", FontFactory.GetFont(FontFactory.HELVETICA, 16, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                doc.Add(title);

                doc.Add(new Paragraph($"From: {startDate:yyyy-MM-dd}  To: {endDate:yyyy-MM-dd}"));
                doc.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(6) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 10, 20, 15, 15, 15, 15 });

                string[] headers = { "User ID", "User Name", "Category", "Amount (₹)", "Payment Mode", "Date" };
                foreach (var h in headers)
                {
                    var cell = new PdfPCell(new Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.BOLD)))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        BackgroundColor = new BaseColor(220, 220, 220)
                    };
                    table.AddCell(cell);
                }

                foreach (var t in transactions)
                {
                    table.AddCell(new PdfPCell(new Phrase(t.userId)));
                    table.AddCell(new PdfPCell(new Phrase(t.userName)));
                    table.AddCell(new PdfPCell(new Phrase(t.category)));
                    table.AddCell(new PdfPCell(new Phrase(t.amount.ToString("n2"))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    table.AddCell(new PdfPCell(new Phrase(t.paymentMode)));
                    table.AddCell(new PdfPCell(new Phrase(t.transDate)));
                }

                var totalCell = new PdfPCell(new Phrase("Grand Total", FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.BOLD)))
                {
                    Colspan = 3,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                table.AddCell(totalCell);

                var totalAmountCell = new PdfPCell(new Phrase(grandTotal.ToString("n2"), FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.BOLD)))
                {
                    Colspan = 3,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                table.AddCell(totalAmountCell);

                doc.Add(table);
                doc.Close();

                byte[] fileBytes = ms.ToArray();
                return File(fileBytes, "application/pdf", $"ExpenseReport_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                return Content("Error generating report: " + ex.Message);
            }
        }
    }
}
