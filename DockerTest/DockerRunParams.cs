using System.Collections.Generic;
using System.Linq;

namespace DockerTest
{
    public class DockerRunParams
    {
        private readonly List<string> _parameters;

        public DockerRunParams()
        {
            _parameters = new List<string>();
        }

        public DockerRunParams AddParam(string param)
        {
            _parameters.Add(param);
            return this;
        }

        public string Get() =>
            _parameters.Aggregate("run", (acc, parameter) => $"{acc} {parameter}");
    }
}