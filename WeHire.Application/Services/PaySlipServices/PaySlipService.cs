using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.PaySlip;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Domain.Entities;
using WeHire.Application.Services.ExcelServices;
using WeHire.Application.Services.PayPeriodServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.PayPeriodEnum;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Application.Services.PaySlipServices
{
    public class PaySlipService : IPaySlipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPayPeriodService _payPeriodService;

        public PaySlipService(IUnitOfWork unitOfWork, IMapper mapper, IPayPeriodService payPeriodService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _payPeriodService = payPeriodService;
        }

        public List<GetPaySlipModel> GetPaySlipsByPayPeriodId(int payPeriodId, PagingQuery query)
        {
            var paySlips = _unitOfWork.PaySlipRepository.Get(p => p.PayPeriodId ==  payPeriodId)
                                                        .Include(p => p.HiredDeveloper)
                                                        .ThenInclude(h => h.Developer)
                                                        .ThenInclude(d => d.User)
                                                        .AsQueryable();

            paySlips = paySlips.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedPaySlips = _mapper.Map<List<GetPaySlipModel>>(paySlips);
            return mappedPaySlips;
        }

        public async Task<GetUpdatePaySlipResponse> UpdateTotalOvertimeHourAsync(UpdatePaySlipModel requestBody)
        {
            var paySlip = await _unitOfWork.PaySlipRepository.GetByIdAsync(requestBody.PaySlipId)
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "paySlip", "paySlip does not exist!!");

            var payPeriod = await _unitOfWork.PayPeriodRepository.Get(p => p.PayPeriodId == paySlip.PayPeriodId &&
                                                                           p.Status == (int)PayPeriodStatus.Created)
                                                                 .Include(p => p.PaySlips)
                                                                 .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "payPeriod", "payPeriod does not exist!!");

            var updatedPaySlip = _mapper.Map(requestBody, paySlip);
            
            paySlip.TotalEarnings = _payPeriodService.GetTotalEarningByDeveloperCode(paySlip.HiredDeveloperId, paySlip.TotalActualWorkedHours, updatedPaySlip.TotalOvertimeHours);

            _unitOfWork.PaySlipRepository.Update(updatedPaySlip);

            payPeriod.TotalAmount = payPeriod.PaySlips.Sum(p => p.TotalEarnings ?? 0);
            payPeriod.UpdatedAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();

            return new GetUpdatePaySlipResponse
            {
                TotalOvertimeHours = paySlip.TotalOvertimeHours,
                TotalEarnings = paySlip.TotalEarnings?.ToString("#,##0 VND")!,
            };
        }
    }
}

