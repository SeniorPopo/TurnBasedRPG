using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BatMovement : MonoBehaviour
{
    public float speed = 3.0f;

    private bool DetectedPlayer = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //SceneManager.LoadScene("Thief Battle");
            DetectedPlayer = true;
        }
    }
}
