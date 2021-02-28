using static utils.Constants;
using static utils.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // UI Sound
    public AudioSource uISound;
    // Animation on match start
    public Animator transition;

    /// <summary>
    /// Loads the match scene for a new singleplayer match to start.
    /// </summary>
    /// <remarks>
    /// Handler for the "Play" button (set up via inspector).
    /// </remarks>
    public void Play() {
        PlayUIAudioIfEnabled(uISound);
        PlayerPrefs.SetInt("gameMode", SinglePlayer);
        StartCoroutine(LoadMatch());
    }

    /// <summary>
    /// Loads the match scene for a new multiplayer match to start.
    /// </summary>
    /// <remarks>
    /// Handler for the "Multiplayer" button (set up via inspector).
    /// </remarks>
    public void Multiplayer() {
        PlayUIAudioIfEnabled(uISound);
        PlayerPrefs.SetInt("gameMode", MultiPlayer);
        StartCoroutine(LoadMatch());
    }

    /// <summary>
    /// Smoothly loads the match scene.
    /// </summary>
    public IEnumerator LoadMatch() {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(MatchScene + PlayerPrefs.GetInt("boardStyle", DefaultBoardStyle));
    }

    /// <summary>
    /// Loads the load game menu scene.
    /// </summary>
    /// <remarks>
    /// Handler for the "Load Game" button (set up via inspector).
    /// </remarks>
    public void EnterLoadGameMenu() {
        PlayerPrefs.SetInt("lastAdditiveScene", LoadGameScene);
        SceneManager.LoadScene(LoadGameScene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Loads the settings scene.
    /// </summary>
    /// <remarks>
    /// Handler for the "Settings" button (set up via inspector).
    /// </remarks>
    public void EnterSettings() {
        PlayerPrefs.SetInt("lastAdditiveScene", SettingsScene);
        SceneManager.LoadScene(SettingsScene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Loads the scene containing general information about the game and rules.
    /// </summary>
    /// <remarks>
    /// Handler for the "About" button (set up via inspector).
    /// </remarks>
    public void ShowAbout() { 
        PlayerPrefs.SetInt("lastAdditiveScene", AboutScene);
        SceneManager.LoadScene(AboutScene, LoadSceneMode.Additive);
    }
}
