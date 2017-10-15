using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    //damage the player applies to wall when hit
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    private Animator animator;

    private int food; //will save/retrieve to GameManager.playerFoodPoints

    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();

        food = GameManager.Instance.playerFoodPoints;
        base.Start();
	}

    private void OnDisable()
    {
        GameManager.Instance.playerFoodPoints = food;
    }

    private bool IsGameOver { get { return food <= 0; } }

    private void CheckIfGameOver()
    {
        if (IsGameOver)
            GameManager.Instance.GameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        CheckIfGameOver();
        GameManager.Instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //for collision with soda, food and exit
        if (collision.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if(collision.tag == "Food")
        {
            food += pointsPerFood;
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "Soda")
        {
            food += pointsPerSoda;
            collision.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
        if (GameManager.Instance.playersTurn == false)
            return;
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("horizontal");
        vertical = (int)Input.GetAxisRaw("vertical");

        //prevent player from moving diagonally
        if (horizontal != 0)
            vertical = 0;

        if (horizontal == 0 && vertical == 0)
            return;

        //attempt to move into a wall
        AttemptMove<Wall>(horizontal, vertical);
    }
}
