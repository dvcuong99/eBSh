using System.Data;

namespace eBSH.Core
{
    public interface IConnectionFactory
    {
        IDbConnection GetConnection { get; }
        string ConnectionString { get; }
    }
    
}
