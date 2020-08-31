using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

}
