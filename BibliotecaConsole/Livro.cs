using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaConsole
{
    internal class Livro
    {
        public string NomeLivro { get; set; }
        public string NomeAutor { get; set; }
        public int AnoLeitura { get; set; }
        public int AvaliacaoLivro { get; set; }

        public Livro(string nomeLivro, string nomeAutor, int anoLeitura, int avaliacaoLivro)
        {
            this.NomeLivro = nomeLivro;
            this.NomeAutor = nomeAutor;
            this.AnoLeitura = anoLeitura;
            this.AvaliacaoLivro = avaliacaoLivro;
        }
    }
}
