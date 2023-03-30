using Microsoft.AspNetCore.SignalR;
using Chinook.Common.SignalR.Models;

namespace RockShop.SignalR.App.Hubs;

public class BoardHub
    : Hub
{
    private static Dictionary<string, Person> Employees = new();
    public async Task Register(Person register)
    {
        Person employee;
        if (Employees.ContainsKey(register.Name))
        {
            employee = Employees[register.Name];
            if (employee.Topics is not null)
            {
                foreach (var topic in employee.Topics.Split(','))
                {
                    await Groups.RemoveFromGroupAsync(employee.ConnectionId, topic);
                }
            }
            employee.Topics = register.Topics;
            employee.ConnectionId = Context.ConnectionId;
        }
        else
        {
            if (string.IsNullOrEmpty(register.Name))
            {
                register.Name = Guid.NewGuid().ToString();
            }
            register.ConnectionId = Context.ConnectionId;
            Employees.Add(register.Name, register);
            employee = register;
        }

        if (employee.Topics is not null)
        {
            foreach (var topic in employee.Topics.Split(','))
            {
                await Groups.AddToGroupAsync(employee.ConnectionId, topic);
            }
        }

        Suggestion suggestion = new()
        {
            From = "Conversation Hub",
            To = employee.Name,
            Content = $"Ahoy! {employee.Name} - {employee.ConnectionId}"
        };

        IClientProxy proxy = Clients.Client(employee.ConnectionId);
        await proxy.SendAsync("ReceiveSuggestion", suggestion);
    }

    public async Task SendSuggestion(Suggestion suggestion)
    {
        IClientProxy proxy;
        if (string.IsNullOrEmpty(suggestion.To))
        {
            suggestion.To = "Everyone";
            proxy = Clients.All;
            await proxy.SendAsync("ReceiveSuggestion", suggestion);
        }

        var studentOrTopic = suggestion.To.Split(',');
        foreach (var st in studentOrTopic)
        {
            if (Employees.ContainsKey(st))
            {
                suggestion.To = $"Person: {Employees[st].Name}";
                proxy = Clients.Client(Employees[st].ConnectionId);
            }
            else
            {
                suggestion.To = $"Topic: {st}";
                proxy = Clients.Group(st);
            }
            await proxy.SendAsync("ReceiveSuggestion", suggestion);
        }
    }
}