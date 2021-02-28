using static utils.Constants;
using static utils.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NavigationController : MonoBehaviour
{
    public GameObject homeWindow, refreshWindow, footer, footerBar;

    public void Start() {
        // Adjusting the footer to match the wide bottom of the device screen
        float screenRatio = (float) Screen.height / (float) Screen.width;
        float scaleFator = Math.Max(1, 2 / screenRatio);
        footerBar.transform.localScale = new Vector3(scaleFator, 1, 1);
        float deltaY = Math.Min(-(screenRatio / 2f - 1f) * 540f, 0);
        footer.transform.position += new Vector3(0, deltaY, 0);   
    }

    /// <summary>
    /// Exits the match, loading the menu scene.
    /// </summary>
    public void GoToMenu() { 
        SceneManager.LoadScene(MenuScene);
    }

    /// <summary>
    /// Refreshes the game scene in order to start a new match.
    /// </summary>
    public void Refresh() { 
        SceneManager.LoadScene(MatchScene + PlayerPrefs.GetInt("boardStyle", DefaultBoardStyle));
    }

    /// <summary>
    /// Loads a scene containing information/guidance about the game and rules.
    /// </summary>
    public void ShowAbout() { 
        PlayerPrefs.SetInt("lastAdditiveScene", AboutScene);
        SceneManager.LoadScene(AboutScene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Loads the settings scene.
    /// </summary>
    public void OpenSettings() { 
        PlayerPrefs.SetInt("lastAdditiveScene", SettingsScene);
        SceneManager.LoadScene(SettingsScene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Shows a pop-up window asking the user if they are sure about going to the main menu.
    /// </summary>
    public void OnHomeClick() {
        refreshWindow.SetActive(false);
        homeWindow.SetActive(!homeWindow.activeSelf);
    }

    /// <summary>
    /// Shows a pop-up window asking the user if they are sure about refreshing the match.
    /// </summary>
    public void OnRefreshClick() {
        homeWindow.SetActive(false);
        refreshWindow.SetActive(!refreshWindow.activeSelf);
    }
}


