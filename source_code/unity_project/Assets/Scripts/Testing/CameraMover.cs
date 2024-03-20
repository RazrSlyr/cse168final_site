using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField]
    private float speed;

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Forward");
        Vector3 moveAmount = new Vector3(hInput, 0, vInput) * speed * Time.deltaTime;
        transform.Translate(moveAmount);
    }
}
