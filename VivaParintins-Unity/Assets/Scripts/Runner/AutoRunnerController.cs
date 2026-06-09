using UnityEngine;
using VivaParintins.Core;
using VivaParintins.Data;

namespace VivaParintins.Runner
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class AutoRunnerController : MonoBehaviour
    {
        [Header("Movement")]
        public float runSpeed = 5f;
        public float acceleration = 0.3f;

        [Header("Jump")]
        public float jumpForce = 12f;
        public float holdGravityScale = 1f;
        public float defaultGravityScale = 3f;
        public float maxJumpHoldTime = 0.35f;
        public bool canDoubleJump = false;

        [Header("Ground Detection")]
        public Transform groundCheck;
        public float groundCheckRadius = 0.15f;
        public LayerMask groundLayer;

        [Header("Sprites")]
        public Sprite torcedorGarantidoSprite;
        public Sprite torcedorCaprichosoSprite;

        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private bool isGrounded;
        private bool isJumping;
        private bool hasDoubleJumped;
        private float jumpHoldTimer;
        private bool inputHeld;
        private bool isDead;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb.gravityScale = defaultGravityScale;
        }

        void Start()
        {
            ApplyTeamSprite();
        }

        void ApplyTeamSprite()
        {
            if (GameManager.Instance == null) return;
            bool isGarantido = GameManager.Instance.selectedTeam == Team.Garantido;
            if (spriteRenderer != null)
                spriteRenderer.sprite = isGarantido ? torcedorGarantidoSprite : torcedorCaprichosoSprite;
        }

        void Update()
        {
            if (isDead) return;
            CheckGround();
            HandleInput();
            UpdateSpeed();
            UpdateAnimator();
        }

        void CheckGround()
        {
            if (groundCheck == null) return;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            if (isGrounded)
            {
                hasDoubleJumped = false;
                isJumping = false;
            }
        }

        void HandleInput()
        {
            bool inputDown = false;
            bool inputUp = false;
            inputHeld = false;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0)) inputDown = true;
            if (Input.GetMouseButtonUp(0)) inputUp = true;
            if (Input.GetMouseButton(0)) inputHeld = true;
#endif

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) inputDown = true;
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) inputUp = true;
                if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) inputHeld = true;
            }

            if (inputDown)
            {
                if (isGrounded)
                {
                    PerformJump();
                }
                else if (canDoubleJump && !hasDoubleJumped)
                {
                    hasDoubleJumped = true;
                    PerformJump();
                }
            }

            if (inputUp)
            {
                isJumping = false;
            }

            if (isJumping && inputHeld)
            {
                jumpHoldTimer += Time.deltaTime;
                rb.gravityScale = jumpHoldTimer < maxJumpHoldTime ? holdGravityScale : defaultGravityScale;
                if (jumpHoldTimer >= maxJumpHoldTime) isJumping = false;
            }
            else
            {
                rb.gravityScale = defaultGravityScale;
            }
        }

        void PerformJump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
            jumpHoldTimer = 0f;
            if (animator != null) animator.SetTrigger("Jump");
        }

        void UpdateSpeed()
        {
            runSpeed += acceleration * Time.deltaTime;
            rb.linearVelocity = new Vector2(runSpeed, rb.linearVelocity.y);
        }

        void UpdateAnimator()
        {
            if (animator == null) return;
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsRunning", true);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (isDead) return;

            if (other.CompareTag("Collectible"))
            {
                RunnerLevelManager.Instance?.OnCollect(other.transform.position);
            }
            else if (other.CompareTag("Obstacle"))
            {
                if (animator != null) animator.SetTrigger("Hit");
                RunnerLevelManager.Instance?.OnPlayerHit();
            }
        }

        public void Die()
        {
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
