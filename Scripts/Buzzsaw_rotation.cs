using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buzzsaw_rotation : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, 600f) * Time.deltaTime);    
    }
}
