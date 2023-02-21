using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorTalk : MonoBehaviour
{
    private AudioSource DoctorSound;
    private bool playing;
    private bool StartTalking;
    private NPCTalkTrigger nPCTalkTrigger;
    // Start is called before the first frame update
    private void Awake()
    {
        DoctorSound = this.GetComponent<AudioSource>();
        nPCTalkTrigger = GameObject.Find("NPCTalkTrigger").GetComponent<NPCTalkTrigger>();
        // playing = false;
    }

    // Update is called once per frame
    void Update()
    {
        StartTalking = nPCTalkTrigger.GetStartTalking();
        if (StartTalking == true && !DoctorSound.isPlaying)
        {
            DoctorSound.Play();
        }
    }
}
