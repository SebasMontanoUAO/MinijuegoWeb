using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int width, height;

    [SerializeField]
    private Tile tilePrefab;

    private Dictionary<Vector2, Tile> _tiles;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(transform.position.x + x, transform.position.y + y -1.2f), Quaternion.identity);
                spawnedTile.name = $"Tile {x}, {y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(this, isOffset, width, height, x, y);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }

            foreach (var kvp in _tiles)
            {
                Vector2 pos = kvp.Key;
                Tile tile = kvp.Value;

                tile.onTop = tile.TileOnTop();
                tile.onBottom = tile.TileOnBottom();
                tile.onLeft = tile.TileOnLeft();
                tile.onRight = tile.TileOnRight();
            }
        }
    }

    public Tile GetTile(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }

    public bool IsFull()
    {
        if(_tiles == null || _tiles.Count == 0)
        return false;

        Tile startTile = GetTile(Vector2.zero);
        if (startTile == null)
            return false;

        HashSet<Tile> visited = new HashSet<Tile>();
        return CheckTileRecursively(startTile, visited);
    }

    private bool CheckTileRecursively(Tile current, HashSet<Tile> visited)
    {
        if (current == null || visited.Contains(current))
            return true;

        visited.Add(current);

        if (!current.HasElement)
            return false;

        if (!CheckTileRecursively(current.TileOnTop(), visited)) return false;
        if (!CheckTileRecursively(current.TileOnBottom(), visited)) return false;
        if (!CheckTileRecursively(current.TileOnLeft(), visited)) return false;
        if (!CheckTileRecursively(current.TileOnRight(), visited)) return false;

        return true;
    }

    public Tile NearestTileAvailable(Tile startTile)
    {
        if (startTile == null)
            return null;

        HashSet<Tile> visited = new HashSet<Tile>();
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(startTile);
        visited.Add(startTile);

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();

            if (!current.HasElement)
                return current;

            Tile right = current.TileOnRight();
            Tile bottom = current.TileOnBottom();
            Tile top = current.TileOnTop();
            Tile left = current.TileOnLeft();

            if (top != null && !visited.Contains(top))
            {
                visited.Add(top);
                queue.Enqueue(top);
            }

            if (bottom != null && !visited.Contains(bottom))
            {
                visited.Add(bottom);
                queue.Enqueue(bottom);
            }

            if (left != null && !visited.Contains(left))
            {
                visited.Add(left);
                queue.Enqueue(left);
            }

            if (right != null && !visited.Contains(right))
            {
                visited.Add(right);
                queue.Enqueue(right);
            }
        }

        return null;
    }
}
