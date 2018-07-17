using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour {

	public void StartHost()
    {
        NetworkManager.singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.singleton.networkAddress = GameObject.Find("IP").GetComponent<InputField>().text;
        NetworkManager.singleton.StartClient();
    }

    public void StopHost()
    {
        NetworkManager.singleton.StopHost();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OffLineSet()
    {
        GameObject.Find("Host").GetComponent<Button>().onClick.AddListener(StartHost);
        GameObject.Find("Client").GetComponent<Button>().onClick.AddListener(StartClient);
        Debug.Log("OffLine...\n");
    }

    public void OnLineSet()
    {
        GameObject.Find("ReturnButton").GetComponent<Button>().onClick.AddListener(StopHost);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 0:
                //OffLineSet();
                break;
            case 1:
                OnLineSet();
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
