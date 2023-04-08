using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Configuration
{
    public static int ePuzzleHorizontalSize = 3;
    public static int ePuzzleVerticalSize = 3;
    public static AspectRatio ePuzzleAspectRatio = AspectRatio.Square;
    public static string ePuzzleImage = "beach";
    
    public enum AspectRatio
    {
        Square,
        Landscape,
        Portrait,
        Widescreen
    }
}
