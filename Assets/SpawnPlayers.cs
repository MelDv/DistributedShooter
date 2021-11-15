using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Net.NetworkInformation;



public class SpawnPlayers : MonoBehaviour
{
    // Form for saving player data.
    private string gameDataURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfbfbAE94xjXdLvtGggdgVrFi1lm1Yrv3ozDNd8K7zvga8o5w/formResponse";
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;

    private void Start()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        StartCoroutine(UploadForm());
    }

    IEnumerator UploadForm()
    {
        WWWForm form = new WWWForm();

        // Add start time to form.
        form.AddField("entry.1338148533", System.DateTime.Now.ToString());

        // Add player ID to form.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        int viewID = player.GetComponent<PhotonView>().ViewID;
        form.AddField("entry.924040491", ("Player ID: " + viewID));

        // Add room name to form.
        form.AddField("entry.953771128", PhotonNetwork.CurrentRoom.Name);

        using (var w = UnityWebRequest.Post(gameDataURL, form))
        {
            yield return w.SendWebRequest();
            if (w.result != UnityWebRequest.Result.Success)
            {
                print(w.error);
            }
            else
            {
                print("Start time uploaded.");
            }
        }
    }
}
