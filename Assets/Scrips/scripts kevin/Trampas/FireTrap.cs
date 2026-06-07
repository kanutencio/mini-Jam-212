using UnityEngine;
using UnityEngine.InputSystem;

// Direction enum kept for legacy visual support but not used for aiming

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
    // We'll aim directly at the hero; the direction enum is no longer needed for aiming

    [Header("Puntos de disparo por dirección")]
    public Transform firePoint; // Assign a child empty at the muzzle of the cannon

    [Header("Objetos Visuales (Sprites) por dirección")]
    [Tooltip("Puedes meter aquí el objeto que tiene el dibujo de la trampa para cada lado. Se apagará/encenderá automáticamente.")]
    public GameObject visualArriba;
    public GameObject visualAbajo;
    public GameObject visualIzquierda;
    public GameObject visualDerecha;

    private float _timer;
    private bool orientada = false;

    // Si es true, la trampa usará la rotación del Transform en vez de orientarse automáticamente hacia el camino.
    [Header("Configuración Manual")]
    [Tooltip("Activar para fijar la rotación de la trampa al colocarla en la escena")]
    public bool manualRotation = false;

    [Header("Sonido")]
    [SerializeField] private AudioSource AS;

    private void OnEnable()
    {
        // Empieza cargada para disparar al instante la primera vez
        _timer = fireRate;
        // Si se habilita la rotación manual, determinamos la dirección basada en la rotación actual del objeto
        if (manualRotation)
        {
            UpdateDirectionFromRotation();
            ActualizarVisual();
            orientada = true;
        }

        AS = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Optional manual rotation via R key (debug)
        bool mouseEncima = EstaCursorEncima();
        if (mouseEncima && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            // rotate 90° for quick testing
            transform.Rotate(0f, 0f, 90f);
        }

        // Manual rotation handling (once)
        if (manualRotation && !orientada)
        {
            UpdateDirectionFromRotation();
            orientada = true;
        }
        // Timer always advances
        _timer += Time.deltaTime;

        GameObject heroe = HeroSpawner.Instance != null ? HeroSpawner.Instance.GetHeroeActivo() : null;
        if (heroe == null) return;

        float distancia = Vector3.Distance(transform.position, heroe.transform.position);
        if (distancia > rango) return;

        // Aim instantly at hero
        Vector3 dirHeroe = (heroe.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dirHeroe.y, dirHeroe.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

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

    // Deprecated: angle is now taken from the transform's Z rotation

    // Cycles direction for testing with the R key: Arriba → Derecha → Abajo → Izquierda
    void CycleDirection()
    {
        switch (direccion)
        {
            case Direccion.Arriba:
                direccion = Direccion.Derecha;
                break;
            case Direccion.Derecha:
                direccion = Direccion.Abajo;
                break;
            case Direccion.Abajo:
                direccion = Direccion.Izquierda;
                break;
            case Direccion.Izquierda:
                direccion = Direccion.Arriba;
                break;
        }
    }


    // esto es para que  determine la dirección basada en la rotación Z del Transform (en grados)
    void UpdateDirectionFromRotation()
    {
        float z = transform.eulerAngles.z;
        z = (z % 360 + 360) % 360;
        if (z >= 315 || z < 45)
            direccion = Direccion.Derecha;
        else if (z >= 45 && z < 135)
            direccion = Direccion.Arriba;
        else if (z >= 135 && z < 225)
            direccion = Direccion.Izquierda;
        else
            direccion = Direccion.Abajo;
    }

    // esto es para que detecte si el mouse esta encima de la trampa
    bool EstaCursorEncima()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return false;

        return col.OverlapPoint(mouseWorld);
    }

    // esto es para que al darle clic a la trampa rote solo esta
    void OnMouseDown()
    {
        CycleDirection();
        ActualizarVisual();
        orientada = true;
    }

    void ActualizarVisual()
    {
        // Visuals can be controlled via manual rotation if needed; left unchanged for legacy support
    }

    Transform GetActiveFirePoint()
    {
        return firePoint;
    }

    void Shoot()
    {
        if (flamePrefab == null) return;

        AS.Play();

        Transform activePoint = GetActiveFirePoint();
        if (activePoint == null)
        {
            Debug.LogWarning($"FireTrap: Faltó asignar el FirePoint para la dirección {direccion}");
            return;
        }

        // Use the current rotation of the cannon as the firing angle
        float angle = transform.eulerAngles.z;
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