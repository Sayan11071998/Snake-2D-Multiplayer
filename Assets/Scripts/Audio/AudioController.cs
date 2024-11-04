using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum Sounds
{
    Eat = 0,
    Poison = 1,
    Death = 3
}

[RequireComponent(typeof(AudioSource))]
public class AudioContoller : MonoBehaviour
{
    public AudioClip eat;
    public AudioClip poison;
    public AudioClip death;

    private AudioSource m_Source;

    void Awake()
    {
        m_Source = GetComponent<AudioSource>();
        m_Source.playOnAwake = false;
    }

    public void Play(Sounds sound)
    {
        switch (sound)
        {
            case Sounds.Eat:
                m_Source.clip = eat;
                break;
            case Sounds.Poison:
                m_Source.clip = poison;
                break;
            case Sounds.Death:
                m_Source.clip = death;
                break;
        }
        m_Source.pitch = Random.Range(0.95f, 1.1f);
        m_Source.Play();
    }
}