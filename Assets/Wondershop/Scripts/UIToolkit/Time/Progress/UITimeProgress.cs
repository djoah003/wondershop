using UnityEngine;
using UnityEngine.UIElements;

public class UITimeProgress : MonoBehaviour, ITimed
{
    [SerializeField] private GameObject prefab;

    private GameObject _uiObject;
    private UIDocument _uiDocument;
    private UITimeProgressController _uiController;

    /**
     * Unity life-cycle
     *
     */
    private void Awake()
    {
        _uiObject = Instantiate(prefab);
        _uiDocument = _uiObject.GetComponent<UIDocument>();
        _uiController = new UITimeProgressController();
        if (_uiDocument == null) Debug.LogError($"{prefab.name} does not contain UI document");
        else _uiController.SetVisualElement(_uiDocument.rootVisualElement);
    }

    private void Start() => SetTime(1f);

    /**
     * Progress interface
     *
     */
    public void SetTime(float value) => _uiController?.SetTime(value);
}