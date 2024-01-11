using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AlterunaFPS
{
	
	public class Health : MonoBehaviour
	{

		public Health Parent;
		
		[Space]
		public EfxManager.ImpactType MaterialType = EfxManager.ImpactType.Stone;
		
		public float PenetrationResistance = 0.5f;
		public float DamageMultiplier = 1f;
		public float HealthPoints = 0f;
		
		public UnityEvent<ushort> OnDeath;
		
		// Only apply once per health family.
		private float _lastDamage;
		private int _lastDamageIndex;

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

				if (transform.root.CompareTag("Player"))
                {
					ScoreBoard.Instance.AddScore(senderID, (int)damage);
				}

				if (HealthPoints <= 0f)
				{
					HealthPoints = 0f;
					OnDeath.Invoke(senderID);
				}
			}
		}
	}
}