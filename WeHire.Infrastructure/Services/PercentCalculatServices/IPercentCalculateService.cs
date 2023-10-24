using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Domain.Entities;

namespace WeHire.Infrastructure.Services.PercentCalculatServices
{
    public interface IPercentCalculateService
    {
        public MatchingPercentage CalculateMatchingPercentage(HiringRequest request, Developer developer);
    }
}
