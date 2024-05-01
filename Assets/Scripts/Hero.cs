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


    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;

    public static Hero Instance { get; set; }

    private void Awake()
    {
        lives = 3;
        health = lives;
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
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
    
    // немного переименовал переменные
    //     подправил функции

    private void CheckGround()
    {
        var pos = transform.position;
        var results = new Collider2D[2];
        var size = Physics2D.OverlapCircleNonAlloc(pos, 0.3f, results);
        isGrounded = size > 1;
    }
}