﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public Transform ObstaclePrefab;
    public Transform TilePrefab;
    public Vector2 MapSize;

    [Range(0, 1)]
    public float OutlinePercent;
    public int Seed = 10;

    private List<Coord> _allTileCoords;
    private Queue<Coord> _shuffledTileCoords;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        _allTileCoords = new List<Coord>();

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                _allTileCoords.Add(new Coord(x, y));
            }
        }

        _shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), Seed));

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
                Vector3 tilePosition = CoordToPosition(x, y);

                Transform newTile = Instantiate(TilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - OutlinePercent);

                newTile.parent = mapHoder;
            }
        }

        int obstacleCount = 10;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            Vector3 obstaclePosition = CoordToPosition(randomCoord.X, randomCoord.Y);

            Transform newObstacle = Instantiate(ObstaclePrefab, obstaclePosition + Vector3.up * 0.5f , Quaternion.identity) as Transform;
            newObstacle.parent = mapHoder;
        }
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-MapSize.x / 2 + 0.5f + x, 0, -MapSize.y / 2 + 0.5f + y);
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = _shuffledTileCoords.Dequeue();

        _shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    public struct Coord
    {
        public int X;
        public int Y;

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
