using System;
using System.Collections.Generic;

namespace DeployerTool.Core.Exceptions
{
    public class ExecutionFailedException : InvalidOperationException
    {
        private readonly IEnumerable<string> _messages;

        public override string Message
        {
            get
            {
                return string.Join(Environment.NewLine, _messages);
            }
        }

        public ExecutionFailedException(IEnumerable<string> messages)
        {
            _messages = messages;
        }
    }
}
