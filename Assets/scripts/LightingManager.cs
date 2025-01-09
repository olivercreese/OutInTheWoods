using UnityEngine;

[ExecuteAlways] 
public class LightingManager : MonoBehaviour //https://www.youtube.com/watch?v=m9hj9PdO328
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField] private Material skyboxMaterial;
    [SerializeField,Range(0,24)] public float TimeOfDay;

    private void Update()
    {
        if (preset == null)
            return;

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime/12;
            TimeOfDay %= 24;
            UpdateLighting(TimeOfDay / 24f);

        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }


    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
    }

    private void OnValidate()
    {
        if (directionalLight != null)
            return;
        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
        
    }



}
