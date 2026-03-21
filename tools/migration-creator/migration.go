package main

import (
	"fmt"
	"os"
	"path/filepath"
	"regexp"
	"strconv"
	"strings"
)

var orderRegex = regexp.MustCompile(`Order\(\)\s*=>\s*(\d+)`)

func scanMaxOrder(migrationsPath string) (int, error) {
	entries, err := os.ReadDir(migrationsPath)
	if err != nil {
		return 0, fmt.Errorf("cannot read migrations dir: %w", err)
	}

	max := 0
	for _, e := range entries {
		if e.IsDir() || !strings.HasSuffix(e.Name(), ".cs") {
			continue
		}
		data, err := os.ReadFile(filepath.Join(migrationsPath, e.Name()))
		if err != nil {
			continue
		}
		matches := orderRegex.FindAllSubmatch(data, -1)
		for _, m := range matches {
			if n, err := strconv.Atoi(string(m[1])); err == nil && n > max {
				max = n
			}
		}
	}
	return max, nil
}

func buildBoilerplate(name string, order int) string {
	return fmt.Sprintf(`using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class %s(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    public override int Order() => %d;

    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            -- TODO: Add your SQL DDL here
            """;

        await command.ExecuteNonQueryAsync();
    }
}
`, name, order)
}

func createMigration(migrationsPath, name string, order int) (string, error) {
	fileName := name + ".cs"
	filePath := filepath.Join(migrationsPath, fileName)

	if _, err := os.Stat(filePath); err == nil {
		return "", fmt.Errorf("file already exists: %s", fileName)
	}

	content := buildBoilerplate(name, order)
	if err := os.WriteFile(filePath, []byte(content), 0644); err != nil {
		return "", fmt.Errorf("failed to write file: %w", err)
	}
	return filePath, nil
}
