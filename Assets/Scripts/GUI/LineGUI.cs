using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A line used in the focus tree
/// </summary>
public class LineGUI : MonoBehaviour
{
    private RectTransform object1;
    private RectTransform object2;
    private Image image;
    private RectTransform rectTransform;

    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Sets the inforamtions of the line
    /// </summary>
    /// <param name="one">Start point</param>
    /// <param name="two">End point</param>
    /// <param name="col">Line color</param>
    public void SetObjects(GameObject one, GameObject two, Color col)
    {
        object1 = one.GetComponent<RectTransform>();
        object2 = two.GetComponent<RectTransform>();
        image.color = col;

        RectTransform aux;
        if (object1.localPosition.x > object2.localPosition.x)
        {
            aux = object1;
            object1 = object2;
            object2 = aux;
        }
        Init();
    }

    /// <summary>
    /// Initiliaze the line
    /// </summary>
    void Init()
    {
        if (object1.gameObject.activeSelf && object2.gameObject.activeSelf)
        {
            rectTransform.localPosition = (object1.localPosition + object2.localPosition) / 2;
            Vector3 dif = object2.localPosition - object1.localPosition;
            rectTransform.sizeDelta = new Vector3(dif.magnitude, 5);
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
    }
}
