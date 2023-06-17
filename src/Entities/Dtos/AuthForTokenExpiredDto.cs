using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Entities.Dtos
{
    public class AuthForTokenExpiredDto : IDto
    {
        public string Token { get; set; }
    }
}
