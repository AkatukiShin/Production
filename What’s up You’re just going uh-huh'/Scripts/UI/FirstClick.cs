using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstClick : MonoBehaviour
{
    private GameObject firstClickButton;
    private string sceneName;
    private static HashSet<string> shownScenes = new HashSet<string>();

    [SerializeField] private GameObject playBGM;

    void Awake()
    {
        firstClickButton = this.gameObject;
        sceneName = SceneManager.GetActiveScene().name;

        bool shown = shownScenes.Contains(sceneName);
        firstClickButton.SetActive(!shown);
        playBGM.SetActive(shown);
    }
    public void OnButtonClick()
    {
        firstClickButton.SetActive(false);
        playBGM.SetActive(true);
        shownScenes.Add(sceneName);
    }
}
