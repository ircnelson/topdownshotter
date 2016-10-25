using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{

    public AudioClip MainTheme;
    public AudioClip MenuTheme;

    void Start()
    {
        AudioManager.Instance.PlayMusc(MenuTheme, 2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.PlayMusc(MainTheme, 3);
        }
    }
}
