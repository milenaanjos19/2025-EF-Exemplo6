﻿using System.Globalization;
using EF.Exemplo6;
using Microsoft.EntityFrameworkCore;
public class OperacoesGenero
{
    public static void Incluir()
    {
        using var db = new AplicacaoDbContext();
        var genero = new Genero();

        Console.Write("Nome do gênero: ");
        genero.Nome = Console.ReadLine();

        db.Genero.Add(genero);
        try
        {
            db.SaveChanges();
            Console.WriteLine("Gênero adicionado!");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Erro ao salvar: {ex.Message}");
        }
    }

    public static void Listar()
    {
        using var db = new AplicacaoDbContext();
        var generos = db.Genero.AsNoTracking();

        Console.WriteLine("ID, Nome");
        foreach (var genero in generos)
        {
            Console.WriteLine($"{genero.GeneroID}, {genero.Nome}");
        }
    }

    public static void Alterar()
    {
        using var db = new AplicacaoDbContext();
        Console.WriteLine("Selecione o ID do gênero a partir da lista:");
        Listar();

        Console.Write("ID: ");
        if (int.TryParse(Console.ReadLine(), out var generoId))
        {
            var genero = db.Genero.Find(generoId);
            if (genero == null)
            {
                Console.WriteLine("Gênero não encontrado. Selecione um gênero da lista!");
                return;
            }

            Console.WriteLine($"Nome atual: {genero.Nome}");
            var novoNome = Console.ReadLine().Trim();
            if (!string.IsNullOrWhiteSpace(novoNome))
            {
                genero.Nome = novoNome;
            }

            try
            {
                db.SaveChanges();
                Console.WriteLine("Gênero atualizado!");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erro ao atualizar: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("ID inválido!");
        }
    }

    public static void Remover()
    {
        using var db = new AplicacaoDbContext();
        Console.WriteLine("Selecione o ID do gênero a partir da lista:");
        Listar();

        Console.Write("ID: ");
        if (int.TryParse(Console.ReadLine(), out var generoId))
        {
            var genero = db.Genero.Find(generoId);
            if (genero == null)
            {
                Console.WriteLine("Gênero não encontrado. Selecione um gênero da lista!");
                return;
            }

            db.Genero.Remove(genero);
            try
            {
                db.SaveChanges();
                Console.WriteLine("Gênero removido!");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erro ao remover: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("ID inválido!");
        }
    }
}