using UnityEngine;

namespace AlterunaFPS
{
	public partial class PlayerController
	{
		
		private Vector3 NormalizeEulerAngles(Vector3 eulerAngles)
		{
			eulerAngles.x = NormalizeAngle(eulerAngles.x);
			eulerAngles.y = NormalizeAngle(eulerAngles.y);
			eulerAngles.z = NormalizeAngle(eulerAngles.z);
			return eulerAngles;
		}
		
		private Vector3 NormalizeEulerAnglesXY(Vector3 eulerAngles)
		{
			eulerAngles.x = NormalizeAngle(eulerAngles.x);
			eulerAngles.y = NormalizeAngle(eulerAngles.y);
			//eulerAngles.z = NormalizeAngle(eulerAngles.z);
			return eulerAngles;
		}
		
		private Vector2 NormalizeEulerAngles(Vector2 eulerAngles)
		{
			eulerAngles.x = NormalizeAngle(eulerAngles.x);
			eulerAngles.y = NormalizeAngle(eulerAngles.y);
			return eulerAngles;
		}

		private static float NormalizeAngle(float angle)
		{
			while (angle > 180f)
				angle -= 360f;
			while (angle < -180f)
				angle += 360f;
			return angle;
		}

		private static float NormalizeAnglePos(float angle) => (angle + 360f) % 360f;
		
		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		public static float GetAngleBetweenAngles(float angleA, float angleB)
		{
			float clockwiseAngle = NormalizeAngle(angleB - angleA);
			float counterClockwiseAngle = NormalizeAngle(angleA - angleB);

			if (Mathf.Abs(clockwiseAngle) < Mathf.Abs(counterClockwiseAngle))
			{
				return clockwiseAngle;
			}

			return counterClockwiseAngle;
		}
	}
}