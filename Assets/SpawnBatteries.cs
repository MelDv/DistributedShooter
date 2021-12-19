using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBatteries : MonoBehaviour
{
    public GameObject batteryPrefab;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;

    private void Start()
    {
        StartCoroutine(batterySpawner());
    }

    private IEnumerator batterySpawner()
    {
        yield return new WaitForSeconds(2f);
        // Create batteries in random locations
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(batteryPrefab.name, randomPosition, Quaternion.identity);
    }
}
