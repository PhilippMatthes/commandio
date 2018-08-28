using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class LinkNavigationVelocityToAnimationSpeed : MonoBehaviour {

    private Animator animator;
    private NavMeshAgent agent;
    public float multiplier = 0.01f;

	void Start () {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
	}

    void Update()
    {
        animator.speed = (agent.velocity.magnitude / Time.deltaTime) * multiplier;
    }

}
