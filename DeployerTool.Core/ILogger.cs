namespace DeployerTool.Core
{
    public interface ILogger
    {
        void Success(string format, params object[] parameters);

        void Information(string format, params object[] parameters);

        void Failure(string format, params object[] parameters);

        void Verbose(string format, params object[] parameters);
    }
}
