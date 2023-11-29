using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.WorkLog;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.PayPeriodServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Infrastructure.Services.WorkLogServices
{
    public class WorkLogService : IWorkLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayPeriodService _payPeriodService;
        private readonly IMapper _mapper;

        public WorkLogService(IUnitOfWork unitOfWork, IMapper mapper, IPayPeriodService payPeriodService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _payPeriodService = payPeriodService;
        }

        public async Task<List<GetWorkLogModel>> GetWorkLogByPaySlipIdAsync(int paySlipId)
        {
            var workLogs = await _unitOfWork.WorkLogRepository.Get(w => w.PaySlipId == paySlipId)
                                                              .OrderBy(x => x.WorkDate)
                                                              .ToListAsync();
            var mappedWorkLog = _mapper.Map<List<GetWorkLogModel>>(workLogs);
            return mappedWorkLog;
        }

        public async Task<WorkLogResponseModel> UpdateWorkLogAsync(UpdateWorkLogModel requestBody)
        {
            var workLog = await _unitOfWork.WorkLogRepository.GetByIdAsync(requestBody.WorkLogId)
                    ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "workLog", "WorkLog does not exist!!");
            var oldWorkLog = new WorkLog();

            oldWorkLog.TimeIn = workLog.TimeIn;
            oldWorkLog.TimeOut = workLog.TimeOut;

            var updatedWorkLog = _mapper.Map(requestBody, workLog);

            var paySlip = await _unitOfWork.PaySlipRepository.GetByIdAsync(updatedWorkLog.PaySlipId);
            var payPeriod = await _unitOfWork.PayPeriodRepository.Get(p => p.PayPeriodId == paySlip.PayPeriodId)
                                                                 .Include(p => p.PaySlips)
                                                                 .SingleOrDefaultAsync();

            var hourWorkInDayOld = ConvertTime.CalculateTotalWorkTime(oldWorkLog.TimeIn, oldWorkLog.TimeOut);
            var hourWorkInDayNew = ConvertTime.CalculateTotalWorkTime(updatedWorkLog.TimeIn, updatedWorkLog.TimeOut);

            var totalActualWorkedHours = (paySlip.TotalActualWorkedHours - hourWorkInDayOld) + hourWorkInDayNew;

            paySlip.TotalActualWorkedHours = totalActualWorkedHours;

            paySlip.TotalEarnings = _payPeriodService.GetTotalEarningByDeveloperCode(paySlip.HiredDeveloperId, totalActualWorkedHours, paySlip.TotalOvertimeHours); 

            _unitOfWork.WorkLogRepository.Update(updatedWorkLog);

            payPeriod.TotalAmount = payPeriod.PaySlips.Sum(p => p.TotalEarnings ?? 0);
            await _unitOfWork.SaveChangesAsync();

            return new WorkLogResponseModel
            {
                PayPeriodId = payPeriod.PayPeriodId,
                PaySlipId = paySlip.PaySlipId,
                WorkLogId = workLog.WorkLogId,
                TotalAmount = payPeriod.TotalAmount?.ToString("#,##0 VND"),
                TotalActualWorkedHours = paySlip.TotalActualWorkedHours,
                TotalEarnings = paySlip.TotalEarnings?.ToString("#,##0 VND"),
                HourWorkInDay = Math.Round(hourWorkInDayNew, 1)
            };
        }
    }
}
