using Alteruna;
using Alteruna.Trinity;
using UnityEngine;

namespace AlterunaFPS
{
	// Handle player synchronisation, like rotation and some of the position data.
	public partial class PlayerController
	{
		private float _lastYaw;
		private float _lastPitch;
		private Vector2 _lastPosition;
		private bool _force;
		
		private bool _isOwner = true;
		private bool _isHost = false;
		private bool _offline;
		private bool _possesed;

		private void InitializeNetworking()
		{
			// assume as owner if no avatar attached
			if (Avatar == null)
			{
				_isOwner = true;
				_isHost = true;
				_offline = true;
				OnPossession();
			}
			else
			{
				_isOwner = Avatar.IsOwner;
				_isHost = Avatar.Possessor.IsHost;
				_possesed = Avatar.IsPossessed;
				
				Avatar.OnPossessed.AddListener(_ =>
				{
					_lastSpawnIndex = Avatar.Possessor.Index;
					_isOwner = Avatar.IsOwner;
					_possesed = true;
					OnPossession();
				});
				if (Avatar.IsPossessed) OnPossession();
			}
		}

		private void Sync()
		{
			if (_isOwner && !_offline)
			{
				SyncUpdate();
			}
		}

		public override void Serialize(ITransportStreamWriter processor, byte LOD, bool forceSync = false)
		{
			_force = forceSync;
			base.Serialize(processor, LOD, forceSync);
		}

		public override void AssembleData(Writer writer, byte LOD = 100)
		{
			var p = transform.position;
			byte flags = _force
				? (byte)255
				: (byte)(
					(Mathf.Abs(_lastYaw - _cinemachineTargetYaw) > 0.1f ? 1 : 0) |
					(Mathf.Abs(_lastPitch - _cinemachineTargetPitch) > 0.1f ? 2 : 0) |
					(Mathf.Abs(_lastPosition.x - p.x) + Mathf.Abs(_lastPosition.y - p.y) > 0.1f ? 4 : 0) |
					(gameObject.activeSelf ? 16 | 4 : 0)
				);

			writer.Write(flags);
			//Debug.Log("AssembleData " + flags);

			if ((flags & 1) != 0)
				writer.Write(_cinemachineTargetYaw);
			if ((flags & 2) != 0)
				writer.Write(_cinemachineTargetPitch);
			if ((flags & 4) != 0)
				writer.Write(p);
			if ((flags & 8) != 0)
				writer.Write((byte)_gunMagazine);
		}

		public override void DisassembleData(Reader reader, byte LOD = 100)
		{
			byte flags = reader.ReadByte();
			//Debug.Log("DisassembleData " + flags);

			if ((flags & 1) != 0)
				_cinemachineTargetYaw = reader.ReadFloat();
			if ((flags & 2) != 0)
				_cinemachineTargetPitch = reader.ReadFloat();
			if ((flags & 4) != 0)
			{
				_controller.enabled = false;
				transform.position = reader.ReadVector3();
				_controller.enabled = true;
			}

			if ((flags & 8) != 0)
				_gunMagazine = reader.ReadByte();
			
			gameObject.SetActive((flags & 16) != 0);
		}
	}
}