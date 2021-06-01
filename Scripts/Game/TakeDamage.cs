using UnityEngine;
using MLAPI;

public class TakeDamage : MonoBehaviour
{
    public float health = 50f;

    public void damage(float amount)    //gets run from external scripts, lowers the players hp by the supplied float.
    {
        health -= amount;
        if (health <= 0)
        {
            Debug.Log("you died!!");
            Application.Quit(); //simply quits the game when your hp is lower than 0, never added a death menu.
            //UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
