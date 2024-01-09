var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButtonKorisnik").disabled = true;

connection.on("NBP_Chat", function (user, message) {

    connection.invoke("rek").then(function (user) {
        var u = document.getElementById("userKorisnik").innerHTML = user;
    });

    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);

    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {

    connection.invoke("rek").then(function (u) {
        document.getElementById("userKorisnik").innerHTML = u;
    });

    document.getElementById("sendButtonKorisnik").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById("sendButtonKorisnik").addEventListener("click", function (event) {
    var user = document.getElementById("userKorisnik").innerHTML;
    var message = document.getElementById("messageInput").value;
    //connection.invoke("SendMessageNovo", user, message).catch(function (err) {
    //    return console.error(err.toString());
    //});
    connection.invoke("Pub", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("Sub").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
