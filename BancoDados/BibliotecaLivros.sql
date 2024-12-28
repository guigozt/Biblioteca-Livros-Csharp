CREATE DATABASE projetoBibliotecaLivros;
USE projetoBibliotecaLivros;

-- Criação da tabela de Usuário
CREATE TABLE Usuario (
    id_usuario INT PRIMARY KEY AUTO_INCREMENT,
    nome_usuario VARCHAR(100) NOT NULL,
    email_usuario VARCHAR(100) NOT NULL UNIQUE,  -- Email único
    nick_name VARCHAR(50) NOT NULL UNIQUE,       -- Nickname único
    senha VARCHAR(100) NOT NULL                  -- Senha com tamanho adequado para hashes
);

-- Criação da tabela de Livro
CREATE TABLE Livro (
    id_livro INT PRIMARY KEY AUTO_INCREMENT,
    nome_livro VARCHAR(100) NOT NULL,
    autor_livro VARCHAR(100) NOT NULL,
    ano_leitura YEAR NOT NULL,                   -- Tipo específico para ano
    avaliacao_livro INT(1) NOT NULL CHECK (avaliacao_livro BETWEEN 1 AND 5),  -- Avaliação entre 1 e 5
    id_usuario INT,  -- Referência ao ID do usuário
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE -- Exclui os livros quando o usuário é excluído
);

select * from Usuario;