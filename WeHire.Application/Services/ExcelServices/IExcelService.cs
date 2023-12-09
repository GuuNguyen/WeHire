using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.PayPeriod;

namespace WeHire.Application.Services.ExcelServices
{
    public interface IExcelService
    {
        public byte[] ExportExcelFile(string companyName, string startDate, string endDate, string payMonth, int daysInMonth, List<string> dayRangeInMonth, List<PaySlipModel> paySlips);
        public ImportPaySlipModel ImportExcelFile(IFormFile file);
    }
}
