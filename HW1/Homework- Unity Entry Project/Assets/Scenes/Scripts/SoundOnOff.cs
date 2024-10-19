using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnOff : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isPlaying = true;

    void Start()
    {
       
        audioSource = GetComponent<AudioSource>();

       
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.M))
        {
           
            if (isPlaying)
            {
                audioSource.Pause();  
            }
            else
            {
                audioSource.Play();   
            }

            isPlaying = !isPlaying;  
        }
    }
}
