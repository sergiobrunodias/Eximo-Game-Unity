
using static utils.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour {

    public Animator transition;

    public void Start() {
        StartCoroutine(LoadMenu());
    }

    /// <summary>
    /// Loads the main menu after a 1 second transition.
    /// </summary>
    public IEnumerator LoadMenu() {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(MenuScene);
    }
}