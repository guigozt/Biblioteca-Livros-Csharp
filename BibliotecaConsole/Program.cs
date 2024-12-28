using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace BibliotecaConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (MySqlConnection connection = GetMySqlConnection())
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexão com o banco de dados foi bem sucedida!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine("Teste");
        }

        private static MySqlConnection GetMySqlConnection() 
        {
            string connectionString = "server=localhost; port=3306; Database=projetoBibliotecaLivros; uid=root; Pwd=123;";
            return new MySqlConnection(connectionString);
        }
    }
}
