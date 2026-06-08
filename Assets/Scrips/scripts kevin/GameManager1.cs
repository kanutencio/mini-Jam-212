using UnityEngine;

/// <summary>
/// GameManager del Nivel 1 (índice 0).
/// Coloca este script en el GameObject "GameManager_Nivel1" dentro del nivel 1 de la escena.
/// Configura: camino (WaypointPath del nivel 1) y delayInicio en el Inspector.
/// </summary>
public class GameManager1 : GameManager
{
    private void Reset()
    {
        // Valor por defecto al añadir el componente
        nivelIndex = 0;
        delayInicio = 3f;
    }
}
