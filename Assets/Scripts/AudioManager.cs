using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel {  Master, Sfx, Music };
    public static AudioManager Instance;

    public float MasterVolumePercent = .2f;
    public float MusicVolumePercent = 1;
    public float SfxVolumePercent = 1;

    private AudioSource _sfx2dSource;
    private AudioSource[] _musicSources;
    private int _activeMusicSourceIndex;

    private Transform _audioListener;
    private Transform _playerTransform;

    private SoundLibrary _library;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            _library = GetComponent<SoundLibrary>();

            _musicSources = new AudioSource[2];

            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                _musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }

            GameObject newSfx2Dsource = new GameObject("Sfx source");
            _sfx2dSource = newSfx2Dsource.AddComponent<AudioSource>();
            newSfx2Dsource.transform.parent = transform;

            _audioListener = FindObjectOfType<AudioListener>().transform;
            _playerTransform = FindObjectOfType<Player>().transform;

            MasterVolumePercent = PlayerPrefs.GetFloat("master vol", MasterVolumePercent);
            SfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", SfxVolumePercent);
            MusicVolumePercent = PlayerPrefs.GetFloat("music vol", MusicVolumePercent);
        }
    }

    void OnLevelWasLoaded(int index)
    {
        if (_playerTransform == null)
        {
            if (FindObjectOfType<Player>() != null)
            {
                _playerTransform = FindObjectOfType<Player>().transform;
            }
        }
    }

    void Update()
    {
        if (_playerTransform != null)
        {
            _audioListener.position = _playerTransform.position;
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                MasterVolumePercent = volumePercent;
                break;

            case AudioChannel.Sfx:
                SfxVolumePercent = volumePercent;
                break;

            case AudioChannel.Music:
                MusicVolumePercent = volumePercent;
                break;
        }

        _musicSources[0].volume = MusicVolumePercent * MasterVolumePercent;
        _musicSources[1].volume = MusicVolumePercent * MasterVolumePercent;

        PlayerPrefs.SetFloat("master vol", MasterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", SfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", MusicVolumePercent);

        PlayerPrefs.Save();
    }

    public void PlayMusc(AudioClip clip, float fadeDuration = 1)
    {
        _activeMusicSourceIndex = 1 - _activeMusicSourceIndex;

        _musicSources[_activeMusicSourceIndex].clip = clip;
        _musicSources[_activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, SfxVolumePercent * MasterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(_library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        _sfx2dSource.PlayOneShot(_library.GetClipFromName(soundName), SfxVolumePercent * MasterVolumePercent);
    }

    private IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;

            _musicSources[_activeMusicSourceIndex].volume = Mathf.Lerp(0, MusicVolumePercent * MasterVolumePercent, percent);
            _musicSources[1 - _activeMusicSourceIndex].volume = Mathf.Lerp(MusicVolumePercent * MasterVolumePercent, 0, percent);

            yield return null;
        }
    }
}
