using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel {  Master, Sfx, Music };
    public static AudioManager Instance;

    private float _masterVolumePercent = .2f;
    private float _sfxVolumePercent = 1;
    private float _musicVolumePercent = 1;

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

            _masterVolumePercent = PlayerPrefs.GetFloat("master vol", _masterVolumePercent);
            _sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", _sfxVolumePercent);
            _musicVolumePercent = PlayerPrefs.GetFloat("music vol", _musicVolumePercent);
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
                _masterVolumePercent = volumePercent;
                break;

            case AudioChannel.Sfx:
                _sfxVolumePercent = volumePercent;
                break;

            case AudioChannel.Music:
                _musicVolumePercent = volumePercent;
                break;
        }

        _musicSources[0].volume = _musicVolumePercent * _masterVolumePercent;
        _musicSources[1].volume = _musicVolumePercent * _masterVolumePercent;

        PlayerPrefs.SetFloat("master vol", _masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", _sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", _musicVolumePercent);
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
            AudioSource.PlayClipAtPoint(clip, pos, _sfxVolumePercent * _masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(_library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        _sfx2dSource.PlayOneShot(_library.GetClipFromName(soundName), _sfxVolumePercent * _masterVolumePercent);
    }

    private IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;

            _musicSources[_activeMusicSourceIndex].volume = Mathf.Lerp(0, _musicVolumePercent * _masterVolumePercent, percent);
            _musicSources[1 - _activeMusicSourceIndex].volume = Mathf.Lerp(_musicVolumePercent * _masterVolumePercent, 0, percent);

            yield return null;
        }
    }
}
