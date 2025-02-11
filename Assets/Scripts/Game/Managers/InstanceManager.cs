﻿using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class InstanceManager : MonoBehaviourPunCallbacks {

    #region Singleton

    public static InstanceManager instanceManager;
    public bool offlineMode;
    public bool debugMode;
    public bool noCosts;
    public bool instantInstantiation;

    public void Awake()
    {
        instanceManager = this;
        if (offlineMode)
            PhotonNetwork.OfflineMode = offlineMode;
    }

    #endregion

    protected int race;
    protected int team;
    protected int color;

    string botPrefab = "IA/BotPrefab";

    protected string[] townhalls = new string[2] { "Buildings/TownHall/TownHall", "Buildings/TownHall/TownHall" };
    protected string[] builders = new string[2] { "Units/Builder", "Units/Builder" };

    protected int botIndex;

    [SerializeField] GameObject audioManagerPrefab;
    void Start()
    {
        
        bulletHolder = GameObject.Find("BulletsHolder").transform;
        botIndex = -1;
        InitStartingTroops(InitProp());
        if (PhotonNetwork.IsMasterClient)
            InitBots();
        if (SceneManager.GetActiveScene().name == "GameTest" || SceneManager.GetActiveScene().name == "NoMansSpace")
        {
            if (FindFirstObjectByType<AudioManager>() == null)
            {
                Instantiate(audioManagerPrefab);
            }
            FindFirstObjectByType<AudioManager>().PlayRandomSound(new []{"UniverseMusic","09. Genesis","06. Spatial Lullaby"} );
        }
    }

    public float timer { get; private set; }
    void Update()
    {
        timer += Time.deltaTime;
    }

    void InitBots()
    {
        int i = 1;
        Hashtable myTable = PhotonNetwork.CurrentRoom.CustomProperties;
        while (myTable.ContainsKey("Bot" + i + "Race"))
        {
            IAManager bot = PhotonNetwork.Instantiate(botPrefab, Vector3.zero, Quaternion.identity).GetComponent<IAManager>();
            bot.gameObject.name = "Bot" + i;
            bot.Init(i, (int)myTable["Bot" + i + "Race"], (int)myTable["Bot" + i + "Team"], (int)myTable["Bot" + i + "Color"], (Vector3)myTable["Bot" + i + "MyCoords"]);
            i++;
        }
        if (debugMode)
        {
            IAManager bot = Instantiate((GameObject)Resources.Load(botPrefab)).GetComponent<IAManager>();
            bot.gameObject.name = "Bot0";
            bot.Init(0, 1, 1, 1, new Vector3 (15, 1, 15));
        }
    }

    Vector3 InitProp()
    {
        Vector3 myCoords;
        if (!PhotonNetwork.OfflineMode && SceneManager.GetActiveScene().name != "Tutorial" || (SceneManager.GetActiveScene().name == "GameTest" || SceneManager.GetActiveScene().name == "NoMansSpace") && !offlineMode)
        {
            myCoords = (Vector3)PhotonNetwork.CurrentRoom.CustomProperties["Player" + PhotonNetwork.LocalPlayer.ActorNumber + "MyCoords"];

            Hashtable customProp = PhotonNetwork.CurrentRoom.CustomProperties;
            race = (int)customProp["Player" + PhotonNetwork.LocalPlayer.ActorNumber + "Race"];
            team = (int)customProp["Player" + PhotonNetwork.LocalPlayer.ActorNumber + "Team"];
            color = (int)customProp["Player" + PhotonNetwork.LocalPlayer.ActorNumber + "Color"];
        }
        else
        {
            if(SceneManager.GetActiveScene().name == "Mission2")
                myCoords = new Vector3(-40,0,-40);
            else
                myCoords = new Vector3(0, 0, 0);
            race = 0;
            team = 0;
            color = 0;
        }
        return myCoords;

    }

    void InitStartingTroops(Vector3 coords)
    {
        GameObject tmp = InstantiateUnit(townhalls[race], new Vector3(coords.x + 2, 0.5f, coords.z + 2), Quaternion.Euler(0, 0, 0), -1);
        PlayerManager.playerManager.AddHome(tmp.GetComponent<TownHall>());
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            PlayerManager.playerManager.Pay(new int[] { 0, 0, 0, 0 }, 4, false);
            InstantiateUnit(builders[race], new Vector3(coords.x, 0.5f, coords.z), Quaternion.Euler(0, 0, 0), -1);
            InstantiateUnit(builders[race], new Vector3(coords.x+1, 0.5f, coords.z+1), Quaternion.Euler(0, 0, 0), -1);
            InstantiateUnit(builders[race], new Vector3(coords.x, 0.5f, coords.z+1), Quaternion.Euler(0, 0, 0), -1);
            InstantiateUnit(builders[race], new Vector3(coords.x+1, 0.5f, coords.z), Quaternion.Euler(0, 0, 0), -1);
        }

        Camera.main.GetComponent<CameraController>().LookTo(PlayerManager.playerManager.GetHomes()[0].transform.position);
    }

    public void CheckWin()
    {
        if (GetNearestEnemy() == null)
        {
            GameObject.Find("WinScreen").GetComponent<WinScreen>().Show();
        }
    }

    [PunRPC]
    public void RPCCheckWin()
    {
        CheckWin();
    }

    public void CheckDeath()
    {
        if (mySelectableObjs.Count == 0)
        {
            ShowDeath();
        }
        else if (SceneManager.GetActiveScene().name == "Mission" || SceneManager.GetActiveScene().name == "EndlessMode")
        {
            bool townhall = false;
            for (int i = 0; i < mySelectableObjs.Count && !townhall; i++)
            {
                if (mySelectableObjs[i].GetComponent<TownHall>() != null)
                    townhall = true;
            }
            if (!townhall)
            {
                ShowDeath();
            }
        }
    }

    void ShowDeath()
    {
        GameObject.Find("DeathScreen").GetComponent<DeathScreen>().Show();
    }

    public List<SelectableObj> allSelectableObjs = new List<SelectableObj>();
    public List<SelectableObj> mySelectableObjs = new List<SelectableObj>();
    public List<ResourceUnit> allResourceUnits = new List<ResourceUnit>();

    public GameObject InstantiateUnit(string prefab, Vector3 pos, Quaternion rot, int botIndex)
    {
        if (botIndex == -1)
        {
            GameObject obj = PhotonNetwork.Instantiate(prefab, pos, rot);
            obj.GetComponent<SelectableObj>().InitUnit(-1);
            return obj;
        }
        else
        {
            return GetBot(botIndex).InstantiateUnit(prefab, pos, rot);
        }

    }

    #region Network

    public override void OnLeftRoom()
    {
        Debug.Log("Leave room");
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.Disconnect();
    }

    #endregion

    public int GetTeam()
    {
        return team;
    }

    public int GetRace()
    {
        return race;
    }

    public Color32 GetColor()
    {
        return MultiplayerTools.Int2Color(color);
    }

    public Color32 GetPlayerColor(Player player)
    {
        if (player == PhotonNetwork.LocalPlayer)
        {
            return MultiplayerTools.Int2Color(color);
        }
        return MultiplayerTools.Int2Color((int)PhotonNetwork.CurrentRoom.CustomProperties["Player" + player.ActorNumber + "Color"]);
    }

    public Color32 GetBotColor(int index)
    {
        return GameObject.Find("Bot" + index).GetComponent<IAManager>().GetColor();
    }

    protected int colorLevel = 1;
    public void ChangeColorLevel()
    {
        if (++colorLevel > 2)
            colorLevel = 0;

        foreach (SelectableObj obj in allSelectableObjs)
        {
            obj.ToggleColor(colorLevel);
        }
    }

    public bool IsEnemy(SelectableObj unit)
    {
        return MultiplayerTools.GetTeamOf(unit) != team;
    }

    public void AllSelectableRemoveAt(int i)
    {
        if (PhotonNetwork.OfflineMode)
            return;
        photonView.RPC("RPCAllSelectableRemoveAt", RpcTarget.Others, i);
    }

    [PunRPC]
    public void RPCAllSelectableRemoveAt(int i)
    {
        allSelectableObjs.RemoveAt(i);
    }

    public IAManager GetBot(int index)
    {
        return GameObject.Find("Bot" + index).GetComponent<IAManager>();
    }

    public void AllResourceUnitsRemoveAt(int i)
    {
        if (PhotonNetwork.OfflineMode)
            return;
        photonView.RPC("RPCAllResourceUnitsRemoveAt", RpcTarget.Others, i);
    }

    [PunRPC]
    public void RPCAllResourceUnitsRemoveAt(int i)
    {
        allResourceUnits.RemoveAt(i);
    }

    public Transform bulletHolder { get; private set; }







    public DestructibleUnit GetNearestEnemy()
    {
        DestructibleUnit res = null;
        List<Holder> enemies = GetEnemyHolders();
        Vector3 myPos = PlayerManager.playerManager.GetHomes()[0].transform.position;

        foreach (Holder enemy in enemies)
        {
            DestructibleUnit tmp = GetNearestBuildingOf(enemy);
            if (tmp == null) tmp = GetNearestTroopOf(enemy);
            if (res == null || Vector3.Distance(myPos, tmp.transform.position) > Vector3.Distance(myPos, res.transform.position))
                if (tmp != null)
                    res = tmp.GetComponent<DestructibleUnit>();
        }
        return res;
    }

    public DestructibleUnit GetNearestBuildingOf(Holder x)
    {
        DestructibleUnit res = null;
        Transform buildings = x.transform.GetChild(0).Find("Buildings");
        Vector3 myPos = PlayerManager.playerManager.GetHomes()[0].transform.position;

        foreach (Transform child in buildings)
        {
            if (res == null || Vector3.Distance(myPos, child.position) > Vector3.Distance(myPos, res.transform.position))
                res = child.GetComponent<DestructibleUnit>();
        }
        return res;
    }

    public DestructibleUnit GetNearestTroopOf(Holder x)
    {
        DestructibleUnit res = null;
        Transform buildings = x.transform.GetChild(0).Find("Movable");
        Vector3 myPos = PlayerManager.playerManager.GetHomes()[0].transform.position;

        foreach (Transform child in buildings)
        {
            if (res == null || Vector3.Distance(myPos, child.position) > Vector3.Distance(myPos, res.transform.position))
                res = child.GetComponent<DestructibleUnit>();
        }
        return res;
    }

    public List<Holder> GetEnemyHolders()
    {
        List<Holder> enemies = new List<Holder>();

        if (!PhotonNetwork.OfflineMode)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (GameObject.Find("Holder" + player.NickName).GetComponent<Holder>().team != GetTeam())
                {
                    enemies.Add(GameObject.Find("Holder" + player.NickName).GetComponent<Holder>());
                }
            }
        }

        for (int i = 1; i <= PlayerPrefs.GetInt("BotNumber"); i++)
        {
            if (GameObject.Find("Holder" + i).GetComponent<Holder>().team != GetTeam())
                enemies.Add(GameObject.Find("Holder" + i).GetComponent<Holder>());
        }
        return enemies;
    }
}
