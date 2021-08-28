namespace DockerTest
{
    public static class UnixStyle
    {
        public static string Path(string path) =>
            path.Replace(@"\", "/").Replace(@":/", "/");
    }
}