using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public static string lobbyAction = "";

    // Input fields for the room name.
    public UnityEngine.UI.InputField createInput;
    public UnityEngine.UI.InputField joinInput;

    // When you create a room, you automatically join it.
    public void CreateRoom()
    {
        //todo naming and game clock 
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.PublishUserId = true;
        roomOptions.MaxPlayers = 4;
        lobbyAction = "create";
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
        print("*** Room created");
    }

    public void JoinRoom()
    {
        lobbyAction = "join";
        PhotonNetwork.JoinRoom(joinInput.text);
        print("*** Room joined");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
        print("*** Game loaded");
    }
}