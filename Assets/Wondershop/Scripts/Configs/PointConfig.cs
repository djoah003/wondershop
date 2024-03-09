using System;
using UnityEngine;

[Serializable]
public class PointConfig : ScriptableObject
{
	[SerializeField, Tooltip("Distinct code for the points")]
	public int id;

	[SerializeField, Tooltip("Short name for the points")]
	public string title;

	[SerializeField, Tooltip("Description about the points")]
	public string desc;

	[SerializeField, Tooltip("Sprite icon for the points")]
	public Sprite sprite;
}