using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace EFDataExample;

public class RulesEngineDemoContext : RulesEngineContext
{
    public RulesEngineDemoContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Combine(path, "RulesEngineDb");

        if (!Directory.Exists(DbPath))
        {
            Directory.CreateDirectory(DbPath);
        }

        DbPath = Path.Combine(DbPath, "RulesEngineDemo.db");
    }

    public string DbPath { get; private set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}