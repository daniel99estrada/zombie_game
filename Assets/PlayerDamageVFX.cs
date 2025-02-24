using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using System.Diagnostics;

public class PlayerDamageVFX : MonoBehaviour
{
    public float maxIntensity = 0.4f;
    public float FXStrength = 0.1f;

    private PostProcessVolume volume;
    private Vignette vignette;

    public static PlayerDamageVFX Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        if (volume.profile.TryGetSettings<Vignette>(out vignette))
        {
            vignette.enabled.Override(false);
        }
    }

    public void DoDamageEffect()
    {
        StartCoroutine(TakeDamageEffect());
    }

    private IEnumerator TakeDamageEffect()
    {
        float intensity = maxIntensity;

        vignette.enabled.Override(true);
        vignette.intensity.Override(intensity);

        yield return new WaitForSeconds(0.4f);

        while (intensity > 0)
        {
            intensity -= Time.deltaTime * FXStrength;
            if (intensity < 0)
            {
                intensity = 0;
            }
            vignette.intensity.Override(intensity);
            yield return new WaitForSeconds(0.1f);
        }
        vignette.enabled.Override(false);
    }
}
