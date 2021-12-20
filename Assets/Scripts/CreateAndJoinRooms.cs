using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public static string lobbyAction = "";
    private bool roomCreator = false;
    private System.Random random = new System.Random();

    // Input fields for the room name.
    public UnityEngine.UI.InputField createInput;
    public UnityEngine.UI.InputField joinInput;
    private List<RoomInfo> rList;

    public GameObject create;
    public GameObject join;

    [SerializeField]
    private Text roomInfo;
    [SerializeField]
    private Text rooms;
    [SerializeField]
    private Text logs;
    private string onCreateName;
    private string onJoinName;

    private void Awake()
    {
        roomInfo = GameObject.FindWithTag("RoomInfo").GetComponent<Text>();
        rooms = GameObject.FindWithTag("RoomNames").GetComponent<Text>();
        logs = GameObject.FindWithTag("Messages").GetComponent<Text>();
        create = GameObject.FindGameObjectWithTag("create");
        join = GameObject.FindGameObjectWithTag("join");
        create.SetActive(true);
        join.SetActive(true);
    }

    public void OnCreateClicked()
    {
        create.SetActive(false);
        onCreateName = createInput.text;
        CreateRoom();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        rList = new List<RoomInfo>();
        base.OnRoomListUpdate(roomList);
        StringBuilder currentRooms = new StringBuilder();

        foreach (RoomInfo r in roomList)
        {
            if (r.RemovedFromList)
            {
                rList.Remove(r);
                print("removed " + r.Name);
            }
            if (r.IsOpen)
            {
                rList.Add(r);
                currentRooms.Append("Room name: " + r.Name + "\n     Players: " + r.PlayerCount + "\n");
            }
        }
        roomInfo.text = "Rooms available: " + roomList.Count;

        if (roomList.Count > 0)
        {
            rooms.text = "Current rooms: \n" + currentRooms.ToString();
        }
        else
        {
            rooms.text = "No rooms available.";
        }
    }

    // When you create a room, you automatically join it.
    public void CreateRoom()
    {
        create.SetActive(false);
        join.SetActive(false);
        onCreateName = createInput.text;
        print("CREATEROOM onCreateName: " + onCreateName);

        if (!TestName(onCreateName))
        {
            logs.text = "Room name not acceptable. \nUse numbers or letters only.";
            create.SetActive(true);
            join.SetActive(true);
            return;
        }

        roomCreator = true;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.PublishUserId = true;
        lobbyAction = "create";
        PhotonNetwork.CreateRoom(onCreateName, roomOptions);
        NamePlayers(onCreateName);

        print("*** Room created");
    }

    private IEnumerator Wait(float v)
    {
        yield return new WaitForSeconds(v);
    }

    public void JoinRoom()
    {
        join.SetActive(false);
        onJoinName = joinInput.text;
        NamePlayers(onJoinName);
        lobbyAction = "join";
        PhotonNetwork.JoinRoom(onJoinName);
        print("*** Room joined");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
        print("*** Game loaded");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Room creation failed with error code {0} and error message {1}", returnCode, message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        join.SetActive(true);
        Debug.LogErrorFormat("Joining room failed with error code {0} and error message {1}", returnCode, message);
        logs.text = "Room doesn't exist. Try again.";
        onJoinName = "";
    }

    //Naming algorithm
    private void NamePlayers(String roomName)
    {
        StringBuilder newName = new StringBuilder();
        newName.Append(roomName);
        if (roomCreator)
            newName.Append("-Master");
        else
            newName.Append("-Dweller" + RandomString(4));
        PhotonNetwork.NickName = newName.ToString();
    }

    private string RandomString(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }

    private bool TestName(string inputText)
    {
        if (String.IsNullOrEmpty(inputText))
        {
            return false;
        }
        foreach (char c in inputText)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
        }
        return true;
    }
}