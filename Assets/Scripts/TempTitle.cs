using UnityEngine;
using TMPro;
using System.Collections.Generic; // List를 사용하기 위해 필요
using System.Linq; // 평균(Average) 계산을 위해 필요

public class WeatherTextDisplay : MonoBehaviour
{
    [Header("UI 텍스트 설정")]
    public TextMeshProUGUI temperatureText;

    [Header("제어할 모든 온도계")]
    // [중요] TempControl 하나가 아닌, 여러 개를 담을 수 있는 리스트(List)로 변경합니다.
    public List<TempControl> allThermometers = new List<TempControl>();

    [Header("날씨 데이터")]
    // 이제 개별 온도가 아닌 '평균 온도'를 표시합니다.
    public float currentAverageTemperature = 25.0f;
    public string locationName = "서울";

    void Start()
    {
        if (temperatureText == null)
        {
            Debug.LogError("Temperature Text가 연결되지 않았습니다!");
        }
        // 리스트가 비어있는지 확인합니다.
        if (allThermometers.Count == 0)
        {
            Debug.LogError("제어할 온도계가 하나도 연결되지 않았습니다! 인스펙터 창에서 7개를 모두 연결해주세요.");
        }
        
        UpdateWeatherDisplay();
    }

    // 날씨 정보를 텍스트 UI에 업데이트하는 함수
    private void UpdateWeatherDisplay()
    {
        if (temperatureText != null)
        {
            // 여러 온도계의 '평균' 온도를 표시하도록 문구를 변경합니다.
            temperatureText.text = $"{locationName} 평균 온도: {currentAverageTemperature:F1}도";
        }
    }
    
    // --- 계절 변경 함수들 (수정됨) ---

    public void ChangeToSpring()
    {
        if (allThermometers.Count == 0) return; 
        
        locationName = "봄날의 서울";
        
        // 각 온도계의 새로운 온도를 저장할 리스트를 만듭니다.
        List<float> newTemperatures = new List<float>();

        // [중요] 리스트에 있는 모든 온도계에 대해 반복 실행합니다.
        foreach (TempControl thermo in allThermometers)
        {
            // 각 온도계의 함수를 호출하고, 반환된 랜덤 온도를 리스트에 추가합니다.
            newTemperatures.Add(thermo.SetTemperatureForSpringOrAutumn());
        }

        // 모든 온도의 평균을 계산합니다.
        currentAverageTemperature = newTemperatures.Average();
        UpdateWeatherDisplay();
    }
    
    public void ChangeToSummer()
    {
        if (allThermometers.Count == 0) return;

        locationName = "한여름의 서울";
        List<float> newTemperatures = new List<float>();

        foreach (TempControl thermo in allThermometers)
        {
            newTemperatures.Add(thermo.SetTemperatureForSummer());
        }
        
        currentAverageTemperature = newTemperatures.Average();
        UpdateWeatherDisplay();
    }

    public void ChangeToAutumn()
    {
        if (allThermometers.Count == 0) return;
        
        locationName = "가을의 서울";
        List<float> newTemperatures = new List<float>();

        foreach (TempControl thermo in allThermometers)
        {
            newTemperatures.Add(thermo.SetTemperatureForSpringOrAutumn());
        }
        
        currentAverageTemperature = newTemperatures.Average();
        UpdateWeatherDisplay();
    }

    public void ChangeToWinter()
    {
        if (allThermometers.Count == 0) return;
        
        locationName = "한겨울의 서울";
        List<float> newTemperatures = new List<float>();

        foreach (TempControl thermo in allThermometers)
        {
            newTemperatures.Add(thermo.SetTemperatureForWinter());
        }
        
        currentAverageTemperature = newTemperatures.Average();
        UpdateWeatherDisplay();
    }
}

