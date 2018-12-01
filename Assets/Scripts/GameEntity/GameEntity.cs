﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameEntity : MonoBehaviour
{
    public const string NameRenderer = "Renderer";
    public const string NameCollider = "Collider";
    public const string NameAttached = "Attached";
    public GameObject Renderer => transform.Find(NameRenderer).gameObject;
    public GameObject Collider => transform.Find(NameCollider).gameObject;
    

    public event Action OnUpdate;

    // Use this for initialization
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        OnUpdate?.Invoke();
    }

    public static GameEntity GetEntity(Component obj)
    {
        return obj.GetComponent<GameEntity>() ?? obj.GetComponentInParent<GameEntity>();
    }

    public void Attach(GameObject obj, Vector3 offset)
    {
        obj.transform.parent = transform;
        obj.transform.localPosition = offset;
    }

    public void Attach(GameObject obj)
    {
        Attach(obj, obj.transform.position - transform.position);
    }
}
