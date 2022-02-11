using UnityEngine;

public class EscapeToExit : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown( KeyCode.Escape ))
        {
            Application.Quit();
        }
    }
}