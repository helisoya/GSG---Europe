using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An event shows flavor text to embelish and affect the gameplay
/// </summary>
public class GameEvent
{
    public string id;
    public string title;
    public string description;
    public EventButton[] buttons;


    public GameEvent()
    {
        buttons = new EventButton[4];
    }


    public class EventButton
    {
        public string label;
        public string[] effects;
    }
}
