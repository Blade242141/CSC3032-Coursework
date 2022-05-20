using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    public VoiceRecognition vr;
    public PlayerMovement pm;
    public AudioSource audioSource;

    public DebugManager dm;

    bool playMusic = false;

    public List<AudioClip> music;

    int currentSongNo = 0;

    void Start() {
         music.AddRange(Resources.LoadAll<AudioClip> ("Music"));
        audioSource.clip = music [currentSongNo];
    }

    public void MusicCmd() {
        string str = vr.GetResults ();

        if (str.Contains ("play"))
            MusicPlay ();
        else if (str.Contains ("volume") || str.Contains ("down") || str.Contains ("up") || str.Contains ("max"))
            MusicVol (str);
        else if (str.Contains ("pause") || str.Contains ("stop"))
            MusicPause ();
        else if (str.Contains ("next"))
            MusicNext ();
        else if (str.Contains ("back") || str.Contains ("previous"))
            MusicBack ();
    }

    public void MusicVol(string str) {
        if (str.Contains ("up"))
            audioSource.volume += 0.2f;
        else if (str.Contains("down"))
            audioSource.volume -= 0.2f;
        else if (str.Contains ("max"))
            audioSource.volume = 1;
        dm.DebugOut ("System", "Music - Volume Adjusted", true, true);
    }

    public void MusicPlay() {
        playMusic = true;
        audioSource.Play ();
        pm.Dance ();
        dm.DebugOut ("System", "Music - Play", true, true);
    }

    public void MusicPause() {
        playMusic = false;
        audioSource.Pause ();
        pm.StopDancing ();
        dm.DebugOut ("System", "Music - Stopped", true, true); 
    }

    public void MusicNext() {
        currentSongNo++;
        if (currentSongNo >= music.Count)
            currentSongNo = 0;

        audioSource.clip = music [currentSongNo];
        audioSource.Play ();
        dm.DebugOut ("System", "Music - Next Song", true, true);
    }

    public void MusicBack() {
        currentSongNo--;
        if (currentSongNo <= 0)
            currentSongNo = music.Count;

        audioSource.clip = music [currentSongNo];
        audioSource.Play ();
        dm.DebugOut ("System", "Music - Back", true, true);
    }
}
