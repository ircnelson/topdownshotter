using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject FlashHolder;

    public Sprite[] FlashSprites;
    public SpriteRenderer[] SpriteRenderers;

    public float FlashTime;

    void Start()
    {
        Deactivate();
    }

	public void Activate()
    {
        FlashHolder.SetActive(true);

        int flashSpriteIndex = Random.Range(0, FlashSprites.Length);

        for (int i = 0; i < SpriteRenderers.Length; i++)
        {
            SpriteRenderers[i].sprite = FlashSprites[flashSpriteIndex];
        }

        Invoke("Deactivate", FlashTime);
    }

    public void Deactivate()
    {
        FlashHolder.SetActive(false);
    }
}
