using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Product : BaseEntity
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

        public virtual User User { get; set; }
    }
}
