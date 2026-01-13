
using ChatBox;
using UnityEngine;
using CriWare;
using CriWare.Assets;

public class Cutin : MonoBehaviour
{
    private Animator animator;
    private CriAtomSourceForAsset criAtomSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (AudioManager.audioManagerInstance != null)
        {
            criAtomSource = AudioManager.audioManagerInstance.criAtomSources[4];
            criAtomSource.volume = AudioManager.audioManagerInstance.SetSeVolume;
        }
        else
        {
            criAtomSource = GetComponent<CriAtomSourceForAsset>();
            criAtomSource.volume = 1f;
        }
        
        // CharacterTalkController.instance.TalkActions.Add(CutinPlay);
    }

    public void CutinPlay()
    {
        if (!animator) return;        // ★ Destroy 済みセーフガード
        criAtomSource.Play();
        animator.SetTrigger("CutinPlay");
    }
}
