using System.Data;

namespace Titan.Library.Infrastructure.Migrations;

public interface ISqlMigration
{
    string Key();
    Task Apply();
}
