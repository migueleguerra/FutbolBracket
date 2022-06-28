namespace FutbolDataService
{
    public class FutbolWorkerOptions
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string FutbolApiBaseAddress { get; set; }

        public string FutbolTokenName { get; set; }

        public string FutbolApiToken { get; set; }

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string CompetitionContainerName { get; set; }

        public string TeamContainerName { get; set; }

        public string MatchesContainerName { get; set; }

        public Dictionary<string, int> Workers { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
