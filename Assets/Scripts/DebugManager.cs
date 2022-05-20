using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugManager : MonoBehaviour {

    public TMP_Text logTxt;
    public TMP_InputField userInput;

    List<string> log = new List<string> ();
    int lineNo = 0;
    public int lineLimit = 18;

    public GameObject debugPanel;
    public GameObject demoScreen;

    public VoiceRecognition vr;
    public ResourceMonitering rm;

    // Start is called before the first frame update
    void Start() {
        logTxt.text = "";
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown (KeyCode.BackQuote)) {
            debugActive ();
        }
    }

    public void debugActive() {
        debugPanel.SetActive (!debugPanel.activeSelf);
        demoScreen.SetActive (!debugPanel.activeSelf);
        
    }

    public void DebugOut(string entry, string str, bool test, bool latency) {
        if (latency)
            rm.StopLatencyCounter ();
        if (test)
            rm.StopMonitering ();
        logTxt.text = "";
        log.Add(entry + " - " + str + "\n");
        Debug.Log (entry + " - " + str);

        for (int i = 0; i < log.Count; i++) {
            logTxt.text += log [i];
        }

        lineNo++;

        if (log.Count >= lineLimit) {
            log.RemoveAt (0);
            lineNo--;
        }
    }

    public void ClearDebug() {
        log.RemoveRange (0, log.Count);
    }

    public void OnOffBtn() {
        if (vr.playerActive)
            vr.DebugOverride ("system off");
        else
            vr.DebugOverride ("system");
    }

    public void ButtonCommand(string command) {
        vr.DebugOverride (command);
        DebugOut ("btn", command + "\n", false, false);
    }

    public void UInput() {
        DebugOut ("user", userInput.text + "\n", false, false);
        vr.DebugOverride (userInput.text);
        userInput.text = "";
    }
}
