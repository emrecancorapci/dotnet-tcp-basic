using System.Net;
using System.Net.Sockets;
using System.Text;

await StartServer();
Console.Read();

async Task StartServer()
{
    IPHostEntry host = await Dns.GetHostEntryAsync("localhost");
    IPAddress ipAddress = host.AddressList[0];
    const int port = 60_000;
    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

    try
    {
        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Console.WriteLine("Socket created with TCP Protocol.");

        listener.Bind(localEndPoint);
        Console.WriteLine($"Endpoint bind at [{ipAddress}:{port}] .");

        listener.Listen(10);
        Console.WriteLine("Socket started to listen.");

        Socket handler = listener.Accept();
        Console.WriteLine("Connection accepted. Waiting for message...");

        string receivedMessage = "";

        while (true)
        {
            var receivedData = new byte[1024];
            int receivedBytes = await handler.ReceiveAsync(receivedData);
            receivedMessage += Encoding.UTF8.GetString(receivedData, 0, receivedBytes);

            if (receivedMessage.IndexOf("<EOF>", StringComparison.Ordinal) > -1) break;
        }

        Console.WriteLine($"\nMessage received: {receivedMessage[..^5]}");

        byte[] message = Encoding.UTF8.GetBytes(receivedMessage);
        await handler.SendAsync(message);
        Console.WriteLine("Received message sent back.");
        handler.Shutdown(SocketShutdown.Both);
        handler.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
}