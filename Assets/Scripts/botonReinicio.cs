using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class botonReinicio : MonoBehaviour
{
    public void ReiniciarJuego()
    {
        // Recarga la escena activa
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Debug.Log("Juego reiniciado"); // Para verificar en la consola
    }
}
