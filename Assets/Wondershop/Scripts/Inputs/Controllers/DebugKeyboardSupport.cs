using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugKeyboardSupport : MonoBehaviour
{
	private void Awake() => enabled = Application.isEditor;

	private void Update() {
		// null check the Keyboard
		if (Keyboard.current == null) return;
		// actually check the input
		if (!Keyboard.current.nKey.wasPressedThisFrame) return;
		// move into the next scene (or lobby)
		MainManager.Instance.DebugNextScene();
	}
}