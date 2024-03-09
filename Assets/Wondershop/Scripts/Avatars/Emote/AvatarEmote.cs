using UnityEngine;

public class AvatarEmote : AvatarBehaviour, IAvatarPlugin
{
	[SerializeField] private AnimationClip defaultEmote;


	/**
	* IAvatarPlugin
	*
	*/
	public void Register() {
	}
	public void Unregister() {
	}

	/**
	 * Feature implementation
	 *
	 */
	public AnimationClip Emote() => defaultEmote;
}