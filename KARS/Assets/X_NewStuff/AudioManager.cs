using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get { return instance; } }
    private static AudioManager instance;

    [SerializeField]
    private Transform AudioPoolTransform;
    [SerializeField]
    private List<AudioSource> AudioObjects_List;

    [SerializeField]
    private AudioSource BGM_Source;
    [SerializeField]
    private AudioSource OneShot_Source;

    [SerializeField]
    private AudioSource[] LoopSource;


    [SerializeField]
    private List<AUDIO_DATA> audioData;

    [SerializeField]
    private Transform BGM_Distributed;

    void Awake()
    {
        instance = this;
        AudioObjects_List = new List<AudioSource>();
        int i = 0;
        foreach ( Transform T in AudioPoolTransform)
        {
            AudioObjects_List.Add(T.GetComponent<AudioSource>());
            T.gameObject.name = "SoundObject_"+i.ToString();
            i++;
        }
    }

    public void Play_Oneshot(AUDIO_CLIP _clip)
    {
        int q = audioData.FindIndex(i => i.audiotype == _clip);

        OneShot_Source.clip = audioData[q].audioClip.clip;
        OneShot_Source.loop = false;
        OneShot_Source.Play();
    }

    public void Play_Loop(AUDIO_CLIP _clip)
    {
        int q = audioData.FindIndex(i => i.audiotype == _clip);
        for (int i = 0; i < LoopSource.Length; i++)
        {
            if (!LoopSource[i].isPlaying)
            {
                LoopSource[i].clip = audioData[q].audioClip.clip; 
                LoopSource[i].Play();
            }
        }
    }

    public void SpawnableAudio(Vector3 _var , AUDIO_CLIP _clip)
    {
        int q = audioData.FindIndex(i => i.audiotype == _clip);

        if (AudioPoolTransform.childCount < 2)
        {
            BGM_Distributed.GetChild(BGM_Distributed.childCount - 1).SetParent(AudioPoolTransform);
            SpawnableAudio(_var, _clip);
        }
        else
        {
            Transform temp = AudioPoolTransform.GetChild(0);
            temp.SetParent(BGM_Distributed);
            temp.position = _var;
            AudioSource tempSource = temp.GetComponent<AudioSource>();
            tempSource.Stop();
            tempSource.clip = audioData[q].audioClip.clip;
            tempSource.loop = false;
            tempSource.Play();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Play_Oneshot(AUDIO_CLIP.CAR_START);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Play_Oneshot(AUDIO_CLIP.CAR_DRIFT);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Play_Oneshot(AUDIO_CLIP.MISSLE_ACTIVE);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Play_Oneshot(AUDIO_CLIP.CAR_EXPLODE);
        }
    }
}




public enum AUDIO_CLIP
{
    CAR_START,
    CAR_ENGINE,
    CAR_EXPLODE,
    CAR_DRIFT,

    BGM,
    BUTTON,

    STUN,
    CONFUSE,
    MISSLE_ACTIVE,
    MISSLE_HIT,
    SHIELD_ACTIVE
}

[Serializable]
public class AUDIO_DATA
{
    public AUDIO_CLIP audiotype;
    public AudioSource audioClip;
}
