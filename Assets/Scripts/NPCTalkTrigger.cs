using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalkTrigger : MonoBehaviour
{
    public bool NurseTalked;
    public bool StartTalking;
    public GameObject NPCCanvas;
    public GameObject Doctor;
    public GameObject Nurses;
    public float waitingTime = 30.0f;

    private void Awake()
    {
        NurseTalked = false;
        StartTalking = false;
        Doctor.SetActive(false);
        Nurses.SetActive(true);
        NPCCanvas.SetActive(false);

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if(NurseTalked == true)
            {
                Doctor.SetActive(true);
            }

            if (StartTalking == false)
            {

                NPCCanvas.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {                 
                    NPCCanvas.SetActive(false);
                    StartCoroutine(NPCStartTalk());
                    StartTalking = true;
                }
            }

        }

    }

    IEnumerator NPCStartTalk()
    {
        yield return new WaitForSeconds(waitingTime);
        StartTalking = false;
        Nurses.SetActive(false);
        Doctor.SetActive(false);
        NurseTalked = true;// Just let nurses talk when the player first time click listen
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            NPCCanvas.SetActive(false);
        }
    }

    public bool GetStartTalking()
    {
        return StartTalking;
    }






}
