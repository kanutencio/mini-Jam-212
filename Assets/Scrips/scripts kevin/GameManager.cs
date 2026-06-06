using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Niveles")]
    public WaypointPath[] caminosPorNivel;
    public int nivelActual = 0;

    [Header("Puntuación")]
    public int heroesEliminados = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        IniciarNivel();
    }

    void IniciarNivel()
    {
        if (caminosPorNivel != null && caminosPorNivel.Length > nivelActual)
            HeroSpawner.Instance.CambiarCamino(caminosPorNivel[nivelActual]);

        HeroSpawner.Instance.SpawnHeroe();
    }

    public void HeroeMurio()
    {
        heroesEliminados++;
        Debug.Log($"Héroe eliminado. Total: {heroesEliminados}");
    }

    public void HeroeEscapo()
    {
        Debug.Log("El héroe escapó.");
    }

    public void SiguienteNivel()
    {
        nivelActual++;
        IniciarNivel();
    }
}