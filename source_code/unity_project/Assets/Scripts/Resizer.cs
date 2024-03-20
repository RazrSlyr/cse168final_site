using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resizer : MonoBehaviour
{
    [SerializeField]
    private float startingSize;

    public void Resize(float newSize, Bounds currentBounds) {
        float sizeMultiplier = Mathf.Pow(Mathf.Pow(newSize, 3) / VectorUtils.getVolume(currentBounds.size), 1f/3);
        transform.localScale *= sizeMultiplier;
    }
}
