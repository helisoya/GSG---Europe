using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A focus represents what can a country do to improve itself
/// </summary>
public class Focus
{
    public string id;
    public string focusName;
    public string desc;
    public List<string> required;
    public bool requireAll;
    public List<string> exclusive;
    public List<string> effect;
    public int x;
    public int y;
}
