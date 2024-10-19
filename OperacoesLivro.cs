using EF.Exemplo6;
using Microsoft.EntityFrameworkCore;

﻿using System;
using System.Collections.Generic;
using System.Linq;
using EF.Exemplo6;
using Microsoft.EntityFrameworkCore;

public static class OperacoesLivro
{
    public static void Incluir()
    {
        using var db = new AplicacaoDbContext();
        var livro = new Livro();

        Console.Write("ISBN: ");
        livro.ISBN = Console.ReadLine();

        Console.Write("Título: ");
        livro.Titulo = Console.ReadLine();

        Console.Write("Número de Páginas (deixe em branco se não souber): ");
        var paginasInput = Console.ReadLine();
        livro.Paginas = string.IsNullOrWhiteSpace(paginasInput) ? (int?)null : int.Parse(paginasInput);
        
        livro.QuantidadeEmEstoque = 0;
        
        Console.Write("ID do Autor: ");
        if (int.TryParse(Console.ReadLine(), out var autorId))
        {
            var autor = db.Autor.Find(autorId);
            if (autor != null)
            {
                livro.Autor = autor;
            }
            else
            {
                Console.WriteLine("Autor não encontrado!");
                return;
            }
        }
        
        livro.Generos = ObterGenerosDoUsuario(db, livro.ISBN);

        db.Livro.Add(livro);
        try
        {
            db.SaveChanges();
            Console.WriteLine("Livro adicionado!");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Erro ao salvar: {ex.Message}");
        }
    }


    public static void Listar()
    {
        using var db = new AplicacaoDbContext();
        var livros = db.Livro
            .AsNoTracking()
            .Include(l => l.Autor)
            .Include(l => l.Generos)
            .ThenInclude(lg => lg.Genero); // Inclui os gêneros

        Console.WriteLine("ISBN, Título, Páginas, Autor, Gêneros, Estoque");
        foreach (var livro in livros)
        {
            var generos = string.Join(", ", livro.Generos
                .Where(g => g.Genero != null) // Verifica se Genero não é null
                .Select(g => g.Genero.Nome));

            Console.WriteLine($"{livro.ISBN}, {livro.Titulo}, {livro.Paginas}, {livro.Autor?.Nome}, {generos}, {livro.QuantidadeEmEstoque}");
        }
    }


    public static void Alterar()
{
    using var db = new AplicacaoDbContext();
    Console.WriteLine("Selecione o número do livro a partir da lista:");
    ListarComChave();

    Console.Write("ISBN: ");
    var isbn = Console.ReadLine().Trim();

    var livro = db.Livro.Include(l => l.Generos).FirstOrDefault(l => l.ISBN == isbn);
    if (livro == null)
    {
        Console.WriteLine("Livro não encontrado. Selecione um livro da lista!");
        return;
    }


    Console.WriteLine($"Título atual: {livro.Titulo}");
    var novoTitulo = Console.ReadLine().Trim();
    if (!string.IsNullOrWhiteSpace(novoTitulo))
    {
        livro.Titulo = novoTitulo;
    }


    Console.WriteLine($"Número de Páginas atual: {livro.Paginas}");
    var paginasInput = Console.ReadLine().Trim();
    if (!string.IsNullOrWhiteSpace(paginasInput) && int.TryParse(paginasInput, out var paginas))
    {
        livro.Paginas = paginas;
    }


    Console.Write("Novo ID do Autor (deixe em branco para não alterar): ");
    var autorIdInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(autorIdInput) && int.TryParse(autorIdInput, out var novoAutorId))
    {
        var novoAutor = db.Autor.Find(novoAutorId);
        if (novoAutor != null)
        {
            livro.Autor = novoAutor;
        }
        else
        {
            Console.WriteLine("Autor não encontrado!");
        }
    }



    livro.Generos = ObterGenerosDoUsuario(db, livro.ISBN);

    try
    {
        db.SaveChanges();
        Console.WriteLine("Livro atualizado com sucesso!");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine($"Erro ao atualizar: {ex.Message}");
    }
}


    public static void Remover()
    {
        using var db = new AplicacaoDbContext();
        Console.WriteLine("Selecione o número do livro a partir da lista:");
        ListarComChave();

        Console.Write("ISBN: ");
        var isbn = Console.ReadLine().Trim();

        var livro = db.Livro.Find(isbn);
        if (livro == null)
        {
            Console.WriteLine("Livro não encontrado. Selecione um livro da lista!");
            return;
        }

        db.Livro.Remove(livro);
        
        try
        {
            db.SaveChanges();
            Console.WriteLine("Livro removido!");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Erro ao remover: {ex.Message}");
        }
    }

    public static void ListarComChave()
    {
        using var db = new AplicacaoDbContext();
        var livros = db.Livro.AsNoTracking();

        Console.WriteLine("ISBN, Título");
        foreach (var livro in livros)
        {
            Console.WriteLine($"{livro.ISBN}, {livro.Titulo}");
        }
    }

    private static ICollection<LivroGenero> ObterGenerosDoUsuario(AplicacaoDbContext db, string isbn)
    {
        var generos = new List<LivroGenero>();
        Console.WriteLine("Informe os IDs dos gêneros (separados por vírgula): ");
        var ids = Console.ReadLine()?.Split(',');

        if (ids != null)
        {
            foreach (var id in ids)
            {
                if (int.TryParse(id.Trim(), out var generoId))
                {
                    var genero = db.Genero.Find(generoId);
                    if (genero != null)
                    {
                        generos.Add(new LivroGenero { ISBN = isbn, GeneroID = generoId });
                    }
                    else
                    {
                        Console.WriteLine($"Gênero com ID {generoId} não encontrado!");
                    }
                }
            }
        }

        return generos;
    }
    public static void ComprarLivros()
    {
        using var db = new AplicacaoDbContext();
        Console.Write("ISBN do livro: ");
        var isbn = Console.ReadLine().Trim();

        var livro = db.Livro.Find(isbn);
        if (livro == null)
        {
            Console.WriteLine("Livro não encontrado!");
            return;
        }

        Console.Write("Quantidade a ser comprada: ");
        if (int.TryParse(Console.ReadLine(), out var quantidade) && quantidade > 0)
        {
            livro.QuantidadeEmEstoque += quantidade;
            try
            {
                db.SaveChanges();
                Console.WriteLine($"Estoque do livro '{livro.Titulo}' atualizado. Nova quantidade: {livro.QuantidadeEmEstoque}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erro ao atualizar estoque: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Quantidade inválida!");
        }
    }
    public static void VenderLivros()
    {
        using var db = new AplicacaoDbContext();
        Console.Write("ISBN do livro: ");
        var isbn = Console.ReadLine().Trim();

        var livro = db.Livro.Find(isbn);
        if (livro == null)
        {
            Console.WriteLine("Livro não encontrado!");
            return;
        }

        Console.Write("Quantidade a ser vendida: ");
        if (int.TryParse(Console.ReadLine(), out var quantidade) && quantidade > 0)
        {
            if (livro.QuantidadeEmEstoque < quantidade)
            {
                Console.WriteLine("Estoque insuficiente para realizar a venda.");
                return;
            }

            livro.QuantidadeEmEstoque -= quantidade;
            try
            {
                db.SaveChanges();
                Console.WriteLine($"Venda registrada do livro '{livro.Titulo}'. Estoque atual : {livro.QuantidadeEmEstoque}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erro ao registrar venda: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Quantidade inválida! ");
        }
    }

}