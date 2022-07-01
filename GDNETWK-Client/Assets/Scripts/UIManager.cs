using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField]
    private GameObject startMenu;
    public InputField usernameField;

    public InputField ipField;

    public InputField portField;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    /// <summary>Attempts to connect to the server.</summary>
    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        ipField.interactable = false;
        Client.Instance.ip = ipField.text;
        Client.Instance.port = int.Parse(portField.text);
        Client.Instance.ConnectToServer();  
    }
}
