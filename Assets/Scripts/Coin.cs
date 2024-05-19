using UnityEngine;


/// <summary>
/// Represents a coin in the game.
/// </summary>
/// <remarks>
/// The Coin class is responsible for handling the behavior of a coin object in the game.
/// It contains properties for the coin's value, as well as references to particle effects and audio
/// clips that are played when the coin is collected.
/// </remarks>
public class Coin : MonoBehaviour
{
    public int value;

    public ParticleSystem collectParticles;
    public AudioClip collectSound;


    private void Collect()
    {
        if (collectParticles is not null)
        {
            var particles = Instantiate(collectParticles, transform.position, Quaternion.identity);
            Destroy(particles.gameObject, particles.main.duration);
        }

        if (collectSound is not null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);


        CoinCounter.Instance.AddCoins(value);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) Collect();
    }
}