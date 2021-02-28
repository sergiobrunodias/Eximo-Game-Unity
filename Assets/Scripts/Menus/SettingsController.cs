using static utils.Constants;
using static utils.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour {

    // Sliders, toggles
    public GameObject moveTimeSlider, menuSoundToggle, gameSoundToggle;
    // Dropdowns
    public GameObject difficultyDropdown, highlightStyleDropdown, numPieces1, numPieces2;
    // Warning message
    public GameObject warningMessage;
    // Board style selection
    public GameObject[] boardStyles;
    // UI Sound
    public AudioSource uISound;

    public void Start() {
        PlayUIAudioIfEnabled(uISound);
        /* Displays the current settings on each selector */
        difficultyDropdown.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("difficulty", DefaultDifficulty);
        highlightStyleDropdown.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("highlightStyle", DefaultHighlightStyle);
        moveTimeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("moveTime", DefaultMoveTime);
        menuSoundToggle.GetComponent<Slider>().value = PlayerPrefs.GetFloat("menuSound", DefaultMenuSound);
        gameSoundToggle.GetComponent<Slider>().value = PlayerPrefs.GetFloat("gameSound", DefaultGameSound);
        numPieces1.GetComponent<TMP_Dropdown>().value = MapNumPiecesToOptValue(PlayerPrefs.GetInt("numPiecesPlayer1", DefaultNumPieces)); 
        numPieces2.GetComponent<TMP_Dropdown>().value = MapNumPiecesToOptValue(PlayerPrefs.GetInt("numPiecesPlayer2", DefaultNumPieces)); 
        ShowSelectedBoardStyle();
        warningMessage.SetActive(false);
    }

    /// <summary>
    /// Sets the move time delay.
    /// </summary>
    /// <param name="moveTime">Delay time in seconds</param>
    /// <remarks>
    /// Handler for slider (set up via inspector).
    /// </remarks>
    public void SetMoveTime(float moveTime) {
        PlayerPrefs.SetFloat("moveTime", moveTime);
    }

    /// <summary>
    /// Sets the number of initial pieces for the first player.
    /// </summary>
    /// <param name="value">Value of the pressed button</param>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void SetNumPiecesPlayer1(int value) {
        int numPieces = MapOptValueToNumPieces(value);
        PlayerPrefs.SetInt("numPiecesPlayer1", numPieces);
        warningMessage.SetActive(true);
    }

    /// <summary>
    /// Sets the number of initial pieces for the second player.
    /// </summary>
    /// <param name="value">Value of the pressed button</param>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void SetNumPiecesPlayer2(int value) {
        int numPieces = MapOptValueToNumPieces(value);
        PlayerPrefs.SetInt("numPiecesPlayer2", numPieces);
        warningMessage.SetActive(true);
    }

    /// <summary>
    /// Sets the difficulty for the singleplayer mode.
    /// </summary>
    /// <param name="value">Difficulty id</param>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void SetDifficulty(int value) {
        PlayerPrefs.SetInt("difficulty", value);
        warningMessage.SetActive(true);
    }

    /// <summary>
    /// Sets the highlight style for valid moves.
    /// </summary>
    /// <param name="value">Difficulty id</param>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void SetHightlightStyle(int value) {
        PlayerPrefs.SetInt("highlightStyle", value);
    }

    /// <summary>
    /// Sets the board style.
    /// </summary>
    /// <param name="value">Style id</param>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void SetBoardStyle(int value) {
        PlayerPrefs.SetInt("boardStyle", value);
        ShowSelectedBoardStyle();
        warningMessage.SetActive(true);
    }

    /// <summary>
    /// Draws a line around the current selected board style.
    /// </summary>
    public void ShowSelectedBoardStyle() {
        int selectedStyle = PlayerPrefs.GetInt("boardStyle", DefaultBoardStyle);
        for(int boardStyle = 0; boardStyle < boardStyles.Length; boardStyle++) {
            boardStyles[boardStyle].SetActive(selectedStyle == boardStyle);
        }
    }

    /// <summary>
    /// Toggles the menu sound on or off.
    /// </summary>
    /// <param name="value">Toggle value</param>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void ToggleMenuSound(float value) {
        PlayerPrefs.SetFloat("menuSound", value);
    }

    /// <summary>
    /// Toggles the game sound on or off.
    /// </summary>
    /// <param name="value">Toggle value</param>
    /// <remarks>
    /// Handler for button (set up via inspector).
    /// </remarks>
    public void ToggleGameSound(float value) {
        PlayerPrefs.SetFloat("gameSound", value);
    }
    
}