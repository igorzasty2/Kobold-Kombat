using System.Data;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }
    [SerializeField] Transform spellPoint;
    public float speed;
    Rigidbody2D playerbody;
    private bool isFacingRight = true;
    public Animator animator;
    public float attackRate = 2f;
    float nextAttackTime = 0f;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        playerbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        animator.SetFloat("Speed", movement.magnitude);
        if (movement.magnitude != 0)
        {
            if (!(horizontal == 0 || isFacingRight == (horizontal > 0)))
            {
                Flip();
            }
            spellPoint.transform.position = (Vector2)transform.position + movement* new Vector2(0.8f,0.8f) + new Vector2(0,-0.4f);
        }
        playerbody.velocity = movement * speed;
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("AttackFinished");
            if (Input.GetButtonDown("AttackBasic"))
            {
                AttackBasic();
                nextAttackTime = Time.time + 1f / attackRate;
            }
            if (Input.GetButtonDown("AttackSpecial1"))
            {
                AttackSpecial1();
                nextAttackTime = Time.time + 1f / attackRate;
            }
            if (Input.GetButtonDown("AttackSpecial2"))
            {
                AttackSpecial2();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    void AttackBasic()
    {
        animator.SetTrigger("Attack");
        // atak powietrzny
        print("atak powietrzny");
    }
    void AttackSpecial1()
    {
        animator.SetTrigger("Attack");
        // atak lodowy
        print("atak lodowy");
    }
    void AttackSpecial2()
    {
        animator.SetTrigger("Attack");
        // atak ognisty
        print("atak ognisty");
    }
    public Transform GetPlayerTransform()
    {
        return transform;
    }
}
