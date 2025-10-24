using UnityEngine;
using UnityEngine.UI;

public class JoystickUI : MonoBehaviour
{
    [Header("UI 설정")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform joystickPanel;
    [SerializeField] private VirtualJoystick virtualJoystick;
    
    [Header("조이스틱 스타일")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image handleImage;
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Sprite handleSprite;
    
    [Header("위치 설정")]
    [SerializeField] private JoystickPosition joystickPosition = JoystickPosition.BottomLeft;
    [SerializeField] private Vector2 customOffset = Vector2.zero;
    [SerializeField] private float margin = 50f;
    
    [Header("터치 영역")]
    [SerializeField] private bool expandTouchArea = true;
    [SerializeField] private float touchAreaMultiplier = 2f;
    
    public enum JoystickPosition
    {
        BottomLeft,
        BottomRight,
        TopLeft,
        TopRight,
        Custom
    }
    
    private void Awake()
    {
        SetupJoystick();
    }
    
    private void Start()
    {
        PositionJoystick();
        SetupTouchArea();
    }
    
    private void SetupJoystick()
    {
        // 캔버스 찾기
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindObjectOfType<Canvas>();
            }
        }
        
        // 조이스틱 패널 찾기
        if (joystickPanel == null)
        {
            joystickPanel = GetComponent<RectTransform>();
        }
        
        // 가상 조이스틱 찾기
        if (virtualJoystick == null)
        {
            virtualJoystick = GetComponentInChildren<VirtualJoystick>();
        }
        
        // 이미지 컴포넌트 찾기
        if (backgroundImage == null)
        {
            backgroundImage = joystickPanel.GetComponent<Image>();
        }
        
        if (handleImage == null && virtualJoystick != null)
        {
            handleImage = virtualJoystick.transform.GetChild(0)?.GetComponent<Image>();
        }
    }
    
    private void PositionJoystick()
    {
        if (joystickPanel == null || canvas == null) return;
        
        Vector2 position = Vector2.zero;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        
        switch (joystickPosition)
        {
            case JoystickPosition.BottomLeft:
                position = new Vector2(-canvasRect.sizeDelta.x / 2 + margin, 
                                     -canvasRect.sizeDelta.y / 2 + margin);
                break;
                
            case JoystickPosition.BottomRight:
                position = new Vector2(canvasRect.sizeDelta.x / 2 - margin, 
                                     -canvasRect.sizeDelta.y / 2 + margin);
                break;
                
            case JoystickPosition.TopLeft:
                position = new Vector2(-canvasRect.sizeDelta.x / 2 + margin, 
                                     canvasRect.sizeDelta.y / 2 - margin);
                break;
                
            case JoystickPosition.TopRight:
                position = new Vector2(canvasRect.sizeDelta.x / 2 - margin, 
                                     canvasRect.sizeDelta.y / 2 - margin);
                break;
                
            case JoystickPosition.Custom:
                position = customOffset;
                break;
        }
        
        joystickPanel.anchoredPosition = position;
    }
    
    private void SetupTouchArea()
    {
        if (!expandTouchArea || joystickPanel == null) return;
        
        // 터치 영역 확장
        Vector2 originalSize = joystickPanel.sizeDelta;
        joystickPanel.sizeDelta = originalSize * touchAreaMultiplier;
        
        // 터치 영역을 투명하게 만들기
        if (backgroundImage != null)
        {
            Color transparentColor = backgroundImage.color;
            transparentColor.a = 0f;
            backgroundImage.color = transparentColor;
        }
    }
    
    public void SetJoystickPosition(JoystickPosition position)
    {
        joystickPosition = position;
        PositionJoystick();
    }
    
    public void SetCustomOffset(Vector2 offset)
    {
        customOffset = offset;
        if (joystickPosition == JoystickPosition.Custom)
        {
            PositionJoystick();
        }
    }
    
    public void SetJoystickSprites(Sprite background, Sprite handle)
    {
        backgroundSprite = background;
        handleSprite = handle;
        
        if (backgroundImage != null && backgroundSprite != null)
        {
            backgroundImage.sprite = backgroundSprite;
        }
        
        if (handleImage != null && handleSprite != null)
        {
            handleImage.sprite = handleSprite;
        }
    }
    
    public void SetJoystickColors(Color backgroundColor, Color handleColor)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }
        
        if (handleImage != null)
        {
            handleImage.color = handleColor;
        }
    }
    
    public void SetJoystickSize(float size)
    {
        if (joystickPanel != null)
        {
            joystickPanel.sizeDelta = Vector2.one * size;
        }
        
        if (virtualJoystick != null)
        {
            virtualJoystick.SetJoystickRange(size / 2f);
        }
    }
    
    public void SetTouchAreaMultiplier(float multiplier)
    {
        touchAreaMultiplier = multiplier;
        SetupTouchArea();
    }
    
    public void ShowJoystick(bool show)
    {
        gameObject.SetActive(show);
    }
    
    public void SetJoystickAlpha(float alpha)
    {
        Color bgColor = backgroundImage != null ? backgroundImage.color : Color.white;
        Color handleColor = handleImage != null ? handleImage.color : Color.white;
        
        bgColor.a = alpha;
        handleColor.a = alpha;
        
        if (backgroundImage != null)
            backgroundImage.color = bgColor;
        if (handleImage != null)
            handleImage.color = handleColor;
    }
    
    // 조이스틱 입력 값 가져오기
    public Vector2 GetJoystickInput()
    {
        return virtualJoystick != null ? virtualJoystick.GetJoystickInput() : Vector2.zero;
    }
    
    // 조이스틱 활성 상태 확인
    public bool IsJoystickActive()
    {
        return virtualJoystick != null && virtualJoystick.IsJoystickActive();
    }
}