using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Trigger for the tooltip
/// </summary>
public class TooltipTrigger : MonoBehaviour
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
    public void OnMouseEnter(BaseEventData data)
    {
        if (!enabled) return;
        Tooltip.instance.ShowInfo(text);
    }

    /// <summary>
    /// On MouseExit Event
    /// </summary>
    /// <param name="data">Event data</param>
    public void OnMouseExit(BaseEventData data)
    {
        Tooltip.instance.HideInfo();
    }
}
