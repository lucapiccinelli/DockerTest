using System;
using Xunit;
using Xunit.Abstractions;

namespace DockerTest.Tests
{
    public class OutputTests
    {
        private readonly ITestOutputHelper _output;

        public OutputTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanStartMysql()
        {
            MySqlContainer c = new MySqlContainer(_output);
            c.Start();
            c.Stop();
        }
    }
}
