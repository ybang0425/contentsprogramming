using UnityEngine;

public class CompleteTemperatureController : MonoBehaviour
{
    [Header("온도 설정")]
    public float temperature = 25.0f;     // 온도
    public float minTemp = -10.0f;        // 최소 온도
    public float maxTemp = 40.0f;         // 최대 온도 (50→40으로 조정)
    
    [Header("높이 설정")]
    public float minHeight = 0.1f;        // 최소 높이 (Y scale)
    public float maxHeight = 4.0f;        // 최대 높이 (3.0→4.0으로 조정, ScaleMark 최상단 근처)
    
    [Header("재질 설정")]
    public Material coldMaterial;         // Mat_Cold
    public Material warmMaterial;         // Mat_Warm
    public Material hotMaterial;          // Mat_Hot
    
    [Header("참조 오브젝트")]
    public Transform tempBar;             // TempBar 오브젝트
    
    [Header("입력 설정")]
    public bool enableKeyInput = true;    // 키 입력 활성화
    public float tempChangeSpeed = 10.0f; // 온도 변경 속도 (1.0→10.0으로 증가)
    public bool useSelectMode = true;     // 선택 모드 사용
    
    [Header("선택 시각화")]
    public Color selectedColor = Color.yellow;  // 선택됐을 때 프레임 색상
    public bool showSelectionEffect = true;     // 선택 효과 표시
    
    [Header("디버깅")]
    public bool showDebugInfo = true;
    
    private Renderer tempBarRenderer;
    private Renderer tempFrameRenderer;
    private Color originalFrameColor;
    private float nextDebugTime = 0f;
    private static CompleteTemperatureController selectedThermometer;
    
    void Start()
    {
        // TempBar가 할당되지 않았으면 자식에서 찾기
        if (tempBar == null)
        {
            tempBar = transform.Find("TempBar");
            if (tempBar == null)
            {
                Debug.LogError("TempBar를 찾을 수 없습니다!");
                return;
            }
        }
        
        // TempBar의 Renderer 컴포넌트 가져오기
        tempBarRenderer = tempBar.GetComponent<Renderer>();
        
        if (tempBarRenderer == null)
        {
            Debug.LogError("TempBar에 Renderer가 없습니다!");
        }
        
        // TempFrame의 Renderer 가져오기
        Transform tempFrame = transform.Find("TempFrame");
        if (tempFrame != null)
        {
            tempFrameRenderer = tempFrame.GetComponent<Renderer>();
            if (tempFrameRenderer != null)
            {
                originalFrameColor = tempFrameRenderer.material.color;
            }
        }
        
        // Collider 확인 및 추가
        if (GetComponent<Collider>() == null)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(1.5f, 5f, 1.5f);
            boxCollider.center = new Vector3(0, 1.15f, 0);
        }
        
