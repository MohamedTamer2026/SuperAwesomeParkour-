using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    public enum State { Idle, Move, Attack, Damage, Die }
    public State currentState = State.Idle;

    private NavMeshAgent agent;
    private Animator anim;
    public Transform player;

    public float chaseRange = 10f;
    public float attackRange = 2f;

    public float ghostDamage = 10f;
    private float attackCooldown = 2f;
    private float nextAttackTime = 0f;

    public float maxHealth = 100f;
    public float health;
    public Image healthBarFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (player == null) player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle: IdleState(); break;
            case State.Move: MoveState(); break;
            case State.Attack: AttackState(); break;
            case State.Damage: DamageState(); break;
            case State.Die: DieState(); break;
        }
    }

    void IdleState()
    {
        anim.SetFloat("Speed", 0);
        if (Vector3.Distance(transform.position, player.position) < chaseRange)
        {
            currentState = State.Move;
        }
    }

    void MoveState()
    {
        agent.SetDestination(player.position);
        anim.SetFloat("Speed", agent.velocity.magnitude);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = State.Attack;
        }
        else if (Vector3.Distance(transform.position, player.position) > chaseRange)
        {
            currentState = State.Idle;
            agent.ResetPath();
        }
    }

    void AttackState()
    {
        agent.ResetPath();

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (Time.time >= nextAttackTime)
        {
            anim.SetTrigger("AttackTrigger");

            AnimalAbility animal = player.GetComponentInChildren<AnimalAbility>();
            if (animal != null)
            {
                animal.PlayerTakeDamage(ghostDamage);
            }

            nextAttackTime = Time.time + attackCooldown;
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = State.Move;
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentState == State.Die) return;

        health -= amount;

        float calculateHealth = health / maxHealth;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = health / maxHealth;
        }

        if (health <= 0.1f)
        {
            health = 0;
            if (healthBarFill != null) healthBarFill.fillAmount = 0;

            currentState = State.Die;
            DieState();
        } else
        {
            currentState = State.Damage;
        }
    }

    void DamageState()
    {
        anim.SetTrigger("DamageTrigger");
        agent.isStopped = true;

        CancelInvoke("Recover");
        Invoke("Recover", 0.5f);
    }

    void Recover() { 
        if (currentState != State.Die) { 
            agent.isStopped = false; 
            currentState = State.Move; 
        } 
    }
    void DieState()
    {
        if (healthBarFill != null)
        {
            healthBarFill.transform.parent.gameObject.SetActive(false);
        }

        anim.SetTrigger("DieTrigger");
        agent.isStopped = true;
        agent.enabled = false;

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 3f);
        this.enabled = false;
    }
}
