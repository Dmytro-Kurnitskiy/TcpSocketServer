
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private static ConcurrentDictionary<TcpClient, ClientInfo> clients = new ConcurrentDictionary<TcpClient, ClientInfo>();
    
    static async Task Main(string[] args)
    {
        if (args.Length != 1 || !int.TryParse(args[0], out int port))
        {
            Console.WriteLine("Usage: dotnet run <port>");
            return;
        }
        
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Server listening on port {port}...");

        while (true)
        {
            using TcpClient client = await listener.AcceptTcpClientAsync();
            await HandleClient(client);
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        var endPoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
        var info = new ClientInfo(endPoint);
        clients[client] = info;
        try
        {
            await using var stream = client.GetStream();
            await using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            using var reader = new StreamReader(stream, Encoding.UTF8);

            await writer.WriteLineAsync(
                "Welcome to the TcpSocketServer! You can write a one number per time and each time server will add it to the sum of other numbers. Or you can write command \"list\" to check the ips of the connected clients");

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.Trim().ToLower().Equals("list"))
                {
                    var clientList = string.Join("/n",
                        clients.Values.Select(c => $"Address: {c.Endpoint} - Sum: {c.Sum}"));
                    await writer.WriteLineAsync(clientList);
                }
                else if (int.TryParse(line, out int number))
                {
                    info.Sum += number;
                    await writer.WriteLineAsync($"Sum: {info.Sum}");
                }
                else
                {
                    await writer.WriteLineAsync("Invalid input. Please enter a number or \"list\" command.");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error with client {info.Endpoint}: {e.Message}");
        }
        finally
        {
            clients.TryRemove(client, out _);
            client.Close();
            Console.WriteLine($"Connection closed: {info.Endpoint}");
        }
    }
}

public class ClientInfo
{
    public string Endpoint { get; }
    public int Sum { get; set; } = 0;

    public ClientInfo(string endpoint)
    {
        Endpoint = endpoint;
    }
}