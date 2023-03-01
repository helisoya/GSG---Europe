using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    AudioSource source;


    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!source.isPlaying && Manager.instance.picked)
        {
            source.clip = Resources.Load<AudioClip>("Music/"+Manager.instance.cultures.GetRandom_Musique(Manager.instance.player.culture));
            if (source.clip != null)
            {
                source.Play();
            }
        }
    }
}
