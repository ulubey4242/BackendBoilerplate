using Core.Extensions;
using Entities.Concrete;
using System;
using System.Collections.Generic;

namespace Entities.Constants
{
    public static class TestList
    {
        public static List<User> Users() => new List<User> {
            new User
            {
                RowGuid = new Guid("61f09dc5-8fe7-4fc0-ad27-169b0ee9fef7"),
                Email = "admin@localhost.com",
                Password = "YEpLCwXyKZM=".Crypt(),
                FirstName = "Test",
                LastName = "Tester",
                Deleted = false,
                Token = ""
            }
        };
    }
}
