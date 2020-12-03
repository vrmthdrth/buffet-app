"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/buffetclient/chatHub").build();

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " : " + msg;
    var li = document.createElement("li");
    li.className = "message";
    li.textContent = encodedMsg;
    document.getElementById("messagesList").prepend(li); //appendchild
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("messageInput").onkeypress = function (e) {
    if (e.keyCode == 13) {
        var user = document.getElementById("userInput").value;
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
        document.getElementById("messageInput").innerHTML = '';
        e.preventDefault();
    }
}
