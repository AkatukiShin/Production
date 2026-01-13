using UnityEngine;
using UnityEngine.SceneManagement;
using CriWare;
using CriWare.Assets;

public class Tutorial : MonoBehaviour
{
    private CriAtomSourceForAsset aidutiAtomSource;
    private int tutorialFinishCount = 0;
    private int tutorialFinishCountSucces = 0;
    // 矢印キーをまとめた配列
    private readonly KeyCode[] arrowKeys = new KeyCode[]
    {
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow
    };

    [SerializeField] private GameObject startButton;

    void Start()
    {
        aidutiAtomSource = GetComponent<CriAtomSourceForAsset>();
    }

    void Update()
    {
        switch (TutoralStepManager.tutorialStep)
        {
            case 0:
                foreach (KeyCode key in arrowKeys)
                {
                    if (Input.GetKeyDown(key))
                    {
                        OnArrowKeyPressed(key);
                        break;
                    }
                }
                break;

            case 1:
                //if(Input.GetKeyDown(KeyCode.Space))
                //{
                    OnArrowKeyPressed(KeyCode.Space);
                //}
                break;
            
            case 2:
                if(Input.GetKeyDown(KeyCode.LeftArrow) && tutorialFinishCountSucces == 0)
                {
                    OnFinishArrowPressed(KeyCode.LeftArrow);
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow) && tutorialFinishCountSucces == 1)
                {
                    OnFinishArrowPressed(KeyCode.UpArrow);
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow) && tutorialFinishCountSucces == 1)
                {
                    OnFinishArrowPressed(KeyCode.DownArrow);
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow) && tutorialFinishCountSucces == 2)
                {
                    OnFinishArrowPressed(KeyCode.RightArrow);
                }
                break;

            case 3:
                if(Input.anyKeyDown)
                {
                    CriFsPlugin.FinalizeLibrary();
                    SceneManager.LoadScene("InGame");
                }
                break;
        }
        
    }

    // 共通の処理をこのメソッドにまとめる
    private void OnArrowKeyPressed(KeyCode key)
    {
        Debug.Log($"矢印キー {key} が押されました。共通処理を実行します。");
        float currentTime = Time.time;

        if (PlayBGM.judgeTitleWindowStart < 0f || PlayBGM.judgeTitleWindowEnd < 0f)
        {
            Debug.Log("まだ判定範囲がありません");
        }

        if (currentTime >= PlayBGM.judgeTitleWindowStart && currentTime <= PlayBGM.judgeTitleWindowEnd)
        {
            Debug.Log("入力成功！オフセット");
            aidutiAtomSource.Play();
            TutoralStepManager.tutorialStep++;
        }
        else
        {
            Debug.Log($"Miss! 判定時間外");
        }
    }

    private void OnFinishArrowPressed(KeyCode key)
    {
        Debug.Log($"矢印キー {key} が押されました。共通処理を実行します。");
        float currentTime = Time.time;

        if (PlayBGM.judgeTitleWindowStart < 0f || PlayBGM.judgeTitleWindowEnd < 0f)
        {
            Debug.Log("まだ判定範囲がありません");
        }

        if (currentTime >= PlayBGM.judgeTitleWindowStart && currentTime <= PlayBGM.judgeTitleWindowEnd)
        {
            Debug.Log("入力成功！オフセット");
            aidutiAtomSource.Play();
            tutorialFinishCountSucces++;
            if(tutorialFinishCountSucces >= 3)
            {
                TutoralStepManager.tutorialStep++;
            }
        }
        else
        {
            Debug.Log($"Miss! 判定時間外");
        }
        tutorialFinishCount++;
        if (tutorialFinishCount == 3)
        {
            tutorialFinishCountSucces = 0;
            tutorialFinishCount = 0;
        }
    }

}
