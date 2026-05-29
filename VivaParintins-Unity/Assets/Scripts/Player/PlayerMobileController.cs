// Coloque na Hierarquia: GameObject "Player" na MiniGameScene.
// Requer: Rigidbody (Is Kinematic = true), Collider, Animator (opcional).
// Input System clássico (UnityEngine.Input) — compatível com Unity 2022.3 LTS.

using System.Collections;
using UnityEngine;
using VivaParintins.Core;

namespace VivaParintins.Player
{
    [RequireComponent(typeof(Collider))]
    public class PlayerMobileController : MonoBehaviour
    {
        // ── Configuração de pistas ───────────────────────────────────────
        [Header("Pistas (Lanes)")]
        public int totalLanes = 3;
        public float laneWidth = 2.5f;
        public float laneChangeSpeed = 12f;

        [Header("Movimento")]
        public float forwardSpeed = 8f;
        public float dashSpeed = 18f;
        public float dashDuration = 1.2f;

        [Header("Pulo")]
        public float jumpForce = 10f;
        public Transform groundCheck;
        public float groundDistance = 0.3f;
        public LayerMask groundLayer;

        [Header("Animador")]
        public Animator animator;

        // ── Estado interno ───────────────────────────────────────────────
        [HideInInspector] public bool enableDoubleJump; // ativado pela Sombrinha (BuffSystem)
        bool _usedDoubleJump;

        int _currentLane;
        float _targetX;
        bool _isGrounded;
        bool _isDashing;
        bool _isDead;

        Vector3 _velocity;
        float _gravity = -20f;
        float _verticalVelocity;

        // Swipe detection
        Vector2 _touchStart;
        const float SwipeThreshold = 50f;

        // Cache
        CharacterController _cc;

        // ── Unity lifecycle ──────────────────────────────────────────────
        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _currentLane = totalLanes / 2; // começa no centro
            _targetX = LaneToX(_currentLane);
        }

        void Update()
        {
            if (_isDead) return;

            CheckGround();
            HandleTouchInput();
            ApplyGravity();
            MovePlayer();
            AnimatePlayer();
        }

        // ── Colisões ─────────────────────────────────────────────────────
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Obstacle"))
                TakeDamage();
            else if (hit.gameObject.CompareTag("Coin"))
                CollectCoin(hit.gameObject);
            else if (hit.gameObject.CompareTag("Item"))
                CollectItem(hit.gameObject);
        }

        // ── Input ────────────────────────────────────────────────────────
        void HandleTouchInput()
        {
            // Suporte a mouse em editor
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0)) _touchStart = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
            {
                Vector2 delta = (Vector2)Input.mousePosition - _touchStart;
                ProcessSwipe(delta);
            }
            if (Input.GetKeyDown(KeyCode.Space)) TryJump();
#endif
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStart = touch.position;
                    break;
                case TouchPhase.Ended:
                    ProcessSwipe(touch.position - _touchStart);
                    break;
            }
        }

        void ProcessSwipe(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (Mathf.Abs(delta.x) > SwipeThreshold)
                {
                    if (delta.x > 0) ChangeLane(1);
                    else             ChangeLane(-1);
                }
            }
            else
            {
                if (delta.y > SwipeThreshold)  TryJump();
                if (delta.y < -SwipeThreshold) Slide();
            }
        }

        // ── Pistas ───────────────────────────────────────────────────────
        void ChangeLane(int dir)
        {
            int next = Mathf.Clamp(_currentLane + dir, 0, totalLanes - 1);
            if (next == _currentLane) return;
            _currentLane = next;
            _targetX = LaneToX(_currentLane);
            AudioManager.Instance?.PlayTap();
        }

        float LaneToX(int lane)
        {
            float totalWidth = (totalLanes - 1) * laneWidth;
            return lane * laneWidth - totalWidth / 2f;
        }

        // ── Físicas ──────────────────────────────────────────────────────
        void CheckGround()
        {
            _isGrounded = Physics.CheckSphere(
                groundCheck != null ? groundCheck.position : transform.position + Vector3.down * 0.1f,
                groundDistance, groundLayer);
        }

        void ApplyGravity()
        {
            if (_isGrounded && _verticalVelocity < 0)
                _verticalVelocity = -2f;
            _verticalVelocity += _gravity * Time.deltaTime;
        }

        void MovePlayer()
        {
            float speed = _isDashing ? dashSpeed : forwardSpeed;

            // Lateral: interpolação suave
            float currentX = transform.position.x;
            float newX = Mathf.MoveTowards(currentX, _targetX, laneChangeSpeed * Time.deltaTime);

            Vector3 move = new Vector3(
                newX - currentX,
                _verticalVelocity * Time.deltaTime,
                speed * Time.deltaTime);

            _cc.Move(move);
        }

        void AnimatePlayer()
        {
            if (animator == null) return;
            animator.SetBool("IsGrounded", _isGrounded);
            animator.SetFloat("Speed", forwardSpeed);
            animator.SetBool("IsDashing", _isDashing);
        }

        // ── Ações ────────────────────────────────────────────────────────
        void TryJump()
        {
            if (_isGrounded)
            {
                _verticalVelocity = jumpForce;
                _usedDoubleJump = false;
                animator?.SetTrigger("Jump");
            }
            else if (enableDoubleJump && !_usedDoubleJump)
            {
                // Sombrinha Estrelada: pulo duplo / planar
                _verticalVelocity = jumpForce * 0.75f;
                _usedDoubleJump = true;
                animator?.SetTrigger("Jump");
            }
        }

        void Slide()
        {
            if (!_isGrounded) return;
            animator?.SetTrigger("Slide");
        }

        // Chamado pelo GameEconomyManager ao usar Guaraná
        public void ActivateDash()
        {
            if (!_isDashing) StartCoroutine(DashRoutine());
        }

        IEnumerator DashRoutine()
        {
            _isDashing = true;
            yield return new WaitForSeconds(dashDuration);
            _isDashing = false;
        }

        // ── Dano / Moedas / Itens ────────────────────────────────────────
        void TakeDamage()
        {
            if (GameEconomyManager.Instance.TryUseShield())
            {
                animator?.SetTrigger("Shield");
                return;
            }
            _isDead = true;
            animator?.SetTrigger("Die");
            GameManager.Instance?.OnPlayerDied();
        }

        void CollectCoin(GameObject coin)
        {
            GameEconomyManager.Instance.AddMicangas(1);
            VFX.VFXManager.Instance?.LumaCollect(coin.transform.position, GameManager.Instance.selectedTeam);
            Destroy(coin);
        }

        void CollectItem(GameObject item)
        {
            var pickup = item.GetComponent<ItemPickup>();
            if (pickup != null) pickup.Apply(this);
            Destroy(item);
        }

        public void Revive()
        {
            _isDead = false;
            _verticalVelocity = 0;
            animator?.SetTrigger("Revive");
        }
    }
}
