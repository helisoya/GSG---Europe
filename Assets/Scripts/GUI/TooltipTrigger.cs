using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour
{
    [SerializeField] private string text;

    public void OnMouseEnter(BaseEventData data)
    {
        Tooltip.instance.ShowInfo(text);
    }

    public void OnMouseExit(BaseEventData data)
    {
        Tooltip.instance.HideInfo();
    }
}
