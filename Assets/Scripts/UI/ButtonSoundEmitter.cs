using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundEmitter : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler, ISubmitHandler
{

    public static event EventHandler OnAnyButtonSelected;
    public static event EventHandler OnAnyButtonClicked;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnAnyButtonSelected?.Invoke(this, EventArgs.Empty);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnAnyButtonSelected?.Invoke(this, EventArgs.Empty);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnAnyButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnAnyButtonClicked.Invoke(this, EventArgs.Empty);
    }
}
