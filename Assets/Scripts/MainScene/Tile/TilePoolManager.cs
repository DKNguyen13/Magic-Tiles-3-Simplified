using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePoolManager : MonoBehaviour
{
    public static TilePoolManager Instance { get; private set; }

    [SerializeField] private GameObject shortTilePrefab;
    [SerializeField] private Transform tileContainer;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> tilePool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        InitPool();
    }

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tile = Instantiate(shortTilePrefab, Vector3.zero, Quaternion.identity, tileContainer);
            tile.SetActive(false);
            tilePool.Enqueue(tile);
        }
    }

    public GameObject GetTileFromPool()
    {
        if (tilePool.Count > 0)
        {
            GameObject tile = tilePool.Dequeue();
            tile.SetActive(true);
            return tile;
        }
        else
        {
            GameObject tile = Instantiate(shortTilePrefab, Vector3.zero, Quaternion.identity, tileContainer);
            return tile;
        }
    }

    public void ReturnTileToPool(GameObject tile)
    {
        tile.SetActive(false);
        ShortTile shortTile = tile.GetComponent<ShortTile>();
        if (shortTile != null && shortTile.IsTapped)
        {
            shortTile.ResetTile();
        }
        tilePool.Enqueue(tile);
    }

    public void SpawnTileByPattern(Transform spawnPoint, Queue<GameObject> leftQueue, Queue<GameObject> rightQueue)
    {
        float minX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float maxX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        int slotCount = 4;
        float totalWidth = maxX - minX;
        float slotWidth = totalWidth / slotCount;

        bool spawnTwoTiles = Random.value < 0.05f;//95% spawn 1 tile

        if (spawnTwoTiles)
        {
            int leftSlot = Random.Range(0, 2);
            int rightSlot = Random.Range(2, 4);
            SpawnAtSlot(leftSlot);
            SpawnAtSlot(rightSlot);
        }
        else
        {
            int slot = Random.Range(0, 4);
            SpawnAtSlot(slot);
        }

        void SpawnAtSlot(int slot)
        {
            float centerX = minX + slotWidth * slot + slotWidth / 2;
            Vector3 spawnPos = new Vector3(centerX, spawnPoint.position.y, spawnPoint.position.z);

            GameObject tile = GetTileFromPool();
            tile.transform.position = spawnPos;

            ShortTile shortTile = tile.GetComponent<ShortTile>();
            if (shortTile != null)
            {
                shortTile.IsLeftSide = slot < 2;
                if (shortTile.IsLeftSide)
                    leftQueue.Enqueue(tile);
                else
                    rightQueue.Enqueue(tile);
            }
        }
    }
}
