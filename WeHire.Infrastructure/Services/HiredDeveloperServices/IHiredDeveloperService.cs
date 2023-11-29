using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Domain.Entities;

namespace WeHire.Infrastructure.Services.HiredDeveloperServices
{
    public interface IHiredDeveloperService
    {
        Task CreateHiredDeveloper(int? jobPositionId, int? projectId, Developer developer, int contractId, string projectCode);
    }
}
