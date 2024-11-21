using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public TextAsset mapFile; // Map file
    public GameObject[] tilePrefabs; // Array of tile prefabs, stored in order by ID
    public int tileSize = 1; // Size of each tile
    public Vector2 mapOffset = new Vector2(0, 0); // Starting offset of the map
    public Transform player; // Reference to the player, used to calculate the viewing range
    public int screenTileWidth = 10; // Number of tiles to render horizontally
    public int screenTileHeight = 6; // Number of tiles to render vertically

    // Define which tiles are solid (for collision)
    public bool[] solidTiles; // Array indicating if a tile type is solid (true) or not (false)

    public string[][] mapData; // 2D array to store the map data
    private GameObject[,] tileInstances; // Stores the instantiated tiles

    void Start()
    {
        LoadMapData();
        InitializeTileInstances();
        UpdateVisibleTiles();

        
    }

    void Update()
    {
        UpdateVisibleTiles();
    }

    // Read map data into a 2D array
    void LoadMapData()
    {
        string[] lines = mapFile.text.Split('\n');
        mapData = new string[lines.Length][];
        for (int row = 0; row < lines.Length; row++)
        {
            mapData[row] = lines[row].Trim().Split(' ');
        }
    }

    // Initialize all tile objects but do not activate them
    void InitializeTileInstances()
    {
        int mapWidth = mapData[0].Length;
        int mapHeight = mapData.Length;

        tileInstances = new GameObject[mapWidth, mapHeight];

        for (int row = 0; row < mapHeight; row++)
        {
            for (int col = 0; col < mapWidth; col++)
            {
                int tileID;
                if (int.TryParse(mapData[row][col], out tileID) && tileID >= 0 && tileID < tilePrefabs.Length)
                {
                    Vector3 position = new Vector3(col * tileSize + mapOffset.x, -row * tileSize + mapOffset.y, 0);
                    tileInstances[col, row] = Instantiate(tilePrefabs[tileID], position, Quaternion.identity, transform);

                    // Check if the tile is solid and add a BoxCollider2D if necessary
                    if (solidTiles.Length > tileID && solidTiles[tileID])
                    {
                        BoxCollider2D collider = tileInstances[col, row].AddComponent<BoxCollider2D>();
                        collider.isTrigger = false; // Set the collider to be solid
                    }

                    tileInstances[col, row].SetActive(false); // Initially not activated
                }
            }
        }
    }

    // Update the visibility of tiles within the viewport range
    void UpdateVisibleTiles()
    {
        int playerTileX = Mathf.RoundToInt((player.position.x - mapOffset.x) / tileSize);
        int playerTileY = Mathf.RoundToInt((player.position.y - mapOffset.y) / -tileSize);

        for (int row = 0; row < mapData.Length; row++)
        {
            for (int col = 0; col < mapData[0].Length; col++)
            {
                // Calculate if the tile is in the viewport range
                bool inView = Mathf.Abs(col - playerTileX) <= screenTileWidth / 2 &&
                              Mathf.Abs(row - playerTileY) <= screenTileHeight / 2;

                if (tileInstances[col, row] != null)
                {
                    tileInstances[col, row].SetActive(inView); // Activate only the tiles within the viewport
                }
            }
        }
    }
    
}
