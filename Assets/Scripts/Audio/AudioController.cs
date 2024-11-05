using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioContoller : MonoBehaviour
{
    private static AudioContoller instance;
    public static AudioContoller Instance { get { return instance; } }

    public AudioSource audioSourceBGM;
    public AudioSource audioSourceSFX;
    public AudioType[] audioList;

    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetGameVolume(0.2f, 0.5f);
        PlayBGM(global::AudioTypeList.BackGroundMusic);
    }

    public void SetGameVolume(float bgmVolume, float sfxVolume)
    {
        audioSourceBGM.volume = bgmVolume;
        audioSourceSFX.volume = sfxVolume;
    }

    public AudioClip GetAudioClip(AudioTypeList audio)
    {
        AudioType audioItem = Array.Find(audioList, item => item.audioType == audio);

        if (audioItem != null)
            return audioItem.audioClip;
        return null;
    }

    public void PlayBGM(AudioTypeList audio)
    {
        AudioClip clip = GetAudioClip(audio);
        if (clip == null) return;

        audioSourceBGM.clip = clip;
        audioSourceBGM.loop = true;
        audioSourceBGM.Play();
    }

    public void PlaySFX(AudioTypeList audio)
    {
        AudioClip clip = GetAudioClip(audio);
        if (clip == null) return;

        audioSourceSFX.PlayOneShot(clip);
    }
}

[Serializable]
public class AudioType
{
    public AudioTypeList audioType;
    public AudioClip audioClip;
}

public enum AudioTypeList
{
    FruitEat = 0,
    PoisonEat = 1,
    Death = 3,
    BackGroundMusic = 4
}