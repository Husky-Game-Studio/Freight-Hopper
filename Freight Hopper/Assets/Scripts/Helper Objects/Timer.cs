using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer {
    public float duration;
    public float current;

    public Timer(float duration) {
        this.duration = duration;
        current = 0f;
    }

    public void resetTimer() {
        current = duration;
    }

    public void deactivateTimer() {
        current = 0f;
    }

    public bool timerActive() {
        return current > 0;
    }

    public void countDown() {
        current -= Time.deltaTime;
    }
}