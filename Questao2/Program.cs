using Newtonsoft.Json;

public class Program
{
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> GetTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;
        totalGoals += await GetGoalsByTeamRole(team, year, "team1");
        totalGoals += await GetGoalsByTeamRole(team, year, "team2");

        return totalGoals;
    }

    private static async Task<int> GetGoalsByTeamRole(string team, int year, string role)
    {
        int totalGoals = 0;
        int page = 1;
        using (HttpClient client = new HttpClient())
        {
            while (true)
            {
                try
                {
                    string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{role}={team}&page={page}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Verifica se a resposta foi bem-sucedida
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Erro ao acessar a URL: {url}. Código de status: {response.StatusCode}");
                        break;
                    }

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var matchData = JsonConvert.DeserializeObject<FootballApiResponse>(jsonResponse);

                    if (matchData.Data.Count == 0) break;

                    foreach (var match in matchData.Data)
                    {
                        totalGoals += role == "team1" ? int.Parse(match.Team1Goals) : int.Parse(match.Team2Goals);
                    }

                    if (page >= matchData.TotalPages) break;
                    page++;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Erro de requisição ao acessar a URL: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                    break;
                }
            }
        }
        return totalGoals;
    }

}

public class FootballApiResponse
{
    [JsonProperty("page")]
    public int Page { get; set; }

    [JsonProperty("per_page")]
    public int PerPage { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("total_pages")]
    public int TotalPages { get; set; }

    [JsonProperty("data")]
    public List<Match> Data { get; set; }
}

public class Match
{
    [JsonProperty("team1")]
    public string Team1 { get; set; }

    [JsonProperty("team2")]
    public string Team2 { get; set; }

    [JsonProperty("team1goals")]
    public string Team1Goals { get; set; }

    [JsonProperty("team2goals")]
    public string Team2Goals { get; set; }
}