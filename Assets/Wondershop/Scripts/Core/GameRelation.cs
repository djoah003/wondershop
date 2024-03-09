using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRelation : MonoBehaviour
{
	[SerializeField] [Scene] private string parent;

	private void Awake() {
		string sceneName = System.IO.Path.GetFileNameWithoutExtension(parent);
		// after the scene is loaded, it needs to be check without the path(?)
		if (!SceneManager.GetSceneByName(sceneName).IsValid()) StartCoroutine(LoadParent());
	}

	private IEnumerator LoadParent() {
		AsyncOperation loadScene = SceneManager.LoadSceneAsync(parent, LoadSceneMode.Additive);
		while (!loadScene.isDone) yield return null;
		// disable possible level loading from the game state scene
		FindObjectOfType<GameState>().GetComponent<ILevels>().Disable();
	}
}