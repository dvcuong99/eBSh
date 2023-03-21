using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace eBSH.Core
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SQLCyber"].ConnectionString;
        public IDbConnection GetConnection
        {
            get
            {
                var factory = System.Data.Common.DbProviderFactories.GetFactory("System.Data.SqlClient");
                var conn = factory.CreateConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                return conn;
            }
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }
    }
}