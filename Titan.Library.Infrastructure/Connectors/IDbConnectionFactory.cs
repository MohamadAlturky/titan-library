using System.Data;
using System.Data.Common;

namespace Titan.Library.Infrastructure.Connectors;

public interface IDbConnectionFactory
{
    DbConnection CreateDbConnection();
}
