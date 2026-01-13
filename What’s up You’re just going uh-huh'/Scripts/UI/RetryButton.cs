using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    public void OnRetryButton()
    {
        SceneManager.LoadScene("Title");
        AudioManager.audioManagerInstance.SoundStop(1);
    }
}
