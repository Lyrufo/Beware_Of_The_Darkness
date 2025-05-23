using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public static ResolutionManager Instance;

    [Header("Valeurs par défaut")]
    public int defaultWidth = 1920;
    public int defaultHeight = 1080;
    public bool defaultFullscreen = true;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ApplySavedResolution();
    }

    public void ApplySavedResolution()
    {
        int width = PlayerPrefs.GetInt("resolutionWidth", defaultWidth);
        int height = PlayerPrefs.GetInt("resolutionHeight", defaultHeight);
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", defaultFullscreen ? 1 : 0) == 1;

        Screen.SetResolution(width, height, isFullscreen);
    }

    public void SaveResolution(int width, int height, bool fullscreen)
    {
        PlayerPrefs.SetInt("resolutionWidth", width);
        PlayerPrefs.SetInt("resolutionHeight", height);
        PlayerPrefs.SetInt("fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
