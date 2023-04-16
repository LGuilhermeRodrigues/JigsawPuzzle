using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Configuration
{
    public static int ePuzzleColumns = 3;
    public static int ePuzzleRows = 3;
    public static AspectRatio ePuzzleAspectRatio = AspectRatio.Square;
    public static string ePuzzleImage = "cat";
    public static float ePuzzleImageTransparency = 0.4f;
    public static Color ePuzzleBackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f);
    public static Position ePuzzleImagePosition = Position.UpLeft;
    public static Position ePuzzlePiecesPosition = Position.DownRight;
    public static float ePuzzleSocketSize = 0.5f;
    
    public enum AspectRatio
    {
        Square,
        Landscape,
        Portrait,
        Widescreen
    }

    public enum Position
    {
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }
}
