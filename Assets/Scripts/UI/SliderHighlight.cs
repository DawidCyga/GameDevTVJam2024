using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("References")]
    [SerializeField] private Image _fillImage;

    private Color _defaultColour;
    private Color _highlightColour;

    private void Awake() => SetColourValues();

    private void SetColourValues()
    {
        _defaultColour = new Color(247 / 255f, 196 / 255f, 130 / 255f);
        _highlightColour = new Color(156 / 255f, 162 / 255f, 90 / 255f);
    }

    public void OnPointerEnter(PointerEventData eventData) => SetColour(_highlightColour);
    public void OnPointerExit(PointerEventData eventData) => SetColour(_defaultColour);
    public void OnSelect(BaseEventData eventData) => SetColour(_highlightColour);
    public void OnDeselect(BaseEventData eventData) => SetColour(_defaultColour);
    private void SetColour(Color newColour) => _fillImage.color = newColour;
}
