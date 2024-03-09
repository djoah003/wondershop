using System.Collections;
using UnityEngine;

public class AudioManager : ProjectSingleton<AudioManager>
{
	private static AudioSource _audioSource;
	/**
	 * Helpers
	 *
	 */
	private static IEnumerator DelayedCall(System.Action action, float delay) {
		yield return new WaitForSeconds(delay);
		action?.Invoke();
	}

	/**
	 * Unity life-cycle
	 *
	 */
	protected void Start() => _audioSource = GetComponent<AudioSource>();

	/**
	 * Features
	 *
	 */
	public static void PlayMusic(AudioClip music) {
		if (_audioSource == null || music == null) return;
		_audioSource.clip = music;
		_audioSource.loop = true;
		_audioSource.Play();
	}

	public static void StopMusic() => _audioSource.Stop();
}