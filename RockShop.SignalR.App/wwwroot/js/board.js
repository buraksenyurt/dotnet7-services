"use strict";

var conn = new signalR.HubConnectionBuilder().withUrl("/board").build();

document.getElementById("btnRegister").disabled = true;
document.getElementById("btnSend").disabled = true;
document.getElementById("personName").addEventListener(
    "input",
    function () {
        document.getElementById("msgFrom").value = document.getElementById("personName").value;
    }
);

conn.start().then(function () {
    document.getElementById("btnRegister").disabled = false;
    document.getElementById("btnSend").disabled = false;

}).catch(function (err) {
    return console.error(err.toString());
});

conn.on("ReceiveSuggestion", function (suggestion) {
    console.log(suggestion.toString());
    var row = document.createElement("tr");

    var columnFrom = document.createElement("td");
    columnFrom.textContent = suggestion.from;
    row.appendChild(columnFrom);

    var columnTo = document.createElement("td");
    columnTo.textContent = suggestion.to;
    row.appendChild(columnTo);

    var columnContent = document.createElement("td");
    columnContent.textContent = suggestion.content;
    row.appendChild(columnContent);

    document.getElementById("receivedThoughts").appendChild(row);

});

document.getElementById("btnRegister").addEventListener("click", function (event) {
    console.log("Registeration start");
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
        from: document.getElementById("msgFrom").value,
        to: document.getElementById("msgTo").value,
        content: document.getElementById("msgThoughts").value
    };
    conn.invoke("SendSuggestion", suggestion).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});