using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashDamage : MonoBehaviour
{
    [ColorUsage(true, true), SerializeField] private Color _color;
    [SerializeField] private float _flashTime = .2f;

    private SpriteRenderer[] _renderers;
    private Material[] _materials;

    private float _timer;

    private bool _isFlashing = false;

    private int _flashAmountID = Shader.PropertyToID("_FlashAmount");
    private int _flashColorID = Shader.PropertyToID("_FlashColor");

    private void Awake() 
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        _materials = new Material[_renderers.Length];

        for(int i = 0; i < _materials.Length; i++)
        {
            _materials[i] = _renderers[i].material;
        }
    }

    private void Update() 
    {
        if(_isFlashing)
        {
            _timer += Time.deltaTime;

            float lerpedAmount = Mathf.Lerp(1f, 0f, (_timer / _flashTime));

            for(int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetFloat(_flashAmountID, lerpedAmount);
            }

            if(_timer > _flashTime)
            {
                _isFlashing = false;
                _timer = 0f;

                for(int i = 0; i < _materials.Length; i++)
                {
                    _materials[i].SetFloat(_flashAmountID, 0f);
                }
            }
        }
    }

    public void Flash()
    {
        _isFlashing = true;
        _timer = 0f;

        for(int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetFloat(_flashAmountID, 1f);
            _materials[i].SetColor(_flashColorID, _color);
        }
    }
}
