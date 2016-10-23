using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameUI : MonoBehaviour
{
    public Image FadePlane;
    public Transform GameOverUI;

    public RectTransform NewWaveBanner;

    private Spawner _spawner;

    void Awake()
    {
        _spawner = FindObjectOfType<Spawner>();
        _spawner.OnNewWave += OnNewWave;
    }

	void Start () {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
	}
    
    void OnNewWave(int waveNumber)
    {
        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1f;
        float speed = 2.5f;
        float percent = 0;
        int direction = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (percent >= 0)
        {
            percent += Time.deltaTime * speed * direction;

            if (percent >= 1)
            {
                percent = 1;

                if (Time.time > endDelayTime)
                {
                    direction = -1;
                }
            }

            NewWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-420f, 70f, percent);

            yield return null;
        }
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        GameOverUI.gameObject.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;

        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;

            FadePlane.color = Color.Lerp(from, to, percent);

            yield return null;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scene1");
    }
}
