using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.EmploymentType;

namespace WeHire.Application.Services.EmploymentTypeServices
{
    public interface IEmploymentTypeService
    {
        public List<GetEmploymentTypeDTO> GetAllEmployments();
    }
}
