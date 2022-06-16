using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Play Things")]
    [SerializeField] Button playButton;
    [SerializeField] string nameTutorialScene;
    [SerializeField] BackgroundMusic backgroundMusic;
    [Header("Option Things")]
    [SerializeField] Button optionsButton;
    [SerializeField] GameObject canvasOptions;


    private void Awake()
    {
        playButton.onClick.AddListener(Play);
        optionsButton.onClick.AddListener(Options);
    }
    public void Play()
    {
        backgroundMusic.StopMusic();
        SceneManager.LoadScene(nameTutorialScene);
    }
    public void Options()
    {
        canvasOptions.SetActive(true);
    }
}
