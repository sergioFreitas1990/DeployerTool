using System;
using System.Text;
using DeployerTool.Core;
using DeployerTool.DbConnectionEngine;
using Npgsql;

namespace DeployerTool.PostgresSqlConnectionEngine
{
    public class PostgresSqlConnectionEngine : DbConnectionSqlEngine<NpgsqlConnection, NpgsqlTransaction, NpgsqlCommand>, IEngine
    {
        private static readonly Func<string, NpgsqlConnection, NpgsqlTransaction, NpgsqlCommand> _toCommand = 
            (command, conn, transaction) => new NpgsqlCommand(command, conn, transaction);
        private static readonly Func<NpgsqlConnection, NpgsqlTransaction> _beginTransaction =
            conn => conn.BeginTransaction();

        public PostgresSqlConnectionEngine(string connectionString)
            : this(new NpgsqlConnection(connectionString))
        {
        }

        public PostgresSqlConnectionEngine(NpgsqlConnection dbConnection)
            : this(dbConnection, Encoding.UTF8)
        {
        }

        public PostgresSqlConnectionEngine(NpgsqlConnection dbConnection, Encoding fileEncoding)
            : base(dbConnection, fileEncoding, _toCommand, _beginTransaction)
        {
        }
    }
}
