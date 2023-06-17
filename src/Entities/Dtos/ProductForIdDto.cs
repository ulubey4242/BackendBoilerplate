using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Entities.Dtos
{
    public class ProductForIdDto : IDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}
