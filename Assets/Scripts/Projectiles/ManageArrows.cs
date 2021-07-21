using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageArrows : MonoBehaviour
{
    private static int maxArrows = 10;
    public static List<Arrow> arrows = new List<Arrow>();

    public static void AddArrow(Arrow newArrow)
    {
        if(arrows.Count >= maxArrows)
        {
            arrows.RemoveAt(0);
            Destroy(arrows[0].gameObject);
        }
        arrows.Add(newArrow);
    }

    public static void RemoveArrow(Arrow newArrow)
    {
        arrows.Remove(newArrow);
    }
}
