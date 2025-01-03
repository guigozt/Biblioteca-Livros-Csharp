using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace BibliotecaConsole
{
    internal class Program
    {
        private static readonly string connectionString = "server=localhost; port=3306; Database=projetoBibliotecaLivros; uid=root; Pwd='123';";
        private static int usuarioId;

        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== BIBLIOTECA DE LIVROS - LOGIN ===");
                Console.WriteLine("\nDigite [1] para Entrar");
                Console.WriteLine("\nDigite [2] para Cadastrar");
                Console.Write("\nInforme sua escolha: ");

                string escolha = Console.ReadLine();

                switch (escolha)
                {
                    case "1":
                        await RealizarLogin();
                        break;
                    case "2":
                        await RealizarCadastro();
                        break;
                    default:
                        Console.WriteLine("\nOpção inválida!");
                        break;
                }
            }
        }

        private static async Task<MySqlConnection> ObterConexaoAsync()
        {
            var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private static async Task RealizarLogin()
        {
            Console.Clear();
            Console.WriteLine("=== TELA DE LOGIN ===");

            Console.Write("\nDigite seu email: ");
            string emailLogin = Console.ReadLine();

            Console.Write("\nDigite sua senha: ");
            string senhaLogin = Console.ReadLine();

            int? usuarioIdTemp = await VerificarLoginAsync(emailLogin, senhaLogin);

            if (usuarioIdTemp.HasValue)
            {
                Console.WriteLine("\nLogin realizado com sucesso!");
                usuarioId = usuarioIdTemp.Value;
                await Task.Delay(2000); // Aguarda 2 segundos
                await MostrarTelaPrincipal(); // Exibe a tela principal
            }
            else
            {
                Console.WriteLine("\nEmail ou senha incorretos!");
                await Task.Delay(2000); // Aguarda 2 segundos antes de voltar ao menu principal
            }
        }

        private static async Task<int?> VerificarLoginAsync(string email, string senha)
        {
            using (var connection = await ObterConexaoAsync())
            {
                try
                {
                    string query = "SELECT id_usuario FROM Usuario WHERE email_usuario = @Email AND senha = @Senha";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Senha", senha);

                        var result = await command.ExecuteScalarAsync();
                        return result == null ? (int?)null : Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nErro ao verificar Login: {ex.Message}");
                    return null;
                }
            }
        }

        private static async Task RealizarCadastro()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRAR USUÁRIO ===");

            Console.Write("\nDigite seu nome: ");
            string nome = Console.ReadLine();

            Console.Write("\nDigite seu email: ");
            string email = Console.ReadLine();

            Console.Write("\nDigite seu nickname (Apelido): ");
            string nick = Console.ReadLine();

            Console.Write("\nDigite sua senha: ");
            string senha = Console.ReadLine();

            await InserirNovoUsuarioAsync(nome, email, nick, senha);
        }

        private static async Task InserirNovoUsuarioAsync(string nome, string email, string nick, string senha)
        {
            using (var connection = await ObterConexaoAsync())
            {
                try
                {
                    string query = "INSERT INTO Usuario (nome_usuario, email_usuario, nick_name, senha) VALUES (@Nome, @Email, @Nick, @Senha)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nome", nome);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Nick", nick);
                        command.Parameters.AddWithValue("@Senha", senha);

                        int result = await command.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            Console.WriteLine("\nUsuário cadastrado com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("\nErro ao cadastrar usuário!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nErro ao cadastrar usuário: {ex.Message}");
                }
            }
            await Task.Delay(2000);
        }

        private static async Task MostrarTelaPrincipal()
        {
            Console.Clear();

            int opcao;

            do
            {
                Console.WriteLine("=== BIBLIOTECA DE LIVROS - TELA PRINCIPAL ===");
                Console.WriteLine("\nDigite [1] para Adicionar livro");
                Console.WriteLine("\nDigite [2] para Listar livros");
                Console.WriteLine("\nDigite [3] para Atualizar livro");
                Console.WriteLine("\nDigite [4] para Excluir livro");
                Console.WriteLine("\nDigite [0] para Sair");
                Console.Write("\nEscolha uma opção: ");
                opcao = int.Parse(Console.ReadLine());

                switch (opcao)
                {
                    case 1:
                        AdicionarLivro();
                        break;
                    case 2:
                        ListarLivros();
                        break;
                    case 3:
                        AtualizarLivro();
                        break;
                    case 4:
                        ExcluirLivro();
                        break;
                    case 0:
                        Console.WriteLine("\nSaindo do programa...");
                        break;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            } while (opcao != 0);
        }

        private static void AdicionarLivro()
        {
            Console.Clear();
            Console.WriteLine("=== ADICIONAR LIVRO ===");
        }

        private static void ListarLivros()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA DE LIVROS ===");
        }

        private static void AtualizarLivro()
        {
            Console.Clear();
            Console.WriteLine("=== ATUALIZAR LIVRO ===");
        }

        private static void ExcluirLivro()
        {
            Console.Clear();
            Console.WriteLine("=== EXCLUIR LIVRO ===");
        }
    }
}
