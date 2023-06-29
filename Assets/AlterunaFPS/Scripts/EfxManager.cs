using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlterunaFPS
{
	public class EfxManager : MonoBehaviour
	{
		// Singleton instance
		public static EfxManager Instance;
		
		// Prefabs
		public GameObject FleshImpactPrefab;
		public GameObject MetalImpactPrefab;
		public GameObject SandImpactPrefab;
		public GameObject StoneImpactPrefab;
		public GameObject WoodImpactPrefab;
		
		[Space]
		public GameObject BulletEFPrefab;
		
		// Pools
		private Transform _bulletPool;

		// Particles
		private readonly List<ParticleSystem> _fleshImpactParticles = new();
		private readonly List<ParticleSystem> _metalImpactParticles = new();
		private readonly List<ParticleSystem> _sandImpactParticles = new();
		private readonly List<ParticleSystem> _stoneImpactParticles = new();
		private readonly List<ParticleSystem> _woodImpactParticles = new();
		
		private readonly List<ParticleSystem> _bulletParticles = new();

		// Impact types
		public enum ImpactType
		{
			Flesh,
			Metal,
			Sand,
			Stone,
			Wood
		}

		private void Awake()
		{
			// Set singleton instance
			Instance = this;
			
			// Create pools with this object as parent
			_bulletPool = new GameObject("Bullets").transform;
			_bulletPool.SetParent(transform);
		}

		// Play impact particle of given type
		public void PlayImpact(Vector3 pos, Vector3 normal, Transform parent, ImpactType type = ImpactType.Stone)
		{
			switch (type)
			{
				case ImpactType.Flesh:
					PlayImpact(_fleshImpactParticles, parent, FleshImpactPrefab, pos, normal);
					break;
				case ImpactType.Metal:
					PlayImpact(_metalImpactParticles, parent, MetalImpactPrefab, pos, normal);
					break;
				case ImpactType.Sand:
					PlayImpact(_sandImpactParticles, parent, SandImpactPrefab, pos, normal);
					break;
				case ImpactType.Stone:
					PlayImpact(_stoneImpactParticles, parent, StoneImpactPrefab, pos, normal);
					break;
				case ImpactType.Wood:
					PlayImpact(_woodImpactParticles, parent, WoodImpactPrefab, pos, normal);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
		
		// Play impact using pooled particles
		private void PlayImpact(List<ParticleSystem> particles, Transform owner, GameObject prefab, Vector3 pos, Vector3 normal)
		{
			// try get free particle
			ParticleSystem ps = particles.FirstOrDefault(foo => !foo.isPlaying);

			// if no free particles, create new one
			if (ps == null)
			{
				ps = Instantiate(prefab).GetComponent<ParticleSystem>();
				particles.Add(ps);
			}

			// set position and rotation of particle before playing
			Transform t = ps.transform;
			t.SetParent(owner);
			t.position = pos;
			t.rotation = Quaternion.LookRotation(normal);
			ps.Emit(1);
		}
		
		// Play bullet effect
		public void PlayBullet(Vector3 pos, Vector3 dir, float lifeTime)
		{
			// try get free particle
			ParticleSystem ps = _bulletParticles.FirstOrDefault(foo => !foo.isPlaying);

			// if no free particles, create new one
			if (ps == null)
			{
				ps = Instantiate(BulletEFPrefab, _bulletPool).GetComponent<ParticleSystem>();
				_bulletParticles.Add(ps);
			}

			// set position and rotation of particle before playing
			Transform t = ps.transform;
			t.position = pos;
			t.rotation = Quaternion.LookRotation(dir);
			var main = ps.main;
			main.startLifetimeMultiplier = lifeTime;
			ps.Emit(1);
		}
	}
}