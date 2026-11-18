using UnityEngine;
using System.Collections;

public class ShooterEnemy2D : MonoBehaviour
{
    public enum State { Idle, Aiming, Shooting }
    public State currentState = State.Idle;

    [Header("Настройки стрелка")]
    [SerializeField] private float detectionRadius = 6f;
    [SerializeField] private float windUpTime = 0.6f;  // Время предупреждения перед выстрелом
    [SerializeField] private float cooldownTime = 1.6f; // Пауза между выстрелами
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Transform player;
    private bool isAttacking = false;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isAttacking || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRadius && currentState == State.Idle)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 1. Состояние: Aiming (Краснеет и предупреждает игрока)
        currentState = State.Aiming;
        if (spriteRenderer != null) spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(windUpTime);

        // 2. Состояние: Shooting (Поворачивается к игроку и стреляет)
        currentState = State.Shooting;
        if (spriteRenderer != null) spriteRenderer.color = Color.cyan;
        
        if (projectilePrefab != null && player != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
        }

        // 3. Перезарядка
        yield return new WaitForSeconds(cooldownTime);
        currentState = State.Idle;
        isAttacking = false;
    }
}