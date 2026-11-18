using UnityEngine;

public class PatrolEnemy2D : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }
    public State currentState = State.Patrol;

    [Header("Данные баланса (ScriptableObject)")]
    [SerializeField] private EnemyStats stats;

    [Header("Настройки сцены")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float attackRadius = 1.2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;

    private int currentWaypointIndex = 0;
    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (stats == null) return;

        switch (currentState)
        {
            case State.Patrol:
                PatrolLogic();
                CheckForPlayerWithRaycast();
                break;
            case State.Chase:
                ChaseLogic();
                break;
            case State.Attack:
                AttackLogic();
                break;
        }
    }

    void PatrolLogic()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, stats.moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void CheckForPlayerWithRaycast()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stats.detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, stats.detectionRadius, playerLayer | wallLayer);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                currentState = State.Chase;
            }
        }
    }

    void ChaseLogic()
    {
        if (player == null) return;
        transform.position = Vector2.MoveTowards(transform.position, player.position, stats.chaseSpeed * Time.deltaTime);

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRadius)
        {
            currentState = State.Attack;
        }
        else if (distance > stats.detectionRadius * 1.4f)
        {
            currentState = State.Patrol;
        }
    }

    void AttackLogic()
    {
        Debug.Log("Patroller наносит урон: " + stats.damage);
        currentState = State.Patrol;
    }
}