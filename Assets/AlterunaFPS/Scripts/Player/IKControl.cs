using UnityEngine;

namespace AlterunaFPS
{
	[RequireComponent(typeof(Animator))]
	public class IKControl : MonoBehaviour {

		private Animator _animator;
		
		public bool IkActive = true;
		public Transform RightHandTarget;
		public Transform LeftHandTarget;

		void Start()
		{
			_animator = GetComponent<Animator>();
		}

		//a callback for calculating IK
		void OnAnimatorIK(int layerIndex)
		{
			if(_animator) {

				float v = IkActive ? 1 : 0;
					
				_animator.SetIKPositionWeight(AvatarIKGoal.RightHand,v);
				_animator.SetIKRotationWeight(AvatarIKGoal.RightHand,v);
				_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,v);
				_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,v);
				
				if (IkActive)
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