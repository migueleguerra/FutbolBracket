namespace FutbolDataService
{
    using FutbolBracket.Models;
    using FutbolBracket.Services;
    using Newtonsoft.Json.Linq;
    using System.Globalization;

    public class FutbolWorker : BackgroundService
    {
        private readonly ILogger<FutbolWorker> logger;
        private readonly HttpClient footbalClient;
        private readonly IConfiguration configuration;

        private readonly CosmosDbService<CompetitionEntity> competitionService;
        private readonly CosmosDbService<TeamEntity> teamService;

        public FutbolWorker(ILogger<FutbolWorker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.configuration = configuration;

            FutbolWorkerOptions futbolWorkerOptions = configuration.GetSection("CosmosOptions").Get<FutbolWorkerOptions>();

            footbalClient = httpClientFactory.CreateClient("FootbalAPI");
            competitionService = CosmosDbService<CompetitionEntity>.Create(
                futbolWorkerOptions.ConnectionString,
                futbolWorkerOptions.DatabaseName,
                futbolWorkerOptions.CompetitionContainerName);
            teamService = CosmosDbService<TeamEntity>.Create(
                futbolWorkerOptions.ConnectionString,
                futbolWorkerOptions.DatabaseName,
                futbolWorkerOptions.TeamContainerName);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workers = new List<Task>();
                var workersDictionary = configuration.GetSection("WorkerConfig").Get<FutbolWorkerOptions>().Workers;

                foreach (KeyValuePair<string, int> entry in workersDictionary)
                {
                    if (entry.Key == "MatchWorker")
                    {
                        workers.Add(MatchnWorker(entry.Value, stoppingToken));
                    }
                    else if (entry.Key == "CompetitionWorker")
                    {
                        workers.Add(CompetitionWorker(entry.Value, stoppingToken));
                    }
                }

                await Task.WhenAll(workers.ToArray());
            }
        }

        private async Task MatchnWorker(int delay, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation($"{DateTimeOffset.Now}: Match Worker running...");

                // TODO: Add match worker logic.

                logger.LogInformation($"{DateTimeOffset.Now}: Match Worker finish running. Wait for delay: {delay} minute.");
                await Task.Delay(TimeSpan.FromMinutes(delay), stoppingToken);
            }
        }

        private async Task CompetitionWorker(int delay, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation($"{DateTimeOffset.Now}: Competition Worker running...");

                try
                {
                    var json = await footbalClient.GetStringAsync("competitions/2021/teams");

                    JObject competitionJson = JObject.Parse(json);
                    List<TeamEntity> teams = GetAndSetTeams(competitionJson);
                    SetCompetition(competitionJson, teams);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Failed to get competitions", exception);
                }

                logger.LogInformation($"{DateTimeOffset.Now}: Competition Worker finish running. Wait for delay: {delay} minutes.");
                await Task.Delay(TimeSpan.FromMinutes(delay), stoppingToken);
            }
        }

        private async void SetCompetition(JObject competitionJson, List<TeamEntity> teams)
        {
            JToken? competitionInfoJson = competitionJson?.GetValue("competition");
            JToken? seasonInfoJson = competitionJson?.GetValue("season");
            JObject? competitionInfo = JObject.Parse(competitionInfoJson?.ToString());
            JObject? seasonInfo = JObject.Parse(seasonInfoJson?.ToString());
            var cultureInfo = new CultureInfo("en-US");

            CompetitionEntity competition = new CompetitionEntity()
            {
                Id = competitionInfo?.GetValue("id")?.ToString(),
                Name = competitionInfo?.GetValue("name")?.ToString(),
                ShortName = competitionInfo?.GetValue("code")?.ToString(),
                LogoUrl = competitionInfo?.GetValue("emblem")?.ToString(),
                Teams = teams,
                StartDate = DateTime.Parse(seasonInfo?.GetValue("startDate")?.ToString(), cultureInfo).ToUniversalTime(),
                EndDate = DateTime.Parse(seasonInfo?.GetValue("endDate")?.ToString(), cultureInfo).ToUniversalTime(),
                CurrentMatchDay = int.Parse(seasonInfo?.GetValue("currentMatchday")?.ToString()),
                Winner = seasonInfo?.GetValue("winner")?.ToString()
            };

            if (await competitionService.GetEntityAsync(competition.Id, competition.PartitionKey) != null)
            {
                logger.LogInformation($"{DateTimeOffset.Now}: Competition id: ({competition.Id}), name: ({competition.Name}) already exists!");
            }
            else
            {
                var createdCompetition = await competitionService.CreateEntityAsync(competition);
                logger.LogInformation($"{DateTimeOffset.Now}: Created team id: ({createdCompetition.Id}) on Competition container.");
            }
        }

        private List<TeamEntity> GetAndSetTeams(JObject competitionJson)
        {
            List<JToken>? teamsJson = competitionJson?.GetValue("teams")?.ToList();

            List<TeamEntity> teams = new List<TeamEntity>();
            teamsJson?.ForEach(async x =>
            {
                JObject teamJson = JObject.Parse(x.ToString());
                TeamEntity team = new TeamEntity()
                {
                    Id = teamJson?.GetValue("id")?.ToString(),
                    Name = teamJson?.GetValue("name")?.ToString(),
                    LogoUrl = teamJson?.GetValue("crest")?.ToString(),
                    Venue = teamJson?.GetValue("venue")?.ToString()
                };

                if (await teamService.GetEntityAsync(team.Id, team.PartitionKey) != null)
                {
                    logger.LogInformation($"{DateTimeOffset.Now}: Team id: ({team.Id}), name: ({team.Name}) already exists!");
                }
                else
                {
                    var createdTeam = await teamService.CreateEntityAsync(team);
                    logger.LogInformation($"{DateTimeOffset.Now}: Created team id: ({createdTeam.Id}) on Team container.");
                }

                teams.Add(team);
            });

            return teams;
        }
    }
}