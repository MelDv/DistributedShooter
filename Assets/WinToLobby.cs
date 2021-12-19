using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinToLobby : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Waiting();
    }

    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(15f);
        SceneManager.LoadScene("Lobby");
    }
}
