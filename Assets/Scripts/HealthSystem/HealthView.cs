using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private HealthController healthController;

    private void Awake() 
    {
        healthController.onDamageTaken?.AddListener(DamageTakenFeedback);
        healthController.onHeal?.AddListener(HealFeedback);

        slider.maxValue = healthController.MaxHealth;
    }

    private void Start() 
    {
        SetSliderValue(healthController.CurrentHealth);
    }

    public void DamageTakenFeedback(HealthModel arg ,int damage)
    {
        slider.value -= damage;
    }
    public void HealFeedback(HealthModel arg, int heal)
    {
        slider.value += heal;
    }
      public void SetSliderValue(int value)
    {
        slider.value = value;
    }

    private void OnDisable() 
    {
        healthController.onDamageTaken?.RemoveListener(DamageTakenFeedback);
        healthController.onHeal?.RemoveListener(HealFeedback);
    }
}
