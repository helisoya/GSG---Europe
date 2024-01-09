using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Trigger for the tooltip
/// </summary>
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string text;


    /// <summary>
    /// Chagnes the trigger's tooltip text
    /// </summary>
    /// <param name="newText"></param>
    public void SetText(string newText)
    {
        text = newText;
    }

    /// <summary>
    /// On MouseEnter Event
    /// </summary>
    /// <param name="data">Event data</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!enabled) return;
        Tooltip.instance.ShowInfo(text);
    }

    /// <summary>
    /// On MouseExit Event
    /// </summary>
    /// <param name="data">Event data</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.instance.HideInfo();
    }
}
