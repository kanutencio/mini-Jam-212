using UnityEngine;

/// <summary>
/// GameManager del Nivel 2 (índice 1).
/// Coloca este script en el GameObject "GameManager_Nivel2" dentro del nivel 2 de la escena.
/// Configura: camino (WaypointPath del nivel 2) y delayInicio en el Inspector.
/// Debe estar DESACTIVADO al inicio de la escena — se activa cuando la cámara llega al nivel 2.
/// </summary>
public class GameManager2 : GameManager
{
    private void Reset()
    {
        nivelIndex = 1;
        delayInicio = 3f;
    }
}
