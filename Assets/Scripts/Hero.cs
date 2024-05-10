using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class Hero : Entity
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private int health;
    [SerializeField] private float jumpForce = 10f;
    private bool isGrounded = false;

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite aliveHeart;
    [SerializeField] private Sprite deadHeart;

    public bool isAttacking = false;
    public bool isRecharged = true;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;

    private RigidBody2D _rb;
    private Animator _anim;
    private SpriteRenderer _sprite;

    public static Hero Instance { get; set; }

    private void Awake()
    {
        lives = 3;
        health = lives;
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        isRecharged = true;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    public override void GetDamage()
    {
        lives -= 1;
        Debug.Log(lives);
        if (lives < 1)
        {
            Die();
        }
    }

    private void Attack()
    {
    }

    private void Update()
    {
        if (isGrounded) State = States.idle;

        if (Input.GetButton("Horizontal"))
            Run();
        if (isGrounded && Input.GetButtonDown("Jump"))
            Jump();

        if (health > lives) health = lives;

        for (var i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = aliveHeart;
            else
                hearts[i].sprite = deadHeart;

            if (i < lives)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = true;
        }
    }


    public override void Die()
    {
        Destroy(gameObject);
    }


    // TODO: Попробовать использовать (_rb) RigidBody2D.velocity для передвижения
    // https://docs.unity3d.com/ScriptReference/Rigidbody2D-velocity.html

    private void Run()
    {
        if (isGrounded) State = States.run;

        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        var position = transform.position;
        position = Vector3.MoveTowards(position, position + dir, speed * Time.deltaTime);
        transform.position = position;
        _sprite.flipX = !(dir.x > 0.0f);
        //sprite.sprite = "hero-frame-2";
    }

    private void Jump()
    {
        _rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
    

    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;

        if (!isGrounded) State = States.jump;
    }

    public enum States
    {
        idle,
        run,
        jump
    }
    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
        
    }
}