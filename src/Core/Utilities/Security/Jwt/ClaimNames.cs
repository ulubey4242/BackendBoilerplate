using System.ComponentModel;

namespace Core.Utilities.Security.Jwt
{
    public enum ClaimNames
    {
        [Description("Id")]
        Id = 10,

        [Description("Email")]
        Email = 20,

        [Description("Roles")]
        Roles = 30
    }
}
