using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private bool isPlayer1Goal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            if (isPlayer1Goal)
            {
                print("Player 2 Scored!");
                GameManager.instance.Restart(false);
            }
            else
            {
                print("Player 1 Scored!");
                GameManager.instance.Restart(true);
            }
        }
    }
}
