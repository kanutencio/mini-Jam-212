using UnityEngine;

public enum Direccion { Arriba, Abajo, Izquierda, Derecha }

public class FireTrap : MonoBehaviour
{
    [Header("Stats")]
    public float daño = 20f;
    public float fireRate = 2f;
    public float flameDuration = 0.6f;
    public float rango = 20f;

    [Header("Prefab")]
    public GameObject flamePrefab;

    // La dirección ahora se calcula automáticamente
    private Direccion direccion;

    [Header("Puntos de disparo por dirección")]
    public Transform firePointArriba;
    public Transform firePointAbajo;
    public Transform firePointIzquierda;
    public Transform firePointDerecha;

    private float _timer;
    private bool orientada = false;

    void Start()
    {
        // Empieza cargada para disparar al instante la primera vez
        _timer = fireRate;
    }

    void Update()
    {
        if (!orientada)
        {
            orientada = OrientarHaciaCamino();
        }

        // El tiempo de recarga avanza siempre, independientemente de si hay héroe o no
        _timer += Time.deltaTime;

        GameObject heroe = HeroSpawner.Instance != null ? HeroSpawner.Instance.GetHeroeActivo() : null;
        if (heroe == null) return;

        float distancia = Vector3.Distance(transform.position, heroe.transform.position);
        if (distancia > rango) return;

        Vector3 dirHeroe = (heroe.transform.position - transform.position).normalized;
        float dot = 0f;

        switch (direccion)
        {
            case Direccion.Arriba:    dot = Vector3.Dot(dirHeroe, Vector3.up); break;
            case Direccion.Abajo:     dot = Vector3.Dot(dirHeroe, Vector3.down); break;
            case Direccion.Izquierda: dot = Vector3.Dot(dirHeroe, Vector3.left); break;
            case Direccion.Derecha:   dot = Vector3.Dot(dirHeroe, Vector3.right); break;
        }

        if (dot < 0.5f) return;

        if (_timer >= fireRate)
        {
            _timer = 0f;
            Shoot();
        }
    }

    bool OrientarHaciaCamino()
    {
        WaypointPath path = null;
        
        if (HeroSpawner.Instance != null && HeroSpawner.Instance.waypointPath != null)
        {
            path = HeroSpawner.Instance.waypointPath;
        }

        if (path == null || path.WaypointCount < 2)
        {
            WaypointPath[] todosLosCaminos = FindObjectsByType<WaypointPath>(FindObjectsSortMode.None);
            foreach (var c in todosLosCaminos)
            {
                if (c.WaypointCount >= 2)
                {
                    path = c;
                    break;
                }
            }
        }

        if (path == null || path.WaypointCount < 2)
        {
            return false;
        }

        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 closestPoint = myPos;
        float minDistance = float.MaxValue;

        for (int i = 0; i < path.WaypointCount - 1; i++)
        {
            Vector2 a = path.GetWaypoint(i).position;
            Vector2 b = path.GetWaypoint(i + 1).position;

            Vector2 ab = b - a;
            float sqrMag = ab.sqrMagnitude;
            Vector2 pointOnSegment = a;

            if (sqrMag > 0.0001f)
            {
                float t = Vector2.Dot(myPos - a, ab) / sqrMag;
                t = Mathf.Clamp01(t);
                pointOnSegment = a + ab * t;
            }

            float dist = Vector2.Distance(myPos, pointOnSegment);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestPoint = pointOnSegment;
            }
        }

        Vector2 dir = closestPoint - myPos;
        
        if (dir.sqrMagnitude < 0.001f)
        {
            dir = (new Vector2(path.GetWaypoint(0).position.x, path.GetWaypoint(0).position.y) - myPos);
            if (dir.sqrMagnitude < 0.001f) dir = Vector2.up; 
        }

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            direccion = dir.x > 0 ? Direccion.Derecha : Direccion.Izquierda;
        }
        else
        {
            direccion = dir.y > 0 ? Direccion.Arriba : Direccion.Abajo;
        }

        return true;
    }

    float ObtenerAnguloDireccion()
    {
        return direccion switch
        {
            Direccion.Arriba    => 90f,
            Direccion.Abajo     => 270f,
            Direccion.Izquierda => 180f,
            Direccion.Derecha   => 0f,
            _                   => 0f
        };
    }

    Transform GetActiveFirePoint()
    {
        return direccion switch
        {
            Direccion.Arriba    => firePointArriba,
            Direccion.Abajo     => firePointAbajo,
            Direccion.Izquierda => firePointIzquierda,
            Direccion.Derecha   => firePointDerecha,
            _                   => null
        };
    }

    void Shoot()
    {
        if (flamePrefab == null) return;

        Transform activePoint = GetActiveFirePoint();
        if (activePoint == null)
        {
            Debug.LogWarning($"FireTrap: Faltó asignar el FirePoint para la dirección {direccion}");
            return;
        }

        float angle = ObtenerAnguloDireccion();
        GameObject flame = Instantiate(flamePrefab, activePoint.position, Quaternion.Euler(0, 0, angle));
        FlameHitbox hitbox = flame.GetComponent<FlameHitbox>();
        if (hitbox != null) hitbox.Init(flameDuration, daño);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rango);

        Vector3 dir = Vector3.zero;
        switch (direccion)
        {
            case Direccion.Arriba:    dir = Vector3.up; break;
            case Direccion.Abajo:     dir = Vector3.down; break;
            case Direccion.Izquierda: dir = Vector3.left; break;
            case Direccion.Derecha:   dir = Vector3.right; break;
        }
        
        Gizmos.color = Color.yellow;
        Transform activePoint = GetActiveFirePoint();
        Vector3 rayStart = activePoint != null ? activePoint.position : transform.position;
        Gizmos.DrawRay(rayStart, dir * rango);
    }
}