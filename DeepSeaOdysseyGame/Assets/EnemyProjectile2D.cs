using UnityEngine;

public class EnemyProjectile2D : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Снаряд сам исчезнет через 3 секунды
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Снаряд попал в батискаф!");
            Destroy(gameObject);
        }
    }
}