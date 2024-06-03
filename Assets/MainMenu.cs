using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    private List<string> _levels = new() {"SampleScene", "Level2", "Level3", "Level4", "Level5"};

    private void Start()
    {
        _levels = LoadLevels();
    }

    private static List<string> LoadLevels()
    {
        // not implemented
        return new List<string> { "SampleScene" };
    }

    // Начать игру
    public void PlayGame()
    {
        SceneManager.LoadScene(1); // File -> Build settings -> Scenes in Build
    }
    

    // Продолжить игру
    public void Continue()
    {
        SceneManager.LoadScene(_levels[~1]);
    }
}
