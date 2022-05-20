//using System.Collections;
//using System.Collections.Generic;
//using TextSpeech;
//using UnityEngine;

//public class TalkBack : MonoBehaviour {

//    public const string LANG_CODE = "en-US"; // Language option for SpeechToText and TextToSpeech

//    [SerializeField]
//    float ttsRate = 1;
//    [SerializeField]
//    float ttsPitch = 1;

//    bool isSpeaking = false;

//    void Start() {
//        TextToSpeech.instance.Setting (LANG_CODE, ttsPitch, ttsRate);

//        TextToSpeech.instance.onStartCallBack = OnSpeakStart;
//        TextToSpeech.instance.onDoneCallback = OnSpeakStop;
//    }

//    public string GetLangCode() {
//        return (LANG_CODE);
//    }

//    public void StartSpeaking(string msg) {
//        TextToSpeech.instance.StartSpeak (msg);
//    }

//    public void StopSpeaking() {
//        TextToSpeech.instance.StopSpeak ();
//    }

//    void OnSpeakStart() {
//        isSpeaking = true;
//    }

//    void OnSpeakStop() {
//        isSpeaking = false;
//    }
//}
