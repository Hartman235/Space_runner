using UnityEngine;

public class StarTwinkle : MonoBehaviour
{
    private Renderer starRenderer;
    private Color originalColor;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float twinkleSpeed = 0.8f;

    void Start()
    {
        starRenderer = GetComponent<Renderer>();
        originalColor = starRenderer.material.GetColor("_EmissionColor");
    }

    void Update()
    {
        float intensity = Mathf.PingPong(Time.time * twinkleSpeed, maxIntensity - minIntensity) + minIntensity;
        starRenderer.material.SetColor("_EmissionColor", originalColor * intensity);
    }
}
