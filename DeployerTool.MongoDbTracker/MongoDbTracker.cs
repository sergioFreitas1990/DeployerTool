using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeployerTool.Core;
using DeployerTool.Core.Statuses;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeployerTool.MongoDbTracker
{
    public class MongoDbTracker<TTracked> : ITracker
        where TTracked : ScriptVersions, new()
    {
        public const int DefaultTimeout = 500;

        private readonly IMongoDatabase _mongoDatabase;
        private readonly string _collectionName;
        private readonly Func<ScriptVersions, TTracked> _mapFunction;
        private readonly int _defaultConnectionPingTimeout;

        public MongoDbTracker(IMongoDatabase mongoDatabase, string collectionName)
            : this(DefaultTimeout, mongoDatabase, collectionName)
        {
        }

        public MongoDbTracker(int defaultConnectionPingTimeout, 
            IMongoDatabase mongoDatabase, string collectionName)
            : this(defaultConnectionPingTimeout, mongoDatabase, collectionName, 
                t => new TTracked
                {
                    ExecutedTime = t.ExecutedTime,
                    ScriptId = t.ScriptId
                })
        {
        }

        public MongoDbTracker(int defaultConnectionPingTimeout, IMongoDatabase mongoDatabase, 
            string collectionName, Func<ScriptVersions, TTracked> mapFunction)
        {
            _defaultConnectionPingTimeout = defaultConnectionPingTimeout;
            _mongoDatabase = mongoDatabase;
            _collectionName = collectionName;
            _mapFunction = mapFunction;
        }

        public async Task<IScriptHandle> GetHandleAsync(IEnumerable<IScriptHandle> scriptHandles, 
            CancellationToken cancellationToken)
        {
            if (scriptHandles == null)
            {
                throw new ArgumentNullException(nameof(scriptHandles));
            }

            var orderedScriptsToRun = scriptHandles.OrderBy(t => t.ScriptId);
            
            var isDatabaseAlive = _mongoDatabase.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(500);
            if (!isDatabaseAlive)
            {
                // Connection failed, return the first script regardless
                return orderedScriptsToRun.FirstOrDefault();
            }

            var ranScripts = await _mongoDatabase
                .GetCollection<TTracked>(_collectionName)
                .AsQueryable()
                .ToListAsync(cancellationToken);

            return orderedScriptsToRun
                .FirstOrDefault(t => !ranScripts.Any(registeredScript => t.ScriptId == registeredScript.ScriptId));
        }

        public async Task<RegisterResult> RegistHandleAsync(IScriptHandle handler, 
            CancellationToken cancellationToken)
        {
            var mappedEntity = _mapFunction(new ScriptVersions
            {
                ExecutedTime = DateTime.UtcNow,
                ScriptId = handler.ScriptId
            });

            try
            {
                await _mongoDatabase
                    .GetCollection<TTracked>(_collectionName)
                    .InsertManyAsync(new[]
                    {
                        mappedEntity
                    },
                    cancellationToken: cancellationToken);

                return new RegisterResult(RegisterResultStatus.Success);
            }
            catch (Exception ex)
            {
                return new RegisterResult(RegisterResultStatus.Failure, ex.Message);
            }
        }
    }
}
