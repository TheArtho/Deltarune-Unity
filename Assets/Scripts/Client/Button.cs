using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Button : MonoBehaviour
{
    public bool selected;

    public Sprite unselectedSprite;
    public Sprite selectedSprite;

    private Image _image;
    
    void Awake()
    {
        _image = GetComponent<Image>();
    }
    
    void Update()
    {
        _image.sprite = selected ? selectedSprite : unselectedSprite;
    }
}
