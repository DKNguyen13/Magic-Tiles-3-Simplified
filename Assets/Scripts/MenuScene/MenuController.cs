using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        if(playButton != null && exitButton != null)
        {
            playButton.onClick.AddListener(LoadGameScene);
            exitButton.onClick.AddListener(QuitGame);
        }
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Main");
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
