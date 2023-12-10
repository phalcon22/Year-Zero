using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    [SerializeField]
    private GameObject pseudo;
    [SerializeField]
    private GameObject video;
    [SerializeField]
    private GameObject sound;
    [SerializeField]
    GameObject gameplay;

    [SerializeField]
    Text version;

    void Start()
    {
        version.text = "Version " + Application.version;
    }

    public void ChangePseudoMenu()
    {
        pseudo.SetActive(true);
        pseudo.GetComponent<PseudoInputField>().CheckPseudo();
        gameObject.SetActive(false);
    }

    public void VideoMenu()
    {
        video.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SoundMenu()
    {
        sound.SetActive(true);
        gameObject.SetActive(false);
    }

    public void GameplayMenu()
    {
        gameplay.SetActive(true);
        gameObject.SetActive(false);
    }
}
