using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using Entities.Concrete;

namespace Entities.Dtos
{
    public class UserForLoginDataDto : IDto
    {
        public UserForLoginDataDto(string _Token, int _Account_Id, string _Email, string _FirstName, string _LastName, ICollection<UserOperationClaim> _UserOperationClaims)
        {
            Token = _Token;
            Account_Id = _Account_Id;
            Email = _Email;
            UserOperationClaims = _UserOperationClaims;
            FirstName = _FirstName;
            LastName = _LastName;
        }
        public string Token { get; set; }
        public int Account_Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<UserOperationClaim> UserOperationClaims { get; set; }
    }
}
