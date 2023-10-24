using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class DeveloperEnum
    {
        public enum DeveloperStatus
        {
            Unavailable = 0,
            Available = 1,
            SelectedOnRequest = 2,
            OnWorking = 3,
        }
    }
}
