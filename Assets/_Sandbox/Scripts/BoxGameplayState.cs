using System.Collections;
using Cinemachine;
using UnityEngine;

/***
 * You gamestate logic goes here. There are events coming from the ScriptableEvents and the
 * gamestate decides the "rules" on how those events effect other objects in the scene.
 *
 */
public class DemoGameplayState : GameState
{
	// playerBehavior is the collection of components that have their behaviour injected to 
	// new players upon joining. This allows a "plugin" system for avatar, but can be a bit complex.
	// If for some reason it causes more gray here than its worth, you can optionally just create a
	// complete player prefab

	/// <summary>
	/// Current Behavior will be used to switch between behaviors
	/// </summary>
	[SerializeField] private GameObject currentBehavior;

	[SerializeField] private GameObject normalBehavior;

	[SerializeField] private GameObject torchBehaviour;

	[SerializeField] private GameObject pickaxeBehaviour;

	[SerializeField] private GameObject bagBehaviour;

	[SerializeField] private bool itemHeld;

	private ILevels _levels;
	private CinemachineTargetGroup _cameraTargetGroup;

	/**
	 * Helpers
	 *
	 */
	// example of some game specific helpers
	private static void LockPlayer(GameObject avatar, bool value = true) {
		foreach (AvatarBehaviour behaviour in avatar.GetComponents<AvatarBehaviour>()) behaviour.SetActive(!value);
	}

	private static void UnlockPlayer(GameObject avatar, bool value = true) {
		foreach (AvatarBehaviour behaviour in avatar.GetComponents<AvatarBehaviour>()) behaviour.SetActive(value);
	}


	/**
	 * Custom life-cycle
	 *
	 */
	// this is called by our custom GameManager upon loading the scene
	protected override void OnEnter() {
		// load the level
		_levels ??= GetComponent<LevelsSingles>();
		if (_levels != null) _levels.CreateLevel(OnLevelCreated);
		else OnLevelCreated();
	}

	// called after the onEnter is finished
	private void OnLevelCreated() {
		// get the target group camera
		_cameraTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
		// trigger the transitions
		transition?.Open();
	}

	// this should be called when you want the game to end and load next level
	protected override void OnExit() {
		// remove the current level and notify that state in complete
		if (!_levels.Equals(null)) _levels.RemoveLevel(OnStateComplete);
		else OnStateComplete();
	}

	protected override void OnPlayerEnter(GameObject player) {
		
		player.GetComponent<Avatar>().Inject(normalBehavior);
		Debug.Log("Player Name: " + player.name);

		
		_cameraTargetGroup.AddMember(player.transform.GetChild(0), 1f, 1f);
	}


	protected override void OnPlayerExit(GameObject player) {
		player.GetComponent<Avatar>().Deject(normalBehavior);
		_cameraTargetGroup.RemoveMember(player.transform.GetChild(0));
	}


	/**
	 * Event listeners
	 *
	 */
	[EventListener]
	public void OnPlayerJoined(GameObject player) {
		ProjectLogger.Log($"[{GetType().Name}] OnPlayerJoined");
		OnPlayerEnter(player);
	}

	[EventListener]
	public void OnPlayerLeft(GameObject player) {
		ProjectLogger.Log($"[{GetType().Name}] OnPlayerLeft");
	}

