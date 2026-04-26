using UnityEngine;
using UnityEngine.UI;

public class ShotMeter : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform indicator;       // thin vertical bar that sweeps across
    public RectTransform perfectZone;     // green zone in the middle
    public RectTransform barBackground;   // full width of the bar

    [Header("Settings — keep in sync with BallLauncher")]
    public float timingWindowDuration = 1f;
    public float perfectWindowSize = 0.2f;

    private float barHeight;
    private bool running = false;
    private float startTime;

    void Start()
    {
        barHeight = barBackground.rect.height;
        UpdateZoneVisuals();
        Hide();
    }

    void Update()
    {
        if (!running) return;

        float t = Mathf.Clamp01((Time.time - startTime) / timingWindowDuration);
        SetIndicatorPosition(t);

        if (t >= 1f)
            Hide();
    }

    public void StartBar()
    {
        startTime = Time.time;
        running = true;
        gameObject.SetActive(true);
        SetIndicatorPosition(0f);
    }

    public void StopBar()
    {
        running = false;
    }

    public void Hide()
    {
        running = false;
        gameObject.SetActive(false);
    }

    void SetIndicatorPosition(float t)
    {
        float y = Mathf.Lerp(-barHeight / 2f, barHeight / 2f, t);
        indicator.anchoredPosition = new Vector2(0, y);
    }

    void UpdateZoneVisuals()
    {
        float perfectStart = 0.5f - perfectWindowSize / 2f;
        float zoneHeight = perfectWindowSize * barHeight;
        float zoneY = Mathf.Lerp(-barHeight / 2f, barHeight / 2f, perfectStart) + zoneHeight / 2f;

        perfectZone.sizeDelta = new Vector2(perfectZone.sizeDelta.x, zoneHeight);
        perfectZone.anchoredPosition = new Vector2(0, zoneY);
    }
}
