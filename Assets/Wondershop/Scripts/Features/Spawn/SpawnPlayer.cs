using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class SpawnPlayer : MonoBehaviour
{
	[SerializeField] private bool autoSpawnPlayers = true;
	private Collider _spawnArea;
	private Transform[] _spawnPoint;

	/**
	 * Unity life-cycle
	 *
	 */
	private void Awake() {
		if (transform.childCount == 0) SetupSpawnArea();
		else SetupSpawnPoints();
	}

	private void OnEnable() {
		foreach (GameObject player in PlayerManager.Players().ToArray()) AutoSpawn(player);
	}

	/**
	 * Feature implementation
	 *
	 */
	private void SetupSpawnArea() {
		_spawnArea = GetComponent<Collider>();
		if (_spawnArea == null) throw new Exception("No collider area provided for spawn point");
	}

	private void SetupSpawnPoints() {
		_spawnPoint = new Transform[transform.childCount];
		int counter = 0;
		foreach (Transform child in transform) {
			_spawnPoint[counter] = child.transform;
			counter += 1;
		}
	}

	private void AutoSpawn(GameObject player) {
		if (!autoSpawnPlayers) return;
		if (_spawnArea == null) SpawnInPoint(player);
		else SpawnInArea(player);
	}

	private void SpawnInArea(GameObject player) {
		// set initial position to zero
		Vector3 position = RandomPointOnArea(_spawnArea);
		// find free position on the ground
		bool hitSuccess = Physics.Raycast(position, Vector3.down, out RaycastHit hit, 100f);
		// get character layer mask
		IAvatar avatar = player.GetComponent<IAvatar>();
		// if no avatar, can not position player
		if (avatar == null) return;
		avatar.Transform().position = hitSuccess ? hit.point : _spawnArea.transform.position;
		avatar.Transform().rotation = transform.rotation;
	}

	private static Vector3 RandomPointOnArea(Collider area) {
		Bounds bounds = area.bounds;
		float px = Random.Range(bounds.min.x, bounds.max.x);
		float py = bounds.min.y;
		float pz = Random.Range(bounds.min.z, bounds.max.z);
		return new Vector3(px, py + 10f, pz);
	}

	private void SpawnInPoint(GameObject player, int pointIndex = -1) {
		int index = pointIndex == -1 ? player.GetComponent<PlayerInput>().playerIndex : pointIndex;
		if (index < _spawnPoint.Length) {
			player.transform.position = _spawnPoint[index].position;
			player.transform.rotation = _spawnPoint[index].rotation;
		} else {
			player.transform.position = Vector3.zero;
		}
	}

	/**
	 * Event listeners
	 *
	 */
	[EventListener]
	public void OnPlayerJoined(GameObject player) => AutoSpawn(player);

	/**
	 * Editor
	 *
	 */
	private void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		if (GetComponent<Collider>() != null) {
			Collider spawnArea = GetComponent<Collider>();
			Gizmos.DrawWireCube(spawnArea.transform.position, spawnArea.bounds.size);
		} else {
			foreach (Transform child in transform) Gizmos.DrawWireSphere(child.position, radius: 1f);
		}
	}
}