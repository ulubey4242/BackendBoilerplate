using Core.Entities.Concrete;
using System;
using System.Collections.Generic;

namespace Entities.Concrete
{
    public class User:BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public bool Deleted { get; set; }
        public DateTime? TokenExpiredAt { get; set; }

        public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
