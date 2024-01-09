var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButtonBiblioteka").disabled = true;

connection.on("aaaa", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;

});

connection.start().then(function () {

    connection.invoke("re").then(function (u) {
        document.getElementById("userNPB").innerText = u;
    });

    document.getElementById("sendButtonBiblioteka").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("BIBLIOTEKA_USER").value;
//    var message = document.getElementById("messageInput").value;
//    const messageString = JSON.stringify(message);
//    connection.invoke("SendMessage", user, messageString).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});

document.getElementById("sendButtonBiblioteka").addEventListener("click", function (event) {
    var user = document.getElementById("userNPB").innerText;
    //var user = document.getElementById("korisnik_user").innerHTML;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage1", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

