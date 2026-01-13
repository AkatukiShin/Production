using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CriWare;
using CriWare.Assets;

public class PlayBGM : MonoBehaviour
{
    private CriAtomSourceForAsset[] atomSources;
    public CriAtomSourceForAsset songAtomSource;
    public CriAtomSourceForAsset tempoAtomSource;

    public static PlayBGM instance;
    public  int titleBeatCount = 0;

    [SerializeField] private float bpm = 106f;
    [SerializeField] private float beatOffset = 0.5f;
    [SerializeField] private GameObject test1;
    [SerializeField] private GameObject test;
    [SerializeField] private List<GameObject> tutorialTexts = new List<GameObject>();

    private float beatInterval => 60f / bpm;
    private float lastBeatTime = -1f;
    private int testActive = -1;

    public static float judgeTitleWindowStart = -999f;
    public static float judgeTitleWindowEnd = -999f;

    void Awake()
    {
        //singletonの実装
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        titleBeatCount = 0;
        atomSources = GetComponents<CriAtomSourceForAsset>();
        songAtomSource = atomSources[0];
        tempoAtomSource = atomSources[1];

        foreach (var text in tutorialTexts)
        {
            text.SetActive(false);
        }

        CriAtomExBeatSync.OnCallback += CriAtomExBeatSync_OnCallback;

        if(songAtomSource != null)  songAtomSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CriAtomExBeatSync_OnCallback(ref CriAtomExBeatSync.Info info)
    {
        titleBeatCount++;
        lastBeatTime = Time.time - beatOffset;

        if (titleBeatCount <= 4) return;

        testActive++;

        // 判定ウィンドウ設定
        // 次の4拍子が来る時間を予測
        float nextBeatTime = lastBeatTime + beatInterval;
        Debug.Log(nextBeatTime);
        if(titleBeatCount % 3 == 0 && TutoralStepManager.tutorialStep != 2)
        {
            // 判定ウィンドウのスタート・エンドを設定
            judgeTitleWindowStart = lastBeatTime + 0.2f;
            judgeTitleWindowEnd = nextBeatTime + 0.25f;
        }
        else if (TutoralStepManager.tutorialStep == 2)
        {
            // 判定ウィンドウのスタート・エンドを設定
            judgeTitleWindowStart = lastBeatTime + 0.2f;
            judgeTitleWindowEnd = nextBeatTime + 0.25f;
        }
        
        Debug.Log($"testActive{testActive%4}");
        
        if(this == null) return;
        tutorialTexts[testActive % 4].SetActive(true);
        Debug.Log($"titleBeatCount{titleBeatCount}");
        if (titleBeatCount % 4  == 3)
        {
            StartCoroutine(ResetTexts());
        }
        
        tempoAtomSource.Play("testTempo");
    }

    IEnumerator ResetTexts()
    {
        yield return new WaitForSeconds(1f);
        foreach (var text in tutorialTexts)
        {
            text.SetActive(false);
        }
    }
}
