using UnityEngine;
using UnityEngine.InputSystem; // Penting untuk InputValue (New Input System)

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;              // arah gerakan (X,Y)
    private Rigidbody2D rb;                 // physics
    private Animator animator;              // untuk animasi
    private SpriteRenderer spriteRenderer;  // untuk flip sprite

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // ===== DIPANGGIL OTOMATIS OLEH PlayerInput (Send Messages) =====
#if UNITY_STANDALONE || UNITY_WEBGL
    public void OnMove(InputValue value)
    {
        // Input dari keyboard / gamepad
        moveInput = value.Get<Vector2>();
    }
#endif

#if UNITY_ANDROID || UNITY_IOS
    void UpdateTouchInput()
    {
        moveInput = Vector2.zero;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            // Baca posisi touch
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();

            // Contoh: konversi touch ke arah relatif layar (joystick virtual bisa di sini)
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 dir = (touchPos - screenCenter).normalized;
            moveInput = dir;
        }
    }
#endif

    // ===== PERGERAKAN PHYSICS (tiap fixed frame) =====
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // ===== UPDATE ANIMASI & FLIP SPRITE =====
    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        UpdateTouchInput(); // jalankan input khusus mobile
#endif

        // Kirim nilai ke Animator
        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        // Flip sprite (mirror kiri/kanan)
        if (moveInput.x > 0.01f)
            spriteRenderer.flipX = false; // menghadap kanan
        else if (moveInput.x < -0.01f)
            spriteRenderer.flipX = true;  // menghadap kiri
    }
}
