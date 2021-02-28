using static utils.Constants;
using static utils.Utils;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class NavigationBar : MonoBehaviour {

    public GameObject nagivationBar;
    public AudioSource uISound;

    public void Start() {
        float screenRatio = (float) Screen.height / (float) Screen.width;
        float scaleFator = Math.Max(1, 2 / screenRatio);
        nagivationBar.transform.localScale = new Vector3(scaleFator, 1, 1);
    }

    /// <summary>
    /// Loads the previous scene.
    /// </summary>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void GoBack() { 
        PlayUIAudioIfEnabled(uISound);
        DontDestroyOnLoad(uISound);
        SceneManager.UnloadSceneAsync(PlayerPrefs.GetInt("lastAdditiveScene"));
    }
}