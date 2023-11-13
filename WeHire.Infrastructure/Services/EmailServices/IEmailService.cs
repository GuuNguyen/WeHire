using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Infrastructure.Services.EmailServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string fullName, int userId, string confirmationCode, Message message);
    }
}
