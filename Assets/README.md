# Wondershop Sandbox

This is a streamlined project template based on our actual game.
It does not contain some of the paid licensed components we use that would require licenses. Project also does not contains any backend.

---
**TLDR; Game starts from the main scene, player character can be spawned with Enter, despawned with Backspace and the controlled player changed with Tab.
One player uses WASD+Space for action, the other Arrows+Shift that is closest to the arrow keys**
---
## Basics

Use the `_Sandbox` folder for your code. You can find initial Scene and Config setup there:

1. The game is started from the `MainScene` at the root of the project
2. The `MainScene` will load from the main config file the `BOX` **Game Setup** config
3. `BOX` will load the stages of the game, namely the `BOX_game` **Game Stage** config
4. `BOX_game` will load the states of the game, namely the `BOX_gameplay` **Game State** config
5. The `BOX_gameplay` is usually responsible for loading the actual game level via **ILevel**

Why such a complex setup? As our game consists of rotating a lot of different scenes, we
quickly realized that we needed a "simple" way to configure the flow of the game. The idea is
that a single game can have multiple stages: ex. pre-game, gameplay, post-game and these stages
can have multiple states for example pre-game title, pre-game splash, pre-game tutorial etc. 
This is not that relevant in the prototyping context, but we decided to keep it as it provides
some nice hooks into the actual gameplay / level loading.

You can ignore the `_Sandbox/Config` files apart from the `BOX_game` in which you can set
how many rounds your game takes. For the `_Sandbox/Scenes` you need to only focus on the
`BOX_gameplay` scene which is the actual state of the game and the `BOX_level_1` level scene which contains
the environment etc. visuals of the game.

## Player / Avatar

As we have multiple games, before coming up with current plugin system, our avatar ended up being a massive monolith
with over 50 different components affecting the gameplay behaviour. Every game was responsible activating/deactivating 
the components that were actually used and as you can imagine, this caused a lot of game breaking bugs.
This also meant that changing any of the component parameters had often a unintended impact on the other games
ex. different running speeds that would negatively affect the gameplay.

We solved this by breaking up the behaviour into smaller game specific behaviors that we would inject to the player when needed.
We created a bit of boilerplate around this to give us better control and extension possibilities, but it also means that
it is bit more complex that we would prefer. However in production, the benefits outweigh the challenges.

You can decide if you want to create your behavior with the plugin system or create a single prefab for the player:

**If you decide to follow the plugin version** you need to create a empty prefab with the components required by the gameplay
and add that as the **Player Behavior** in the `BOX_gameplay` scene to `GameState`. When you start the game from the start scene, the behavior will be inject to the player character.
There are already two example behaviors: PlayerJumping and PlayerWalking.

**If you decide to create a single player prefab** you need to change the **Default Player Prefab** in the **PlayerManager** from the `MainScene`.
This new prefab will act as you would expect from any other standard Unity project. Remember to remove the injecting code from `BoxGameplayState` script.


## Event system

As we have separated most of the gameplay logic from the level, we wanted to have a clear way for components to interact with each other.
The natural way for Unity GameObjects to communicate with each other is by interacting directly to a specific component that we expect object to have.
This is a fast way to work but when the complexity increases it becomes harder and harder to manage.

We decided to use an event based approach where all the the data flow would be only one-way: from `GameObject` to `GameState` to other `GameObjects`. There are
different types of events defined in the `Resources/Events` folder. These are triggered by the code. Other GameObjects also need to be listening these events:
for this we created a handy **[EventListener]** attribute which automatically hooks up the events when the GameObject include the **EventConnect** component.

You can optionally ignore this setup if you for example are already creating your own player prefab or find it getting in the way.
It is however quite handy to retain the game logic in a single state with one-way events.

## Assets

Prototyping assets can be found from `SyntyStudios/` folder. Some ready made scripts and components are available at `Wondershop/` folder.