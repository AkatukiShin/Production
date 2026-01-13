using UnityEngine;
using UnityEngine.UI;

public class SliderVolumeController : MonoBehaviour
{
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider seVolumeSlider;

    public void Start()
    {
        bgmVolumeSlider.onValueChanged.AddListener(delegate {BgmValueChangeCheck();});
        seVolumeSlider.onValueChanged.AddListener(delegate{SeValueChangeCheck();});
    }

    private void BgmValueChangeCheck()
    {
        PlayBGM.instance.songAtomSource.volume = bgmVolumeSlider.value;
        AudioManager.bgmVolume = bgmVolumeSlider.value;
    }
    
    private void SeValueChangeCheck()
    {
        PlayBGM.instance.tempoAtomSource.volume = seVolumeSlider.value;
        AudioManager.seVolume = seVolumeSlider.value;
    }
}
