var Puzzle = {};
Puzzle.Validate = {};

Puzzle.Ajax = function (Method, Url, Data, OnSuccess, OnFail) {
    $.ajax({
        method: Method,
        url: Url,
        data: Data
    }).done(function (data) {
        try {
            OnSuccess(data);
        } catch (e) {
            console.error(e);
        }
    }).fail(function (jqXHR, textStatus) {
        try {
            OnFail();
        } catch (e) {
            console.error(e);
        }
    });
}

Puzzle.Post = function (Url, Data, OnSuccess, OnFail) {
    Portal.HttpAjax("POST", Url, Data, OnSuccess, OnFail);
}

Puzzle.Get = function (Url, Data, OnSuccess, OnFail) {
    Portal.HttpAjax("GET", Url, Data, OnSuccess, OnFail);
}

Puzzle.Validate.Mobile = function (mobile) {
    var re = /^09([0-9]{9})$/;
    return re.test(String(mobile).toLowerCase());
}

Puzzle.Validate.Email = function (email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

if (!document.location.pathname.toLowerCase().startsWith('/edit/') && !document.location.pathname.toLowerCase().startsWith('/customer/addproject/') && document.location.pathname.toLowerCase() != '/') {
    sessionStorage.setItem("back", document.location.href);
}

var connection = new signalR.HubConnectionBuilder().withUrl("/notif").build()

connection.on("Notification", function (title, body, url) {
    Notif(title, body, url)
})

connection.start().then(function () {
    //alert()
}).catch(function (err) {
    return console.error(err.toString())
})

if (Notification.permission !== 'granted') {
    Notification.requestPermission()
}

function Notif(title, body, url) {

    if (document.getElementById('is-admin').value == 'true') {
        if (Notification.permission !== 'granted')
            Notification.requestPermission();
        else {
            var notification = new Notification(title, {
                icon: 'https://puzzlearchitect.ir/2x2.png',
                body: body,
            });
            notification.onclick = function () {
                window.open(url);
            };
        }
    }
}