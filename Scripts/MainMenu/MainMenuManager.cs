using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    //im not going to comment everything, google playerPerfs if needed.
    
    //sets the playerperfs to all the supplied values from the main menu. Functions are called directly from the input fields/buttons/sliders.
    public void Exit()
    {
        Debug.Log("Quit");
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying=false;
    }

        public void SetFOV(float fov)
    {
        PlayerPrefs.SetFloat("FOV", fov);
        Debug.Log(fov);
    }
    public void SetSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        Debug.Log(sensitivity);
    }
    public void SetIp(string IP){
        PlayerPrefs.SetString("IP",IP);
        Debug.Log(IP);
    }
    public void SetHostPort(string HostPort){
        PlayerPrefs.SetInt("HostPort", int.Parse(HostPort));
        Debug.Log(HostPort);
    }
    public void SetJoinPort(string JoinPort){
        PlayerPrefs.SetInt("JoinPort", int.Parse(JoinPort));
        Debug.Log(JoinPort);
    }
    void Start(){
        SetFOV(75f);
        SetSensitivity(1f);
    }
    public void Join(){
        PlayerPrefs.SetInt("Mode", 0);
        Debug.Log("Join");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");

    }
    public void Host(){
        PlayerPrefs.SetInt("Mode", 1);
        Debug.Log("Host");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
