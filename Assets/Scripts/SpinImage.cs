using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinImage : MonoBehaviour
{
    [SerializeField, Range(-180f, 180f)] private int speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, -speed * Time.deltaTime));
    }
}
