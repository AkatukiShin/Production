using UnityEngine;
using System.Collections;
using CriWare;
using ChatBox;
using System;
using System.Collections.Generic;
using CriWare.Assets;
using UnityEngine.Rendering;

public class TakeRhythm : MonoBehaviour
{
    private CriAtomSourceForAsset aidutiAtomSource;
    private CriAtomSourceForAsset missAtomSource;

    [SerializeField] private float canThroughTime = 0.8f;
    [SerializeField] private GameObject scoreMangerObj;
    private ScoreManager scoreManager;
    private Coroutine  missCoroutine;


    public bool isNotesHit = false;
    public List<Action<ChatBoxType>> SuccessInputActions = new List<Action<ChatBoxType>>(); // 成功したときのアクションを登録するリスト
    public List<Action<ChatBoxType>> MissInputActions = new List<Action<ChatBoxType>>(); // ミスしたときのアクションを登録するリスト
    [SerializeField] private BeatAction beatAction; // BeatActionのインスタンスを保持する変数
    [SerializeField] private GirlChatManager girlChatManager; // GirlChatManagerのインスタンスを保持する変数

    /*======================= ここから追加 =======================*/
    void OnEnable()              // ★ イベント登録を Start → OnEnable に移動
    {
        CharacterTalkController.instance.TalkActions.Add(OnInputKeyPressed);
        beatAction.beatAction.Add(ChangeBeat);
    }

    void OnDisable()             // ★ 破棄時に解除してリークを防止
    {
        CharacterTalkController.instance.TalkActions.Remove(OnInputKeyPressed);
        beatAction.beatAction.Remove(ChangeBeat);

        /* ★ BeatAction に残っている “自分ターゲット” のコールバックを全て除去 */
        if (beatAction != null)
            beatAction.beatAction.RemoveAll(a => a == null || ReferenceEquals(a.Target, this));

        /* ★ コルーチンの停止も忘れずに */
        if (missCoroutine != null)
        {
            StopCoroutine(missCoroutine);
            missCoroutine = null;
        }
    }
    /*======================= ここまで追加 =======================*/

    private void Start()
    {
        if (AudioManager.audioManagerInstance != null)
        {
            aidutiAtomSource = AudioManager.audioManagerInstance.criAtomSources[9];
            missAtomSource = AudioManager.audioManagerInstance.criAtomSources[10];
            aidutiAtomSource.volume = AudioManager.audioManagerInstance.SetSeVolume;
            missAtomSource.volume = AudioManager.audioManagerInstance.SetSeVolume;
        }
        else
        {
            aidutiAtomSource = GetComponent<CriAtomSourceForAsset>();
            aidutiAtomSource.volume = 1f;
        }

        if (scoreMangerObj == null) Debug.LogError("Don't find ScoreMangerObj");
        scoreManager = scoreMangerObj.GetComponent<ScoreManager>();

        ////内野追記
        ////キー入力イベントを登録
        //CharacterTalkController.instance.TalkActions.Add(OnInputKeyPressed);
        //beatAction.beatAction.Add(ChangeBeat); // BeatActionのアクションを登録
    }

    void Update()
    {

    }

   public void OnInputKeyPressed(ChatBoxType type)
    {
        // すでにヒット／ミス判定済みなら無視
        if (isNotesHit) return;

        // **判定対象のビートかどうか** を改めてチェック（BeginNote() が呼ばれていない場合も弾く）
        float now = Time.time;
        Debug.Log($"[Input] t={now:F2}, window=[{BeatAction.judgeWindowStart:F2} ~ {BeatAction.judgeWindowEnd:F2}]");

        // 時間外ならミス
        if ((now < BeatAction.judgeWindowStart || now > BeatAction.judgeWindowEnd)&& type != ChatBoxType.none)
        {
            isNotesHit = true;
            if (missCoroutine != null) { StopCoroutine(missCoroutine); missCoroutine = null; }
            missAtomSource.Play();
            MissInputActions.ForEach(a => a(type));
            return;
        }

        // タイプ不一致ならミス
        if (!JudgeType(type) && type != ChatBoxType.none)
        {
            isNotesHit = true;
            if (missCoroutine != null) { StopCoroutine(missCoroutine); missCoroutine = null; }
            missAtomSource.Play();
            MissInputActions.ForEach(a => a(type));
            return;
        }

        // 成功
        isNotesHit = true;
        if (missCoroutine != null) { StopCoroutine(missCoroutine); missCoroutine = null; }
        PlaySuccesSound();
        SuccessInputActions.ForEach(a => a(type));
        Debug.Log("Success!");
    }

    private bool JudgeInput()
    {
        float currentTime = Time.time;

        if (BeatAction.judgeWindowStart < 0f || BeatAction.judgeWindowEnd < 0f)
        {
            Debug.Log("まだ判定範囲がありません");
            return false;
        }

        if (currentTime >= BeatAction.judgeWindowStart && currentTime <= BeatAction.judgeWindowEnd)
        {
            //Debug.Log("入力成功！オフセット");
            isNotesHit = true;
            return true;
        }

        Debug.Log($"Miss! 判定時間外");
        StopCoroutine(CheckThroughNotes());
        return false;
    }
    private bool JudgeType(ChatBoxType type)
    {
        Debug.Log($"count:{PlayGameManager.instance.beatCount % 4}");
        if (type == PlayGameManager.instance.NowChatBox.girlAnswers[PlayGameManager.instance.beatCount % 4] ||
            type == PlayGameManager.instance.NowChatBox.girlAnswers[(PlayGameManager.instance.beatCount + 3) % 4])
        {
            //正解の処理
            return true;
        }
        return false;
    }

    private void PlaySuccesSound()
    {
        aidutiAtomSource.Play();
    }

    public IEnumerator CheckThroughNotes()
    {
        yield return new WaitForSeconds(canThroughTime);

        if (!isNotesHit)
        {
            if (scoreManager == null) yield return null;
            if(!girlChatManager.nextFinisher())scoreManager.OnMiss(ChatBoxType.none);
            Debug.Log("Miss...");
        }
    }

    public void BeginNote()
    {
        /* ★ オブジェクトがすでに Destroy 済みなら何もしない */
        if (this == null || !isActiveAndEnabled) return;

        // フラグリセット & 前のミスコルーチン停止
        isNotesHit = false;
        if (missCoroutine != null)      // ★ null チェックを追加
        {
            StopCoroutine(missCoroutine);
            missCoroutine = null;       // ★ Stop 後に参照をクリア
        }
        missCoroutine = StartCoroutine(CheckThroughNotes());
    }

    public void ChangeBeat(int _)
    {
        isNotesHit = false; // Beatが変わったらヒット判定をリセット
    }
}