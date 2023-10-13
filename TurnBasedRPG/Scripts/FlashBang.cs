using UnityEngine;
 using UnityEngine.UI;  
 using System.Collections;
 
 public class FlashBang : MonoBehaviour 
 {
 
     public CanvasGroup myCG;
     public Image image = null;
     
     Coroutine currentFlashRoutine = null;
     
     private void Awake()
     {
        image = GetComponent<Image>();
     }

     public void ScreenFlash (float secondsForOneFlash, float maxAlpha, Color newColor)
     {
        image.color = newColor;

        maxAlpha = Mathf.Clamp(maxAlpha, 0, 1);

        if(currentFlashRoutine != null)
        {
            StopCoroutine(currentFlashRoutine);
        }

        currentFlashRoutine = StartCoroutine(Flash(secondsForOneFlash, maxAlpha));
     }

     IEnumerator Flash(float secondsForOneFlash, float maxAlpha)
     {
        float flashInDuration = secondsForOneFlash /2;
        
        for(float time = 0; time <= flashInDuration; time += Time.deltaTime)
        {
            Color colorThisFrame = image.color;
            colorThisFrame.a = Mathf.Lerp(0,maxAlpha, time / flashInDuration);
            image.color = colorThisFrame;

            yield return null;
        }

        float flashOutDuration = secondsForOneFlash / 2;

        for(float time = 0; time <= flashOutDuration; time += Time.deltaTime)
        {
            Color colorThisFrame = image.color;
            colorThisFrame.a = Mathf.Lerp(maxAlpha, 0, time / flashOutDuration);
            image.color = colorThisFrame;
            yield return null;
        }
     }
 }
