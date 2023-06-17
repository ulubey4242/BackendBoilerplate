using Core.Entities.Concrete;
using System.Collections.Generic;

namespace Entities.Concrete
{
    public class OperationClaim:BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }
    }
}
