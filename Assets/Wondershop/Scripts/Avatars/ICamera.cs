using UnityEngine;

public interface ICamera
{
	public bool TryGetCamera(out Camera avatarCamera);
}