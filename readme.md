iiMin
=====

iiMin is a C# library supporting the modification of files relating to M.A.X, the 1996 RTS game developed by Interplay Productions.

| Name   | Read | Write | Comment
|--------|:----:|-------|--------
| CAM    | ✔   |   ✗   | Mission file - campaign: plain text (briefing) / binary (mission)
| INI    | ✗   |   ✗   | Plain text
| DMO    | ✔   |   ✗   | Mission file - Demo
| DTA    | ✔   |   ✗   | Mission - Unknown
| FLC    | ✗   |   ✗   | 
| FON    | ✗   |   ✗   | 
| GFX    | ✔   |   ✗   | Extensionless images contained within RES
| MSC    | ✗   |   ✗   | Music - standard WAV
| MPS    | ✗   |   ✗   | Plain text
| MVE    | ➜   |   ✗   | Interplay movie, see [ii.SingleMve](https://www.nuget.org/packages/ii.SingleMve/)
| PAL    | ✗   |   ✗   | 
| RES    | ✔   |   ✔   | 
| SCE    | ✔   |   ✗   | Mission file - Scenario : plain text (briefing) / binary (mission)
| SPW    | ✗   |   ✗   | Sounds - standard WAV
| TRA    | ✔   |   ✗   | Mission file - Training : plain text (briefing) / binary (mission)
| WRL    | ✔   |   ✔   | Map data

## Usage

Instantiate the relevant class and call the `Read` method passing the filename.

```csharp
var resProcessor = new ResProcessor();
var files = resProcessor.Read(@"D:\data\max\MAX.RES");
for (int i = 0; i < files.Count; i++)
{
    File.WriteAllBytes(@$"D:\data\max\{files[i].Name}", files[i].Content);
}

var WrlProcessor = new WrlProcessor();
var tiles = WrlProcessor.Read(@"D:\data\max\SNOW_2.wrl");
```


## Compiling

To clone and run this application, you'll need [Git](https://git-scm.com) and [.NET](https://dotnet.microsoft.com/) installed on your computer. From your command line:

```
# Clone this repository
$ git clone https://github.com/btigi/iiMin

# Go into the repository
$ cd src

# Build  the app
$ dotnet build
```

## Licencing

iiMin is licenced under the MIT License. Full licence details are available in licence.md

iiMin uses information from https://klei1984.github.io/max/save under MIT licence