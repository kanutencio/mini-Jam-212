using UnityEngine;

/// <summary>
/// Hitbox de la bola con púas (SpikeTrap).
/// Requiere: Collider2D con Is Trigger = true y Rigidbody2D (Kinematic) en el mismo objeto.
/// </summary>
public class SpikeHitbox : MonoBehaviour
{
    [Tooltip("Daño que aplica al héroe por toque.")]
    public float daño = 10f;

    [Tooltip("Segundos mínimos entre golpes (evita spam de daño).")]
    public float cooldown = 0.5f;

    private float _lastHitTime = -999f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Golpear(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Golpear(other);
    }

    void Golpear(Collider2D other)
    {
        if (Time.time - _lastHitTime < cooldown) return;

        HeroMover hero = other.GetComponent<HeroMover>();
        if (hero == null)
            hero = other.GetComponentInParent<HeroMover>();

        if (hero == null) return;

        _lastHitTime = Time.time;
        hero.RecibirDaño(daño);
    }
}
