using System.Collections;
using System.Collections.Generic;
using OVR.OpenVR;
using Unity.VisualScripting;
using UnityEngine;

public class VectorUtils
{
    public static float getVolume(Vector3 v) {
        return v.x * v.y * v.z;
    }
}
