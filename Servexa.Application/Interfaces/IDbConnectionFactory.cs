using System.Data;

namespace Servexa.Application.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
