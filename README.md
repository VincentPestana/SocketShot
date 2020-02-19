
# SocketShot
C# console that takes screenshots and sends them over SignalR, to a html webpage.

Everything is currently designed to run on a single machine, however all 3 parts can be separated completely.

## Software Features
- Dynamic Bitmap to JPEG conversion quality depending on screenshot size
- Simple screenshot logic using built in "normal" C# methods
- Even simpler SignalR hub and connection code
- Bitmap resizing


## Deploy to IIS
- In Visual Studio, create a new publish profile for "File System", point to your folder of choice and hit publish
- Right click on "Defauly Web Site" inside IIS and click "Add Application"
- Name should be something memorable, but unique amongst your IIS
- Point to the directory you deployed to, all settings are default
- Confirm with phone on the same wifi network that site loads