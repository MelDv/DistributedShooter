using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
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