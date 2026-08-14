using UnityEngine;

namespace AlterunaFPS
{
	public partial class PlayerController
	{
		[Header("IK")]
		public Transform RightHandTarget;
		public Transform LeftHandTarget;
		private bool _ikActive = true;
		
		private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDFreeFall;
		private int _animIDMotionSpeed;
		private int _animIDHeadLookX;
		private int _animIDHeadLookY;
		private int _animIDGunFire;
		private int _animIDGunReload;
		
		private Animator _animator;
		
		private bool _hasAnimator;

		private void InitialiseAnimations()
		{
			_hasAnimator = TryGetComponent(out _animator);
			
			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
			
			// setup animation IDs
			_animIDSpeed = Animator.StringToHash("Speed");
			_animIDGrounded = Animator.StringToHash("Grounded");
			_animIDJump = Animator.StringToHash("Jump");
			_animIDFreeFall = Animator.StringToHash("FreeFall");
			_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
			_animIDHeadLookX = Animator.StringToHash("HeadLookX");
			_animIDHeadLookY = Animator.StringToHash("HeadLookY");

			_animIDGunFire = Animator.StringToHash("Fire");
			_animIDGunReload = Animator.StringToHash("Reload");
		}
		
		private void OnFootstep(AnimationEvent animationEvent)
		{
			if (animationEvent.animatorClipInfo.weight > 0.5f)
			{
				if (FootstepAudioClips.Length > 0)
				{
					var index = Random.Range(0, FootstepAudioClips.Length);
					AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
				}
			}
		}

		private void OnLand(AnimationEvent animationEvent)
		{
			if (animationEvent.animatorClipInfo.weight > 0.5f)
			{
				AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
			}
		}

		private void UpdateIK()
		{
			if(_animator) {

				float v = _ikActive ? 1 : 0;
	
				_animator.SetIKPositionWeight(AvatarIKGoal.RightHand,v);
				_animator.SetIKRotationWeight(AvatarIKGoal.RightHand,v);
				_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,v);
				_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,v);

				if (_ikActive)
				{
					// Set the right hand target position and rotation, if one has been assigned
					_animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
					_animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
					_animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
					_animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
				}
			}
		}
	}
}