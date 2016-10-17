using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public Transform NavmeshObstacleMaskPrefab;
    public Transform ObstaclePrefab;
    public Transform TilePrefab;
    public Vector2 MapSize;
    public Vector2 MaxMapSize;
    
    public Transform NavmeshFloor;

    [Range(0, 1)]
    public float OutlinePercent;
    public int Seed = 10;

    public float TileSize = 1;

    [Range(0, 1)]
    public float ObstaclePercent;

    private Coord _mapCentre;

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
        _mapCentre = new Coord((int)MapSize.x / 2, (int)MapSize.y / 2);

        string holderName = "Generated Map";

        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);

                Transform newTile = Instantiate(TilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - OutlinePercent) * TileSize;

                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)MapSize.x, (int)MapSize.y];

        int obstacleCount = (int)(MapSize.x * MapSize.y * ObstaclePercent);

        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.X, randomCoord.Y] = true;
            currentObstacleCount++;

            if (randomCoord != _mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.X, randomCoord.Y);

                Transform newObstacle = Instantiate(ObstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                newObstacle.localScale = Vector3.one * (1 - OutlinePercent) * TileSize;
                newObstacle.parent = mapHolder;
            }
            else
            {
                obstacleMap[randomCoord.X, randomCoord.Y] = false;
                currentObstacleCount--;
            }
        }

        CreateBorders(mapHolder);

        NavmeshFloor.localScale = new Vector3(MaxMapSize.x, MaxMapSize.y) * TileSize;
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = _shuffledTileCoords.Dequeue();

        _shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    private void CreateBorders(Transform mapHolder)
    {
        Transform maskLeft = Instantiate(NavmeshObstacleMaskPrefab, Vector3.left * (MapSize.x + MaxMapSize.x) / 4 * TileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((MaxMapSize.x - MapSize.x) / 2, 1, MapSize.y) * TileSize;

        Transform maskRight = Instantiate(NavmeshObstacleMaskPrefab, Vector3.right * (MapSize.x + MaxMapSize.x) / 4 * TileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((MaxMapSize.x - MapSize.x) / 2, 1, MapSize.y) * TileSize;


        Transform maskTop = Instantiate(NavmeshObstacleMaskPrefab, Vector3.forward * (MapSize.y + MaxMapSize.y) / 4 * TileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(MaxMapSize.x, 1, (MaxMapSize.y - MapSize.y) / 2) * TileSize;

        Transform maskBottom = Instantiate(NavmeshObstacleMaskPrefab, Vector3.back * (MapSize.y + MaxMapSize.y) / 4 * TileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(MaxMapSize.x, 1, (MaxMapSize.y - MapSize.y) / 2) * TileSize;
    }

    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-MapSize.x / 2 + 0.5f + x, 0, -MapSize.y / 2 + 0.5f + y) * TileSize;
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];

        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(_mapCentre);
        mapFlags[_mapCentre.X, _mapCentre.Y] = true;

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

        int targetAccessibleTileCount = (int)(MapSize.x * MapSize.y - currentObstacleCount);

        return targetAccessibleTileCount == accessibleTileCount;
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
        
        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.X == c2.X && c1.Y == c2.Y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }
}
