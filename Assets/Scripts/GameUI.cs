using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image FadePlane;
    public Transform GameOverUI;

	void Start () {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
	}

    void Update () {
	
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
