using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
