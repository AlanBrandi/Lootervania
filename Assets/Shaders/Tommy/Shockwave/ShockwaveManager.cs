using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShockwaveManager : MonoBehaviour
{
    [SerializeField] private float _shockWaveTime = 0.75f;

    private Coroutine _shockWaveCoroutine;

    private Material material;

    private static int _waveDistance = Shader.PropertyToID("WaveDistance");

    private void awake(){
        material = GetComponent<SpriteRenderer>().material;
    }

    private void update(){
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            CallShockWave();
        }
    }

    public void CallShockWave(){
        _shockWaveCoroutine = StartCoroutine(ShockWaveAction(-0.1f, 1f));
    }

    private IEnumerator ShockWaveAction(float startPos, float endPos){
        material.SetFloat(_waveDistance, startPos);
        float lerpedAmount = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < _shockWaveTime){
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / _shockWaveTime));
            material.SetFloat(_waveDistance, lerpedAmount);
            yield return null;
        }
    }
}
