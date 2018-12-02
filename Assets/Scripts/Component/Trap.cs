using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponentInParent<GameEntity>()?.Die();
    }
}