	/**
	 * Collisions
	 *
	 */
	[EventListener]
	public void OnPlayerCollision(ColliderEventArgs colliderEvent) {
		// NOTE: this is called based on the AvatarCollision components layer setup, if layer is everything it gets spammed all the time
		// make sure to set it to the layer that you are actually interested in for example items etc.
		//ProjectLogger.Log($"[{colliderEvent.That}] OnPlayerCollision");
		
		
		GameObject collision = colliderEvent.Other;
		GameObject player = colliderEvent.That;
		
			if(collision.layer == LayerMask.NameToLayer("Item"))
			{
				
				colliderEvent.That.GetComponentInParent<Avatar>().Deject(currentBehavior);
				
				
				if(collision.CompareTag("pickaxe"))
				{
					Debug.Log("Found Pickaxe");
					HoldItem(player,pickaxeBehaviour,collision);
				}
				else if(collision.CompareTag("torch"))
				{
					Debug.Log("Found Torch");
					HoldItem(player,torchBehaviour,collision);
					
				}
				else if(collision.CompareTag("bag"))
				{
					Debug.Log("Found Bag");
					HoldItem(player,bagBehaviour,collision);
				}

				player.GetComponentInParent<Avatar>().Inject(currentBehavior);

				if(collision.GetComponent<StartingCollectable>() == null)
				{
					Destroy(collision);
				}
			}

			if(collision.CompareTag("dropoff"))
			{
				player.BroadcastMessage("DropAtBase");
			}
		
		
		
		if(collision.layer == LayerMask.NameToLayer("ShadowMonster"))
		{
			//player.BroadcastMessage("DropItem");
			
			//player.GetComponentInParent<Avatar>().Deject(currentBehavior);
			Debug.Log("Added Rb");
			//if(player.GetComponent(Rigidbody!))
			Vector3 moveDirection = player.transform.position - collision.transform.position;
			Rigidbody rb = player.AddComponent<Rigidbody>();
        	rb.AddForce( moveDirection.normalized * -1f, ForceMode.Impulse);
			Destroy(player.GetComponent<Rigidbody>());
			StartCoroutine(FreezePlayer(player));

		}
	}

	/**
	 * Triggers
	 *
	 */
	[EventListener]
	public void OnPlayerTriggerEnter(ColliderEventArgs colliderEvent) {
		ProjectLogger.Log($"[{colliderEvent.That}] OnPlayerTriggerEnter");
	}

	[EventListener]
	public void OnPlayerTriggerExit(ColliderEventArgs colliderEvent) {
		ProjectLogger.Log($"[{colliderEvent.That}] OnPlayerTriggerExit");
	}

	/**
	 * Interactions
	 *
	 */
	[EventListener]
	public void OnPlayerInteractionEnter(ColliderEventArgs interaction) {
		interaction.Other.GetComponentInChildren<IInteracting>().EnterInteraction(interaction.That);
		interaction.That.GetComponent<IHighlight>()?.Focus();
	}

	[EventListener]
	public void OnPlayerInteractionStart(ColliderEventArgs interaction) {
		ProjectLogger.Log($"[{interaction.That}] OnPlayerInteractionStart");
		GameObject avatar = interaction.That;
	}

	[EventListener]
	public void OnPlayerInteractionEnd(ColliderEventArgs interaction) {
		ProjectLogger.Log($"[{interaction.That}] OnPlayerInteractionEnd");
		GameObject avatar = interaction.That;
	}

	[EventListener]
	public void OnPlayerInteractionExit(ColliderEventArgs interaction) {
		interaction.Other.GetComponentInChildren<IInteracting>().ExitInteraction(interaction.That);
		interaction.That.GetComponent<IHighlight>()?.Unfocus();
	}

	/**
	 * Actions
	 *
	 */
	[EventListener]
	public void OnPlayerActionPress(GameObject player) {
		// ProjectLogger.Log($"[{GetType().Name}] OnPlayerActionPush");
		player.transform.parent.BroadcastMessage("ActionButton");

	}

	[EventListener]
	public void OnPlayerActionHold(GameObject player) {
		// Currently the hold is very short time, 0.4seconds so this is almost the same 
		// as press, you can either tweak the input settings or figure out other ways to detect hold
		//ProjectLogger.Log($"[{GetType().Name}] OnPlayerActionHold");
		player.transform.parent.BroadcastMessage("HoldActionButton", true);
		//player.transform.parent.BroadcastMessage("pressedTrue");
	}

	 private IEnumerator FreezePlayer(GameObject avatar)
    {
		LockPlayer(avatar,true);
		itemHeld = false;
        yield return new WaitForSeconds(3f);
		LockPlayer(avatar,false);
    }

	private void HoldItem(GameObject player, GameObject newBehaviour, GameObject collectible)
	{
		itemHeld = false;
		if(collectible.GetComponent<StartingCollectable>() != null)
		{
			currentBehavior = newBehaviour;
			itemHeld = true;
		}
		else{
			player.BroadcastMessage("DropItem");
			currentBehavior = newBehaviour;
			itemHeld = true;
		}
		
	}

}