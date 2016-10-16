using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    public Transform TilePrefab;
    public Vector2 MapSize;

    [Range(0, 1)]
    public float OutlinePercent;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        string holderName = "Generated Map";

        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHoder = new GameObject(holderName).transform;
        mapHoder.parent = transform;

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Vector3 tilePosition = new Vector3(-MapSize.x / 2 + 0.5f + x, 0, -MapSize.y / 2 + 0.5f + y);
                Transform newTile = Instantiate(TilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - OutlinePercent);

                newTile.parent = mapHoder;
            }
        }
    }
}
