using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{

    [SerializeField] private float rotationSpeed;

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
