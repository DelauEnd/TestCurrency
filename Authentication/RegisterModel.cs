using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCurrency.Authentication
{
    public class RegisterModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role = nameof(UserRoles.USER);
    }
}
