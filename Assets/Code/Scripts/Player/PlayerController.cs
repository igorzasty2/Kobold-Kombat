using System.Data;
using Unity.VisualScripting;
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
    bool windBallPlaced = false;

    public GameObject windBallPrefab;
    public GameObject windSpellPrefab;
    public GameObject iceProjectilePrefab;
    private GameObject currentWindBall;
    public GameObject slashPrefab;
    public GameObject bigSlashPrefab;

    private float runTime;
    private float finalSpeed;
    private bool isRunning = false;
    private bool canMove = true;
    private int comboStep = 0;
    private float comboTimer = 0f;

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
        if (canMove)
        {
            Move();
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("AttackBasic"))
            {
                AttackBasic();
            }
            if (Input.GetButtonDown("AttackSpecial1"))
            {
                AttackSpecial1();
                nextAttackTime = Time.time + 1f / attackRate;
            }
            if (Input.GetButtonDown("AttackSpecial2"))
            {
                AttackSpecial2();
            }
        }
        
    }
    void Move()
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
            spellPoint.transform.position = (Vector2)transform.position + movement * new Vector2(0.8f, 0.8f) + new Vector2(0, -0.4f);
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            spellPoint.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            runTime = Time.time;
            finalSpeed = speed;
        }

        if (Time.time - runTime > 1.5 && !isRunning)
        {
            finalSpeed = speed * 1.5f;
        }
        playerbody.velocity = movement * finalSpeed;
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
        if (Time.time > comboTimer)
        {
            comboStep = 0;
        }

        switch (comboStep)
        {
            case 0:
            case 1:
                Instantiate(slashPrefab, spellPoint.position, spellPoint.rotation);
                break;
            case 2:
                Instantiate(bigSlashPrefab, spellPoint.position, spellPoint.rotation);
                break;
        }

        comboStep++;
        if (comboStep == 3) { nextAttackTime = nextAttackTime = Time.time + 1f / attackRate; }
        comboStep %= 3;
        comboTimer = Time.time + 0.5f;

        animator.SetTrigger("Attack");
        canMove = false;
    }
    void AttackSpecial1()
    {
        animator.SetTrigger("Attack");
        canMove = false;

        int numProjectiles = 5;
        float spreadAngle = 15f;
        float radius = 0.1f;

        for (int i = 0; i < numProjectiles; i++)
        {
            float angle = (i - (numProjectiles - 1) / 2.0f) * spreadAngle;
            float radianAngle = (spellPoint.eulerAngles.z + angle) * Mathf.Deg2Rad;
            Vector3 spawnPosition = transform.position + new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle), 0) * radius;
            Quaternion rotation = Quaternion.Euler(0, 0, spellPoint.eulerAngles.z + angle);
            GameObject iceProjectile = Instantiate(iceProjectilePrefab, spawnPosition, rotation);
            SpellProjectile spellProjectileScript = iceProjectile.GetComponent<SpellProjectile>();
            spellProjectileScript.Disable();
        }
    }
    void AttackSpecial2()
    {
        if (!windBallPlaced) {
            currentWindBall = Instantiate(windBallPrefab, transform.position, spellPoint.rotation);
            windBallPlaced = true;
        }
        else
        {
            animator.SetTrigger("Attack");
            canMove = false;
            Instantiate(windSpellPrefab, currentWindBall.transform.position, spellPoint.rotation);
            Destroy(currentWindBall);
            windBallPlaced = false;
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }
    public Transform GetPlayerTransform()
    {
        return transform;
    }
    public void OnAttackFinished()
    {
        runTime = Time.time;
        finalSpeed = speed;
        canMove = true;
    }
}
