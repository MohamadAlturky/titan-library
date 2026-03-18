using System.Data;

namespace Titan.Library.Infrastructure.Connectors;

public interface IDbConnectionFactory
{
    IDbConnection CreateDbConnection();
}
