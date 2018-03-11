using System;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeployerTool.Core;
using DeployerTool.Core.Statuses;

namespace DeployerTool.DbConnectionEngine
{
    public class DbConnectionSqlEngine<TDbConnection, TDbTransaction, TDbCommand> : IEngine
        where TDbConnection : DbConnection
        where TDbTransaction : DbTransaction
        where TDbCommand : DbCommand
    {
        private readonly TDbConnection _dbConnection;
        private readonly Encoding _fileEncoding;
        private readonly Func<string, TDbConnection, TDbTransaction, TDbCommand> _createCommand;
        private readonly Func<TDbConnection, TDbTransaction> _beginTransaction;

        public DbConnectionSqlEngine(TDbConnection dbConnection, Encoding fileEncoding, 
            Func<string, TDbConnection, TDbTransaction, TDbCommand> toCommand,
            Func<TDbConnection, TDbTransaction> beginTransaction)
        {
            _dbConnection = dbConnection;
            _fileEncoding = fileEncoding;
            _createCommand = toCommand;
            _beginTransaction = beginTransaction;
        }

        protected Func<string, TDbConnection, TDbTransaction, TDbCommand> CreateCommand
        {
            get
            {
                return _createCommand;
            }
        }

        protected Func<TDbConnection, TDbTransaction> BeginTransaction
        {
            get
            {
                return _beginTransaction;
            }
        }

        public async Task<ExecutionResult> ExecuteAsync(IScriptHandle script, 
            CancellationToken cancellationToken)
        {
            if (script == null)
            {
                throw new ArgumentNullException(nameof(script));
            }

            var sqlScript = await GetFileContentAsync(script, cancellationToken);
            var transaction = default(TDbTransaction);
            try
            {
                await _dbConnection.OpenAsync();
                
                transaction = BeginTransaction(_dbConnection);
                using (var command = CreateCommand(sqlScript, _dbConnection, transaction))
                {
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
                transaction.Commit();

                return new ExecutionResult(ExecutionResultStatus.Success);
            }
            catch (Exception e)
            {
                // Exception happened, roll-back everything.
                if (transaction != default(TDbTransaction))
                {
                    transaction.Rollback();
                }
                return new ExecutionResult(ExecutionResultStatus.Errors, e.Message);
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
                _dbConnection.Close();
                _dbConnection.Dispose();
            }
        }

        private async static Task<string> GetFileContentAsync(IScriptHandle script, CancellationToken cancellationToken)
        {
            using (var stream = await script.GetReadStreamAsync(cancellationToken))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
