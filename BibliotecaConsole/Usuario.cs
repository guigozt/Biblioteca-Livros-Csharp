using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaConsole
{
    internal class Usuario : Login
    {
        public string Name { get; set; }
        public string NickName { get; set; }

        public Usuario(string name, string nick, string email, string password) : base(email, password)
        {
            this.Name = name;
            this.NickName = nick;
        }
    }
}
