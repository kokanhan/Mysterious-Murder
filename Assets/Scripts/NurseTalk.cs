using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseTalk : MonoBehaviour
{
    private AudioSource NurseSound;
    private bool toplay;
    private bool StartTalking;
    private NPCTalkTrigger nPCTalkTrigger;
    // Start is called before the first frame update
    private void Awake()
    {
        NurseSound = this.GetComponent<AudioSource>();
        nPCTalkTrigger = GameObject.Find("NPCTalkTrigger").GetComponent<NPCTalkTrigger>();
        toplay = true;
    }

    // Update is called once per frame
    void Update()
    {
        StartTalking = nPCTalkTrigger.GetStartTalking();
        if (StartTalking == true && !NurseSound.isPlaying && toplay == true)
        {
            NurseSound.Play();
            toplay = false;
           // nPCTalkTrigger.NurseTalked = true;
        }
    }
}
