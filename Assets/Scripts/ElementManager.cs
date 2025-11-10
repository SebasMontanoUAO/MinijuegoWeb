using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour
{
    [SerializeField]
    public List<Element> elements;

    private GridManager _gridManager;

    private void Start()
    {
        _gridManager = transform.GetComponent<GridManager>();
    }

    public void GenerateElement()
    {
        if (!_gridManager.IsFull())
        {
            Tile startTile = _gridManager.GetTile(new Vector2(0, 0));
            Tile tileAvailable = _gridManager.NearestTileAvailable(startTile);
            int randomElement = Random.Range(0, elements.Count);
            var element = Instantiate(elements[randomElement], tileAvailable.transform.position, Quaternion.identity);
            tileAvailable.AssignElement(element);
        }
    }
}
