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

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    public void Play(Sounds sound)
    {
        switch (sound)
        {
            case Sounds.Eat:
                _audioSource.clip = eat;
                break;
            case Sounds.Poison:
                _audioSource.clip = poison;
                break;
            case Sounds.Death:
                _audioSource.clip = death;
                break;
        }

        _audioSource.pitch = Random.Range(0.95f, 1.1f);
        _audioSource.Play();
    }
}