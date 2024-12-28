using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaConsole
{
    internal class Login
    {
        public string UserEmail;
        public string Password;

        public Login(string email, string password)
        {
            this.UserEmail = email;
            this.Password = password;
        }
    }
}
