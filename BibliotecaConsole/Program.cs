﻿using System;
using System.Dynamic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.OpenPgp;

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
            Console.WriteLine("\n=== TELA DE LOGIN ===");

            Console.Write("\nDigite seu email: ");
            string emailLogin = Console.ReadLine();

            Console.Write("\nDigite sua senha: ");
            string senhaLogin = LerSenhaComAsteriscos();

            int? usuarioIdTemp = await VerificarLoginAsync(emailLogin, senhaLogin);

            if (usuarioIdTemp.HasValue)
            {
                Console.WriteLine("\nLogin realizado com sucesso!");
                usuarioId = usuarioIdTemp.Value;
                await Task.Delay(1000); 
                await MostrarTelaPrincipal(); 
            }
            else
            {
                Console.WriteLine("\nEmail ou senha incorretos!");
                await Task.Delay(1000); 
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

            string nome = LerEntradaObrigatoria("\nDigite seu nome: ");

            string email = LerEntradaObrigatoria("\nDigite seu email: ");

            string nick = LerEntradaObrigatoria("\nDigite seu nickname (Apelido): ");

            Console.Write("\nDigite sua senha: ");
            string senha = LerSenhaComAsteriscos();

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
                Console.Clear();
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
                        Console.Clear();
                        await AdicionarLivro(); 
                        break;
                    case 2:
                        Console.Clear();
                        await ListarLivros(); 
                        break;
                    case 3:
                        Console.Clear();
                        await AtualizarLivro(); 
                        break;
                    case 4:
                        Console.Clear();
                        await ExcluirLivro(); 
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

        private static async Task AdicionarLivro()
        {
            Console.WriteLine("=== ADICIONAR LIVRO ===");

            Console.Write("\nDigite o nome do livro: ");
            string nomeLivro = Console.ReadLine();

            Console.Write("\nDigite o nome do autor(a) do livro: ");
            string nomeAutor = Console.ReadLine();

            Console.Write("\nDigite o ano de leitura: ");
            if(!int.TryParse(Console.ReadLine(), out int anoLeitura))
            {
                Console.WriteLine("\nAno inválido!");
                await Task.Delay(1000);
                return;
            }

            Console.Write("\nInforme a sua avaliação deste livro (de 0 a 5): ");
            if(!int.TryParse(Console.ReadLine(), out int avaliacaoLivro) || avaliacaoLivro < 0 || avaliacaoLivro > 5){
                Console.WriteLine("\nFormato de avaliação inválido!");
                await Task.Delay(1000);
                return;
            }

            using (var connection = await ObterConexaoAsync())
            {
                try
                {
                    string query = "INSERT INTO Livro (nome_livro, autor_livro, ano_leitura, avaliacao_livro, id_usuario) VALUES (@NomeLivro, @NomeAutor, @AnoLeitura, @AvaliacaoLivro, @UsuarioId)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NomeLivro", nomeLivro);
                        command.Parameters.AddWithValue("@NomeAutor", nomeAutor);
                        command.Parameters.AddWithValue("@AnoLeitura", anoLeitura);
                        command.Parameters.AddWithValue("@AvaliacaoLivro", avaliacaoLivro);
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId); // Usa o id do usuário logado

                        int result = await command.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            Console.WriteLine("\nLivro adicionado com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("\nErro ao adicionar o livro.");
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nErro ao adicionar livro: {ex.Message}");
                }
            }
            await Task.Delay(1000);
        }

        private static async Task ListarLivros()
        {
            Console.WriteLine("=== LISTA DE LIVROS ===");

            using (var connection = await ObterConexaoAsync())
            {
                try
                {
                    string query = "SELECT nome_livro, autor_livro, ano_leitura, avaliacao_livro FROM Livro WHERE id_usuario = @usuarioId ORDER BY ano_leitura ASC";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    string nomeLivro = reader.GetString(0);
                                    string nomeAutor = reader.GetString(1);
                                    int anoLeitura = reader.GetInt32(2);
                                    int avaliacaoLivro = reader.GetInt32(3);

                                    Console.Write($"\nLivro: {nomeLivro} \nAutor: {nomeAutor} \nAno de leitura: {anoLeitura} \nAvaliação: {avaliacaoLivro} \n");
                                    Console.Write("--------------------------------------------");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nErro ao listar livros: {ex.Message}");
                }
            }
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
            Console.ReadKey();
        }

        private static async Task AtualizarLivro()
        {
            Console.WriteLine("=== ATUALIZAR LIVRO ===");


            string nomeLivroAtual = LerEntradaObrigatoria("\nDigite o nome do livro que deseja atualizar: ");

            string novoNomeLivro = LerEntradaObrigatoria("\nDigite o novo nome do livro: ");

            string novoNomeAutor = LerEntradaObrigatoria("\nDigite o novo nome do autor: ");

            int novoAnoLeitura;
            do
            {
                Console.Write("\nDigite o novo ano de leitura: ");

            } while (!int.TryParse(Console.ReadLine(), out novoAnoLeitura));

            int novaAvaliacao;
            do
            {
                Console.Write("\nInforme a nova avaliação (0 a 5): ");

            }while (!int.TryParse(Console.ReadLine(),out novaAvaliacao) || novaAvaliacao < 0 || novaAvaliacao > 5);

            using (var connection = await ObterConexaoAsync())
            {
                try
                {
                    string query = "UPDATE Livro " +
                        "SET nome_livro = @NovoNomeLivro, " +
                        "    autor_livro = @NovoNomeAutor, " +
                        "    ano_leitura = @NovoAnoLeitura, " +
                        "    avaliacao_livro = @NovaAvaliacao " +
                        "WHERE nome_livro = @NomeLivroAtual AND id_usuario = @UsuarioId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NovoNomeLivro", novoNomeLivro);
                        command.Parameters.AddWithValue("@NovoNomeAutor", novoNomeAutor);
                        command.Parameters.AddWithValue("@NovoAnoLeitura", novoAnoLeitura);
                        command.Parameters.AddWithValue("@NovaAvaliacao", novaAvaliacao);
                        command.Parameters.AddWithValue("@NomeLivroAtual", nomeLivroAtual);
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        int result = await command.ExecuteNonQueryAsync();

                        if (result > 0)
                        {
                            Console.WriteLine("\nLivro atualizado com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("\nLivro não encontrado ou você não tem permissão para atualizá-lo.");

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nErro ao atualizar livro: {ex.Message}");
                }
            }
            await Task.Delay(1000);
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
            Console.ReadKey();
        }

        private static async Task ExcluirLivro()
        {
            Console.Clear();
            Console.WriteLine("=== EXCLUIR LIVRO ===");

            Console.Write("\nDigite o nome do livro que você deseja excluir: ");
            string nomeLivro = Console.ReadLine();

            using (var connection = await ObterConexaoAsync())
            {
                try
                {
                    string query = "DELETE FROM Livro WHERE nome_livro = @NomeLivro AND id_usuario = @UsuarioId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NomeLivro", nomeLivro);
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);

                        int result = await command.ExecuteNonQueryAsync();

                        if(result > 0)
                        {
                            Console.WriteLine("\nLivro excluído com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("\nLivro não encontrado ou você não tem permissão para excluí-lo!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nErro em excluír o livro desejado: {ex.Message}");
                }
            }
            await Task.Delay(1000);
        }

        private static string LerEntradaObrigatoria(string mensagem)
        {
            string entrada;
            do
            {
                Console.Write(mensagem);
                entrada = Console.ReadLine();

            }
            while (string.IsNullOrWhiteSpace(entrada));

            return entrada;
        }

        private static string LerSenhaComAsteriscos()
        {
            string senha = string.Empty;
            ConsoleKey key;

            do
            {
                var tecla = Console.ReadKey(intercept: true);
                key = tecla.Key;

                if (key == ConsoleKey.Backspace && senha.Length > 0)
                {
                    senha = senha.Substring(0, senha.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(tecla.KeyChar))
                {
                    senha += tecla.KeyChar;
                    Console.Write("*");
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();
            return senha;
        }
    }
}
