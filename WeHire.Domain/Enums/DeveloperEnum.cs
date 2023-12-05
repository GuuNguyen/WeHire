using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class DeveloperEnum
    {
        public enum DeveloperStatus
        {
            [Description("Available")]
            Available = 1,

            [Description("Selected On Request")]
            SelectedOnRequest = 2,

            [Description("On Working")]
            OnWorking = 3,
        }
    }
}
