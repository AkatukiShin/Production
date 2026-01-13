using System;
using System.Collections;
using System.Collections.Generic;
using CriWare;
using CriWare.Assets;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManagerInstance;
    public List<CriAtomSourceForAsset> criAtomSources = new List<CriAtomSourceForAsset>();
    public List<CriAtomCueReference> criAtomCueReferences = new List<CriAtomCueReference>();
    public CriAtomExPlayer soundPlayer;
    public static float seVolume = 0.7f;
    public static float bgmVolume = 0.5f;

    void Awake()
    {
        if(audioManagerInstance == null)
        {
            audioManagerInstance = this;
            criAtomSources = new List<CriAtomSourceForAsset>(GetComponentsInChildren<CriAtomSourceForAsset>());
            
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        soundPlayer = new CriAtomExPlayer();
    }

    public void SoundPlay(int criAtomSourcesNumber)
    {
        criAtomSources[criAtomSourcesNumber].Play();
    }

    public void SoundStop(int criAtomSourcesNumber)
    {
        criAtomSources[criAtomSourcesNumber].Stop();
    }

    public float SetSeVolume
    {
        get { return seVolume; }
    }

    public float SetBgmVolume
    {
        get { return bgmVolume; }
    }
}
