using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
		private readonly List<EfxPoolObject> _fleshImpactParticles = new();
		private readonly List<EfxPoolObject> _metalImpactParticles = new();
		private readonly List<EfxPoolObject> _sandImpactParticles = new();
		private readonly List<EfxPoolObject> _stoneImpactParticles = new();
		private readonly List<EfxPoolObject> _woodImpactParticles = new();
		
		private readonly List<EfxPoolObject> _bulletParticles = new();

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
		private void PlayImpact(List<EfxPoolObject> particles, Transform owner, GameObject prefab, Vector3 pos, Vector3 normal)
		{
			// try get free particle
			EfxPoolObject efx = particles.FirstOrDefault(foo => !foo.IsPlaying);
			
			
			// if no free particles, create new one
			if (efx == null || efx.Obj == null)
			{
				if (efx != null)
					Debug.LogWarning(efx.Obj);
				efx = new EfxPoolObject();
				efx.Obj = Instantiate(prefab);
				efx.HaveParticle = efx.Obj.TryGetComponent(out efx.Particle);
				
				particles.Add(efx);
			}

			// set position and rotation of particle before playing
			efx.SetPosRot(owner, pos, normal);
			efx.Emit(1);
		}
		
		// Play bullet effect
		public void PlayBullet(Vector3 pos, Vector3 dir, float lifeTime)
		{
			// try get free particle
			EfxPoolObject efx = _bulletParticles.FirstOrDefault(foo => !foo.IsPlaying);

			// if no free particles, create new one
			if (efx == null || efx.Obj == null)
			{
				if (efx != null)
					Debug.LogWarning(efx.Obj);
				efx = new EfxPoolObject();
				efx.Obj = Instantiate(BulletEFPrefab);
				efx.HaveParticle = efx.Obj.TryGetComponent(out efx.Particle);
				
				_bulletParticles.Add(efx);
			}

			// set position and rotation of particle before playing
			efx.SetPosRot(pos, dir);
			efx.StartLifetimeMultiplier(lifeTime);
			efx.Emit(1);
		}
		
		// support both particle systems bse effects and self managed objects
		private class EfxPoolObject
		{
			public GameObject Obj;
			public ParticleSystem Particle;
			public bool HaveParticle;
			public bool IsPlaying => Obj != null && Obj.activeSelf && (!HaveParticle || Particle.time < Particle.main.duration);

			public void SetPosRot(Transform parent, Vector3 pos, Vector3 normal)
			{
				Transform t = Obj.transform;
				t.SetParent(parent);
				t.position = pos;
				t.rotation = Quaternion.FromToRotation(Vector3.forward, normal);
				t.lossyScale.Set(1, 1, 1);
			}
			
			public void SetPosRot(Vector3 pos, Vector3 dir)
			{
				Transform t = Obj.transform;
				t.position = pos;
				t.rotation = Quaternion.LookRotation(dir);
			}
			
			public void Emit(int count = 1)
			{
				Obj.SetActive(true);
				if (HaveParticle)
					Particle.Emit(count);
			}

			public void StartLifetimeMultiplier(float lifeTime)
			{
				if (!HaveParticle) return;
				var main = Particle.main;
				main.startLifetimeMultiplier = lifeTime;
			}
		}
	}
}