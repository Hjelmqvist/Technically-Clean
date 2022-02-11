using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneButton : MonoBehaviour
{
    [SerializeField] KeyCode keyToResetScene = KeyCode.R;

    private void Update()
    {
        if (Input.GetKeyDown( keyToResetScene ))
        {
            SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
        }
    }
}