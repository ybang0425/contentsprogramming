using UnityEngine;
using System.Collections;

public class TempControl : MonoBehaviour
{
    [Header("온도 설정")]
    public float temperature = 25.0f;
    public float minTemp = -10.0f;
    public float maxTemp = 40.0f;

    [Header("애니메이션 설정")]
    public float animationDuration = 1.0f;

    [Header("높이 설정")]
    public float minHeight = 0.1f;
    public float maxHeight = 4.0f;

    [Header("재질 설정")]
    public Material coldMaterial;
    public Material warmMaterial;
    public Material hotMaterial;

    [Header("참조 오브젝트")]
    public Transform tempBar;

    private Renderer tempBarRenderer;
    private Coroutine activeCoroutine;


    public float SetTemperatureForWinter()
    {
        float tempRange = maxTemp - minTemp;
        float coldThreshold = minTemp + tempRange / 3;
        float targetTemperature = Random.Range(minTemp, coldThreshold);
        StartTemperatureAnimation(targetTemperature);
        return targetTemperature;
    }

    public float SetTemperatureForSpringOrAutumn()
    {
        float tempRange = maxTemp - minTemp;
        float coldThreshold = minTemp + tempRange / 3;
        float warmThreshold = minTemp + 2 * tempRange / 3;
        float targetTemperature = Random.Range(coldThreshold, warmThreshold);
        StartTemperatureAnimation(targetTemperature);
        return targetTemperature;
    }

    public float SetTemperatureForSummer()
    {
        float tempRange = maxTemp - minTemp;
        float warmThreshold = minTemp + 2 * tempRange / 3;
        float targetTemperature = Random.Range(warmThreshold, maxTemp);
        StartTemperatureAnimation(targetTemperature);
        return targetTemperature;
    }

    private void StartTemperatureAnimation(float newTargetTemp)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(AnimateTemperatureChange(newTargetTemp));
    }

    private IEnumerator AnimateTemperatureChange(float targetTemp)
    {
        float elapsedTime = 0f;
        float startTemp = temperature;

        while (elapsedTime < animationDuration)
        {
            temperature = Mathf.Lerp(startTemp, targetTemp, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        temperature = targetTemp;
        activeCoroutine = null;
    }


    void Start()
    {
        if (tempBar == null) tempBar = transform.Find("TempBar");
        if (tempBar != null)
        {
            tempBarRenderer = tempBar.GetComponent<Renderer>();
        }
        UpdateTemperatureDisplay();
    }
    
    void Update()
    {
        UpdateTemperatureDisplay();
    }
    
    private void UpdateTemperatureDisplay()
    {
        if (tempBar == null) return;

        // 1. 온도를 0-1 범위로 정규화 (Normalization)
        float normalizedTemp = Mathf.InverseLerp(minTemp, maxTemp, temperature);

        // 2. TempBar 높이(Y scale) 조절
        float newHeight = Mathf.Lerp(minHeight, maxHeight, normalizedTemp);
        Vector3 currentScale = tempBar.localScale;
        currentScale.y = newHeight;
        tempBar.localScale = currentScale;

        // --- 여기가 수정된 핵심 부분입니다! ---
        // 3. 높이 변화에 맞춰 TempBar의 Y 위치(localPosition)도 함께 조절
        // 막대가 아래에서부터 차오르는 것처럼 보이게 만듭니다.
        Vector3 currentPos = tempBar.localPosition;
        currentPos.y = newHeight / 2.0f - 0.85f; // 원래 코드에 있던 위치 보정 로직
        tempBar.localPosition = currentPos;

        // 4. 온도에 따른 재질 변경
        if (tempBarRenderer != null)
        {
            float tempRange = maxTemp - minTemp;
            float coldThreshold = minTemp + tempRange / 3;
            float warmThreshold = minTemp + 2 * tempRange / 3;

            if (temperature < coldThreshold)
            {
                if (coldMaterial != null) tempBarRenderer.material = coldMaterial;
            }
            else if (temperature < warmThreshold)
            {
                if (warmMaterial != null) tempBarRenderer.material = warmMaterial;
            }
            else
            {
                if (hotMaterial != null) tempBarRenderer.material = hotMaterial;
            }
        }
    }
}

