<h1 align="center">NeatInput</h1>
<div align="center">

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/built-with-love.svg)](https://forthebadge.com)

![GitHub License](https://img.shields.io/github/license/LegendaryB/Nimble)

Nimble is a simple and flexible C# framework for creating HTTP servers and serving static files.

<sub>Built with ❤ by LegendaryB</sub>
</div>

## 🌟 Motivation
I created Nimble for scenarios where a full-featured framework like ASP.NET Core would be overkill, offering a lightweight and flexible HTTP server — and a fun little project to experiment with.

---

## 🚀 Features
* Create and run a lightweight HTTP server in C#
* Add routes using controllers
* Serve static files with directory listing
* Embedded HTML templates for static content
* Simple and async-friendly API
* Middleware-ready design for future extensibility
* Fully self-contained, no external dependencies required

---

## 📝 Example Usage
```csharp
var server = new HttpServer("http://localhost:5000/")
    .AddRoute<ExampleController>("/api")
    .AddStaticRoute("/static", "./wwwroot");

await server.RunAsync();
```

Open your browser and navigate to `http://localhost:5000/static` to see the static files served.

## 🖥️ Nimble Console Example
Head over to [here](https://github.com/LegendaryB/Nimble/tree/main/src/Nimble.ConsoleApp) to check it out!