// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");

//Function that will find all devices on the network

IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

foreach (IPAddress ip in host.AddressList) {
    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
        string subnet = GetSubnet(ip);
        if (subnet != null) {
            var tasks = new Task<PingReply>[254];
            for (int i = 1; i < 255; i++) {
                using Ping ping = new();
                string address = subnet + "." + i.ToString();
                //ping.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
                var replyTask = ping.SendPingAsync(address, 1000);
                tasks[i - 1] = replyTask;
            }

            await Task.WhenAll(tasks);

            for (int i = 0; i < tasks.Length; i++) {
                var reply = await tasks[i];
                if (reply is null || reply.Status == IPStatus.TimedOut) continue;
                Console.WriteLine("Device Details Here");
                Console.WriteLine("Address: {0}", reply.Address.ToString());
                Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                Console.WriteLine("Time to live: {0}", reply.Options?.Ttl);
                Console.WriteLine("Don't fragment: {0}", reply.Options?.DontFragment);
                Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                Console.WriteLine("MAC: {0}", GetMacAddress(reply.Address.ToString()));
                Console.WriteLine("-------------------------------------------------------------");
            }
        }
    }
}

Console.ReadLine(); // wait for ping responses to complete

static string GetMacAddress(string ipAddress) {
    string macAddress = string.Empty;
    System.Diagnostics.Process pProcess = new();
    pProcess.StartInfo.FileName = "arp";
    pProcess.StartInfo.Arguments = "-a " + ipAddress;
    pProcess.StartInfo.UseShellExecute = false;
    pProcess.StartInfo.RedirectStandardOutput = true;
    pProcess.StartInfo.CreateNoWindow = true;
    pProcess.Start();
    string strOutput = pProcess.StandardOutput.ReadToEnd();
    string[] substrings = strOutput.Split('-');
    if (substrings.Length >= 8) {
        macAddress = substrings[3][Math.Max(0, substrings[3].Length - 2)..]
                 + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                 + "-" + substrings[7] + "-"
                 + substrings[8][..2];
        return macAddress;
    } else {
        return "not found";
    }
}

static string GetSubnet(IPAddress ip) {
    byte[] bytes = ip.GetAddressBytes();
    if (bytes[0] == 10) {
        return $"{bytes[0]}.{bytes[1]}.{bytes[2]}";
    } else if (bytes[0] == 172 && (bytes[1] >= 16 && bytes[1] <= 31)) {
        return $"{bytes[0]}.{bytes[1]}.{bytes[2]}";
    } else if (bytes[0] == 192 && bytes[1] == 168) {
        return $"{bytes[0]}.{bytes[1]}.{bytes[2]}";
    } else {
        return string.Empty;
    }
}