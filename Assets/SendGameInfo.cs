using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Photon.Pun;
using System;

public class SendGameInfo : MonoBehaviour
{
    private WWWForm form;
    private GameObject player;
    private static string lobbyAction = "";
    private static string playerId = "";
    private static string roomId = "";
    private int viewID;
    // Form address for saving player data.
    // private string gameDataURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfbfbAE94xjXdLvtGggdgVrFi1lm1Yrv3ozDNd8K7zvga8o5w/formResponse";
    private string gameDataURL = "http://128.214.253.86/api";

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // Currently not sending the viewID but Unity's GetInstanceID, which is shown on screen also. Change if necessary.
        viewID = player.GetComponent<PhotonView>().ViewID;

        // get lobby action ("create" or "join")
        lobbyAction = CreateAndJoinRooms.lobbyAction;

        StartCoroutine(SendStartInfo());
        StartCoroutine(Ping());
    }

    private IEnumerator Ping()
    {
        while (true)
        {
            // yield return new WaitForSeconds(0.5f); // ping every half second
            yield return new WaitForSeconds(1f); // ping every second

            if (playerId != "") // don't ping until playerId has been set
            {
                var pingUrl = gameDataURL + "/pingPlayer/" + playerId;

                // create form with current location
                form = new WWWForm();
                form.AddField("x", player.transform.position.x.ToString("0"));
                form.AddField("z", player.transform.position.z.ToString("0"));

                using (var w = UnityWebRequest.Post(pingUrl, form))
                {
                    yield return w.SendWebRequest();
                    if (w.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("*** Ping failed." + w.result);
                    }
                    // else
                    // {
                    //     Debug.Log("*** Ping succeeded.");
                    // }
                }
            }
        }
    }

    private IEnumerator SendStartInfo()
    {
        form = new WWWForm();

        // Add room and player names to form.
        form.AddField("name", PhotonNetwork.CurrentRoom.Name);
        form.AddField("playerName", PhotonNetwork.NickName);

        // set endpoint based on lobby action
        var endpoint = "";
        if (lobbyAction == "create")
        {
            endpoint = "/createRoom";
        }
        else
        {
            endpoint = "/joinRoom";
        }
        print("*** SendStartInfo: endpoint = " + endpoint);

        using (var w = UnityWebRequest.Post(gameDataURL + endpoint, form))
        {
            yield return w.SendWebRequest();
            if (w.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("*** Sending start information failed." + w.result);
            }
            else
            {
                JSONObject responseJson = new JSONObject(w.downloadHandler.text);
                playerId = responseJson.GetField("playerId").str;
                roomId = responseJson.GetField("roomId").str;
                print("*** Successfully completed " + lobbyAction + " room action.");
                print("*** playerId = " + playerId);
                print("*** roomId = " + roomId);
            }
        }
    }
}