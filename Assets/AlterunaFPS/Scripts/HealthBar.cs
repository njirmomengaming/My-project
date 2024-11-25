using System;
using UnityEngine;
using UnityEngine.UI;
using Alteruna;
using AlterunaFPS;

public class HealthBar : AttributesSync
{
    public Slider healthSlider;
    public Gradient gradient;
    public Image fill;
    public Health health;
    public Alteruna.Avatar avatar;

    void Start()
    {
        avatar = health.avatar;

        if (!avatar.IsMe)
            return;

        if (health == null && avatar.IsMe)
        {
            health = avatar.gameObject.GetComponent<Health>();
        }

        if (health != null)
        {
            SetMaxHealth(health.HealthPoints);
        }
    }

    void Update()
    {
        if (!avatar.IsMe)
            return;

        if (health != null)
        {
            SetHealth(health.HealthPoints);
        }
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
        fill.color = gradient.Evaluate(healthSlider.normalizedValue);
    }
}
