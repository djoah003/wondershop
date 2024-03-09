using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightEmission : MonoBehaviour, IHighlight
{
	[SerializeField] private Color color = new Color(10, 10, 10, 1f);
	[SerializeField] private GameObject target;

	private Renderer _renderer;
	private List<Color> _initialColors = new List<Color>();
	private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

	public void Start() {
		_renderer = target.GetComponent<Renderer>();
		foreach (Material material in _renderer.materials) material.EnableKeyword("_EMISSION");
		_initialColors = _renderer.materials.ToList().Select(material => material.GetColor(EmissionColor)).ToList();
	}

	public void Focus() {
		if (_renderer == null) return;
		foreach (Material material in _renderer.materials) material.SetColor(EmissionColor, color);
	}

	public void Unfocus() {
		if (_renderer == null) return;
		int materialIndex = 0;
		foreach (Material material in _renderer.materials) material.SetColor(EmissionColor, _initialColors[materialIndex++]);
	}
}