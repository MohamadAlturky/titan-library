using System.Text;
using System.Text.Json;
using Spectre.Console;

static class Program
{
    static int Main()
    {
        var migrationsPath = LoadMigrationsPath();

        AnsiConsole.Clear();
        Header(migrationsPath);

        if (!Directory.Exists(migrationsPath))
            AnsiConsole.MarkupLine(
                "[yellow]Warning:[/] migrations directory not found — it will be created."
            );

        // 1. Ask for migration name
        var name = AnsiConsole
            .Ask<string>("[grey]Migration name[/] [dim](without 'Migration.cs')[/]:")
            .Trim();

        if (string.IsNullOrEmpty(name))
        {
            AnsiConsole.MarkupLine("[red]Aborted:[/] empty name.");
            return 1;
        }

        // 2. Build class name with datetime prefix (EF Core style)
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var className = $"_{timestamp}_{name}Migration";
        var fileName = $"{className}.cs";
        var filePath = Path.Combine(migrationsPath, fileName);

        // 3. Preview
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[grey]Preview[/]").LeftJustified());

        var grid = new Grid().AddColumn().AddColumn();
        grid.AddRow("[grey]Class name:[/]", $"[springgreen3]{Markup.Escape(className)}[/]");
        grid.AddRow("[grey]File:[/]", $"[springgreen3]{Markup.Escape(fileName)}[/]");
        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();

        var boilerplate = BuildBoilerplate(className);
        AnsiConsole.Write(
            new Panel(new Text(boilerplate))
                .BorderColor(Color.Grey35)
                .Header("[grey]Boilerplate[/]")
        );

        // 4. Confirm
        AnsiConsole.WriteLine();
        if (!AnsiConsole.Confirm("Create this migration?"))
        {
            AnsiConsole.MarkupLine("[grey]Cancelled.[/]");
            return 0;
        }

        // 5. Write file
        try
        {
            if (File.Exists(filePath))
            {
                AnsiConsole.MarkupLine(
                    $"[red]Error:[/] file already exists: {Markup.Escape(fileName)}"
                );
                return 1;
            }

            Directory.CreateDirectory(migrationsPath);
            File.WriteAllText(filePath, boilerplate, Encoding.UTF8);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold green]Created![/]");
            AnsiConsole.MarkupLine($"[dim]{Markup.Escape(filePath)}[/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {Markup.Escape(ex.Message)}");
            return 1;
        }
    }

    static string LoadMigrationsPath()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        if (File.Exists(configPath))
        {
            try
            {
                var doc = JsonDocument.Parse(File.ReadAllText(configPath));
                if (
                    doc.RootElement.TryGetProperty("migrations_path", out var prop)
                    && prop.GetString() is { Length: > 0 } path
                )
                    return path;
            }
            catch
            { /* fall through */
            }
        }

        return Path.Combine(FindSolutionRoot(), "Titan.Library.Infrastructure", "Migrations");
    }

    static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null)
        {
            if (dir.GetFiles("*.sln").Length > 0)
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException(
            "Could not find solution root (.sln file). Run from within the repo or set migrations_path in config.json."
        );
    }

    static string BuildBoilerplate(string className) =>
        "using Titan.Library.Infrastructure.Connectors;\n"
        + "using Titan.Library.Infrastructure.Migrations.Abstractions;\n"
        + "\n"
        + "namespace Titan.Library.Infrastructure.Migrations;\n"
        + "\n"
        + $"public class {className}(IDbConnectionFactory dbConnectionFactory)\n"
        + "    : SqlMigration(dbConnectionFactory)\n"
        + "{\n"
        + "    protected override async Task ApplySqlDdl()\n"
        + "    {\n"
        + "        await using var command = Connection.CreateCommand();\n"
        + "\n"
        + "        command.CommandText = $\"\"\"\n"
        + "            -- TODO: Add your SQL DDL here\n"
        + "            \"\"\";\n"
        + "\n"
        + "        await command.ExecuteNonQueryAsync();\n"
        + "    }\n"
        + "}\n";

    static void Header(string migrationsPath)
    {
        AnsiConsole.Write(new Rule("[bold mediumpurple2]Migration Creator[/]").LeftJustified());
        AnsiConsole.MarkupLine($"[grey]Path:[/] [dim]{Markup.Escape(migrationsPath)}[/]");
        AnsiConsole.WriteLine();
    }
}
