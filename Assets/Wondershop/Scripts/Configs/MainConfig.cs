using System;
using UnityEngine;

[Serializable]
public class MainConfig : ScriptableObject
{
	[Scene] public string scenePath;
	public GameSetupConfig setup;
	public GameSetupConfig[] games;
}