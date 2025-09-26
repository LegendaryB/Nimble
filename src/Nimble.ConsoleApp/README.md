# 🖥️ Nimble Console Example

A simple console application demonstrating how to use Nimble, a lightweight and flexible C# HTTP server framework.

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## 📦 Getting Started

1. **Clone the repository**

```bash
git clone https://github.com/LegendaryB/Nimble.git
cd Nimble/NimbleConsoleApp
```

2. **Build the project**

```bash
dotnet build
```

3. **Run the console app**

**Serve the API**
```bash
dotnet run -- serve-api http://localhost:5000/
```

**Serve static files**
```bash
dotnet run -- serve-static http://localhost:5000/ -p ./
```
* `-p <folder>`: Specify the folder to serve static files from.

You should see the HTTP server start and listening on the configured URL.

---

## 🙏 Credits

This console app uses [Spectre.Console.Cli](https://spectreconsole.net/cli) for building and parsing commands in a clean and expressive way.