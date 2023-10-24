using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Infrastructure.Services.PostedTimeCalculatorService
{
    public interface IPostedTimeCalculatorService
    {
        public string GetElapsedTimeSinceCreation(DateTime? createdAt);
    }
}
