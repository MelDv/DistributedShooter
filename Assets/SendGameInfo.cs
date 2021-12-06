using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Photon.Pun;
using System;

public class SendGameInfo : MonoBehaviour
{
    private WWWForm form;
    private GameObject player;
    private static string playerId = "";
    private static string roomId = "";
    private int viewID;
    // Form address for saving player data.
    // private string gameDataURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfbfbAE94xjXdLvtGggdgVrFi1lm1Yrv3ozDNd8K7zvga8o5w/formResponse";
    private string gameDataURL = "http://128.214.253.86/api";

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // Currently not sendign the viewID but Unity's GetInstanceID, which is shown on screen also. Change if necessary.
        viewID = player.GetComponent<PhotonView>().ViewID;
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
                form.AddField("x", player.transform.position.x.ToString("0.00"));
                form.AddField("y", player.transform.position.y.ToString("0.00"));
                
                using (var w = UnityWebRequest.Post(pingUrl, form))
                {
                    yield return w.SendWebRequest();
                    if (w.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("*** Ping failed.");
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

        // Add room name to form.
        form.AddField("name", PhotonNetwork.CurrentRoom.Name);

        using (var w = UnityWebRequest.Post(gameDataURL + "/createRoom", form))
        {
            yield return w.SendWebRequest();
            if (w.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("*** Sending start information failed.");
            }
            else
            {
                JSONObject responseJson = new JSONObject(w.downloadHandler.text);
                playerId = responseJson.GetField("playerId").str;
                roomId = responseJson.GetField("roomId").str;
                print("*** Successfully created room and player.");
                print("*** playerId = " + playerId);
                print("*** roomId = " + roomId);
            }
        }
    }
}