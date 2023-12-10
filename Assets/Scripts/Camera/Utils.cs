using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles utilities for the Selection class
/// </summary>
public class Utils : MonoBehaviour
{
    static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    /// <summary>
    /// Draw a rectangle on screen
    /// </summary>
    /// <param name="rect">Rectangle</param>
    /// <param name="color">Rectangle color</param>
    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }


    /// <summary>
    /// Draw a rectangle border onscreen
    /// </summary>
    /// <param name="rect">The rectangle</param>
    /// <param name="thickness">Line thickness</param>
    /// <param name="color">Rectangle color</param>
    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    /// <summary>
    /// Returns the coordinates of the rectangle
    /// </summary>
    /// <param name="screenPosition1">Start position</param>
    /// <param name="screenPosition2">End position</param>
    /// <returns>Rectangle coordinates</returns>
    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
