using System;

namespace DeployerTool.Core.Exceptions
{
    public class RegistrationFailedException : InvalidOperationException
    {
        public RegistrationFailedException(string message) : base(message)
        {
        }
    }
}
