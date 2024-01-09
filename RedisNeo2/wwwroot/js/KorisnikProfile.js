var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


connection.start().then(function () {

    connection.invoke("ren").then(function (u) {
        document.getElementById("dobrodosao").innerText = u;
    });

    document.getElementById("sendButtonBiblioteka").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});