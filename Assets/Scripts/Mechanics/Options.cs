using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Handles game settings such as audio volume and display mode.
/// </summary>
/// <remarks>
/// This system:
/// - Adjusts master audio volume using an AudioMixer
/// - Converts linear slider values into logarithmic decibel values
/// - Toggles fullscreen mode at runtime
/// </remarks>
public class Options : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Reference to the audio mixer controlling game volume.</summary>
    public AudioMixer audioMixer;

    /// <summary>UI slider used to control volume.</summary>
    public Slider slider;

    /// <summary>
    /// Initialises the volume slider listener.
    /// </summary>
    void Start()
    {
        slider.onValueChanged.AddListener(SetVolume);
    }

    /// <summary>
    /// Sets the game volume based on slider input.
    /// </summary>
    /// <param name="value">Slider value (expected range 0.0–1.0).</param>
    /// <remarks>
    /// Converts linear slider values to decibel scale using logarithmic conversion.
    /// This provides a more natural volume adjustment for users.
    /// </remarks>
    void SetVolume(float value)
    {
        float volume = Mathf.Log10(value) * 20;
        audioMixer.SetFloat("Volume", volume);
    }

    /// <summary>
    /// Toggles between fullscreen and windowed display modes.
    /// </summary>
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