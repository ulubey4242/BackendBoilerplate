using Core.Entities.Concrete;
using System;

namespace Entities.Concrete
{
    public class SystemLog:BaseEntity
    {
        public string Detail { get; set; }
        public DateTime Date { get; set; }
        public string Audit { get; set; }
    }
}
