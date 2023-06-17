using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Entities.Dtos
{
    public class ProductForUserIdDto : IDto
    {
        public int UserId { get; set; }
    }
}
