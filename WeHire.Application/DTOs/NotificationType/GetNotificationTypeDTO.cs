﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.NotificationType
{
    public class GetNotificationTypeDTO
    {
        public int NotiTypeId { get; set; }
        public string NotiTypeName { get; set; }
        public string Status { get; set; }
    }
}
