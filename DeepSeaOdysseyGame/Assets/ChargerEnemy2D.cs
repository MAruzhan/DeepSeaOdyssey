using UnityEngine;
using System.Collections;

public class ChargerEnemy2D : MonoBehaviour
{
    public enum State { Patrol, WindUp, Charge }
    public State currentState = State.Patrol;

    [Header("Данные баланса (ScriptableObject)")]
    [SerializeField] private EnemyStats stats;

    [Header("Ссылки")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Transform player;
    private Vector2 chargeDirection;
    private bool isCharging = false;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isCharging || player == null || stats == null) return;

        if (Vector2.Distance(transform.position, player.position) <= stats.detectionRadius)
        {
            StartCoroutine(ChargeRoutine());
        }
    }

    IEnumerator ChargeRoutine()
    {
        isCharging = true;

        currentState = State.WindUp;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayWindUp();
        chargeDirection = (player.position - transform.position).normalized;
        if (spriteRenderer != null) spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(stats.windUpTime);

        currentState = State.Charge;
        if (spriteRenderer != null) spriteRenderer.color = Color.magenta;

        float timer = 0f;
        while (timer < 0.45f)
        {
            transform.Translate(chargeDirection * stats.moveSpeed * Time.deltaTime, Space.World);
            timer += Time.deltaTime;
            yield return null;
        }

        if (spriteRenderer != null) spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(stats.cooldownTime);
        currentState = State.Patrol;
        isCharging = false;
    }
}