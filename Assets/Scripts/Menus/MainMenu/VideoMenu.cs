using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoMenu : MonoBehaviour {

    bool ready;
    [SerializeField]
    private Dropdown quality;
    [SerializeField]
    private Toggle fullscreen;
    [SerializeField]
    private Dropdown resolutionDropdown;

    private List<Resolution> resolutions = new();

    void OnEnable()
    {
        ready = false;
        InitResolutionDropDown();
        quality.value = QualitySettings.GetQualityLevel();
        fullscreen.isOn = Screen.fullScreen;
    }

    private void InitResolutionDropDown()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            Resolution cur = Screen.resolutions[i];

            if (Mathf.RoundToInt(cur.width / (float)cur.height * 100) != 178)
                continue;

            string tmp = cur.width + " x " + cur.height + " " +
                Mathf.Round((float)cur.refreshRateRatio.value) + "Hz";
            options.Add(tmp);

            if (cur.width == Screen.width && cur.height == Screen.height)
            {
                currentResolutionIndex = resolutions.Count;
            }

            resolutions.Add(cur);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        ready = true;
    }

    public void SetResolution(int index)
    {
        if (ready)
        {
            Resolution tmp = resolutions[index];
            Screen.SetResolution(tmp.width, tmp.height, Screen.fullScreen);
        }
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen(bool val)
    {
        Screen.fullScreen = val;
    }
}
