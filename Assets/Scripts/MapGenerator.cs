using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour
{
    public Map[] Maps;
    public int MapIndex;

    public Transform NavmeshObstacleMaskPrefab;
    public Transform ObstaclePrefab;
    public Transform TilePrefab;
    public Vector2 MaxMapSize;
    
    public Transform NavmeshFloor;

    [Range(0, 1)]
    public float OutlinePercent;

    public float TileSize = 1;
    
    private List<Coord> _allTileCoords;
    private Queue<Coord> _shuffledTileCoords;

    private Queue<Coord> _shuffledOpenTileCoords;
    private Transform[,] _tileMap;

    Map _currentMap;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        _currentMap = Maps[MapIndex];

        _tileMap = new Transform[_currentMap.MapSize.X, _currentMap.MapSize.Y];

        System.Random prng = new System.Random(_currentMap.Seed);
        GetComponent<BoxCollider>().size = new Vector3(_currentMap.MapSize.X * TileSize, .05f, _currentMap.MapSize.Y * TileSize);

        // Generating coords
        _allTileCoords = new List<Coord>();
        for (int x = 0; x < _currentMap.MapSize.X; x++)
        {
            for (int y = 0; y < _currentMap.MapSize.Y; y++)
            {
                _allTileCoords.Add(new Coord(x, y));
            }
        }

        _shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), _currentMap.Seed));

        // Create map holder object
        string holderName = "Generated Map";

        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // Spawning tiles
        for (int x = 0; x < _currentMap.MapSize.X; x++)
        {
            for (int y = 0; y < _currentMap.MapSize.Y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);

                Transform newTile = Instantiate(TilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - OutlinePercent) * TileSize;
                newTile.parent = mapHolder;

                _tileMap[x, y] = newTile;
            }
        }

        // Spawning obstacles
        bool[,] obstacleMap = new bool[_currentMap.MapSize.X, _currentMap.MapSize.Y];

        int obstacleCount = (int)(_currentMap.MapSize.X * _currentMap.MapSize.Y * _currentMap.ObstaclePercent);
        int currentObstacleCount = 0;

        var allOpenCoords = new List<Coord>(_allTileCoords);

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.X, randomCoord.Y] = true;
            currentObstacleCount++;

            if (randomCoord != _currentMap.MapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(_currentMap.MinObstacleHeight, _currentMap.MaxObstacleHeight, (float)prng.NextDouble());

                Vector3 obstaclePosition = CoordToPosition(randomCoord.X, randomCoord.Y);

                Transform newObstacle = Instantiate(ObstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;
                newObstacle.localScale = new Vector3((1 - OutlinePercent) * TileSize, obstacleHeight, (1 - OutlinePercent) * TileSize);
                newObstacle.parent = mapHolder;

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.Y / (float)_currentMap.MapSize.Y;
                obstacleMaterial.color = Color.Lerp(_currentMap.ForegroundColor, _currentMap.BackgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.X, randomCoord.Y] = false;
                currentObstacleCount--;
            }
        }

        _shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), _currentMap.Seed));

        CreateBorders(mapHolder);

        NavmeshFloor.localScale = new Vector3(MaxMapSize.x, MaxMapSize.y) * TileSize;
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / TileSize + (_currentMap.MapSize.X - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / TileSize + (_currentMap.MapSize.Y - 1) / 2f);

        x = Mathf.Clamp(x, 0, _tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, _tileMap.GetLength(1) - 1);

        return _tileMap[x, y];
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = _shuffledTileCoords.Dequeue();

        _shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = _shuffledOpenTileCoords.Dequeue();

        _shuffledTileCoords.Enqueue(randomCoord);

        return _tileMap[randomCoord.X, randomCoord.Y];
    }

    private void CreateBorders(Transform mapHolder)
    {
        Transform maskLeft = Instantiate(NavmeshObstacleMaskPrefab, Vector3.left * (_currentMap.MapSize.X + MaxMapSize.x) / 4f * TileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((MaxMapSize.x - _currentMap.MapSize.X) / 2f, 1, _currentMap.MapSize.Y) * TileSize;

        Transform maskRight = Instantiate(NavmeshObstacleMaskPrefab, Vector3.right * (_currentMap.MapSize.X + MaxMapSize.x) / 4f * TileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((MaxMapSize.x - _currentMap.MapSize.X) / 2f, 1, _currentMap.MapSize.Y) * TileSize;


        Transform maskTop = Instantiate(NavmeshObstacleMaskPrefab, Vector3.forward * (_currentMap.MapSize.Y + MaxMapSize.y) / 4f * TileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(MaxMapSize.x, 1f, (MaxMapSize.y - _currentMap.MapSize.Y) / 2f) * TileSize;

        Transform maskBottom = Instantiate(NavmeshObstacleMaskPrefab, Vector3.back * (_currentMap.MapSize.Y + MaxMapSize.y) / 4f * TileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(MaxMapSize.x, 1f, (MaxMapSize.y - _currentMap.MapSize.Y) / 2f) * TileSize;
    }
    
    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-_currentMap.MapSize.X / 2f + 0.5f + x, 0, -_currentMap.MapSize.Y / 2f + 0.5f + y) * TileSize;
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];

        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(_currentMap.MapCentre);
        mapFlags[_currentMap.MapCentre.X, _currentMap.MapCentre.Y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.X + x;
                    int neighbourY = tile.Y + y;

                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = _currentMap.MapSize.X * _currentMap.MapSize.Y - currentObstacleCount;

        return targetAccessibleTileCount == accessibleTileCount;
    }

    [Serializable]
    public struct Coord
    {
        public int X;
        public int Y;

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.X == c2.X && c1.Y == c2.Y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

    [Serializable]
    public class Map
    {
        public Coord MapSize;

        [Range(0, 1)]
        public float ObstaclePercent;
        public int Seed;
        public float MinObstacleHeight;
        public float MaxObstacleHeight;
        public Color ForegroundColor;
        public Color BackgroundColor;
        public Coord MapCentre
        {
            get
            {
                return new Coord(MapSize.X / 2, MapSize.Y / 2);
            }
        }
    }
}
