using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CriWare;
using CriWare.Assets;

public class BeatAction : MonoBehaviour
{
    private CriAtomSourceForAsset[] atomSources;
    private CriAtomSourceForAsset songAtomSource;
    private CriAtomSourceForAsset tempoAtomSource;

    public  static int beatCount = 0;
    private TakeRhythm takeRhythm;

    [SerializeField] private float bpm = 106f;

    private float beatInterval => 60f / bpm;

    private float lastBeatTime = -1f;
   
    private bool isResult = false;
    
    [SerializeField] private GameObject test;
    [SerializeField] private GameObject test1;
    [SerializeField] private GameObject resultManager;
    
    public static float judgeWindowStart = -999f;
    public static float judgeWindowEnd = -999f;
    
    public List<Action<int>> beatAction = new List<Action<int>>();

    public static void AddBeat() => beatCount++;

    void Awake()
    {
    #if !UNITY_WEBGL
        CriAtomExBeatSync.OnCallback += CriAtomExBeatSync_OnCallback;
    #endif
    }
    private void Start()
    {
        beatCount = 0;
        GameObject obj = GameObject.FindWithTag("TakeRhythm");
        takeRhythm = obj.GetComponent<TakeRhythm>();

        if (AudioManager.audioManagerInstance != null)
        {
            songAtomSource = AudioManager.audioManagerInstance.criAtomSources[1];
            tempoAtomSource = AudioManager.audioManagerInstance.criAtomSources[2];
            songAtomSource.volume = AudioManager.audioManagerInstance.SetBgmVolume;
            tempoAtomSource.volume = AudioManager.audioManagerInstance.SetSeVolume;
            Debug.Log("ForAsset");
        }
        else
        {
            atomSources = GetComponents<CriAtomSourceForAsset>();
            songAtomSource = atomSources[0];
            tempoAtomSource = atomSources[1];
            songAtomSource.volume = 1f;
            tempoAtomSource.volume = 1f;
        }
        #if !UNITY_WEBGL
            CriAtomExBeatSync.OnCallback += CriAtomExBeatSync_OnCallback;
        #endif
        if (songAtomSource != null)
        {
            songAtomSource.Play();
            //songAtomSource = AudioManager.audioManagerInstance.criAtomSources[0];
        }
    }

    private void Update()
    {
        if (songAtomSource.status == CriAtomSource.Status.PlayEnd && !isResult)
        {
            isResult = true;
            resultManager.SetActive(true);
        }
    }

    private void CriAtomExBeatSync_OnCallback(ref CriAtomExBeatSync.Info info)
{
    beatCount++;
    lastBeatTime = Time.time;

    if (beatCount <= 4) return;

    // 次のビートタイミングを計算
    float nextBeatTime = lastBeatTime + beatInterval;
    judgeWindowStart = lastBeatTime + 0.2f;
    judgeWindowEnd   = nextBeatTime  + 0.4f;

    // 判定ノーツを出すかどうか
    bool isFinisher = PlayGameManager.instance.NowChatBox.isFinisher;
    int mod = beatCount % 4;

    bool shouldSpawnNote = (!isFinisher && mod == 3)|| ( isFinisher && mod >= 0 && mod <= 2);

    if (shouldSpawnNote)
    {
        //Instantiate(test1, Vector3.zero, Quaternion.identity);
        // TakeRhythm 側に判定開始を通知
        takeRhythm.BeginNote();    
    }

    beatAction.ForEach(action => action(beatCount));

    tempoAtomSource.Play();
}


    public static int GetBeatCount()
    {
        return beatCount % 4;
    }

    public void InvokeBeatCallbacks(int cnt)
    {
        beatAction.ForEach(a => a?.Invoke(cnt));
    }
}