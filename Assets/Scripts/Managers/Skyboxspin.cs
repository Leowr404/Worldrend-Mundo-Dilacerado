using System.Collections;
using UnityEngine;

public class Skyboxspin : MonoBehaviour
{
    [Header("Skybox (Dual Panoramic)")]
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;

    [Header("Light & Fog Gradients (0..1)")]
    [SerializeField] private Gradient gradientNightToSunrise;
    [SerializeField] private Gradient gradientSunriseToDay;
    [SerializeField] private Gradient gradientDayToSunset;
    [SerializeField] private Gradient gradientSunsetToNight;

    [Header("Directional Light (Sol)")]
    [SerializeField] private Light globalLight;
    [SerializeField] private AnimationCurve lightIntensityCurve = AnimationCurve.Linear(0, 0.8f, 1, 1.2f);

    [Header("Clock")]
    [Tooltip("Minutos de jogo que passam por segundo real. 1 => 24 min = 1 dia.")]
    [SerializeField] private float minutesPerRealSecond = 1f;
    [Tooltip("Hora inicial (0-23).")]
    [SerializeField] private int startHour = 5;

    [Header("Transições em MINUTOS de jogo")]
    [Tooltip("Duração da transição Noite -> Amanhecer (ex.: 30 = meia hora de jogo).")]
    [SerializeField] private float t_NightToSunrise = 30f;
    [SerializeField] private float t_SunriseToDay = 30f;
    [SerializeField] private float t_DayToSunset = 30f;
    [SerializeField] private float t_SunsetToNight = 30f;

    [Header("Skybox Rotation")]
    [SerializeField] private bool rotateSkybox = true;
    [SerializeField] private float skyboxRotationSpeed = 0.5f;

    // ---- estado do relógio
    [SerializeField] private int minutes;
    public int Minutes { get => minutes; private set { minutes = value; if (minutes >= 60) { minutes = 0; Hours++; } } }

    [SerializeField] private int hours;
    public int Hours { get => hours; private set { hours = (value >= 24) ? 0 : value; } }

    [SerializeField] private int days;
    public int Days { get => days; private set { days = value; } }

    private float minuteAccumulator; // fração do próximo minuto (0..1)

    // horários (em minutos do dia)
    const int H6 = 6 * 60;   // 06:00
    const int H8 = 8 * 60;   // 08:00
    const int H18 = 16 * 60;  // 16:00
    const int H22 = 22 * 60;  // 22:00
    const int DAY_MINUTES = 1440;

    void Start()
    {
        Hours = Mathf.Clamp(startHour, 0, 23);
        Minutes = 0;

        // seta estado coerente no primeiro frame
        ApplyStablePhaseImmediate(CurrentMinutesOfDay());
        //UpdateSunDirection();
    }

    void Update()
    {
        // avança tempo
        minuteAccumulator += Time.deltaTime * minutesPerRealSecond;
        while (minuteAccumulator >= 1f)
        {
            minuteAccumulator -= 1f;
            Minutes++;
            if (Hours == 0 && Minutes == 0) Days++; // virou o dia
        }

        // minuto do dia (float) com fração
        float nowMin = CurrentMinutesOfDay();

        // atualiza skybox (blend contínuo) e luz
        UpdateSkyboxBlend(nowMin);
        UpdateLight(nowMin);

        // rotação opcional
        if (rotateSkybox && RenderSettings.skybox != null)
            RotateDualPanoramicSkybox(RenderSettings.skybox, skyboxRotationSpeed);
    }

    float CurrentMinutesOfDay()
    {
        // soma fração do próximo minuto
        return (Hours * 60f + Minutes + minuteAccumulator) % DAY_MINUTES;
    }

    // ---------- SKYBOX ----------
    void UpdateSkyboxBlend(float nowMin)
    {
        var sky = RenderSettings.skybox;
        if (!sky) return;

        // blocos de transição por janela de minutos de JOGO
        // 1) 06:00 -> fade Night -> Sunrise
        if (InWindow(nowMin, H6, t_NightToSunrise))
        {
            float k = WindowT(nowMin, H6, t_NightToSunrise);
            SetDualPanoramicBlend(sky, skyboxNight, skyboxSunrise, k);
            // cor
            ApplyLightColor(gradientNightToSunrise.Evaluate(k));
            return;
        }

        // 2) 08:00 -> fade Sunrise -> Day
        if (InWindow(nowMin, H8, t_SunriseToDay))
        {
            float k = WindowT(nowMin, H8, t_SunriseToDay);
            SetDualPanoramicBlend(sky, skyboxSunrise, skyboxDay, k);
            ApplyLightColor(gradientSunriseToDay.Evaluate(k));
            return;
        }

        // 3) 18:00 -> fade Day -> Sunset
        if (InWindow(nowMin, H18, t_DayToSunset))
        {
            float k = WindowT(nowMin, H18, t_DayToSunset);
            SetDualPanoramicBlend(sky, skyboxDay, skyboxSunset, k);
            ApplyLightColor(gradientDayToSunset.Evaluate(k));
            return;
        }

        // 4) 22:00 -> fade Sunset -> Night
        if (InWindow(nowMin, H22, t_SunsetToNight))
        {
            float k = WindowT(nowMin, H22, t_SunsetToNight);
            SetDualPanoramicBlend(sky, skyboxSunset, skyboxNight, k);
            ApplyLightColor(gradientSunsetToNight.Evaluate(k));
            return;
        }

        // fora das janelas: fase estável (sem blend)
        ApplyStablePhaseImmediate(nowMin);
    }

