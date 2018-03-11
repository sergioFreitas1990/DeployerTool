using DeployerTool.Core.Statuses;

namespace DeployerTool.Core
{
    public sealed class RegisterResult
    {
        private readonly RegisterResultStatus _status;
        private readonly string _message;

        public RegisterResultStatus Status
        {
            get
            {
                return _status;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public RegisterResult(RegisterResultStatus status, string message = null)
        {
            _status = status;
            _message = message;
        }
    }
}
