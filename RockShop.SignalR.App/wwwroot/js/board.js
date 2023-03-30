"use strict";

var conn = new SignalR.HubConnectionBuilder().withUrl("/board").build();

document.getElementById("btnRegister").disabled = true;
document.getElementById("btnSend").disabled = true;
document.getElementById("personName").addEventListener(
    "input",
    function () {
        document.getElementById("msgFrom").value = document.getElementById("personName").value;
    }
);
document.getElementById("btnRegister").addEventListener("click", function (event) {
    var registered = {
        name: document.getElementById("personName").value,
        topics: document.getElementById("personTopics").value
    };
    conn.invoke("Register", registered).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
document.getElementById("btnSend").addEventListener("click", function (event) {
    var suggestion = {
        from: document.getElementById("msgFrom".value),
        to: document.getElementById("msgTo".value),
        content: document.getElementById("msgThoughts".value)
    };
    conn.invoke("SendSuggestion", suggestion).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

conn.start().then(function () {
    document.getElementById("btnRegister").disabled = false;
    document.getElementById("btnSend").disabled = false;

}).catch(function (err) {
    return console.error(err.toString());
});

conn.on("ReceiveSuggestion", function (suggestion) {
    var listItem = document.createElement("li");
    document.getElementById("receivedThoughts").appendChild(li);
    listItem.textContent = 'To ${suggestion.to}, From ${suggestion.from}: ${suggestion.content}';
})