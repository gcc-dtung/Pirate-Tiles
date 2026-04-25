using UnityEngine;
using PrimeTween;

[RequireComponent(typeof(SpriteRenderer))]
public class CardVFXView : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private MaterialPropertyBlock _mpb;

    private static readonly int BrightnessProp = Shader.PropertyToID("_Brightness");
    private static readonly int DissolveProp = Shader.PropertyToID("_DissolveAmount");

    private Tween _brightnessTween;
    private Tween _dissolveTween;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _mpb = new MaterialPropertyBlock();
        _spriteRenderer.GetPropertyBlock(_mpb);
    }

    public void SetBrightness(float brightness)
    {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(BrightnessProp, brightness);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }

    public void SetDissolve(float amount)
    {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(DissolveProp, amount);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }

    public Tween TweenBrightness(float endValue, float duration)
    {
        _brightnessTween.Stop();
        
        _spriteRenderer.GetPropertyBlock(_mpb);
        // Default brightness is usually 1, if it wasn't set yet, we assume 1.
        float startValue = 1f; 
        
        _brightnessTween = Tween.Custom(this, startValue, endValue, duration, (target, val) => target.SetBrightness(val));
        return _brightnessTween;
    }

    public Tween TweenDissolve(float endValue, float duration)
    {
        _dissolveTween.Stop();

        _spriteRenderer.GetPropertyBlock(_mpb);
        float startValue = 0f;

        _dissolveTween = Tween.Custom(this, startValue, endValue, duration, (target, val) => target.SetDissolve(val));
        return _dissolveTween;
    }
}
