using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Collections;

public class WinnerToLobby : MonoBehaviourPunCallbacks
{
    void Start()
    {
        StartCoroutine(Waiting());
        PhotonNetwork.ConnectUsingSettings();
    }

    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(15f);
    }

    public override void OnConnectedToMaster()
    {
        // Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
