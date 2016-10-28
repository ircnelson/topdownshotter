using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Slider[] Volumes;

    void Start()
    {
        Volumes[0].value = AudioManager.Instance.MasterVolumePercent;
        Volumes[1].value = AudioManager.Instance.MusicVolumePercent;
        Volumes[2].value = AudioManager.Instance.SfxVolumePercent;
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume(float value)
    {
        AudioManager.Instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
}
