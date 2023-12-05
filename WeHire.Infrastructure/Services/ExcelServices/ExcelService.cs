using OfficeOpenXml.Style;
using OfficeOpenXml;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.TermStore;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using WeHire.Application.Utilities.ErrorHandler;
using System.Net;
using WeHire.Application.Utilities.Helper.ConvertDate;
using Microsoft.IdentityModel.Tokens;
using WeHire.Domain.Entities;
using WeHire.Application.DTOs.PayPeriod;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using OfficeOpenXml.DataValidation;
using System.Linq;

namespace WeHire.Infrastructure.Services.ExcelServices
{
    public class ExcelService : IExcelService
    {
        public byte[] ExportExcelFile(string companyName, string startDate, string endDate, string payMonth,
                                      int dayCountInMonth, List<string> dayRangeInMonth, List<PaySlipModel> paySlips)
        {
            Color blueBlurColor = ColorTranslator.FromHtml("#cfe2f3");
            Color blueColor = ColorTranslator.FromHtml("#9fc5e8");
            Color beigeColor = ColorTranslator.FromHtml("#fff2cc");
            Color yellowColor = ColorTranslator.FromHtml("#ffe599");
            Color orangeColor = ColorTranslator.FromHtml("#f9cb9c");
            Color redColor = ColorTranslator.FromHtml("#e3d5ca");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells["A1:E1"].Merge = true;
                worksheet.Cells[1, 1].Value = $"Employee Payroll - {payMonth}";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 15;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(beigeColor);
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheet.Cells["A2:B2"].Merge = true;
                worksheet.Cells["A2:B2"].Value = $"Company: {companyName}";
                worksheet.Cells["A2:B2"].Style.Font.Bold = true;
                worksheet.Cells["A2:B2"].Style.Font.Size = 10;

                worksheet.Cells["A3:B3"].Merge = true;
                worksheet.Cells["A3:B3"].Value = $"Start date: {startDate}";
                worksheet.Cells["A3:B3"].Style.Font.Bold = true;
                worksheet.Cells["A3:B3"].Style.Font.Size = 10;

                worksheet.Cells["A4:B4"].Merge = true;
                worksheet.Cells["A4:B4"].Value = $"End date: {endDate}";
                worksheet.Cells["A4:B4"].Style.Font.Bold = true;
                worksheet.Cells["A4:B4"].Style.Font.Size = 10;

                var cellA2H4 = worksheet.Cells["A2:F4"];
                cellA2H4.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cellA2H4.Style.Fill.BackgroundColor.SetColor(blueBlurColor);

                worksheet.Cells[6, 1].Value = "Number";
                worksheet.Cells[6, 2].Value = "Full name";
                worksheet.Cells[6, 3].Value = "Code name";
                worksheet.Cells[6, 4].Value = "Basic salary";
                worksheet.Cells[6, 5].Value = "Overtime hours";

                worksheet.Cells["A6:E6"].Style.Font.Bold = true;
                worksheet.Cells["A6:E6"].Style.Font.Size = 10;
                worksheet.Cells["A6:E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A6:E6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A6:D6"].Style.Fill.BackgroundColor.SetColor(blueColor);
                worksheet.Cells["E6:E6"].Style.Fill.BackgroundColor.SetColor(orangeColor);

                var endColumn = dayCountInMonth + 5;
                var columnChar = GetExcelColumnName(endColumn);
                var cellG6 = worksheet.Cells[$"F6:{columnChar}6"];

                for (int i = 0; i < dayRangeInMonth.Count; i++)
                {
                    worksheet.Cells[6, i + 6].Value = dayRangeInMonth[i];
                }

                cellG6.Style.Font.Bold = true;
                cellG6.Style.Font.Size = 10;
                cellG6.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cellG6.Style.Fill.BackgroundColor.SetColor(yellowColor);
                cellG6.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                var convertedDateRange = ConvertDateTime.ConvertStringListToDateList(dayRangeInMonth);
                var filteredPaySlips = paySlips
                    .Where(paySlip => convertedDateRange.Any(d => d >= paySlip.FromDate && d <= paySlip.ToDate))
                    .ToList();

                for (int i = 0; i < filteredPaySlips.Count; i++)
                {
                    var paySlip = filteredPaySlips[i];
                    int row = i + 7;

                    worksheet.Cells[row, 1].Value = i + 1;
                    worksheet.Cells[row, 2].Value = paySlip.Fullname;
                    worksheet.Cells[row, 3].Value = paySlip.CodeName;
                    worksheet.Cells[row, 4].Value = paySlip.BasicSalary;
                    worksheet.Cells[row, 5].Value = 0.00;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "0.00";
                    worksheet.Cells[row, 5].Style.Locked = false;

                    for (int j = 0; j < dayRangeInMonth.Count; j++)
                    {
                        int column = j + 6;
                        if (ConvertDateTime.ConvertStringToDateTimeExcel(dayRangeInMonth[j]) < paySlip.FromDate ||
                            ConvertDateTime.ConvertStringToDateTimeExcel(dayRangeInMonth[j]) > paySlip.ToDate)
                        {
                            worksheet.Cells[row, column].Value = "X";
                            worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(redColor);
                        }
                        else
                        {
                            worksheet.Cells[row, column].Value = "hh:mm - hh:mm";
                            worksheet.Cells[row, column].Style.Numberformat.Format = "HH:mm";
                            worksheet.Cells[row, column].Style.Locked = false;
                        }
                        worksheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                worksheet.Cells.AutoFitColumns();
                worksheet.View.FreezePanes(6, 6);
                worksheet.Protection.IsProtected = true;
                worksheet.Protection.AllowSelectUnlockedCells = true;
                return package.GetAsByteArray();
            }
        }

        public ImportPaySlipModel ImportExcelFile(IFormFile file)
        {
            var paySlips = new List<PaySlipModel>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];

                var startDate = worksheet.Cells["A3"].Value.ToString()!.Split(':')[1].Trim();
                var endDate = worksheet.Cells["A4"].Value.ToString()!.Split(':')[1].Trim();

                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns;
    
                for (int i = 7; i <= rows; i++)
                {
                    if (worksheet.Cells[i, 3].Value == null) continue;
                    var paySlip = new PaySlipModel();
                    paySlip.WorkLogs = new List<WorkLogModel>();
                   
                    paySlip.Fullname = worksheet.Cells[i, 2].Value.ToString();
                    paySlip.CodeName = worksheet.Cells[i, 3].Value.ToString();
                    paySlip.BasicSalary = decimal.Parse(worksheet.Cells[i, 4].Value.ToString() ?? "");
                    paySlip.TotalOvertimeHours = int.Parse(worksheet.Cells[i, 5].Value.ToString() ?? "");
                    CheckForNullFields(paySlip);
                    for (int j = 6; j <= columns; j++)
                    {
                        string cellValue = worksheet.Cells[i, j].Value?.ToString();
                        if (cellValue.Equals("X"))
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(cellValue) && cellValue.Contains("-") && !cellValue.Contains("hh:mm"))
                        {
                            string[] timeParts = cellValue.Split('-');
                            if (timeParts.Length == 2)
                            {
                                if (TimeSpan.TryParse(timeParts[0].Trim(), out TimeSpan timeIn) &&
                                    TimeSpan.TryParse(timeParts[1].Trim(), out TimeSpan timeOut))
                                {
                                    var workLog = new WorkLogModel();
                                    workLog.WorkDate = worksheet.Cells[6, j].Value.ToString() ?? "";
                                    workLog.TimeIn = timeIn;
                                    workLog.TimeOut = timeOut;
                                    CheckForNullWorklog(workLog);
                                    paySlip.WorkLogs.Add(workLog);
                                }
                                else
                                {
                                    throw new ExceptionResponse(HttpStatusCode.BadRequest, "Excel format", $"Can not parse Time in and Time out in line {i}!!");
                                }
                            }
                            else
                            {
                                throw new ExceptionResponse(HttpStatusCode.BadRequest, "Excel format", $"Data format error in line {i}!!");
                            }
                        }
                        else
                        {
                            throw new ExceptionResponse(HttpStatusCode.BadRequest, "Excel format", $"Data format error or empty in line {i}!!");
                        }
                    }
                    paySlips.Add(paySlip);
                }
                return new ImportPaySlipModel
                {
                    StartDate = ConvertDateTime.ConvertStringToDateTimeExcel(startDate),
                    EndDate = ConvertDateTime.ConvertStringToDateTimeExcel(endDate),
                    PaySlips = paySlips
                };
            }
        }

        public static void CheckForNullFields(PaySlipModel paySlip)
        {
            if (paySlip == null)
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "PaySlip", "PaySlipModel cannot be null.");
            }
            if (paySlip.TotalOvertimeHours < 0)
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "TotalHourWorked", "Total Hour Worked cannot be null.");
            }
        }

        public static void CheckForNullWorklog(WorkLogModel workLog)
        {
            if (workLog == null)
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "WorkLog", "WorkLog cannot be null.");
            }

            if (string.IsNullOrEmpty(workLog.WorkDate))
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "WorkDate", "WorkDate cannot be null or empty.");
            }

            if (workLog.TimeIn == null)
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "WorkDate", "WorkDate cannot be null or empty.");

            }
        }

        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }
    }
}
