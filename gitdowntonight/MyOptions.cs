namespace gitdowntonight.models
{
    public class MyOptions
    {
        public string GithubBaseUrl { get;} = "https://api.github.com";
        public string GhAccessToken { get; set; }
        public string Organization { get; set; } = "Github";
    }
}