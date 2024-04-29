using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEventConduit : MonoBehaviour
{
    public delegate void ColliderEvent(Collider2D other);
    public delegate void CollisionEvent(Collision2D other);
    public event CollisionEvent OnCollisionEnter2DEvent;
    public event CollisionEvent OnCollisionExit2DEvent;
    public event CollisionEvent OnCollisionStay2DEvent;
    public event ColliderEvent OnTriggerEnter2DEvent;
    public event ColliderEvent OnTriggerExit2DEvent;
    public event ColliderEvent OnTriggerStay2DEvent;

    private void OnCollisionEnter2D(Collision2D other) => OnCollisionEnter2DEvent?.Invoke(other);
    private void OnCollisionExit2D(Collision2D other) => OnCollisionExit2DEvent?.Invoke(other);
    private void OnCollisionStay2D(Collision2D other) => OnCollisionStay2DEvent?.Invoke(other);
    private void OnTriggerEnter2D(Collider2D other) => OnTriggerEnter2DEvent?.Invoke(other);
    private void OnTriggerExit2D(Collider2D other) => OnTriggerExit2DEvent?.Invoke(other);
    private void OnTriggerStay2D(Collider2D other) => OnTriggerStay2DEvent?.Invoke(other);
}