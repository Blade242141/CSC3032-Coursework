using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public GameObject player;
    public Animator anim;
    public GameObject surface;
    public GameObject joints;


    public bool isActive = false;
    public bool doingAction = false;
    float idleTimer = 0.0f;
    public float minIdleWaitTime = 10.0f;
    public float maxIdleWaitTime = 30.0f;
    float randomIdleWait = 0.0f;

    float enteranceTimer = 1.0f; // 1 = invisible, -1 = visible
    public float enterSpeed = 1.0f;
    public float exitSpeed = 0.5f;

    public Speech speech;
    public VoiceRecognition vr;

    // Start is called before the first frame update
    void Start() {
        randomIdleWait = Random.Range (minIdleWaitTime, maxIdleWaitTime);
    }

    // Update is called once per frame
    void Update() {
        BoredIdle ();

        if (!isActive && enteranceTimer < 1.0f) { // Exit
            enteranceTimer += Time.deltaTime * exitSpeed;
        } else if (isActive && enteranceTimer > -1.0f) { // Enter
            enteranceTimer -= Time.deltaTime * enterSpeed;
        }

        surface.GetComponent<Renderer> ().sharedMaterial.SetFloat ("Vector1_6f9eb3b7f6964d719fcbdecb03db5955", enteranceTimer);
        joints.GetComponent<Renderer> ().sharedMaterial.SetFloat ("Vector1_6f9eb3b7f6964d719fcbdecb03db5955", enteranceTimer);

        if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) {
            anim.SetBool ("jump", false);
            doingAction = false;
        }
    }

    void BoredIdle() {
        if (doingAction && isActive) {
            idleTimer = 0.0f;
        } else {
            if (idleTimer >= randomIdleWait) {
                randomIdleWait = Random.Range (minIdleWaitTime, maxIdleWaitTime);
                doingAction = true;
                anim.SetBool ("idleBored", true);
            } else {
                idleTimer += Time.deltaTime;
            }
        }

        if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle"))
            doingAction = false;
    }

    public void DisplayWeather() {
        anim.SetBool ("isWeather", true);
        StartCoroutine (WaitForAnimationToStart ());
    }

    IEnumerator WaitForAnimationToStart() {
        yield return new WaitForSeconds (2);

        anim.SetBool ("isWeather", false);
    }

    #region Commands

    public void Enterance() {
        player.transform.rotation = Quaternion.identity;
        anim.SetBool ("isActive", true);
        isActive = true;
        doingAction = false;

        if (System.DateTime.Now.Hour > 12)
            speech.Say ("Good Afternoon");
        else
            speech.Say ("Good Morning");

        vr.playerActive = true;
    }

    public void Dance() {
        anim.SetBool ("isDancing", true);
        doingAction = true;
    }

    public void StopDancing() {
        anim.SetBool ("isDancing", false);
        doingAction = false;
    }

    public void Jump() {
        anim.SetBool ("jump", true);
        doingAction = true;
    }

    public void PlayerExit() {
        isActive = false;
        anim.SetBool ("isActive", false);
        anim.SetBool ("jump", false);
        anim.SetBool ("isDancing", false);
        doingAction = false;
        vr.playerActive = false;
    }

    #endregion
}
