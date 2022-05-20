using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextSpeech;
using UnityEngine.Android;
using TMPro;

public class Speech : MonoBehaviour {

    const string LANG_CODE = "en-GB";

    public DebugManager dm;
    public VoiceRecognition vr;

    public TMP_Text listeningTxt;

    bool processing = false;

    // Start is called before the first frame update
    void Start() {
        SetUp ();
    }

    void SetUp() {
        SpeechToText.instance.Setting (LANG_CODE);
        TextToSpeech.instance.Setting (LANG_CODE, 1, 1);

        SpeechToText.instance.onResultCallback = OnFinalSpeechResult;
#if UNITY_ANDROID
        SpeechToText.instance.onPartialResultsCallback = OnPartialSpeechResult;
        if (!Permission.HasUserAuthorizedPermission (Permission.Microphone)) {
            Permission.RequestUserPermission (Permission.Microphone);
        }
#endif
        TextToSpeech.instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.instance.onDoneCallback = OnSpeakStop;

        //SpeechToText.instance.onBeginningOfSpeechCallback = OnStartSpeaking;
        //SpeechToText.instance.onEndOfSpeechCallback = OnStopSpeach;
    }

    private void Update() {
        if (Input.touchCount > 0 && processing == false) {
            StartListening ();
            processing = true;
            dm.DebugOut("Debug", "Listening", false, false);
        } else if (Input.touchCount == 0 && processing == true) {
            StopListening ();
            processing = false;
            dm.DebugOut ("test", "Stopped Listening", false, false);
        }
    }

    #region STT
    public void StartListening() {
        //SpeechToText.instance.StartRecording ();
        //listeningTxt.text = "Listening";
    }

    public void StopListening() {
        //SpeechToText.instance.StopRecording ();
        //listeningTxt.text = "Processing";
    }

    void OnFinalSpeechResult(string result) {
        dm.DebugOut ("STT", "Heard, " + result, false, false);
        //vr.VoiceResults (result);
    }

    void OnPartialSpeechResult(string result) {
        dm.DebugOut ("STT", "Partial Heard, " + result, false, false);
    //    vr.VoiceResults (result);
    //    //StopListening ();
    //    //StartListening ();
    }

    //void OnStartSpeaking() {
    //    dm.DebugOut ("Speach", "Speak Begin", false, false);
    //}

    //void OnStopSpeach() {
    //    dm.DebugOut ("Speach", "Speak End", false, false);
    //    //StopListening ();
    //}

    #endregion

    #region TTS
    public void Say(string msg) {
        TextToSpeech.instance.StartSpeak (msg);
        dm.DebugOut ("Speech", "Response - " + msg, false, false);
    }

    public void StopSpeaking() {
        TextToSpeech.instance.StopSpeak ();
    }

    void OnSpeakStart() {
        dm.DebugOut ("TTS", "Speaking...", false, false);
    }

    void OnSpeakStop() {
        dm.DebugOut ("TTS", "Stopped Speaking", false, false);
    }
#endregion
}
