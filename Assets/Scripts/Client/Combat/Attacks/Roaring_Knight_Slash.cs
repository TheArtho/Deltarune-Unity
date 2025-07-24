using UnityEngine;

public class Roaring_Knight_Slash : MonoBehaviour
{
    public float rotation = 110f;
    public float time = 1f;
    
    void OnEnable()
    {
        transform.position = new Vector3(SoulController.Player.transform.position.x, SoulController.Player.transform.position.y, transform.position.z);
        transform.eulerAngles =  new Vector3(0, 0, UnityEngine.Random.Range(0, 90));
        LeanTween.rotateZ(gameObject, transform.eulerAngles.z + rotation, time).setEase(LeanTweenType.easeOutQuad);
    }
}
