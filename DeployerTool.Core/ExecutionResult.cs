using System.Collections.Generic;
using DeployerTool.Core.Statuses;

namespace DeployerTool.Core
{
    public sealed class ExecutionResult
    {
        private readonly ExecutionResultStatus _status;
        private readonly IEnumerable<string> _messages;

        public ExecutionResultStatus Status
        {
            get
            {
                return _status;
            }
        }

        public IEnumerable<string> Messages
        {
            get
            {
                return _messages;
            }
        }

        public ExecutionResult(ExecutionResultStatus status, params string[] messages)
            : this (status,(IEnumerable<string>)messages)
        {
        }

        public ExecutionResult(ExecutionResultStatus status, IEnumerable<string> messages)
        {
            _status = status;
            _messages = messages;
        }
    }
}
