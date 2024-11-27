using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Alteruna;

namespace AlterunaFPS
{

	public class Health : AttributesSync
	{

		public Health Parent;

		[Space]
		public EfxManager.ImpactType MaterialType = EfxManager.ImpactType.Stone;

		public float PenetrationResistance = 0.5f;
		public float DamageMultiplier = 1f;
		public float HealthPoints = 50f;

		public UnityEvent<ushort> OnDeath;

		// Only apply once per health family.
		private float _lastDamage;
		private int _lastDamageIndex;

		public Slider healthSlider, healthSlider2; // Reference to the health slider
		public Gradient healthGradient, healthGradient2; // Optional gradient for health color
		public Image healthFill, healthFill2; // Reference to the fill image of the slider
		public float maxHealth; // Store the initial health points
		public Alteruna.Avatar avatar;


		void Start()
		{
			avatar = GetComponent<Alteruna.Avatar>();
			if (avatar.IsMe)
				healthSlider2.gameObject.SetActive(true);
			maxHealth = HealthPoints; // Set max health
			if (healthSlider != null)
			{
				healthSlider.maxValue = maxHealth;
				healthSlider2.maxValue = maxHealth;
				Debug.Log("max value : " + healthSlider.maxValue);
				healthSlider.value = HealthPoints;
				healthSlider2.value = HealthPoints;
				BroadcastRemoteMethod("UpdateHealthBarColor");
			}
		}

		// void Update()
		// {
		// 	BroadcastRemoteMethod("UpdateHealthBar");
		// }

		[SynchronizableMethod]
		private void UpdateHealthBar()
		{
			if (healthSlider != null)
			{
				healthSlider.value = HealthPoints;
				Debug.Log("slider 1 : " + healthSlider.value);
				healthSlider2.value = HealthPoints;
				Debug.Log("slider 2 : " + healthSlider2.value);
				BroadcastRemoteMethod("UpdateHealthBarColor");
			}
		}

		[SynchronizableMethod]
		private void ResetHealthBar()
		{
			if (healthSlider != null)
			{
				healthSlider.value = healthSlider.maxValue;
				Debug.Log("slider 1 : " + healthSlider.value);
				healthSlider2.value = healthSlider.maxValue;
				Debug.Log("slider 2 : " + healthSlider2.value);
				BroadcastRemoteMethod("UpdateHealthBarColor");
			}
		}


		[SynchronizableMethod]
		private void UpdateHealthBarColor()
		{
			if (healthFill != null && healthGradient != null)
			{
				float normalizedHealth = HealthPoints / maxHealth;
				healthFill.color = healthGradient.Evaluate(normalizedHealth);
				healthFill2.color = healthGradient2.Evaluate(normalizedHealth);
			}
		}

		public bool Alive
		{
			get
			{
				return HealthPoints > 0f;
			}
		}

		public void TakeDamage(ushort senderID, float damage) => TakeDamage(senderID, damage, Time.frameCount);

		private void TakeDamage(ushort senderID, float damage, int damageIndex)
		{
			damage *= DamageMultiplier;

			// Check if damage is already applied.
			if (_lastDamageIndex == damageIndex)
			{
				// Undo last damage before applying new damage.
				if (damage > _lastDamage)
					TakeDamage(senderID, -_lastDamage, damageIndex);
				// If new damage is less than last damage, ignore.
				else return;
			}
			_lastDamage = damage;
			_lastDamageIndex = damageIndex;

			// apply damage
			if (Parent != null)
			{
				Parent.TakeDamage(senderID, damage);
			}
			else if (Alive)
			{
				HealthPoints -= damage;
				Debug.Log("health point = " + HealthPoints);
				BroadcastRemoteMethod("UpdateHealthBar"); // Update the health bar

				Debug.Log("health point = " + healthSlider.value);

				if (transform.root.CompareTag("Player"))
				{
					ScoreBoard.Instance.AddScore(senderID, (int)damage);
				}

				if (HealthPoints <= 0f)
				{
					HealthPoints = 0f;
					OnDeath.Invoke(senderID);
					BroadcastRemoteMethod("ResetHealthBar");
				}
			}
		}

		public void CallUpdateHealthBar()
		{
			BroadcastRemoteMethod("UpdateHealthBar");
		}
	}
}