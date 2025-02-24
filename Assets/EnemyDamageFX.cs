using System.Collections;
using UnityEngine;

public class EnemyDamageFX : MonoBehaviour
{
    private Material material;
    private Color originalColor;
    private static readonly int BaseMap = Shader.PropertyToID("_BaseColor");
    public Enemy enemy;
    public float duration = 0.1f;
    
    void Awake()
    {
        enemy.OnAttacked += TakeDamageEffect;
    }
    void Start()
    {
        if (TryGetComponent<Renderer>(out Renderer renderer))
        {
            material = renderer.material;
            originalColor = material.GetColor(BaseMap);
        }
    }

    public void TakeDamageEffect()
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
