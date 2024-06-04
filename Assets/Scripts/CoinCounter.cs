using UnityEngine;
using TMPro;


/// <summary>
/// Manages the coin count within the game. This class follows the Singleton pattern to ensure only one instance exists.
/// </summary>
/// <remarks>
/// Use the Instance property to access the unique CoinCounter instance.
/// Ensure that this component is attached to only one GameObject.
/// To modify the coin count, use the AddCoins method.
/// </remarks>
public class CoinCounter : MonoBehaviour
{
    public static CoinCounter Instance { get; private set; }

    [SerializeField] private TMP_Text coinText;

    private int _currentCoins; // default value is 0

    public int GetCurrentCoins() => _currentCoins;

    private void Awake()
    {
        if (Instance != null) Debug.LogWarning("Instance already exists.");
        else Instance = this;
        
        if (coinText == null) Debug.LogError("Coin Text is not assigned.");
        else UpdateCoinText();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) Debug.LogWarning($"Adding negative coins. {amount}");
        _currentCoins += amount;
        UpdateCoinText();
    }

    private void UpdateCoinText() => coinText.text = _currentCoins.ToString();
}