using System.Diagnostics;
using System.Net.NetworkInformation;
try
{
    Console.SetWindowSize(40, 2);
    Console.ForegroundColor = ConsoleColor.White;
    Console.BackgroundColor = ConsoleColor.DarkYellow;

    var nics = NetworkInterface.GetAllNetworkInterfaces();
    var upNetworks = nics.Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)!;

    if (!upNetworks.Any())
    {
        Console.Clear();
        Console.WriteLine("*****No Internet Connection*****");
        for (int i = 0; i < 2; i++)
        {
            Console.Beep(1000, 100);
        }
        await Task.Delay(1500);
        return;
    }
    if (upNetworks.Count() > 1)
    {
        Console.Clear();
        Console.WriteLine("*****Turn The VPN Off*****");
        for (int i = 0; i < 2; i++)
        {
            Console.Beep(1000, 100);
        }
        await Task.Delay(1500);
        return;
    }


    var targetNetwork = upNetworks.SingleOrDefault();

    var processInfo = new ProcessStartInfo
    {
        Verb = "runas",
        LoadUserProfile = true,
        FileName = "cmd.exe",
        UseShellExecute = true,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden
    };

    if (targetNetwork.GetIPProperties().DnsAddresses[0].ToString() != "178.22.122.100")
    {
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.Clear();
        processInfo.Arguments = $"/c netsh interface ipv4 add dnsservers \"{targetNetwork.Name}\" address=178.22.122.100 index=1 && " +
                                $"netsh interface ipv4 add dnsservers \"{targetNetwork.Name}\" address=185.51.200.2 index=2";
        Console.WriteLine("*****Enabling Shecan*****");
        for (int i = 0; i < 3; i++)
        {
            Console.Beep(1000, 100);
        }
    }
    else
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.Clear();
        processInfo.Arguments = $"/c netsh interface ipv4 set dnsservers \"{targetNetwork.Name}\" dhcp";
        Console.WriteLine("*****Disabling Shecan*****");
        Console.Beep(1000, 100); 
    }

    Process.Start(processInfo);
    await Task.Delay(100);
}
catch (Exception e)
{
    File.AppendAllText($"{Directory.GetCurrentDirectory()}/Log.txt",$"{DateTime.Now} => {e.Message}\n\n");
    Console.Clear();
    Console.WriteLine("*****Exception ! See Log.txt*****");
    for (int i = 0; i < 2; i++)
    {
        Console.Beep(1000, 100);
    }
    await Task.Delay(1500);
}
