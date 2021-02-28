using System;
using UnityEngine;

[Serializable]
public class Clock
{
    private int minutes;
    private float seconds;

    public Clock() {
        seconds = 0;
        minutes = 0;
    }

    public void Update() {
        seconds += Time.deltaTime;
        if(seconds >= 60){
             seconds = 0;
             minutes++;
        }
    }

    public override string ToString() {
        string minutesString = (minutes < 10? "0" : "") + minutes; 
        string secondsString = (seconds < 10? "0" : "") + (int) seconds;
        return minutesString + ":" + secondsString; 
    }
}