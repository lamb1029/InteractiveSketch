using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    public CurvePath path;
    public float radian;

    public void CreatePath()
    {
        path = new CurvePath(transform.position);
    }
}
