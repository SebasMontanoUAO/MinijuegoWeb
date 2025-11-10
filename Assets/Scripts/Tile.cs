using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Color baseColor, offsetColor;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private GameObject highlight;

    private GridManager _gridManager;
    private Element _element;

    private bool hasElement;
    private int _x, _y, _width, _height;

    public Tile onTop, onBottom, onRight, onLeft;

    public bool HasElement => hasElement;
    public string GridName => _gridManager.name;
    public Element Element => _element;

    public void Init(GridManager gridManager, bool isOffset, int width, int height, int x, int y)
    {
        _gridManager = gridManager;
        spriteRenderer.color = isOffset ? offsetColor : baseColor;
        _width = width;
        _height = height;
        _x = x;
        _y = y;
        hasElement = false;
    }

    private void OnMouseEnter()
    {
        GameManager.Instance.HoverTile(this);
        SetHighlightState(true);
    }
    private void OnMouseDown()
    {
        if (hasElement) GameManager.Instance.StartDrag(this);
    }


    private void OnMouseExit()
    {
        SetHighlightState(false);
        GameManager.Instance.HoverTile(null);
    }

    public Tile TileOnTop()
    {
        Tile tile = null;
        if (_y != _height - 1)
        {
            tile = _gridManager.GetTile(new Vector2(_x, _y + 1));
            return tile;
        }

        return tile;
    }

    public Tile TileOnBottom()
    {
        Tile tile = null;
        if (_y != 0)
        {
            tile = _gridManager.GetTile(new Vector2(_x, _y - 1));
            return tile;
        }

        return tile;
    }

    public Tile TileOnRight()
    {
        Tile tile = null;
        if (_x != _width - 1)
        {
            tile = _gridManager.GetTile(new Vector2(_x + 1, _y));
            return tile;
        }

        return tile;
    }

    public Tile TileOnLeft()
    {
        Tile tile = null;
        if (_x != 0)
        {
            tile = _gridManager.GetTile(new Vector2(_x - 1, _y));
            return tile;
        }

        return tile;
    }
    
    public void SetHasElement(bool element)
    {
        hasElement = element;
    }

    private void SetHighlightState(bool state)
    {
        highlight.SetActive(state);
    }

    public void AssignElement(Element element)
    {
        _element = element;
        hasElement = true;
        _element.gameObject.transform.position = this.transform.position;
    }

    public void UnlinkElement()
    {
        _element = null;
        hasElement = false;
    }

    public void CheckForMerge()
    {
        if(_element == null) return;

        string searchTag = _element.gameObject.tag;
        HashSet<Tile> connectedTiles = new HashSet<Tile>();

        FindConnectedTiles(this, searchTag, connectedTiles);

        if (connectedTiles.Count >= 3)
        {
            Tile mergeTile = GetCentralTile(connectedTiles);
            mergeTile.MergeTiles(connectedTiles);
        }
    }

    private Tile GetCentralTile(HashSet<Tile> tiles)
    {
        if (tiles == null || tiles.Count == 0) return this;

        float avgX = 0;
        float avgY = 0;

        foreach (Tile t in tiles)
        {
            avgX += t.transform.position.x;
            avgY += t.transform.position.y;
        }

        avgX /= tiles.Count;
        avgY /= tiles.Count;

        Tile closest = null;
        float closestDist = float.MaxValue;

        foreach (Tile t in tiles)
        {
            float dist = Vector2.Distance(new Vector2(avgX, avgY), t.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = t;
            }
        }

        return closest;
    }

    private void FindConnectedTiles(Tile tile, string tag, HashSet<Tile> connected)
    {
        if (tile == null || tile.HasElement == false || connected.Contains(tile))
            return;

        Element element = tile.Element;
        if (tile.Element.gameObject.tag != tag)
            return;

        int referenceLevel = connected.Count > 0 ? connected.First().Element.GetLevel() : _element.GetLevel();

        if (element.GetLevel() != referenceLevel)
            return;

        connected.Add(tile);

        FindConnectedTiles(tile.onTop, tag, connected);
        FindConnectedTiles(tile.onBottom, tag, connected);
        FindConnectedTiles(tile.onLeft, tag, connected);
        FindConnectedTiles(tile.onRight, tag, connected);
    }

    private void MergeTiles(HashSet<Tile> tiles)
    {
        Tile mergeTile = GetCentralTile(tiles);

        foreach (Tile t in tiles)
        {
            if (t == mergeTile) continue;

            if (t.Element != null)
            {
                Destroy(t.Element.gameObject);
            }
            t.UnlinkElement();
        }

        if (mergeTile.Element != null)
        {
            mergeTile.Element.LevelUp();
        }

        Debug.Log($"Se combinaron {tiles.Count} tiles de tipo {mergeTile.Element.tag} nivel {mergeTile.Element.GetLevel()}!");
        CheckForMerge();
    }
}
