using UnityEngine;
using System.Collections;

public class ShooterEnemy2D : MonoBehaviour
{
    public enum State { Idle, Aiming, Shooting }
    public State currentState = State.Idle;

    [Header("Данные баланса (ScriptableObject)")]
    [SerializeField] private EnemyStats stats;

    [Header("Ссылки")]
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
        if (isAttacking || player == null || stats == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= stats.detectionRadius && currentState == State.Idle)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        currentState = State.Aiming;
        if (spriteRenderer != null) spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(stats.windUpTime);

        currentState = State.Shooting;
        if (spriteRenderer != null) spriteRenderer.color = Color.cyan;

        if (projectilePrefab != null && player != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
        }

        yield return new WaitForSeconds(stats.cooldownTime);
        currentState = State.Idle;
        isAttacking = false;
    }
}