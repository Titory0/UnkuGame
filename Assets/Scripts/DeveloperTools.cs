using UnityEngine;

public class DevTools : MonoBehaviour
{

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            Time.timeScale = 1f; 
            Debug.Log("TimeScale = " + Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Time.timeScale = Time.timeScale/2;
            Debug.Log("TimeScale = " + Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Time.timeScale = Time.timeScale * 2;
            Debug.Log("TimeScale = " + Time.timeScale);
        }

    }
}
