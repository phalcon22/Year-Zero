using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateGameMenu : MonoBehaviour {

    private string gameName;
    private string mapScene;

    private int maxPlayer;

    [SerializeField]
    private MapList maps;

    [SerializeField]
    private GameObject mapButton;

    [SerializeField]
    private InputField gameNameText;
    [SerializeField]
    private Dropdown maxPlayerDropdown;
    [SerializeField]
    private Button createGameButton;
    [SerializeField]
    private GameObject selectionBox;
    [SerializeField]
    private Transform scrollViewContent;

    [SerializeField]
    GameObject gameNameObj;
    [SerializeField]
    GameObject maxPlayerObj;

    [SerializeField]
    TemporaryMenuMessage noMap;
    [SerializeField]
    TemporaryMenuMessage noName;

    Transform selectedMapButton;

    #region INIT

    void Awake()
    {
        InitMaxPlayerDropdown();
        InitMapButtons();
    }

    void OnEnable()
    {
        InitOffline();
        SelectMap(scrollViewContent.GetChild(0));
    }

    void InitOffline()
    {
        gameNameObj.SetActive(!PhotonNetwork.OfflineMode);
        maxPlayerObj.SetActive(!PhotonNetwork.OfflineMode);
        if (!PhotonNetwork.OfflineMode)
            gameNameText.ActivateInputField();
    }

    void InitMapButtons()
    {
        foreach (MapList.Map map in maps.maps)
        {
            GameObject obj = Instantiate(mapButton, scrollViewContent);
            obj.GetComponentInChildren<Text>().text = map.name;
        }
    }

    void InitMaxPlayerDropdown()
    {
        List<string> tmp = new List<string>();
        for (int i = 2; i <= 8; i++)
        {
            tmp.Add(i.ToString());
        }
        maxPlayerDropdown.AddOptions(tmp);
        maxPlayerDropdown.value = maxPlayerDropdown.options.Count - 1;
        maxPlayer = int.Parse(maxPlayerDropdown.options[^1].text);
    }

    #endregion

    void Update()
    {
        if (selectedMapButton != null)
        {
            selectionBox.transform.position = new Vector3(selectionBox.transform.position.x, selectedMapButton.position.y, selectionBox.transform.position.z);
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            TryCreateGame();
        }
    }

    public void TryCreateGame()
    {
        if (mapScene == null)
        {
            noMap.Activate();
        }
        else if (!PhotonNetwork.OfflineMode && (gameName == string.Empty || gameName == null))
        {
            noName.Activate();
        }
        else
            CreateGame();
     }

    public void SetGameName(string value)
    {
        gameName = value;
    }

    public void SetMaxPlayer(int value)
    {
        maxPlayer = value + 2;
    }

    void CreateGame()
    {
        PlayerPrefs.SetString("MapScene", mapScene);
        PlayerPrefs.SetInt("MaxPlayers", maxPlayer);
        PhotonNetwork.CreateRoom(gameName, new RoomOptions { MaxPlayers = Mathf.Min(maxPlayer, 4) });
    }

    public void SelectMap(Transform button)
    {
        selectedMapButton = button;
        mapScene = maps.GetSceneName(button.GetComponentInChildren<Text>().text);
        selectionBox.SetActive(true);
        selectionBox.transform.position = new Vector3(selectionBox.transform.position.x, button.position.y, selectionBox.transform.position.z);
        if (!PhotonNetwork.OfflineMode)
            gameNameText.ActivateInputField();
    }

    [SerializeField] GameObject multiplayerMenu;
    [SerializeField] GameObject singleplayerMenu;
    public void Back()
    {
        if (!PhotonNetwork.OfflineMode)
            multiplayerMenu.SetActive(true);
        else
            singleplayerMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