        Debug.Log("온도계 시작! 초기 온도: " + temperature + "도");
        Debug.Log($"온도 범위: {minTemp}도 ~ {maxTemp}도");
        Debug.Log($"색상 변화 기준 - 파란색: {minTemp}~{minTemp + (maxTemp-minTemp)/3:F1}도, " +
                  $"녹색: {minTemp + (maxTemp-minTemp)/3:F1}~{minTemp + 2*(maxTemp-minTemp)/3:F1}도, " +
                  $"빨간색: {minTemp + 2*(maxTemp-minTemp)/3:F1}~{maxTemp}도");
    }
    
    void Update()
    {
        // 선택 모드일 때 마우스 클릭 처리
        if (useSelectMode)
        {
            HandleMouseSelection();
        }
        
        // 키 입력 처리
        if (enableKeyInput)
        {
            if (useSelectMode)
            {
                // 선택 모드: 선택된 온도계만 제어
                if (selectedThermometer == this)
                {
                    HandleKeyInput();
                }
            }
            else
            {
                // 일반 모드: 모든 온도계 제어
                HandleKeyInput();
            }
        }
        
        UpdateTemperatureDisplay();
        UpdateSelectionVisual();
    }
    
    void HandleMouseSelection()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                CompleteTemperatureController clickedThermometer = 
                    hit.collider.GetComponent<CompleteTemperatureController>();
                
                if (clickedThermometer != null)
                {
                    // 이전 선택 해제
                    if (selectedThermometer != null && selectedThermometer != clickedThermometer)
                    {
                        Debug.Log($"{selectedThermometer.gameObject.name} 선택 해제");
                    }
                    
                    // 새로운 온도계 선택
                    selectedThermometer = clickedThermometer;
                    Debug.Log($"{clickedThermometer.gameObject.name} 선택됨! 현재 온도: {clickedThermometer.temperature:F1}도");
                }
            }
            else
            {
                // 빈 공간 클릭 시 선택 해제
                if (selectedThermometer != null)
                {
                    Debug.Log("선택 해제됨");
                    selectedThermometer = null;
                }
            }
        }
        
        // ESC키로 선택 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedThermometer = null;
            Debug.Log("ESC키: 선택 해제됨");
        }
    }
    
    void UpdateSelectionVisual()
    {
        if (!showSelectionEffect || tempFrameRenderer == null) return;
        
        if (selectedThermometer == this)
        {
            // 선택됨: 프레임 색상 변경 + 애니메이션
            float pulse = Mathf.PingPong(Time.time * 2f, 1f);
            Color currentColor = Color.Lerp(originalFrameColor, selectedColor, pulse * 0.7f);
            tempFrameRenderer.material.color = currentColor;
            
            // 크기 애니메이션 (옵션)
            float scale = 1f + pulse * 0.05f;
            transform.localScale = Vector3.one * scale;
        }
        else
        {
            // 선택 안됨: 원래 상태로
            tempFrameRenderer.material.color = originalFrameColor;
            transform.localScale = Vector3.one;
        }
    }
    
    void HandleKeyInput()
    {
        // 위쪽 화살표: 온도 증가
        if (Input.GetKey(KeyCode.UpArrow))
        {
            temperature += tempChangeSpeed * Time.deltaTime;
            temperature = Mathf.Clamp(temperature, minTemp, maxTemp);
            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log($"[{gameObject.name}] 온도 상승 중... 현재: {temperature:F1}도");
            }
        }
        
        // 아래쪽 화살표: 온도 감소
        if (Input.GetKey(KeyCode.DownArrow))
        {
            temperature -= tempChangeSpeed * Time.deltaTime;
            temperature = Mathf.Clamp(temperature, minTemp, maxTemp);
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log($"[{gameObject.name}] 온도 하강 중... 현재: {temperature:F1}도");
            }
        }
        
        // 추가 기능: 숫자 키로 빠른 설정 (조정된 값)
        if (Input.GetKeyDown(KeyCode.Alpha1)) temperature = 0;   // 1키: 0도
        if (Input.GetKeyDown(KeyCode.Alpha2)) temperature = 10;  // 2키: 10도
        if (Input.GetKeyDown(KeyCode.Alpha3)) temperature = 20;  // 3키: 20도
        if (Input.GetKeyDown(KeyCode.Alpha4)) temperature = 30;  // 4키: 30도
        if (Input.GetKeyDown(KeyCode.Alpha5)) temperature = 40;  // 5키: 40도
        
        // R키: 리셋 (20도로)
        if (Input.GetKeyDown(KeyCode.R))
        {
            temperature = 20.0f;
            Debug.Log($"[{gameObject.name}] 온도를 20도로 리셋했습니다.");
        }
    }
    
    void UpdateTemperatureDisplay()
    {
        if (tempBar == null) return;
        
        // 1. 온도를 0-1 범위로 정규화
        float normalizedTemp = Mathf.InverseLerp(minTemp, maxTemp, temperature);
        
        // 2. TempBar 높이 조절 (Y scale만 변경)
        float newHeight = Mathf.Lerp(minHeight, maxHeight, normalizedTemp);
        Vector3 currentScale = tempBar.localScale;
        currentScale.y = newHeight;
        tempBar.localScale = currentScale;
        
        // 3. TempBar 위치 조절 (높이에 따라 Y position 조정)
        Vector3 currentPos = tempBar.localPosition;
        currentPos.y = newHeight / 2.0f - 0.85f;
        tempBar.localPosition = currentPos;
        
        // 4. 온도에 따른 재질 변경 (새로운 기준)
        if (tempBarRenderer != null)
        {
            // 온도 범위를 3등분
            float tempRange = maxTemp - minTemp;  // 50도 범위
            float coldThreshold = minTemp + tempRange / 3;      // 약 6.7도
            float warmThreshold = minTemp + 2 * tempRange / 3;  // 약 23.3도
            
            if (temperature < coldThreshold)
            {
                // 추운 날씨 - Mat_Cold (-10도 ~ 6.7도)
                if (coldMaterial != null)
                    tempBarRenderer.material = coldMaterial;
            }
            else if (temperature < warmThreshold)
            {
                // 적당한 날씨 - Mat_Warm (6.7도 ~ 23.3도)
                if (warmMaterial != null)
                    tempBarRenderer.material = warmMaterial;
            }
            else
            {
                // 더운 날씨 - Mat_Hot (23.3도 ~ 40도)
                if (hotMaterial != null)
                    tempBarRenderer.material = hotMaterial;
            }
        }
        
        // 5. 디버그 정보
        if (showDebugInfo && Time.time >= nextDebugTime)
        {
            string materialName = GetCurrentMaterialName();
            string selected = (selectedThermometer == this) ? " [선택됨]" : "";
            Debug.Log($"[{gameObject.name}]{selected} 온도: {temperature:F1}도, 높이: {newHeight:F2}, 재질: {materialName}");
            nextDebugTime = Time.time + 1.0f;
        }
    }
    
    string GetCurrentMaterialName()
    {
        float tempRange = maxTemp - minTemp;
        float coldThreshold = minTemp + tempRange / 3;
        float warmThreshold = minTemp + 2 * tempRange / 3;
        
        if (temperature < coldThreshold) return "Mat_Cold";
        else if (temperature < warmThreshold) return "Mat_Warm";
        else return "Mat_Hot";
    }
    
    void OnValidate()
    {
        if (Application.isPlaying && tempBar != null)
        {
            UpdateTemperatureDisplay();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 1.15f, new Vector3(1.5f, 5f, 1.5f));
    }
}