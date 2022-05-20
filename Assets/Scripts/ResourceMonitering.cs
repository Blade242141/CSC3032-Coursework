
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class ResourceMonitering : MonoBehaviour {
    string LOC;
    public string saveFolder = "D:/OneDrive/Desktop/CSC3031/CourseworkTests";

    ArrayList memoryUsage = new ArrayList ();
    ArrayList fps = new ArrayList ();
    ArrayList latency = new ArrayList ();

    int callingCmd = 0;

    int calledNo = 0;
    public int testAmt = 10;
    public int waitSec = 4;

    bool latencyTiming = false;
    float latencyTimer = 0.0f;

    public VoiceRecognition vr;
    public DebugManager dm;
    public string testCommand = "weather";

    public bool testing = false;

    // Start is called before the first frame update
    void Start() {
        //LOC = Directory.GetParent (Application.dataPath).ToString () + "/tests/"; //Add file name + .txt
    }

    [ContextMenu ("Moniter")]
    public void StartMonitering() {
        latencyTimer = 0.0f;
        latencyTiming = true;
        testing = true;
        dm.DebugOut ("Test", "Monitering", false, false);
        vr.DebugOverride (testCommand);

        callingCmd = 1;
    }

    public void StopLatencyCounter() {
        latencyTiming = false;
        latency.Add (latencyTimer);
    }

    public void StopMonitering() {
        if (testing) {
            callingCmd = 0;
            dm.DebugOut ("Test", "Monitering Stopped - " + calledNo, false, false);
            calledNo++;

            if (calledNo >= testAmt) {
                LOC = saveFolder + "/tests/";

                if (!File.Exists (LOC)) {
                    Directory.CreateDirectory (LOC);
                    LOC += testCommand + "X" + DateTime.Now.ToString ("dd-MM-yyyy") + "T" + DateTime.Now.ToString ("HH-mm-ss") + ".txt";
                    //File.WriteAllText (LOC, "");

                    Debug.Log ("Gathering Findings");
                    //Calling Cmd Finished
                    calledNo = 0;
                    testing = false;

                    //highestMemory = memoryUsage.IndexOf (1, 1, 1);

                    foreach (long memory in memoryUsage) {
                        if (memory >= highestMemory)
                            highestMemory = memory;

                        avgMemory += memory;
                    }

                    foreach (float fp in fps) {
                        if (fp <= lowestFPS)
                            lowestFPS = fp;

                        avgFPS += fp;
                    }

                    foreach (float late in latency) {
                        if (late <= shortestLatency)
                            shortestLatency = late;
                        if (late >= longestLatency)
                            longestLatency = late;

                        avgLatency += late;
                    }

                    avgMemory = avgMemory / memoryUsage.Count;
                    avgFPS = avgFPS / fps.Count;
                    avgLatency = avgLatency / latency.Count;
                    dm.ClearDebug ();

                    string tempStr = "Highest Memory - " + highestMemory + "\n" + "Average Memory - " + avgMemory + "\n" + "Lowest FPS - " + lowestFPS + "\n" + "Average FPS - " + avgFPS + "\n" + "Longest Latency - " + longestLatency + "\n" + "Shortest Latency - " + shortestLatency + "\n" + "Average Latency - " + avgLatency;

                    dm.ClearDebug ();
                    dm.DebugOut ("Test Script", testCommand + " test has finished", false, false);
                    dm.DebugOut ("Test Script", tempStr, false, false);


                    FileStream fs = File.Create (LOC);
                    fs.Close ();
                    StreamWriter sw = new StreamWriter (LOC, true, Encoding.ASCII);
                    sw.WriteLine (tempStr);
                    sw.Close ();
                }
            } else {
                StartCoroutine (WaitTime ());
            }
        }
    }

    IEnumerator WaitTime() {
        yield return new WaitForSeconds (waitSec);

        StartMonitering ();
    }

    long highestMemory;
    float longestLatency;
    float shortestLatency = 1000000000;
    float lowestFPS = 1000000000;

    long avgMemory;
    float avgFPS;
    float avgLatency;

    private void Update() {
        if (callingCmd == 1) {
            //Calling Cmd
            memoryUsage.Add ((long) GC.GetTotalMemory (false));
            fps.Add ((float) Time.frameCount / Time.time);
        }

        if (latencyTiming) {
            latencyTimer += ((float) Time.deltaTime * 1000);
        }
    }
}

