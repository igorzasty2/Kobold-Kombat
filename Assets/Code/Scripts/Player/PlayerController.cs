using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }
    [SerializeField] Transform spellPoint;
    [SerializeField] public float speed;
    [SerializeField] public int maxHealth;
    Rigidbody2D playerbody;
    Collider2D playerCollider;
    DamageDisplay damageDisplay;
    private bool isFacingRight = true;
    public Animator animator;
    [SerializeField] private float attackRate = 2f;
    float nextAttackTime = 0f;
    bool windBallPlaced = false;
    [SerializeField] EndScreen endScreen;

    [SerializeField] public GameObject windBallPrefab;
    [SerializeField] public GameObject windSpellPrefab;
    [SerializeField] public GameObject iceProjectilePrefab;
    private GameObject currentWindBall;
    [SerializeField] public GameObject slashPrefab;
    [SerializeField] public GameObject bigSlashPrefab;


    private float runTime;
    private float finalSpeed;
    private bool isRunning = false;
    private bool canMove = true;
    private bool canAttack = true;
    private int comboStep = 0;
    private float comboTimer = 0f;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        playerbody = GetComponent<Rigidbody2D>();
        damageDisplay = FindObjectOfType<DamageDisplay>();
        playerCollider = GetComponent<Collider2D>();
        damageDisplay.UpdateHealth(GameManager.health, maxHealth);
    }

    void Update()
    {
        if (canMove)
        {
            Move();
        }

        if (Time.time >= nextAttackTime && canAttack)
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
    public void Damage(int damage)
    {
        GameManager.health -= damage;
        GameManager.damageTaken += damage;
        if (damageDisplay != null)
        {
            damageDisplay.ShowDamage(transform.position, damage);
            damageDisplay.UpdateHealth(GameManager.health, maxHealth);
        }
        if (GameManager.health > 0)
        {
            animator.SetTrigger("Hurt");
            canMove = false;
            canAttack = false;
            runTime = Time.time;
            finalSpeed = speed;
        }
        else
        {
            endScreen.FinishLevel(false);
            animator.SetTrigger("Dead");
            playerCollider.enabled = false;
            canMove = false;
            canAttack = false;
        }

    }
    void NoHurtie()
    {
        canMove = true;
        canAttack = true;
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
    void Dash()
    {
        //animator.SetTrigger("Attack");
        // = false;

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
