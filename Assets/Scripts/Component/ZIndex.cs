using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ZIndex : MonoBehaviour
{
    public float zIndex = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position.Set(z: zIndex);
    }
}
