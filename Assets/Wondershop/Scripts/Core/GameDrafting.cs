using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDraft
{
	public void Draft(List<GameObject> players, UnityAction<GameObject> callback);
	public void Cancel();
	public void OnPlayerDrafted(GameObject player);
}

public class GameDrafting : MonoBehaviour, IDraft
{
	public void Draft(List<GameObject> players, UnityAction<GameObject> callback) =>
		callback(players[Random.Range(0, players.Count)]);

	public void Cancel() => ProjectLogger.Log("Drafting canceled");

	public void OnPlayerDrafted(GameObject player) => ProjectLogger.Log($"{player} drafted");
}