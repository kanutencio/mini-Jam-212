using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroSpawner : MonoBehaviour
{
    [Header("Prefabs de Enemigos")]
    [Tooltip("Prefab del héroe final del nivel.")]
    public GameObject heroPrefab;
    [Tooltip("Prefab de los soldaditos que aparecen antes del héroe.")]
    public GameObject soldadoPrefab;

    [Header("Configuración de Oleada")]
    [Tooltip("Cantidad de soldados base en el primer nivel (nivel 0).")]
    public int baseSoldiersCount = 2;
    [Tooltip("Cuántos soldados adicionales se agregan por cada nivel extra.")]
    public int soldiersIncreasePerLevel = 1;
    [Tooltip("Tiempo en segundos entre la aparición de cada soldado.")]
    public float tiempoEntreSpawns = 1.0f;

    [Header("Camino activo")]
    public WaypointPath waypointPath;

    private List<GameObject> enemigosActivos = new List<GameObject>();
    private int soldadosPorSpawnear = 0;
    private bool esperandoAlHeroe = false;

    public static HeroSpawner Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void SpawnHeroe()
    {
        enemigosActivos.RemoveAll(item => item == null);

        if (enemigosActivos.Count > 0)
        {
            Debug.LogWarning("[HeroSpawner] Ya hay una oleada en progreso. Cantidad activa: " + enemigosActivos.Count);
            return;
        }

        int nivelActual = GameManager.Instance != null ? GameManager.Instance.nivelActual : 0;
        soldadosPorSpawnear = baseSoldiersCount + (nivelActual * soldiersIncreasePerLevel);
        esperandoAlHeroe = false;

        Debug.Log($"[HeroSpawner] Iniciar Spawn. nivelActual={nivelActual}, baseSoldiersCount={baseSoldiersCount}, soldadosPorSpawnear={soldadosPorSpawnear}, soldadoPrefabIsNull={(soldadoPrefab == null)}, heroPrefabIsNull={(heroPrefab == null)}");

        if (soldadosPorSpawnear > 0 && soldadoPrefab != null)
        {
            Debug.Log("[HeroSpawner] Iniciando corrutina para spawnear soldaditos...");
            StartCoroutine(SpawnSoldadosCoroutine());
        }
        else
        {
            Debug.Log($"[HeroSpawner] Saltando soldados (soldadosPorSpawnear={soldadosPorSpawnear}, soldadoPrefabIsNull={(soldadoPrefab == null)}). Spawneando héroe directamente.");
            SpawnHeroeFinal();
        }
    }

    private IEnumerator SpawnSoldadosCoroutine()
    {
        int count = 0;
        while (soldadosPorSpawnear > 0)
        {
            if (soldadoPrefab == null || waypointPath == null)
            {
                Debug.LogError($"[HeroSpawner] Error en corrutina: soldadoPrefabIsNull={(soldadoPrefab == null)}, waypointPathIsNull={(waypointPath == null)}");
                yield break;
            }

            Vector3 spawnPos = waypointPath.GetWaypoint(0).position;
            
            // Obtenemos la Z que el prefab tiene configurada en su HeroMover
            float targetZ = 4f;
            HeroMover moverPrefab = soldadoPrefab != null ? soldadoPrefab.GetComponent<HeroMover>() : null;
            if (moverPrefab != null) targetZ = moverPrefab.posicionZ;
            spawnPos.z = targetZ;

            GameObject soldado = Instantiate(soldadoPrefab, spawnPos, Quaternion.identity);
            enemigosActivos.Add(soldado);
            count++;

            Debug.Log($"[HeroSpawner] Soldadito #{count} instanciado exitosamente.");

            HeroMover mover = soldado.GetComponent<HeroMover>();
            if (mover != null)
            {
                mover.SetPath(waypointPath);
                mover.onReachedEnd = () => OnEnemigoFinalizado(soldado, true, false);
                mover.onDeath = () => OnEnemigoFinalizado(soldado, false, false);
            }
            else
            {
                Debug.LogWarning("[HeroSpawner] El prefab del soldado no tiene el componente HeroMover.");
            }

            soldadosPorSpawnear--;

            if (soldadosPorSpawnear > 0)
            {
                yield return new WaitForSeconds(tiempoEntreSpawns);
            }
        }
        Debug.Log("[HeroSpawner] Todos los soldaditos de la oleada han sido instanciados.");
    }

    private void SpawnHeroeFinal()
    {
        if (heroPrefab == null || waypointPath == null)
        {
            Debug.LogError($"[HeroSpawner] Error al spawnear héroe: heroPrefabIsNull={(heroPrefab == null)}, waypointPathIsNull={(waypointPath == null)}");
            return;
        }

        esperandoAlHeroe = true;
        Debug.Log("[HeroSpawner] Instanciando al Héroe Final...");

        Vector3 spawnPos = waypointPath.GetWaypoint(0).position;
        
        // Obtenemos la Z que el prefab tiene configurada en su HeroMover
        float targetZ = 4f;
        HeroMover moverPrefab = heroPrefab != null ? heroPrefab.GetComponent<HeroMover>() : null;
        if (moverPrefab != null) targetZ = moverPrefab.posicionZ;
        spawnPos.z = targetZ;

        GameObject heroe = Instantiate(heroPrefab, spawnPos, Quaternion.identity);
        enemigosActivos.Add(heroe);

        HeroMover mover = heroe.GetComponent<HeroMover>();
        if (mover != null)
        {
            mover.SetPath(waypointPath);
            mover.onReachedEnd = () => OnEnemigoFinalizado(heroe, true, true);
            mover.onDeath = () => OnEnemigoFinalizado(heroe, false, true);
        }
        else
        {
            Debug.LogWarning("[HeroSpawner] El prefab del héroe no tiene el componente HeroMover.");
        }
    }

    private void OnEnemigoFinalizado(GameObject enemigo, bool escapo, bool esElHeroe)
    {
        if (enemigosActivos.Contains(enemigo))
        {
            enemigosActivos.Remove(enemigo);
        }

        Debug.Log($"[HeroSpawner] Enemigo finalizado: {(esElHeroe ? "HEROE" : "SOLDADO")}, escapó={escapo}, activos restantes={enemigosActivos.Count}");

        if (!esElHeroe)
        {
            if (enemigosActivos.Count == 0 && soldadosPorSpawnear == 0 && !esperandoAlHeroe)
            {
                SpawnHeroeFinal();
            }
        }
        else
        {
            if (escapo)
            {
                GameManager.Instance.HeroeEscapo();
            }
            else
            {
                GameManager.Instance.HeroeMurio();
            }
        }
    }

    public void CambiarCamino(WaypointPath nuevoCamino)
    {
        waypointPath = nuevoCamino;
    }

    public bool HayHeroeActivo()
    {
        enemigosActivos.RemoveAll(item => item == null);
        return enemigosActivos.Count > 0;
    }

    public GameObject GetHeroeActivo()
    {
        enemigosActivos.RemoveAll(item => item == null);
        if (enemigosActivos.Count > 0)
        {
            return enemigosActivos[0];
        }
        return null;
    }

    public List<GameObject> GetTodosLosEnemigosActivos()
    {
        enemigosActivos.RemoveAll(item => item == null);
        return enemigosActivos;
    }
}