using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class VoiceRecognition : MonoBehaviour {

    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action> ();

    public PlayerMovement playerMovement;
    public Data data;
    public Music music;

    ResourceMonitering rm;

    public bool playerActive = false;

    string voiceResults;
    string voiceFullResults;

    // Start is called before the first frame update
    void Start() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        LoadCommands ();
    }

    void LoadCommands() {
        keywords.Clear ();

        Command[] commands = Resources.LoadAll <Command>("Commands");
        foreach (var c in commands) {
            if (c.commandType == Command.CommandTypes.movement) {
                Type type = typeof (PlayerMovement);
                MethodInfo info = type.GetMethod (c.name);
                for (int i = 0; i < c.keyword.Length; i++) {
                    keywords.Add (c.keyword [i], () => { info.Invoke (playerMovement, null); });
                }

            } else if (c.commandType == Command.CommandTypes.data) {
                Type type = typeof (Data);
                MethodInfo info = type.GetMethod (c.name);
                for (int i = 0; i < c.keyword.Length; i++) {
                    keywords.Add (c.keyword [i], () => { info.Invoke (data, null); });
                }
            } else if (c.commandType == Command.CommandTypes.music) {
                Type type = typeof (Music);
                MethodInfo info = type.GetMethod (c.name);
                for (int i = 0; i < c.keyword.Length; i++) {
                    keywords.Add (c.keyword [i], () => { info.Invoke (music, null); });
                }
            } else {
                Debug.LogError ("No valid script for command type - " + c.commandType);
            }
        }
    }

    void KeywordRecognized(string keyword, string results) {
        Debug.Log ("Keyword - " + keyword);
        voiceFullResults = results;
        voiceResults = results.Replace(keyword, "");
            if (keywords.TryGetValue (keyword, out Action function)) {
                function.Invoke ();
            }
    }

    public string GetResults() {
        return voiceResults;
    }

    public string GetFullResults() {
        return voiceFullResults;
    }

    public void VoiceResults(string str) {
        voiceResults = "";
        voiceFullResults = "";
        string results = str.ToLower ();
        string[] resultsArr = results.Split (' ');

        for (int i = 0; i < resultsArr.Length; i++) {
            if (keywords.ContainsKey (resultsArr [i])) {
                KeywordRecognized (resultsArr [i], results);
            }
        }        
    }

    public void DebugOverride(string args) {
        VoiceResults (args);
    }
}
