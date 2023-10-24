using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Entity.IRepositories;

namespace WeHire.Infrastructure.Services.NotificationServices
{
    internal class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task SendNotificationToManager(int receiverId)
        {
            throw new NotImplementedException();
        }
    }
}
