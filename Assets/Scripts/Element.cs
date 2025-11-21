using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Element : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private float productionInterval = 2f;
    [SerializeField]
    private int baseProduction = 1;

    private int level;
    private bool alreadyGenerating;

    private void Start()
    {
        level = 1;
        alreadyGenerating = false;
    }

    private void Update()
    {
        levelText.text = $"Lvl. {level}";
    }

    public int GetLevel()
    {
        return level;
    }

    internal void LevelUp()
    {
        level++;
        Debug.Log($"Un {gameObject.tag} ha subido a nivel {level}");

        if (level >= 10)
        {
            GameManager.Instance.TriggerWin(this);
        }
    }

    public void startGenerating()
    {
        if (!alreadyGenerating)
        {
            alreadyGenerating = true;
            StartCoroutine(GenerateMoneyRoutine());
        }
    }

    private IEnumerator GenerateMoneyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionInterval);

            int amount = baseProduction * level;
            GameManager.Instance.money += amount;
        }
    }
}
