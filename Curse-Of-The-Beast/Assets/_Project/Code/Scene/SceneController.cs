using UnityEngine;
using UnityEngine.SceneManagement;

namespace KnoxGameStudios
{
    public class SceneController : MonoBehaviour
    {
        public static void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }
    }
}