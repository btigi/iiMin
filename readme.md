iiMin
=====

iiMin is a C# library supporting the modification of files relating to M.A.X, the 1996 RTS game developed by Interplay Productions.

| Name   | Read | Write | Comment
|--------|:----:|-------|--------
| CAM    | ✗   |   ✗   | Plain text / binary
| INI    | ✗   |   ✗   | Plain text
| DMO    | ✗   |   ✗   |  
| DTA    | ✗   |   ✗   | 
| FLC    | ✗   |   ✗   | 
| FON    | ✗   |   ✗   | 
| MSC    | ✗   |   ✗   | 
| MPS    | ✗   |   ✗   | Plain text
| MVE    | ✗   |   ✗   | 
| PAL    | ✗   |   ✗   | 
| RES    | ✔   |   ✔   | 
| SCE    | ✗   |   ✗   | Plain text / binary
| SPW    | ✗   |   ✗   | 
| TRA    | ✗   |   ✗   | Plain text
| WRL    | ✔   |   ✔   | 

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