    void ApplyStablePhaseImmediate(float nowMin)
    {
        var sky = RenderSettings.skybox;
        if (!sky) return;

        // define qual textura fica "sozinha" (Blend = 0)
        if (nowMin < H6)
            SetDualPanoramicStable(sky, skyboxNight, gradientSunsetToNight, 1f);
        else if (nowMin < H8)
            SetDualPanoramicStable(sky, skyboxSunrise, gradientNightToSunrise, 1f);
        else if (nowMin < H18)
            SetDualPanoramicStable(sky, skyboxDay, gradientSunriseToDay, 1f);
        else if (nowMin < H22)
            SetDualPanoramicStable(sky, skyboxSunset, gradientDayToSunset, 1f);
        else
            SetDualPanoramicStable(sky, skyboxNight, gradientSunsetToNight, 1f);
    }

    void SetDualPanoramicStable(Material sky, Texture2D tex, Gradient g, float eval)
    {
        if (sky.HasProperty("_Texture1")) sky.SetTexture("_Texture1", tex);
        else if (sky.HasProperty("_Tex1")) sky.SetTexture("_Tex1", tex);

        if (sky.HasProperty("_Blend")) sky.SetFloat("_Blend", 0f);

        ApplyLightColor(g.Evaluate(eval));
    }

    void SetDualPanoramicBlend(Material sky, Texture2D texA, Texture2D texB, float k)
    {
        if (sky.HasProperty("_Texture1")) sky.SetTexture("_Texture1", texA);
        else if (sky.HasProperty("_Tex1")) sky.SetTexture("_Tex1", texA);

        if (sky.HasProperty("_Texture2")) sky.SetTexture("_Texture2", texB);
        else if (sky.HasProperty("_Tex2")) sky.SetTexture("_Tex2", texB);

        if (sky.HasProperty("_Blend")) sky.SetFloat("_Blend", Mathf.Clamp01(k));
    }

    void RotateDualPanoramicSkybox(Material sky, float speed)
    {
        float rot = 0f;
        if (sky.HasProperty("_Rotation1")) rot = sky.GetFloat("_Rotation1");
        else if (sky.HasProperty("_Rotation2")) rot = sky.GetFloat("_Rotation2");
        else if (sky.HasProperty("_Rotation")) rot = sky.GetFloat("_Rotation");

        rot = (rot + speed * Time.deltaTime) % 360f;

        if (sky.HasProperty("_Rotation1")) sky.SetFloat("_Rotation1", rot);
        if (sky.HasProperty("_Rotation2")) sky.SetFloat("_Rotation2", rot);
        if (sky.HasProperty("_Rotation")) sky.SetFloat("_Rotation", rot);
    }

    // ---------- LUZ ----------
    void UpdateLight(float nowMin)
    {
        if (!globalLight) return;

        // intensidade ao longo do dia (0..1)
        float dayT = nowMin / DAY_MINUTES;
        globalLight.intensity = lightIntensityCurve.Evaluate(dayT);

        // direção do sol (elevação no eixo X)
        float sunX = (dayT * 360f) - 90f;
        globalLight.transform.rotation = Quaternion.Euler(sunX, 30f, 0f);
    }
    /*private void UpdateSunDirection()
    {
        if (globalLight == null) return;

        float minutesOfDay = Hours * 60f + Minutes;
        float dayT = minutesOfDay / 1440f; // 1440 min = 24h

        // Rotação do sol (elevação). Ajuste o Y se quiser azimute fixo (ex.: 30f).
        float sunX = (dayT * 360f) - 90f;
        globalLight.transform.rotation = Quaternion.Euler(sunX, 0f, 0f);

        // Intensidade baseada na curva ao longo do dia
        globalLight.intensity = lightIntensityCurve.Evaluate(dayT);
    }*/

    void ApplyLightColor(Color c)
    {
        if (!globalLight) return;
        globalLight.color = c;
        RenderSettings.fogColor = c;
    }

    // ---------- helpers de janela ----------
    bool InWindow(float nowMin, float startMin, float durationMin)
    {
        return nowMin >= startMin && nowMin < (startMin + durationMin);
    }

    float WindowT(float nowMin, float startMin, float durationMin)
    {
        return Mathf.Clamp01((nowMin - startMin) / Mathf.Max(0.0001f, durationMin));
    }
}
