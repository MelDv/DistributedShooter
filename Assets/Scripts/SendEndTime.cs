using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Globalization;
using Photon.Pun;

public class SendEndTime : MonoBehaviour
{
    private string gameDataURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfbfbAE94xjXdLvtGggdgVrFi1lm1Yrv3ozDNd8K7zvga8o5w/formResponse";

    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {
        StartCoroutine(UploadForm());
    }

    void OnApplicationQuit()
    {
        StartCoroutine(UploadForm());
    }

    IEnumerator UploadForm()
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.2033011394", DateTime.Now.ToString());

        // Add player ID to form.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        int viewID = player.GetComponent<PhotonView>().ViewID;
        form.AddField("entry.924040491", ("Player ID: " + viewID));

        // Upload to a cgi script
        using (var w = UnityWebRequest.Post(gameDataURL, form))
        {
            yield return w.SendWebRequest();
            if (w.result != UnityWebRequest.Result.Success)
            {
                print(w.error);
            }
            else
            {
                print("End time uploaded.");
            }
        }
    }
}
