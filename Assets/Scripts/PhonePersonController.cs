using UnityEngine;
using UnityEngine.AI;

public class PhonePersonController : MonoBehaviour
{
    [Header("Phone Settings")]
    public float minPhoneTime = 3f;     // how long they talk on phone
    public float maxPhoneTime = 8f;
    public float phoneChance = 0.4f;    // 40% chance to go on phone when idle

    private Animator animator;
    private NavMeshAgent agent;
    private bool isOnPhone = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>(); 
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (animator == null || agent == null) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        // Only go on phone when standing still
        if (speed < 0.1f && !isOnPhone)
        {
            if (Random.value < phoneChance * Time.deltaTime)
                StartCoroutine(PhoneRoutine());
        }

        // Stop phone animation if walking
        if (speed > 0.1f && isOnPhone)
        {
            isOnPhone = false;
            animator.SetBool("isOnPhone", false);
        }
    }

    System.Collections.IEnumerator PhoneRoutine()
    {
        isOnPhone = true;
        animator.SetBool("isOnPhone", true);

        float talkTime = Random.Range(minPhoneTime, maxPhoneTime);
        yield return new WaitForSeconds(talkTime);

        isOnPhone = false;
        animator.SetBool("isOnPhone", false);
    }
}