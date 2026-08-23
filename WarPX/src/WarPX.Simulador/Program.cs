using Microsoft.AspNetCore.SignalR.Client;

const string serverUrl = "http://localhost:5100/pixelHub";
const int totalBotUsers = 50;
const int delayBetweenPaintsMs = 2;

Console.WriteLine($"🚀 Iniciando simulação com {totalBotUsers} usuários concorrentes...");
Console.WriteLine($"Target: {serverUrl}\n");

var colors = new[] { "#ffffff", "#070707", "#FF0000", "#00FF00", "#0000FF", "#FFFF00", "#FF00FF", "#00FFFF", "#FF8800", "#5f2e75" };
var random = new Random();
var tasks = new List<Task>();

for (int i = 1; i <= totalBotUsers; i++)
{
    int botId = i;
    tasks.Add(Task.Run(async () =>
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .WithAutomaticReconnect()
            .Build();

        try
        {
            await connection.StartAsync();
            Console.WriteLine($"[Bot #{botId}] Conectado ao Hub.");

            while (true)
            {
                int x = random.Next(0, 100);
                int y = random.Next(0, 100);
                string color = colors[random.Next(colors.Length)];

                try
                {
                    var result = await connection.InvokeAsync<dynamic>("PaintPixel", x, y, color);
                    Console.WriteLine($"[Bot #{botId}] Pintou em ({x},{y}) com {color}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Bot #{botId}] Erro ao pintar: {ex.Message}");
                }

                await Task.Delay(delayBetweenPaintsMs);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Bot #{botId}] Falha na conexão: {ex.Message}");
        }
    }));
}

await Task.WhenAll(tasks);