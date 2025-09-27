# 📁 Nimble.StaticFiles

A lightweight C# library to serve static files using Nimble.

## 📦 Installation

```bash
dotnet add package NotLegendaryB.Nimble.StaticFiles
```

## 🛠️ Usage

```csharp
using Nimble;
using Nimble.StaticFiles;

var server = new HttpServer("http://localhost:5000/")
    .AddStaticRoute("/static", "./");

await server.RunAsync();
```

Open in browser:

```
http://localhost:5000/static
```

You should now see the static directory listing page:

![Screenshot](../../assets/directoryListing.png "App Screenshot")