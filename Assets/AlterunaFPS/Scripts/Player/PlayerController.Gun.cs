using System.Linq;
using UnityEngine;

namespace AlterunaFPS
{
	public partial class PlayerController
	{
		
		[Header("Gun")]
		public Transform GunRoot;
		public Transform FirePoint;
		public IKControl IKController;
		public Animator GunAnimator;
		public LayerMask BulletCollisionLayers = ~0;
		public int GunMagazineSize = 5;
		public float GunFireTime = 0.2f;
		public float GunReloadTime = 2.35f;
		public float DistanceFromBody = 0.3f;
		[Header("Aiming")]
		public float ZoomFov = 30f;
		public float ZoomInTime = 0.2f;
		public float ZoomOutTime = 0.18f;
		
		private float _gunFireCooldown;
		private float _gunReloadCooldown;
		private int _gunMagazine;
		
		private float _gunBaseHeight;
		
		private Transform _gunLooker;

		private void InitializeGun()
		{
			_gunBaseHeight = GunRoot.localPosition.y;
			
			// create a new object to help manage the gun rotation
			_gunLooker = new GameObject("GunLooker").transform;
			_gunLooker.SetParent(GunRoot);
			_gunLooker.position = GunRoot.position;
		}

		private void ResetAmmo()
		{
			_gunMagazine = GunMagazineSize;
		}

		private void GunAction(bool lockInput = false)
		{
			if (Input.GetKey(KeyCode.Mouse1))
			{
				CinemachineVirtualCameraInstance.Instance.SetFov(ZoomFov, ZoomInTime);
			}
			else
			{
				CinemachineVirtualCameraInstance.Instance.ResetFov(ZoomOutTime);
			}
			
			if (GunAnimator != null)
			{

				if (_firstPerson || Quaternion.Angle(_cameraTarget.rotation, GunRoot.parent.rotation) < 50)
				{
					
					var camForward = _cameraTarget.forward;
					
					// move the gun to follow the camera
					var rad = GetAngleBetweenAngles(_cameraTarget.eulerAngles.y, GunRoot.parent.eulerAngles.y) * Mathf.Deg2Rad;
					GunRoot.localPosition = new Vector3(Mathf.Sin(rad / 2f) * DistanceFromBody, _gunBaseHeight + camForward.y * DistanceFromBody * 0.95f, Mathf.Cos(rad) * DistanceFromBody);
					
					// point that the gun is aiming at
					
					Vector3 point;
					// aim gun at camera target
					if (Physics.Raycast(_cameraTarget.position + camForward, camForward, out var hit, 50f) && hit.collider.gameObject != gameObject)
					{
						point = hit.point;
					}
					else
					{
						// if nothing is hit, aim at a point far away
						point = camForward * 100 + _cameraTarget.position;
					}
					
					
					// rotate the gun to look at the point with some smoothing
					_gunLooker.LookAt(point);
					GunRoot.rotation = Quaternion.Lerp(GunRoot.rotation, _gunLooker.rotation, Time.deltaTime * 10);
					
				}

				IKController.IkActive = true;
				
				// if the gun is firing or reloading, don't allow any other actions
				if ((_gunFireCooldown -= Time.deltaTime) > 0 || (_gunReloadCooldown -= Time.deltaTime) > 0)
				{
					return;
				}
				
				// Only allow sending the fire command if the player is owner.
				if (_isOwner)
				{
					if (Input.GetKeyDown(KeyCode.Mouse0) && _gunMagazine > 0)
					{
						if (_offline)
							FireBullet(FirePoint.position, FirePoint.forward);
						else
							BroadcastRemoteMethod(nameof(FireBullet), FirePoint.position, FirePoint.forward, 10f, 10f);
						//FireBullet(FirePoint.position, FirePoint.forward);
						return;
					}
				} 
				
				if (_reload || _gunMagazine <= 0)
				{
					GunAnimator.Play(_animIDGunReload);
					_gunReloadCooldown = GunReloadTime;
					_gunMagazine = GunMagazineSize;
				}
			}
			else
			{
				IKController.IkActive = false;
			}
		}
		
		[SynchronizableMethod]
		private void FireBullet(Vector3 origin, Vector3 direction, float penetration = 10f, float damage = 10f)
		{
			GunAnimator.Play(_animIDGunFire);
			
			_gunFireCooldown = GunFireTime;
			_gunMagazine--;
			
			float currentPenetration = penetration;
			float hitDistance = 40f;
			
			// Raycast with penetration
			RaycastHit[] hits = Physics.RaycastAll(origin, direction, hitDistance, BulletCollisionLayers);
			// Sort the hits by distance
			hits = hits.OrderBy(h => h.distance).ToArray();
			int l = hits.Length;
			
			for (int i = 0; i < l; i++)
			{
				if (hits[i].collider.TryGetComponent(out Health target))
				{
					EfxManager.Instance.PlayImpact(hits[i].point, hits[i].normal, hits[i].transform, target.MaterialType);
					
					float distanceDamageDropoff = 10f / (hits[i].distance + 10f);
					if ((currentPenetration - target.PenetrationResistance) / penetration * distanceDamageDropoff <= 0)
					{
						DrawLine(i, Color.red);
						// fragmentation damage
						target.TakeDamage(Mathf.Min(2 * currentPenetration / penetration, 1f) * damage * distanceDamageDropoff);
						hitDistance = hits[i].distance;
						
						// If penetration is not enough to go through the target, stop the bullet
						break;
					}

					// penetration damage with dropoff
					target.TakeDamage(currentPenetration / penetration * damage * distanceDamageDropoff);
					DrawLine(i, Color.yellow);

					// decreases its penetration after the projectile have exited the target
					currentPenetration -= target.PenetrationResistance;
				}
				else
				{
					if (hits[i].collider.gameObject.layer == 0) // default layer
					{
						EfxManager.Instance.PlayImpact(hits[i].point, hits[i].normal, hits[i].transform);
						currentPenetration -= 5f;

						DrawLine(i, Color.grey);
					}
					else
						DrawLine(i, Color.black);
				}
			}

			EfxManager.Instance.PlayBullet(origin, direction, hitDistance / 100f);

			void DrawLine(int i, Color color, float duration = 1f)
			{
				if (i == 0)
					Debug.DrawLine(origin, hits[i].point, color, duration);
				else
					Debug.DrawLine(hits[i - 1].point, hits[i].point, color, duration);
			}
		}
	}
}