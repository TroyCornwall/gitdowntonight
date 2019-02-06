namespace gitdowntonight
{
    public class MyOptions
    {
        public string GithubBaseUrl { get;} = "https://api.github.com";
        public string GhAccessToken { get; set; }
        public string Organization { get; set; } = "Github";
        public int ResultLimit { get; set; } = -1;
        public int SleepPeriod { get; set; } = 30;
        public string AspnetcoreEnvironment { get; set; } = "Production";
    }
}