using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHideMenu : MonoBehaviour
{
    public void ToggleActive()
    {   
        if(gameObject.activeSelf == false)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
        foreach (Transform childButton in transform)
        {
            if (childButton.gameObject.activeSelf == false) 
                childButton.gameObject.SetActive(true);
            else
                childButton.gameObject.SetActive(false);

        }
    }

}
