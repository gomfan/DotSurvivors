using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    
    [Header("애니메이션 설정")]
    [SerializeField] private Animator animator;
    [SerializeField] private string walkAnimationName = "Walk";
    [SerializeField] private string idleAnimationName = "Idle";
    
    [Header("스프라이트 설정")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool flipSpriteOnDirection = true;
    
    private Vector2 currentVelocity;
    private Vector2 targetVelocity;
    private bool isMoving = false;
    private bool facingRight = true;
    
    // 컴포넌트 참조
    private Rigidbody2D rb;
    private VirtualJoystick joystick;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // 조이스틱 찾기
        joystick = FindObjectOfType<VirtualJoystick>();
        if (joystick == null)
        {
            Debug.LogWarning("VirtualJoystick을 찾을 수 없습니다!");
        }
        
        // 애니메이터 찾기
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // 스프라이트 렌더러 찾기
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    
    private void Start()
    {
        // 조이스틱 이벤트 구독
        if (joystick != null)
        {
            joystick.OnJoystickInput += OnJoystickInput;
        }
        
        // 물리 설정
        rb.gravityScale = 0f; // 2D 게임에서 중력 비활성화
        rb.linearDamping = 0f; // 드래그 비활성화 (우리가 직접 제어)
    }
    
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (joystick != null)
        {
            joystick.OnJoystickInput -= OnJoystickInput;
        }
    }
    
    private void OnJoystickInput(Vector2 input)
    {
        targetVelocity = input * moveSpeed;
        isMoving = input.magnitude > 0.1f;
        
        // 방향 전환 처리
        if (isMoving && flipSpriteOnDirection)
        {
            bool shouldFaceRight = input.x > 0;
            if (shouldFaceRight != facingRight)
            {
                facingRight = shouldFaceRight;
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = !facingRight;
                }
            }
        }
    }
    
    private void FixedUpdate()
    {
        // 부드러운 이동을 위한 속도 보간
        if (isMoving)
        {
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
        
        // 물리 이동 적용
        rb.linearVelocity = currentVelocity;
        
        // 애니메이션 업데이트
        UpdateAnimation();
    }
    
    private void UpdateAnimation()
    {
        if (animator == null) return;
        
        // 이동 애니메이션
        if (isMoving)
        {
            if (!string.IsNullOrEmpty(walkAnimationName))
            {
                animator.SetBool("IsWalking", true);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(idleAnimationName))
            {
                animator.SetBool("IsWalking", false);
            }
        }
        
        // 속도 기반 애니메이션 파라미터
        animator.SetFloat("Speed", currentVelocity.magnitude);
    }
    
    // 외부에서 이동 속도 설정
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    // 외부에서 가속도 설정
    public void SetAcceleration(float accel)
    {
        acceleration = accel;
    }
    
    // 외부에서 감속도 설정
    public void SetDeceleration(float decel)
    {
        deceleration = decel;
    }
    
    // 현재 이동 상태 확인
    public bool IsMoving()
    {
        return isMoving;
    }
    
    // 현재 속도 가져오기
    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }
    
    // 현재 이동 방향 가져오기
    public Vector2 GetMoveDirection()
    {
        return currentVelocity.normalized;
    }
    
    // 강제로 정지
    public void StopMovement()
    {
        currentVelocity = Vector2.zero;
        targetVelocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
    }
    
    // 특정 방향으로 즉시 이동
    public void MoveInDirection(Vector2 direction, float speed)
    {
        direction = direction.normalized;
        currentVelocity = direction * speed;
        targetVelocity = currentVelocity;
        rb.linearVelocity = currentVelocity;
        isMoving = true;
    }
}
