using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy2 : Entity
{
    [SerializeField] private int startingHealth = 3;
    public ColliderEventConduit detectionEvent;
    private Animator _anim;
    private SpriteRenderer _sprite;
    private void Awake()
    {
        lives = startingHealth;
        _anim = GetComponent<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // detectionEvent = GetComponent<ColliderEventConduit>();
        detectionEvent.OnTriggerEnter2DEvent += PlayerEnter;
        detectionEvent.OnTriggerExit2DEvent += PlayerExit;
    }

    private void PlayerEnter(Collider2D other)
    {
        if (other.gameObject == Hero.Instance.gameObject)
        {
            State = States.attack;
            Hero.Instance.GetDamage();
        }
        if (lives < 1)
        {
            Die();
        }
    }
    private void PlayerExit(Collider2D other)
    {
        if (other.gameObject == Hero.Instance.gameObject)
        {
            State = States.idle;
        }

    }

    private void FixedUpdate()
    {
        var controller = GetComponent<EnemyController>();
        var velocity = controller.GetVelocity();
        //Debug.Log($"{velocity}, {State}");
        if (State == States.attack) { return; }
        if (velocity != Vector2.zero && State != States.attack)
        {
            _sprite.flipX = velocity.x > 0.0f;
            State = States.run;
        }
        else
        {
            State = States.idle;
        }
    }

    public enum States
    {
        idle,
        run,
        jump,
        attack
    }
    private States State
    {
        get => (States)_anim.GetInteger("state");
        set => _anim.SetInteger("state", (int)value);
    }
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject == Hero.Instance.gameObject)
    //     {
    //         Hero.Instance.GetDamage();
    //         lives--;
    //         Debug.Log("хп врага " +  lives);
    //     }
    //     if (lives < 1)
    //     {
    //         Die();
    //     }
    // }
    //
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject == Hero.Instance.gameObject)
    //     {
    //         Hero.Instance.GetDamage();
    //         lives--;
    //         Debug.Log("хп врага " +  lives);
    //     }
    //     if (lives < 1)
    //     {
    //         Die();
    //     }
    // }
}
