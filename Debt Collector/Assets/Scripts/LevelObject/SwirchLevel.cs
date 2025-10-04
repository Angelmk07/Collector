using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwirchLevel : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerStatus.completedLevels++;
            playerStatus.SaveVariables();
        }
    }
}