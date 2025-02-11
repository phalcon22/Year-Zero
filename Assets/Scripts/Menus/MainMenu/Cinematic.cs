﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Cinematic : MonoBehaviour
{
    [SerializeField] GameObject rawImage;
    [SerializeField] VideoPlayer player;
    [SerializeField] AudioManager music;

    bool started;

    private void Start()
    {
        started = false;
        player.loopPointReached += EndReached;
        if (!PlayerPrefs.HasKey("Cinematic") || PlayerPrefs.GetInt("Cinematic") == 0)
        {
            Show();
            PlayerPrefs.SetInt("Cinematic", 1);
        }
    }

    public void Show()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        started = true;
        if (music != null)
            music.Pause(0);
        rawImage.SetActive(true);
        player.Play();
    }

    public void Stop()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        started = false;
        if (music != null)
            music.UnPause(0);
        player.Stop();
        rawImage.SetActive(false);
        if (SceneManager.GetActiveScene().name == "CinematicScene")
        {
            PhotonNetwork.LoadLevel("LoadingScene");
        }
    }

    void EndReached(VideoPlayer vp)
    {
        Stop();
    }

    private void Update()
    {
        if (started && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Return)))
        {
            Stop();
        }
    }
}
