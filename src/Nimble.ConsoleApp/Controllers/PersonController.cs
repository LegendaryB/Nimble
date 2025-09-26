using System.Net;
using Nimble.ConsoleApp.Contracts;
using Nimble.Controllers;
using Nimble.Extensions;

namespace Nimble.ConsoleApp.Controllers;

public class PersonController : Controller
{
    public override Task GetAsync(
        HttpListenerRequest request,
        HttpListenerResponse response,
        CancellationToken cancellationToken = default)
    {
        var persons = new List<PersonContract>
        {
            new PersonContract
            {
                Name = "Peter Brown",
                Age = 25,
                Gender = "Male",
            },
            new PersonContract
            {
                Name = "Sally Red",
                Age = 31,
                Gender = "Female",
            },
            new PersonContract
            {
                Name = "David Senosa",
                Age = 45,
                Gender = "Male",
            },
            new PersonContract
            {
                Name = "Tucker",
                Age = 65,
                Gender = "Male",
            },
        };
        
        return response.RespondWithJsonAsync(
            persons,
            cancellationToken: cancellationToken);
    }
}