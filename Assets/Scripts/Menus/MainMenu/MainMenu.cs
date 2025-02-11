﻿using UnityEngine;
using UnityEngine.Audio;
using Photon.Pun;
using Photon.Realtime;
using System;

public class MainMenu : MonoBehaviourPunCallbacks {

    const string playerNamePrefKey = "PlayerName";

    string gameVersion = "1";

    bool clickedMulti = false;

    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    GameObject singleplayerMenu;
    [SerializeField]
    private GameObject multiplayerMenu;
    [SerializeField]
    private GameObject optionsMenu;
    [SerializeField]
    private GameObject connection;
    [SerializeField]
    private GameObject createGameMenu;

    [SerializeField]
    AudioMixer audioMixer;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        mainMenu.SetActive(true);
        CheckPseudo();
        CheckGameplay();
        CheckSound();
    }

    void CheckPseudo()
    {
        if (!PlayerPrefs.HasKey(playerNamePrefKey))
        {
            PlayerPrefs.SetString(playerNamePrefKey, Environment.UserName);
        }
        PhotonNetwork.NickName = PlayerPrefs.GetString(playerNamePrefKey);
    }

    void CheckGameplay()
    {
        InitFloatPrefs("camMoveMouseSpeed", 0.5f);
        InitIntPrefs("camMoveMouse", 1);
        InitFloatPrefs("camMoveKeySpeed", 0.5f);
        InitIntPrefs("helpBubble", 1);
    }

    void CheckSound()
    {
        InitFloatPrefs("GeneralAudio", 0.5f);
        InitFloatPrefs("SoundAudio", 0.5f);
        InitFloatPrefs("MusicAudio", 0.5f);

        audioMixer.SetFloat("GeneralVolume", PlayerPrefs.GetFloat("GeneralAudio"));
        audioMixer.SetFloat("SoundVolume", PlayerPrefs.GetFloat("SoundAudio"));
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicAudio"));
    }

    void InitFloatPrefs(string key, float value)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetFloat(key, value);
        }
    }

    void InitIntPrefs(string key, int value)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, value);
        }
    }

    public void Singleplayer()
    {
       if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
        else
        {
            GotoSingleplayerMenu();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.DisconnectByClientLogic)
            GotoSingleplayerMenu();
        else
        {
            connection.SetActive(false);
            mainMenu.SetActive(true);
            GetComponentInChildren<TemporaryMenuMessage>().Activate();
        }
    }

    void GotoSingleplayerMenu()
    {
        PhotonNetwork.OfflineMode = true;
        mainMenu.SetActive(false);
        singleplayerMenu.SetActive(true);
    }

    public void Multiplayer()
    {
        if (!NoInternet())
        {
            clickedMulti = true;
            Connect();
        }
        else
        {
            GetComponentInChildren<TemporaryMenuMessage>().Activate();
        }
    }

    private void Connect()
    {
        if (PhotonNetwork.OfflineMode)
            PhotonNetwork.OfflineMode = false;
        mainMenu.SetActive(false);

        if (PhotonNetwork.IsConnected && !PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        else if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
        {
            JoinOrCreateGameMenu();
        }
        else
        {
            connection.SetActive(true);
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void JoinOrCreateGameMenu()
    {
        connection.SetActive(false);
        mainMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        if (clickedMulti && !PhotonNetwork.OfflineMode)
            PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (clickedMulti && !PhotonNetwork.OfflineMode)
        {
            JoinOrCreateGameMenu();
            clickedMulti = false;
        }
    }

    public void OptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CreateGame()
    {
        multiplayerMenu.SetActive(false);
        createGameMenu.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("WaitingRoom");
        }
    }

    bool NoInternet()
    {
        return (Application.internetReachability == NetworkReachability.NotReachable);
    }

    [SerializeField]
    GameObject creditsMenu;
    public void Credits()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void OpenWebSite()
    {
        Application.OpenURL("https://enguerrandvie.wixsite.com/yearzero/avancement");
    }
}