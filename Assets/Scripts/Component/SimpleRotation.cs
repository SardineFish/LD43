using UnityEngine;
using System.Collections;

public class SimpleRotation : MonoBehaviour
{
    public float AngularSpeed = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, AngularSpeed * Time.deltaTime));
    }
}
