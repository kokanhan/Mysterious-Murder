using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTV : MonoBehaviour
{
    private AudioSource TVSound;
    public GameObject TVCanvas;
    public GameObject TVScreen;
    private bool playing;

    private void Awake()
    {
        TVSound = this.GetComponent<AudioSource>();
        TVCanvas.SetActive(false);
        TVScreen.SetActive(false);
        playing = false;
    }

    private void Update()
    {
        if (!TVSound.isPlaying)
        {
            playing = false;
            TVScreen.SetActive(false);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && playing==false)
        {
            TVCanvas.SetActive(true);
            //startFollowing = true;
            //contactTimes++;
            //LeftEye.GetComponent<MeshRenderer>().material = Materials[1];
            //RightEye.GetComponent<MeshRenderer>().material = Materials[1];
            if (Input.GetMouseButtonDown(0) && !TVSound.isPlaying)
            {
                TVSound.Play();
                playing = true;
                TVScreen.SetActive(true);
                TVCanvas.SetActive(false);
            }
         
                  
        }

    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            TVCanvas.SetActive(false);
        }
    }
}
