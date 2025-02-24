using System.Collections;
using UnityEngine;

public class EnemyDamageFX : MonoBehaviour
{
    private Material material;
    private Color originalColor;
    private static readonly int BaseMap = Shader.PropertyToID("_BaseColor"); // For URP/Lit Shader

    void Start()
    {
        if (TryGetComponent<Renderer>(out Renderer renderer))
        {
            material = renderer.material;
            originalColor = material.GetColor(BaseMap);
        }
    }

    public void TakeDamageEffect(float duration = 0.1f)
    {
        if (material != null)
        {
            StopAllCoroutines();
            StartCoroutine(DamageFlash(duration));
        }
    }

    private IEnumerator DamageFlash(float duration)
    {
        material.SetColor(BaseMap, Color.red);
        yield return new WaitForSeconds(duration);
        material.SetColor(BaseMap, originalColor);
    }
}
