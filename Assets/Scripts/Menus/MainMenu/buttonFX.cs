using System.Collections;
using UnityEngine;

public class buttonFX : MonoBehaviour
{
    public AudioSource myFX;
    public AudioClip hoverFX;
    public AudioClip clickFX;

    public void HoverSound()
    {
        myFX.PlayOneShot(hoverFX);
    }

    public void ClickSound()
    {
        if (clickFX != null)
            myFX.PlayOneShot(clickFX);
    }
}
