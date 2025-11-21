using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject panelGameOver;
    [SerializeField]
    private GameObject panelWin;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void ShowGameOverPanel()
    {
        panelGameOver.SetActive(true);
    }
    public void ShowWinPanel()
    {
        panelWin.SetActive(true);
    }
}
