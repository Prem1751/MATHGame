using UnityEngine;

public class DarkLightingSetup : MonoBehaviour
{
    [Header("Dark Lighting Settings")]
    public Color ambientColor = new Color(0.1f, 0.1f, 0.1f);
    public float ambientIntensity = 0.2f;
    public float reflectionIntensity = 0.5f;
    public float fogDensity = 0.05f;
    public Color fogColor = new Color(0.05f, 0.05f, 0.1f);

    void Start()
    {
        SetupDarkEnvironment();
    }

    public void SetupDarkEnvironment()
    {
        // ตั้งค่า Ambient Light
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientIntensity = ambientIntensity;

        // ตั้งค่า Reflection
        RenderSettings.reflectionIntensity = reflectionIntensity;

        // ตั้งค่า Fog
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogColor = fogColor;

        // ตั้งค่า Skybox
        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetFloat("_Exposure", 0.3f);
        }

        Debug.Log("Dark environment setup complete!");
    }

    public void AdjustDarkness(float intensityMultiplier)
    {
        RenderSettings.ambientIntensity = ambientIntensity * intensityMultiplier;
        RenderSettings.fogDensity = fogDensity * (2 - intensityMultiplier);
    }
}