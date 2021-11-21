using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    // Input fields for the room name.
    public UnityEngine.UI.InputField createInput;
    public UnityEngine.UI.InputField joinInput;

    // When you create a room, you automatically join it.
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
        print("Room created");
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
        print("Room joined");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
        print("Game loaded");
    }
}