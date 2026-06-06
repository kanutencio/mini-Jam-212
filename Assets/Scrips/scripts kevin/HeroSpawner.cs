using UnityEngine;

public class HeroSpawner : MonoBehaviour
{
    [Header("Héroe")]
    public GameObject heroPrefab;

    [Header("Camino activo")]
    public WaypointPath waypointPath;

    private GameObject heroActivo = null;

    public static HeroSpawner Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void SpawnHeroe()
    {
        if (heroActivo != null)
        {
            Debug.Log("Ya hay un héroe en camino.");
            return;
        }

        if (heroPrefab == null)
        {
            Debug.LogWarning("HeroSpawner: no hay prefab asignado.");
            return;
        }

        if (waypointPath == null)
        {
            Debug.LogWarning("HeroSpawner: no hay WaypointPath asignado.");
            return;
        }

        Vector3 spawnPos = waypointPath.GetWaypoint(0).position;
        heroActivo = Instantiate(heroPrefab, spawnPos, Quaternion.identity);

        HeroMover mover = heroActivo.GetComponent<HeroMover>();
        if (mover != null)
        {
            mover.SetPath(waypointPath);
            mover.onReachedEnd = OnHeroeEscapo;
            mover.onDeath = OnHeroeMurio;
        }
    }

    public void CambiarCamino(WaypointPath nuevoCamino)
    {
        waypointPath = nuevoCamino;
    }

    void OnHeroeEscapo()
    {
        heroActivo = null;
        GameManager.Instance.HeroeEscapo();
    }

    void OnHeroeMurio()
    {
        heroActivo = null;
        GameManager.Instance.HeroeMurio();
    }

    public bool HayHeroeActivo() => heroActivo != null;
    public GameObject GetHeroeActivo() => heroActivo;
}