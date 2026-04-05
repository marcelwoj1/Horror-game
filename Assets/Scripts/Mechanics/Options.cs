using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider slider;

    void Start()
    {
        slider.onValueChanged.AddListener(SetVolume);
    }

    void SetVolume(float value)
    {
        float volume = Mathf.Log10(value) * 20;
        audioMixer.SetFloat("Volume", volume);
    }
    public void Fullscreen()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
    }
}
