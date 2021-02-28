using static utils.Constants;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class SliderHandler : MonoBehaviour, IDragHandler, IEndDragHandler {

    // Sliding animation parameters
    private Vector3 panelLocation;
    private float easing = 0.5f;
    public int totalPages;
    private int currentPage = 1;

    public void Start() {
        panelLocation = transform.position;
    }

    /// <summary>
    /// Shifts the scene in right or left direction in response to the sliding movement.
    /// </summary>
    /// <remarks>
    /// Handler for sliding animation.
    /// </remarks>
    public void OnDrag(PointerEventData data) {
        float dragDistance = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(dragDistance, 0, 0);
    }

    /// <summary>
    /// Processes a sliding movement at its end, changing page if necessary.
    /// </summary>
    /// <remarks>
    /// Handler for sliding animation.
    /// </remarks>
    public void OnEndDrag(PointerEventData data) {
        float dragPercentage = (data.pressPosition.x - data.position.x) / Screen.width;
        // If the slide gesture is long enough, the scene is shifted right or left to display the next page.
        if(Mathf.Abs(dragPercentage) >= 0.05) {
            Vector3 newLocation = panelLocation;
            float deltaX = Math.Min(Screen.width, Screen.height/2);
            if(dragPercentage > 0 && currentPage < totalPages) { // Sliding to the left
                currentPage++; 
                newLocation += new Vector3(-deltaX, 0, 0);
            } else if(dragPercentage < 0 && currentPage > 1) { // Sliding to the right
                currentPage--;
                newLocation += new Vector3(deltaX, 0, 0);
            }
            StartCoroutine(ShowSmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        } else {
            StartCoroutine(ShowSmoothMove(transform.position, panelLocation, easing));
        }
    }
    
    /// <summary>
    /// Smoothly turns a page forwards or backwards by incrementally translating the scene.
    /// </summary>
    /// <remarks>
    /// The game mode of the new match should be specified in PlayerPrefs beforehand. By default, it is singleplayer.
    /// </remarks>
    public IEnumerator ShowSmoothMove(Vector3 startpos, Vector3 endpos, float seconds) {
        float t = 0f;
        while(t <= 1.0) {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}