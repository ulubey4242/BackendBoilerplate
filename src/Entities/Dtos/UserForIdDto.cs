using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Entities.Dtos
{
    public class UserForIdDto : IDto
    {
        public int UserId { get; set; }
    }
}
