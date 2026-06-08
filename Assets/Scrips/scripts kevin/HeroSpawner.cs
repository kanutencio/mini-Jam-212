using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroSpawner : MonoBehaviour
{
    [Header("Prefabs de Enemigos")]
    public GameObject heroPrefab;
    public GameObject soldadoPrefab;

    [Header("Configuración de Oleada")]
    public int baseSoldiersCount = 2;
    public int soldiersIncreasePerLevel = 1;
    public float tiempoEntreSpawns = 1.0f;

    [Header("Camino activo")]
    public WaypointPath waypointPath;

    private List<GameObject> enemigosActivos = new List<GameObject>();
    private int soldadosPorSpawnear = 0;
    private bool esperandoAlHeroe = false;
    private bool nivelTerminado = false; // evita que múltiples soldados disparen el cambio

    public static HeroSpawner Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void SpawnHeroe()
    {
        enemigosActivos.RemoveAll(item => item == null);
        nivelTerminado = false;

        if (enemigosActivos.Count > 0)
        {
            Debug.LogWarning("[HeroSpawner] Ya hay una oleada en progreso.");
            return;
        }

        int nivelActual = GameManager.nivelActual;
        soldadosPorSpawnear = baseSoldiersCount + (nivelActual * soldiersIncreasePerLevel);
        esperandoAlHeroe = false;

        Debug.Log($"[HeroSpawner] Iniciar Spawn. nivelActual={nivelActual}, soldados={soldadosPorSpawnear}");

        if (soldadosPorSpawnear > 0 && soldadoPrefab != null)
            StartCoroutine(SpawnSoldadosCoroutine());
        else
            SpawnHeroeFinal();
    }

    private IEnumerator SpawnSoldadosCoroutine()
    {
        int count = 0;
        while (soldadosPorSpawnear > 0)
        {
            if (nivelTerminado) yield break; // si ya terminó el nivel, parar

            if (soldadoPrefab == null || waypointPath == null)
            {
                Debug.LogError("[HeroSpawner] Faltan referencias en la corrutina.");
                yield break;
            }

            Vector3 spawnPos = waypointPath.GetWaypoint(0).position;
            float targetZ = 4f;
            HeroMover moverPrefab = soldadoPrefab.GetComponent<HeroMover>();
            if (moverPrefab != null) targetZ = moverPrefab.posicionZ;
            spawnPos.z = targetZ;

            GameObject soldado = Instantiate(soldadoPrefab, spawnPos, Quaternion.identity);
            enemigosActivos.Add(soldado);
            count++;

            Debug.Log($"[HeroSpawner] Soldadito #{count} instanciado.");

            HeroMover mover = soldado.GetComponent<HeroMover>();
            if (mover != null)
            {
                mover.SetPath(waypointPath);
                mover.onReachedEnd = () => OnEnemigoFinalizado(soldado, true, false);
                mover.onDeath = () => OnEnemigoFinalizado(soldado, false, false);
            }

            soldadosPorSpawnear--;

            if (soldadosPorSpawnear > 0)
                yield return new WaitForSeconds(tiempoEntreSpawns);
        }
        Debug.Log("[HeroSpawner] Todos los soldaditos instanciados.");
    }

    private void SpawnHeroeFinal()
    {
        if (heroPrefab == null || waypointPath == null)
        {
            Debug.LogError("[HeroSpawner] Faltan referencias para spawnear héroe.");
            return;
        }

        esperandoAlHeroe = true;
        Debug.Log("[HeroSpawner] Instanciando al Héroe Final...");

        Vector3 spawnPos = waypointPath.GetWaypoint(0).position;
        float targetZ = 4f;
        HeroMover moverPrefab = heroPrefab.GetComponent<HeroMover>();
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
    }

    private void OnEnemigoFinalizado(GameObject enemigo, bool escapo, bool esElHeroe)
    {
        if (enemigosActivos.Contains(enemigo))
            enemigosActivos.Remove(enemigo);

        Debug.Log($"[HeroSpawner] {(esElHeroe ? "HEROE" : "SOLDADO")} finalizado, escapó={escapo}, activos={enemigosActivos.Count}");

        if (!esElHeroe)
        {
            if (escapo)
            {
                if (nivelTerminado) return; // ya se procesó un soldado que escapó
                nivelTerminado = true;

                // Detener spawns y destruir soldados restantes
                StopAllCoroutines();
                soldadosPorSpawnear = 0;
                esperandoAlHeroe = false;

                // Destruir soldados que quedaron en camino
                foreach (var e in enemigosActivos)
                {
                    if (e != null) Destroy(e);
                }
                enemigosActivos.Clear();

                GameManager.Instance?.SoldadoEscapo();
            }
            else if (enemigosActivos.Count == 0 && soldadosPorSpawnear == 0 && !esperandoAlHeroe && !nivelTerminado)
            {
                SpawnHeroeFinal();
            }
        }
        else
        {
            if (escapo)
                GameManager.Instance.HeroeEscapo();
            else
                GameManager.Instance.HeroeMurio();
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
        return enemigosActivos.Count > 0 ? enemigosActivos[0] : null;
    }

    public List<GameObject> GetTodosLosEnemigosActivos()
    {
        enemigosActivos.RemoveAll(item => item == null);
        return enemigosActivos;
    }
}