# USB Trojan
Super simple loader that spreads over removable drives (USB flash drives, portable and network drives, SD cards).
## Features
 - You can add the HWID of your PC to the black list and trojan will ignore it
 - You can add any payload (executable file)
 - Slient work
## Structure of the program
 - `Program.cs` - Main part of the program
 - `HWID.cs` - HWID generator
 - `Config.cs` - Configuration file
 - `Shortcut.cs` - This class creates shortcuts
 - `Tools.cs` - Tools for trojan (net utils, and your payload)
 - `USB.cs` - Infects disks and manages files
## Build
1. Open `Config.cs` and edit HWID settings.
2. Open `Tools.cs`, go to 50 line and modify `RunPayload(object)` method.
3. Use Visual Studio 2015 or 2017 to build this project.

**Warning!**
This program may break your system or someone else's, be careful! Use VirtualBox and disable all external devices to test the program. I do not bear any material or moral responsibility. The software is provided as-is and is under development.
