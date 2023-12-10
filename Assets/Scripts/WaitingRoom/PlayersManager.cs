using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayersManager : MonoBehaviourPunCallbacks {

    Hashtable customProp;
    [SerializeField]
    Button startButton;
    [SerializeField]
    Toggle readyToggle;
    [SerializeField]
    GameObject addBotButton;
    [SerializeField]
    Transform playersList;

    [SerializeField]
    List<Vector3> topLeft;
    [SerializeField]
    List<Vector3> bottomLeft;
    [SerializeField]
    List<Vector3> topRight;
    [SerializeField]
    List<Vector3> bottomRight;
    List<Vector3>[] coords;

    [SerializeField]
    TemporaryMenuMessage notReady;
    [SerializeField]
    TemporaryMenuMessage lobbyFull;
    [SerializeField]
    Text playerAmountText;
    [SerializeField]
    GameObject waittingMessage;
    [SerializeField]
    Text waittingDots;
    float timer;

    PlayerSettings playerSettings;

    int maxPlayers;
    int botCount = 0;

    void Start()
    {
        maxPlayers = PlayerPrefs.GetInt("MaxPlayers");
        timer = 0.3f;
        if (PhotonNetwork.IsMasterClient)
            coords = new List<Vector3>[4] { topLeft, bottomLeft , topRight , bottomRight };
        playerSettings = PhotonNetwork.Instantiate("UI/WaitingRoom/PlayerSettingsPrefab", Vector3.zero, Quaternion.identity).GetComponent<PlayerSettings>();
        InitSettings();
        UpdatePlayerAmountText();
    }

    void InitSettings()
    {
        readyToggle.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        addBotButton.SetActive(PhotonNetwork.IsMasterClient);

        customProp = new Hashtable
        {
            { "IsReady", PhotonNetwork.IsMasterClient }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProp);
    }

    public void TryAddBot()
    {
        if (!LobbyFull())
        {
            AddBot();
        }
        else
        {
            lobbyFull.Activate();
        }
    }

    void AddBot()
    {
        botCount++;
        PhotonNetwork.Instantiate("UI/WaitingRoom/BotSettingsPrefab", Vector3.zero, Quaternion.identity);

        if (LobbyFull())
            PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void OnBotRemoved()
    {
        botCount--;
        if (!LobbyFull())
            PhotonNetwork.CurrentRoom.IsOpen = true;
    }

    void Update()
    {
        UpdatePlayerAmountText();
        UpdateWaitingMessage();
        if (PhotonNetwork.IsMasterClient && Input.GetKeyUp(KeyCode.Return))
        {
            TryStartGame();
        }
    }

    bool LobbyFull()
    {
        return GetPlayerCount() >= maxPlayers;
    }

    int GetPlayerCount()
    {
        if (PhotonNetwork.OfflineMode)
            return 1 + botCount;
        else
            return PhotonNetwork.CurrentRoom.PlayerCount + botCount;
    }

    public void TryStartGame()
    {
        if (CheckIsReady())
        {
             StartGame();
        }
        else
        {
            notReady.Activate();
        }
    }

    void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        SetRoomProperties();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("GameReady"))
        {
            string mapName = PlayerPrefs.GetString("MapScene");
            PhotonNetwork.LoadLevel(mapName);
        }
    }

    void SetRoomProperties()
    {
        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        foreach (Transform playerSettings in playersList)
        {
            if (playerSettings.GetComponent<PlayerSettings>() != null)
            {
                PlayerSettings settings = playerSettings.GetComponent<PlayerSettings>();
                string prefix = "Player" + playerSettings.GetComponent<PhotonView>().Owner.ActorNumber.ToString();
                roomProperties.Add(prefix + "Race", settings.raceDropdown.value);
                roomProperties.Add(prefix + "Team", settings.teamDropdown.value);
                roomProperties.Add(prefix + "Color", settings.colorDropdown.value);
                roomProperties.Add(prefix + "MyCoords", SetCoords(settings.teamDropdown.value));
            }
        }
        int i = 1; //Starts at 1 because there is special Bot0 for debug mode
        foreach (Transform playerSettings in playersList)
        {
            if (playerSettings.GetComponent<BotSettings>() != null)
            {
                BotSettings settings = playerSettings.GetComponent<BotSettings>();
                string prefix = "Bot" + i;
                roomProperties.Add(prefix + "Race", settings.raceDropdown.value);
                roomProperties.Add(prefix + "Team", settings.teamDropdown.value);
                roomProperties.Add(prefix + "Color", settings.colorDropdown.value);
                roomProperties.Add(prefix + "MyCoords", SetCoords(settings.teamDropdown.value));
                i++;
            }
        }
        PlayerPrefs.SetInt("BotNumber", i - 1);
        roomProperties.Add("GameReady", true);
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    Vector3 SetCoords(int team)
    {
        int tmp = Random.Range(0, coords[team].Count - 1);
        Vector3 res = coords[team][tmp];
        coords[team].RemoveAt(tmp);
        return res;
    }

    public void ToggleReady(bool val)
    {
        customProp["IsReady"] = val;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProp);

        if (val)
            playerSettings.Lock();
        else
            playerSettings.Unlock();
    }

    bool CheckIsReady()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (!(bool)players[i].CustomProperties["IsReady"])
                return false;
        }
        return true;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Leave room");
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!LobbyFull())
            PhotonNetwork.CurrentRoom.IsOpen = true;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Master Client switched to" + newMasterClient.NickName);
        PhotonNetwork.LeaveRoom();
    }

    void UpdatePlayerAmountText()
    {
        if (playersList == null || !PhotonNetwork.InRoom) return; 
        playerAmountText.text = GetPlayerCount() + "/" + maxPlayers;
    }

    [SerializeField] Canvas cnvs;
    void UpdateWaitingMessage()
    {
        if (playersList == null || !PhotonNetwork.InRoom) return;
        waittingMessage.SetActive(!LobbyFull());
        if (PhotonNetwork.OfflineMode) return;
        UpdateDots();
        UpdateWaitingMessagePos();
    }

    void UpdateDots()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0.3f;
            waittingDots.text = (waittingDots.text.Length == 5) ? "" : new string('.', waittingDots.text.Length + 1);
        }
    }

    void UpdateWaitingMessagePos()
    {
        Vector3 tmp = waittingMessage.transform.position;
        if (playersList.childCount > 0)
            tmp.y = playersList.GetChild(playersList.childCount - 1).position.y - 40 * cnvs.scaleFactor;
        waittingMessage.transform.position = tmp;
    }
}
