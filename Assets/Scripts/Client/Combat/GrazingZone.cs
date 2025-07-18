using UnityEngine;

public class GrazingZone : MonoBehaviour
{
    [SerializeField] private SoulController soulController;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            Debug.Log($"Graze by projectile {other.name}");
            soulController.Graze();
        }
    }
}
