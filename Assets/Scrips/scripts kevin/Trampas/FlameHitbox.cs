using UnityEngine;

public class FlameHitbox : MonoBehaviour
{
    private float _daño;

    public void Init(float duration, float daño)
    {
        _daño = daño;
        Debug.Log($"FlameHitbox: iniciada, duración={duration}, daño={daño}");
        Destroy(gameObject, duration);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        HeroMover heroe = other.GetComponent<HeroMover>();
        if (heroe != null)
        {
            Debug.Log($"FlameHitbox: golpeó a {other.name}, aplicando daño {_daño}");
            heroe.RecibirDaño(_daño);
        }
    }
}