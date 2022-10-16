// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Hello, World!");

var ip = new IPEndPoint(0, 0);
var port = 9876;

var udpClient = new UdpClient();
udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

var source = new CancellationTokenSource();

var task = Task.Run(async () => {
    while (!source.Token.IsCancellationRequested) {
        var data = await udpClient.ReceiveAsync();
        var x = Encoding.UTF8.GetString(data.Buffer);
        Console.WriteLine($"Data Here : {x}");
    }
});

var counter = 0;

while (!source.Token.IsCancellationRequested) {
    counter++;
    if (counter > 20) break;
    var text = Console.ReadLine() ?? "";
    var data = Encoding.UTF8.GetBytes(text);
    udpClient.Send(data, data.Length, "255.255.255.255", port);
    if (text.Equals("EXIT", StringComparison.OrdinalIgnoreCase)) source.Cancel();
}

task.Wait();
Console.WriteLine("CHEERS");