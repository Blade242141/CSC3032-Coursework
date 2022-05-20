using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown {
    public string name;
    public float length;
    public bool isFinished;

    public CountDown(string name, float length, bool isFinished) {
        this.name = name;
        this.length = length;
        this.isFinished = isFinished;
    }

    public bool AddTime(float t) {
        if (!isFinished) {
            length += t;
            return true;
        } else {
            return false;
        }
    }

    public void Finished() {
        isFinished = true;
    }

    public void DisableAlarm() {

    }
}
