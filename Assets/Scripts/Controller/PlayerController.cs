﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public int FaceDirection = 1;
    public bool OnGround;
    public int JumpCount = 1;
    public int MaxJumpCount = 1;
    public float Speed = 10;
    public float RunSpeed = 20;
    public Vector2 velocity = Vector2.zero;
    public ControlSequence ControlSequence;
    public List<ControlDetail> ControlRecord = new List<ControlDetail>();
    MultiPlayer.PlayerSnapShot lastSnapshot;
    public MultiPlayer.PlayerSnapShot LastSnapshot
    {
        get
        {
            var snapshot = lastSnapshot;
            lastSnapshot = null;
            return snapshot;
        }
        private set
        {
            lastSnapshot = value;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var control = ControlSequence?.GetControl();
        if (control == null)
        {
            Move(0);
            return;
        }

        switch (control.Action)
        {
            case PlayerAction.Move:
                Move(control.Direction);
                break;
            case PlayerAction.Jump:
                if (!Jump(control.Direction))
                    control.Action = PlayerAction.Move;
                break;
            case PlayerAction.Run:
                Run(control.Direction);
                break;
            case PlayerAction.Idle:
                Move(0);
                break;
        }

        // Record
        if (ControlRecord.Count <= 0)
            ControlRecord.Add(control);
        else if (!control.IsSame(ControlRecord[ControlRecord.Count - 1]))
        {
            ControlRecord.Add(control);
            LastSnapshot = new MultiPlayer.PlayerSnapShot()
            {
                Tick = control.Tick,
                Position = MultiPlayer.Utilities.Vector2List(transform.position),
                Velocity = MultiPlayer.Utilities.Vector2List(GetComponent<Rigidbody2D>().velocity),
                Control = new MultiPlayer.PlayerControl()
                {
                    Action = (int)control.Action,
                    Direction = control.Direction,
                }
            };
        };

    }

    private void LateUpdate()
    {
        //velocity.y += -PhysicsSystem.Instance.Gravity * Time.deltaTime;
        var v = GetComponent<Rigidbody2D>().velocity;
        v.x = velocity.x;
        if (velocity.y != 0)
        {
            v.y = velocity.y;
        }
        GetComponent<Rigidbody2D>().velocity = v;
        velocity = Vector2.zero;
    }
    private void FixedUpdate()
    {
        OnGround = false;
    }


    public void Move(float direction)
    {
        velocity.x = direction * Speed;
        ApplyDirection(direction);
    }

    public bool Jump(float direction)
    {
        if (OnGround)
        {
            velocity.y = PhysicsSystem.Instance.JumpVelocoty;
            Move(direction);
            return true;
        }
        else
        {
            Move(direction);
            return false;
        }
    }

    public void Run(float direction)
    {
        velocity.x = direction * RunSpeed;
        ApplyDirection(direction);
    }

    public void ApplyDirection(float direction)
    {
        var dir = MathUtility.SignInt(direction);
        if(dir!=0)
        {
            FaceDirection = dir;
        }
        if (FaceDirection > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            var contract = collision.GetContact(i);
            var localPoint = transform.worldToLocalMatrix.MultiplyPoint(contract.point);
            if (Mathf.Abs(localPoint.y) <= PhysicsSystem.Instance.OnGroundThreshold && contract.relativeVelocity.y >= 0)
            {
                JumpCount = MaxJumpCount;
                OnGround = true;
                velocity.y = 0;
            }
        }
    }
}