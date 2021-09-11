using Xunit.Abstractions;

namespace DockerTest
{
    public class MySqlContainer
    {
        private readonly ITestOutputHelper _output;
        private readonly DockerContainer _container;
        private const string MySqlStartedString = "MySQL init process done. Ready for start up";
        private string Password { get; }
        public int ExternalPort { get; }

        private const int InternalPort = 3306;


        public MySqlContainer(ITestOutputHelper output = null, int? port = null, string password = null)
        {
            _output = output;
            ExternalPort = port ?? 3306;
            Password = password ?? "sa";

            DockerRunParams parameters = new DockerRunParams()
                .AddParam($"-p {ExternalPort}:{InternalPort}")
                .AddParam($"--env MYSQL_ROOT_PASSWORD={Password}")
                .AddParam($"--env MYSQL_DATABASE=Test")
                .AddParam("-d")
                .AddParam($"mysql");
            _container = new DockerContainer(parameters, output);
        }

        public void Start()
        {
            _output?.WriteLine("Starting MySql container");
            string containerId = _container.Run();
            _output?.WriteLine($"MySql container started with container Id {containerId}");
            _container.WaitForLog($"logs {containerId}", MySqlStartedString);
            _output?.WriteLine($"MySql container is ready");
        }

        public void Stop()
        {
            _output?.WriteLine($"Stopping MySql container");
            _container.Stop();
            _output?.WriteLine($"Stopped MySql container");
        }
    }
}
