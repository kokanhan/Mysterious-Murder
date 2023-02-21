using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNurseSound : MonoBehaviour
{
    private AudioSource adSource;
    //public AudioClip[] Dialogues = new AudioClip[5];
    public AudioClip[] Dialogues;
    private bool StartTalking;
    private NPCTalkTrigger nPCTalkTrigger;
    private bool playing;
    private void Start()
    {
        //adSource = this.GetComponent<AudioSource>();
        nPCTalkTrigger = GameObject.Find("NPCTalkTrigger").GetComponent<NPCTalkTrigger>();
    }



    void Update()
    {
        adSource = this.GetComponent<AudioSource>();
        StartTalking = nPCTalkTrigger.GetStartTalking();
        if (StartTalking == true && !adSource.isPlaying)
        //if (StartTalking == true)
        {
            StartCoroutine(PlaySounds());
        }
    }

    IEnumerator playAudioSequentiallyO()
    {
        yield return null;

        //1.Loop through each AudioClip
        for (int i = 0; i < Dialogues.Length; i++)
        {//
            playing = true;
            //2.Assign current AudioClip to audiosource
            adSource.clip = Dialogues[i];
            Debug.Log("adSource.clip = Dialogues[i] "+ i);

            //3.Play Audio
            //if (adSource.clip)
            //{
            //    playing = true;
            //}
            adSource.Play();

            //4.Wait for it to finish playing
            while (adSource.isPlaying&& playing ==true)
            {
                yield return new WaitForSeconds(adSource.clip.length);
                playing = false;
            }

            //5. Go back to #2 and play the next audio in the adClips array
        }
    }

    IEnumerator playAudioSequentially()
    {
        yield return null;

        //1.Loop through each AudioClip
        for (int i = 0; i < Dialogues.Length; i++)
        {//
   
            //2.Assign current AudioClip to audiosource
            adSource.clip = Dialogues[i];
            Debug.Log("adSource.clip = Dialogues[i] " + i);

            //3.Play Audio

            adSource.Play();

            //4.Wait for it to finish playing
            while (adSource.isPlaying)
            {
                yield return null;

            }

            //5. Go back to #2 and play the next audio in the adClips array
        }
    }


    IEnumerator PlaySounds()
    {
        //if (Dialogues.Length > 0)
        //{
            for (int i = 0; i < Dialogues.Length; i++)
            {
            //yield return new WaitForSeconds(Dialogues[i].length + 0.1f);
            //adSource.PlayOneShot(Dialogues[i]);
            adSource.clip = Dialogues[i];
                adSource.Play();

                // wait for the lenght of the clip to finish playing
                Debug.Log("Dialogues[i].length " + Dialogues[i].length);

            yield return new WaitForSeconds(Dialogues[i].length + 0.1f);
            


        }
        //}
        yield return null;
    }



}
