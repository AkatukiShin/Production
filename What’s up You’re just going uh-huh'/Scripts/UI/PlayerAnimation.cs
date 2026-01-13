using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChatBox;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private BeatAction beatAction; // BeatActionのインスタンスを保持する変数

    private Animator animator;

    private bool isAnimation = false;
    // Start is called before the first frame update
    void Awake()
    {
        // Animator はシーン遷移前にキャッシュしておく
        animator = GetComponent<Animator>();

        // 参照が無い場合はシーンから検索（任意）
        if (beatAction == null)
            beatAction = FindObjectOfType<BeatAction>();
    }

    void OnEnable()
    {
        // === コールバック登録（重複防止で一度 Remove してから Add） ===
        if (beatAction != null)
        {
            beatAction.beatAction.Remove(OnBeatAction);
            beatAction.beatAction.Add(OnBeatAction);
        }

        if (CharacterTalkController.instance != null)
        {
            CharacterTalkController.instance.TalkActions.Remove(PlayAnimaton);
            CharacterTalkController.instance.TalkActions.Add(PlayAnimaton);
        }
    }

    void OnDisable()
    {
        // === 解除し忘れ防止 ===
        if (beatAction != null)
            beatAction.beatAction.Remove(OnBeatAction);

        if (CharacterTalkController.instance != null)
            CharacterTalkController.instance.TalkActions.Remove(PlayAnimaton);
    }

    /*------------------------------ 外部コールバック ------------------------------*/

    /// <summary>ChatBox から呼ばれる再生リクエスト</summary>
    public void PlayAnimaton(ChatBoxType chatBox)
    {
        if (isAnimation || animator == null) return;

        switch (chatBox)
        {
            case ChatBoxType.up: animator.SetTrigger("UnUn_Up"); break;
            case ChatBoxType.right: animator.SetTrigger("Majika_Right"); break;
            case ChatBoxType.left: animator.SetTrigger("Eraine_Left"); break;
            case ChatBoxType.down: animator.SetTrigger("Nanisore_Down"); break;
        }

        StartCoroutine(AnimationCoolDown());
    }

    /// <summary>ビート同期トリガー</summary>
    private void OnBeatAction(int _)
    {
        if (!animator) return;          // Destroy 済みチェック
        animator.SetTrigger("puni");
    }

    /*------------------------------ Helper ------------------------------*/

    private IEnumerator AnimationCoolDown()
    {
        isAnimation = true;
        yield return new WaitForSeconds(1f);  // アニメ再生中は他入力を無視
        isAnimation = false;
    }
}
