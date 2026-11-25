using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boostMultiplier = 1.8f;
    [SerializeField] private float moveSoundInterval = 0.45f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveSoundTimer = 0f;
    private bool isBoosting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Управление стрелками или клавишами W, A, S, D
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Проигрываем звук движения (пузырьки/мотор) с интервалом, когда батискаф плывёт
        if (movement.sqrMagnitude > 0.01f)
        {
            moveSoundTimer -= Time.deltaTime;
            if (moveSoundTimer <= 0f)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayMove();
                moveSoundTimer = moveSoundInterval;
            }
        }
        else
        {
            moveSoundTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = isBoosting ? moveSpeed * boostMultiplier : moveSpeed;
        rb.MovePosition(rb.position + movement.normalized * currentSpeed * Time.fixedDeltaTime);
    }

    // Публичный метод для кнопки Boost в UI
    public void TriggerBoost()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMove();
        CancelInvoke(nameof(StopBoost));
        isBoosting = true;
        Invoke(nameof(StopBoost), 1.0f);
    }

    void StopBoost()
    {
        isBoosting = false;
    }
}