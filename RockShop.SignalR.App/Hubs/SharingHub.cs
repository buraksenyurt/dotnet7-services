using Microsoft.AspNetCore.SignalR;
using Chinook.Common;

namespace RockShop.SignalR.App.Hubs;

public class SharingHub
    : Hub
{
    private static Dictionary<string, StudentModel> Students = new();
    public async Task Register(StudentModel newStudent)
    {
        StudentModel student;
        if (Students.ContainsKey(newStudent.Name))
        {
            student = Students[newStudent.Name];
            if (student.Topics is not null)
            {
                foreach (var topic in student.Topics.Split(','))
                {
                    await Groups.RemoveFromGroupAsync(student.ConnectionId, topic);
                }
            }
            student.Topics = newStudent.Topics;
            student.ConnectionId = Context.ConnectionId;
        }
        else
        {
            if (string.IsNullOrEmpty(newStudent.Name))
            {
                newStudent.Name = Guid.NewGuid().ToString();
            }
            newStudent.ConnectionId = Context.ConnectionId;
            Students.Add(newStudent.Name, newStudent);
            student = newStudent;
        }

        if (student.Topics is not null)
        {
            foreach (var topic in student.Topics.Split(','))
            {
                await Groups.AddToGroupAsync(student.ConnectionId, topic);
            }
        }

        SuggestionModel suggestion = new()
        {
            From = "Conversation Hub",
            To = student.Name,
            Content = $"Merhaba {student.Name} - {student.ConnectionId}"
        };

        IClientProxy proxy = Clients.Client(student.ConnectionId);
        await proxy.SendAsync("ReceiveMessage", suggestion);
    }
}