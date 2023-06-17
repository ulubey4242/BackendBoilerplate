using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Concrete
{
    public class BaseEntity : IEntity
    {
        [Key]
        public int Id { get; set; }

        public Guid RowGuid { get; set; } = Guid.NewGuid();
    }
}
