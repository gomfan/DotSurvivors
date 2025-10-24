using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("조이스틱 설정")]
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private float joystickRange = 50f;
    [SerializeField] private float deadZone = 0.1f;
    
    [Header("시각적 피드백")]
    [SerializeField] private bool showJoystickOnTouch = true;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);
    
    private Vector2 joystickInput;
    private bool isJoystickActive = false;
    private Vector2 joystickCenter;
    private Image backgroundImage;
    private Image handleImage;
    
    // 이벤트
    public System.Action<Vector2> OnJoystickInput;
    
    private void Awake()
    {
        // 초기 설정
        joystickCenter = joystickBackground.anchoredPosition;
        backgroundImage = joystickBackground.GetComponent<Image>();
        handleImage = joystickHandle.GetComponent<Image>();
        
        // 초기 상태 설정
        SetJoystickVisibility(false);
    }
    
    private void Start()
    {
        // 조이스틱 범위 설정
        joystickBackground.sizeDelta = Vector2.one * joystickRange * 2f;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isJoystickActive = true;
        SetJoystickVisibility(true);
        
        // 터치 위치로 조이스틱 중심 이동
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );
        
        joystickBackground.anchoredPosition = localPoint;
        joystickCenter = localPoint;
        
        // 시각적 피드백
        SetJoystickColors(activeColor);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isJoystickActive = false;
        joystickInput = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
        
        // 시각적 피드백
        SetJoystickColors(inactiveColor);
        
        if (showJoystickOnTouch)
        {
            SetJoystickVisibility(false);
        }
        
        // 입력 이벤트 발생
        OnJoystickInput?.Invoke(Vector2.zero);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isJoystickActive) return;
        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );
        
        // 조이스틱 입력 계산
        joystickInput = localPoint.normalized;
        
        // 범위 제한
        float distance = Vector2.Distance(Vector2.zero, localPoint);
        if (distance > joystickRange)
        {
            joystickInput = localPoint.normalized;
            localPoint = joystickInput * joystickRange;
        }
        
        // 핸들 위치 업데이트
        joystickHandle.anchoredPosition = localPoint;
        
        // 데드존 적용
        if (joystickInput.magnitude < deadZone)
        {
            joystickInput = Vector2.zero;
        }
        
        // 입력 이벤트 발생
        OnJoystickInput?.Invoke(joystickInput);
    }
    
    private void SetJoystickVisibility(bool visible)
    {
        if (backgroundImage != null)
            backgroundImage.enabled = visible;
        if (handleImage != null)
            handleImage.enabled = visible;
    }
    
    private void SetJoystickColors(Color color)
    {
        if (backgroundImage != null)
            backgroundImage.color = color;
        if (handleImage != null)
            handleImage.color = color;
    }
    
    // 조이스틱 입력 값 가져오기
    public Vector2 GetJoystickInput()
    {
        return joystickInput;
    }
    
    // 조이스틱 활성 상태 확인
    public bool IsJoystickActive()
    {
        return isJoystickActive;
    }
    
    // 조이스틱 설정 변경
    public void SetJoystickRange(float range)
    {
        joystickRange = range;
        joystickBackground.sizeDelta = Vector2.one * joystickRange * 2f;
    }
    
    public void SetDeadZone(float deadZoneValue)
    {
        deadZone = Mathf.Clamp01(deadZoneValue);
    }
}