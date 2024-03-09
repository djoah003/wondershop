using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;


public class PlayerManagerBots : ProjectSingleton<PlayerManagerBots>
{
    [SerializeField] private ScriptableEventForGameObject onBotJoined;
    [SerializeField] private ScriptableEventForGameObject onBotLeft;

    private const string InterfaceName = "FAKE";
    private const string ProductName = "Virtual";

    /**
     * Unity life-cycle
     * 
     */
    protected override void Awake()
    {
        base.Awake();
        // register the fake device layout for bot players
        InputDeviceMatcher deviceFilter =
            new InputDeviceMatcher().WithInterface(InterfaceName).WithProduct(ProductName);
        InputSystem.RegisterLayout<VirtualInputDevice>(matches: deviceFilter);
    }

    public void OnEnable()
    {
        // GameBotSettings settings = FindObjectOfType<GameBotSettings>();
        // if (settings != null) Enable(settings.playerCount, settings.botsCount);
    }

    private void Enable(int minPlayerCount = 2, int minBotsCount = 4)
    {
        if (minPlayerCount <= PlayerManager.Players().Count) return;
        int currentBotsCount = Bots().Count;
        for (int i = 0; i < minBotsCount - currentBotsCount; i++) CreateBotPlayer();
    }

    public void OnDisable()
    {
        foreach (GameObject bot in Bots().Where(bot => bot != null)) Destroy(bot.gameObject);
        List<VirtualInputDevice> devices = VirtualInputDevice.all.ToList();
        foreach (VirtualInputDevice d in devices) InputSystem.RemoveDevice(d);
    }


    /**
     * Player(s) management
     * 
     */
    private PlayerInput CreateFakePlayer()
    {
        // create "fake" virtual device
        InputDevice inputDevice = InputSystem.AddDevice(new InputDeviceDescription
            { interfaceName = InterfaceName, product = ProductName });
        PlayerInput playerInput = gameObject.GetComponent<PlayerInputManager>().JoinPlayer(pairWithDevice: inputDevice);
        playerInput.SwitchCurrentActionMap("Player");
        playerInput.neverAutoSwitchControlSchemes = true;
        // fake players have their input disables
        return playerInput;
    }

    private void CreateBotPlayer()
    {
        // create bot player via input system
        PlayerInput playerInput = CreateFakePlayer();
        // add the bot logic to the new player
        // playerInput.gameObject.AddComponent<GameBotPlayer>();
        // notify about new bot joining the game
        onBotJoined.TriggerEvent(playerInput.gameObject);
    }

    private void RemoveBotPlayer(GameObject bot)
    {
        onBotLeft.TriggerEvent(bot);
        Destroy(bot);
    }


    /**
     * Event listeners
     * 
     */
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        // if no bots or no player input manager return early
        if (Bots().Count == 0 || PlayerInputManager.instance == null) return;
        // if still space for players, do not remove any of the bots 
        if (PlayerManager.Players().Count <= PlayerManager.MaxPlayerCount) return;
        // remove bot if more human players joining
        RemoveBotPlayer(Bots().First());
    }


    /**
     * Bots interface
     * 
     */
    public static List<GameObject> Bots() => PlayerInput.all.Where(PlayerManager.IsBot)
        .Select(input => input.gameObject).ToList();
}