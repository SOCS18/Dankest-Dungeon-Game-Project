using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    Resolution[] resolutions;
    [SerializeField] private Dropdown resolutionsDropdown;
    [SerializeField] private AudioMixer audioMixer;
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        int currResolutionIndex = 0;
        for( int i = 0; i < resolutions.Length; i++)
        {
            string resoSize = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(resoSize);

            if( resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(resolutionOptions);
        resolutionsDropdown.value = currResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        gameObject.SetActive(true);
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resol = resolutions[resolutionIndex];
        Screen.SetResolution(resol.width, resol.height, Screen.fullScreen);
    }

    public void SetVolume(float volume)
    {
        Debug.Log("Volume changed: " + volume);
        audioMixer.SetFloat("Volume", volume);
    }
   
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
