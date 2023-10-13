using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public Camera MenuCamera;
    public AudioSource CameraAudioSource;
    public AudioClip PlayButtonSfx;
   
    private void Start()
    {
        CameraAudioSource = MenuCamera.GetComponent<AudioSource>();
    }
    public void PlayGame()
    {
        StartCoroutine(DelayedLevelLoad());
    }
    IEnumerator DelayedLevelLoad()
    {
        CameraAudioSource.PlayOneShot(PlayButtonSfx);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("OverWorld");
    }
    public void QuitGame()
    {
        Debug.Log("Quit!");

        Application.Quit();
    }
}
