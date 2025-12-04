using TMPro;
using UnityEngine;

public class TimeSelectorController : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    public enum TimeMode { Hour, Minute }
    public TimeMode currentMode = TimeMode.Hour;

    private int hour = 12;
    private int minute = 0;
    private bool isActive = false;

    public System.Action onSelectionComplete; // 콜백 연결용

    void Start()
    {
        gameObject.SetActive(false); // 시작 시 안 보이게
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMode == TimeMode.Hour)
                hour = (hour < 24) ? hour + 1 : 1;
            else
                minute = (minute + 1) % 60;

            UpdateText();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMode == TimeMode.Hour)
                hour = (hour > 1) ? hour - 1 : 24;
            else
                minute = (minute + 59) % 60;

            UpdateText();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            isActive = false;
            gameObject.SetActive(false);
            onSelectionComplete?.Invoke(); // 엔터 치면 자동으로 대사 연결
        }
    }

    void UpdateText()
    {
        if (currentMode == TimeMode.Hour)
            timeText.text = $"{hour} 시";
        else
            timeText.text = $"{minute} 분";
    }

    public void Activate(TimeMode mode)
    {
        currentMode = mode;
        isActive = true;
        gameObject.SetActive(true);
        UpdateText();
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    public int GetHour() => hour;
    public int GetMinute() => minute;
    public bool IsActive() => isActive;
}
