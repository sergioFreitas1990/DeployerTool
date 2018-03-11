using System;
using MongoDB.Bson.Serialization.Attributes;

namespace DeployerTool.MongoDbTracker
{
    public class ScriptVersions
    {
        public DateTime ExecutedTime { get; set; }

        [BsonId]
        public string ScriptId { get; set; }
    }
}
