using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    private int level;

    private void Start()
    {
        level = 1;
    }

    public int GetLevel()
    {
        return level;
    }

    internal void LevelUp()
    {
        level++;
        Debug.Log($"Un {gameObject.tag} ha subido a nivel {level}");
    }
}
