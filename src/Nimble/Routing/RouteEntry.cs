using System.Text.RegularExpressions;
using Nimble.Controllers;

namespace Nimble.Routing;

internal record RouteEntry(
    string RoutePattern,
    Controller Controller,
    bool IsPrefix,
    Regex? ParameterRegex);