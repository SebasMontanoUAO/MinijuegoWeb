using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElementManager : MonoBehaviour
{
    [SerializeField]
    public List<Element> elements;
    [SerializeField]
    private TextMeshProUGUI costText;

    private GridManager _gridManager;

    private int adoptCost = 5;
    private int adoptedCats = 0;

    private void Start()
    {
        _gridManager = transform.GetComponent<GridManager>();
    }
    private void Update()
    {
        costText.text = $"Cost: {adoptCost}";
    }

    public void GenerateElement()
    {
        if (!_gridManager.IsFull() && GameManager.Instance.money >= adoptCost)
        {
            GameManager.Instance.money -= adoptCost;
            Tile startTile = _gridManager.GetTile(new Vector2(0, 0));
            Tile tileAvailable = _gridManager.NearestTileAvailable(startTile);
            int randomElement = Random.Range(0, elements.Count);
            var element = Instantiate(elements[randomElement], tileAvailable.transform.position, Quaternion.identity);
            tileAvailable.AssignElement(element);
            adoptedCats++;
            UpdateAdoptCost();
        }
    }

    private void UpdateAdoptCost()
    {
        adoptCost = 5 * (1 + (adoptedCats / 5));
    }
}
