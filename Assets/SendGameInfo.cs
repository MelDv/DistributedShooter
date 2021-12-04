using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Photon.Pun;
using System;

public class SendGameInfo : MonoBehaviour
{
    private WWWForm form;
    private GameObject player;
    private int viewID;
    // Form address for saving player data.
    private string gameDataURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfbfbAE94xjXdLvtGggdgVrFi1lm1Yrv3ozDNd8K7zvga8o5w/formResponse";

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
            yield return new WaitForSeconds(0.5f);
            form = new WWWForm();
            form.AddField("entry.2033011394", ("Player ID: " + player.GetInstanceID() + ", x: " + player.transform.position.x.ToString("0.00") + ", y: " + player.transform.position.y.ToString("0.00")));
            using (var w = UnityWebRequest.Post(gameDataURL, form))
            {
                yield return w.SendWebRequest();
                if (w.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Sending information failed.");
                }
                else
                {
                    Debug.Log("Sending ping succeeded.");
                }
            }
        }
    }

    private IEnumerator SendStartInfo()
    {
        form = new WWWForm();
        // Add start time to form.
        form.AddField("entry.1338148533", System.DateTime.Now.ToString());

        // Add player ID to form.
        form.AddField("entry.924040491", ("Player ID: " + player.GetInstanceID()));

        // Add room name to form.
        form.AddField("entry.953771128", PhotonNetwork.CurrentRoom.Name);
        using (var w = UnityWebRequest.Post(gameDataURL, form))
        {
            yield return w.SendWebRequest();
            if (w.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Sending information failed.");
            }
            else
            {
                Debug.Log("Sending start information succeeded.");
            }
        }
    }
}
