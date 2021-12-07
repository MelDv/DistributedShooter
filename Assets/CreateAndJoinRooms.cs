using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using System.Text;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public static string lobbyAction = "";
    private bool roomCreator = false;
    private Random random = new Random();

    // Input fields for the room name.
    public UnityEngine.UI.InputField createInput;
    public UnityEngine.UI.InputField joinInput;

    // When you create a room, you automatically join it.
    public void CreateRoom()
    {
        //todo game clock 
        roomCreator = true;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.PublishUserId = true;
        roomOptions.MaxPlayers = 4;
        lobbyAction = "create";
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
        NamePlayers(createInput.text);
        print("*** Room created");
    }

    public void JoinRoom()
    {
        NamePlayers(joinInput.text);
        lobbyAction = "join";
        PhotonNetwork.JoinRoom(joinInput.text);
        print("*** Room joined");
    }

    private void NamePlayers(String roomName)
    {
        StringBuilder newName = new StringBuilder();
        newName.Append(roomName);
        if (roomCreator)
            newName.Append("Master");
        else
            newName.Append("Dweller" + RandomString(4));
        PhotonNetwork.NickName = newName.ToString();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
        print("*** Game loaded");
    }

    private string RandomString(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}