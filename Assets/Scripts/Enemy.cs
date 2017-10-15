using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int damageToPlayer;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    protected override void Start () {
        GameManager.Instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}
	
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        //moving up/down?
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //move enemy every other turn
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir); 
    }

    protected override void OnCantMove<T>(T component)
    {
        //collision with the player?
        var player = component as Player;
        animator.SetTrigger("enemyAttack");
        player.LoseFood(damageToPlayer);
    }
}