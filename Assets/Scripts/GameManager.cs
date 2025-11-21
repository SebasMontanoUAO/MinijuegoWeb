using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private float dragReturnSpeed = 10f;
    [SerializeField]
    public int money = 25;
    [SerializeField]
    private TextMeshProUGUI moneyText;

    private Tile _selectedTile;
    private Tile _targetTile;
    private GameObject _draggedElement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        moneyText.text = money.ToString();

        if (_draggedElement == null)
            return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        _draggedElement.transform.position = mouseWorldPos;

        if (Input.GetMouseButtonUp(0))
            HandleGlobalMouseUp();
    }

    private void HandleGlobalMouseUp()
    {
        if (_draggedElement != null)
        {
            if (_selectedTile == null || _targetTile == null || _targetTile == _selectedTile || _targetTile.HasElement)
            {
                StopAllCoroutines(); // seguridad extra
                StartCoroutine(ReturnToOrigin());
                return;
            }

            MoveElement(_selectedTile, _targetTile);
        }
    }

    public void StartDrag(Tile tile)
    {
        if (tile.HasElement && _draggedElement == null)
        {
            if (tile.GridName != "GridManager")
            {
                _selectedTile = tile;
                _draggedElement = tile.Element.gameObject;
                Debug.Log($"Arrastrando desde {tile.name} en {tile.GridName}");
            }
        }
    }

    public void HoverTile(Tile tile)
    {
        if (_draggedElement != null)
            _targetTile = tile;
    }

    public void EndDrag()
    {
        if (_selectedTile != null && _targetTile != null && _targetTile != _selectedTile)
        {
            MoveElement(_selectedTile, _targetTile);
        }

        _selectedTile = null;
        _targetTile = null;
    }

    private void MoveElement(Tile from, Tile to)
    {
        if (!to.HasElement)
        {
            to.SetHasElement(true);
            to.AssignElement(_draggedElement.GetComponent<Element>());
            _draggedElement = null;
            from.SetHasElement(false);
            Debug.Log($"Elemento movido de {from.name} en {from.GridName} a {to.name} en {to.GridName}");
            to.CheckForMerge();
            to.CheckForFullGrid();
        }
        else
        {
            Debug.Log($"No se puede mover, {to.name} en {to.GridName} ya tiene elemento");
            StartCoroutine(ReturnToOrigin());
        }
    }

    private IEnumerator ReturnToOrigin()
    {
        var element = _draggedElement;
        var originTile = _selectedTile;

        if (element == null || originTile == null)
            yield break;

        Vector3 startPos = element.transform.position;
        Vector3 endPos = originTile.transform.position;
        float t = 0;

        var cachedDrag = _draggedElement;
        _draggedElement = null;

        while (t < 1f)
        {
            if (element == null) yield break;
            t += Time.deltaTime * dragReturnSpeed;
            element.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        if (originTile != null)
        {
            originTile.SetHasElement(true);
            originTile.AssignElement(element.GetComponent<Element>());
        }

        element.transform.position = endPos;

        if (_selectedTile == originTile) _selectedTile = null;
        if (_targetTile == originTile) _targetTile = null;
        if (_draggedElement == cachedDrag) _draggedElement = null;
    }

    public void TriggerGameOver()
    {
        Debug.Log($"Se ha llenado la grid");

        UIManager.Instance.ShowGameOverPanel();
    }

    public void TriggerWin(Element element)
    {
        Debug.Log($"GAME OVER: {element.gameObject.tag} alcanzó el nivel 10");

        UIManager.Instance.ShowWinPanel();
    }
}
