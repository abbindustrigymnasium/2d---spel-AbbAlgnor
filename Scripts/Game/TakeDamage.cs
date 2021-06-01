using UnityEngine;
using MLAPI;

public class TakeDamage : MonoBehaviour
{
    public float health = 50f;

    public void damage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Debug.Log("you died!!");
            Application.Quit();
            //UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
