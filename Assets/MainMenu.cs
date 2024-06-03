using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    // Начать игру
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    

    // Продолжить игру
    public void Continue() { }
    
    // Бестиарий
    public void Bestiary() { }
    
    // Настройки
    public void Settings() { }
}
