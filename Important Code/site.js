
let transportType = signalR.TransportType.WebSockets;
let http = new signalR.HttpConnection(`http://${document.location.host}/tictactoeroom`, { transport: transportType });
let connection = new signalR.HubConnection(http);
connection.start();

//Callback entries for Server Side SignalR
connection.on('DisplayMessage', appendMessage);
connection.on('SetPlayer', setPlayer);
connection.on('ResetGame', resetGame);
connection.on('ToggleGameMode', () => {
    toggleControl('inLobby');
    toggleControl('inGame');
});
connection.on('UpdateTile', updateTile);
connection.on('SetRoomID', setRoomID); //used in the case server generates a unique room id for AI match


document.getElementById('sendMessage').addEventListener('submit', event => {
    let message = document.getElementById('message').value;
    let roomID = document.getElementById('roomId').value;
    document.getElementById('message').value = '';
    connection.invoke('BroadcastMessage', message, roomID);
    event.preventDefault();
});

document.getElementById('joinRoom').addEventListener('submit', event => {
    let roomID = document.getElementById('roomID').value;
    connection.invoke('JoinRoom', roomID);
    event.preventDefault();
});

document.getElementById('playComputer').addEventListener('submit', event => {
    connection.invoke('PlayComputer');
    event.preventDefault();
});

document.getElementById('leaveRoom').addEventListener('submit', event => {
    connection.invoke('LeaveRoom');
    document.getElementById('messages').innerHTML = '';
    event.preventDefault();
});

let game = document.getElementById('game');
Array.from(game.getElementsByClassName('btnBoard')).forEach(function (tile, i) {
    tile.onclick = () => {
        let roomID = document.getElementById('roomID').value;
        connection.invoke('MakeMove', roomID, i);
    };
});

function appendMessage(message) {
    let messages = document.getElementById('messages');
    let line = document.createElement('li');
    line.innerText = message;
    messages.appendChild(line);
    if (messages.getElementsByTagName('li').length > 10)
        messages.removeChild(messages.getElementsByTagName('li')[0]);
}

function setPlayer(playerID) {
    document.getElementById('playerDetails').innerHTML = 'You are ' + playerID;
}

function resetGame() {
    let game = document.getElementById('game');
    Array.from(game.getElementsByClassName('btnBoard')).forEach(function (tile) {
        tile.value = '-';
    });
}

function toggleControl(controlName) {
    let control = document.getElementById(controlName);
    if (control.style.display === 'block')
        control.style.display = 'none';
    else
        control.style.display = 'block';
}

function updateTile(pos, symbol) {
    let game = document.getElementById('game');
    game.getElementsByClassName('btnBoard')[pos].value = symbol;
}

function setRoomID(roomID) {
    document.getElementById('roomID').value = roomID;
}