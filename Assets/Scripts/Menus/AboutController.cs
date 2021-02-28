using static utils.Constants;
using static utils.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class AboutController : MonoBehaviour {

    // UI Sound
    public AudioSource uISound;

    public void Start() {
        PlayUIAudioIfEnabled(uISound);
    }

    /// <summary>
    /// Opens the game reference in the browser.
    /// </summary>
    /// <remarks>
    /// Handler for button/hyperlink (set up via inspector).
    /// </remarks>
    public void LoadGameWebsite() {
        Application.OpenURL("https://boardgamegeek.com/boardgame/137916/eximo");
    }
}