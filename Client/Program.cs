using System.Net;
using System.Net.Sockets;
using System.Text;

await StartClient();

async Task StartClient()
{
    byte[] data = new byte[1024];

    try
    {
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = host.AddressList[0];
        const int port = 60_000;
        IPEndPoint remoteEndPoint = new (ipAddress, port);

        Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Console.WriteLine("Socket created with TCP Protocol.");

        try
        {
            await sender.ConnectAsync(remoteEndPoint);
            Console.WriteLine($"Socket connected to [{ipAddress}:{port}]");

            string? messageString;
            Console.WriteLine("Enter the message.");
            messageString = Console.ReadLine();

            byte[] messageData = Encoding.UTF8.GetBytes(messageString + "<EOF>");

            int byteSent = await sender.SendAsync(messageData);
            Console.WriteLine("Message sent.");
            int byteReceived = await sender.ReceiveAsync(data);
            Console.WriteLine("Data received.");
            string receivedMessage = Encoding.UTF8.GetString(messageData, 0, byteReceived);

            Console.WriteLine($"\nEchoed data: {receivedMessage[..^5]}");

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Argument Null Exception : {ex.Message}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Socket Exception : {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Exception : {ex.Message}");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}