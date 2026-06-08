using UnityEngine;

/// <summary>
/// GameManager del Nivel 3 (índice 2).
/// Coloca este script en el GameObject "GameManager_Nivel3" dentro del nivel 3 de la escena.
/// Configura: camino (WaypointPath del nivel 3) y delayInicio en el Inspector.
/// Debe estar DESACTIVADO al inicio de la escena — se activa cuando la cámara llega al nivel 3.
/// </summary>
public class GameManager3 : GameManager
{
    private void Reset()
    {
        nivelIndex = 2;
        delayInicio = 3f;
    }
}
