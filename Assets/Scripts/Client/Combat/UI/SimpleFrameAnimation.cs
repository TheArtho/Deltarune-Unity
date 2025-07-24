using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimpleFrameAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] animation;
    [SerializeField] private float delay = 0.5f;
    
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine("Animation");
    }

    private void OnDisable()
    {
        StopCoroutine("Animation");
    }

    private IEnumerator Animation()
    {
        if (animation.Length == 0) yield break;
        
        int index = 0;
        while (true)
        {
            image.sprite = animation[index];
            index = (index + 1) % animation.Length;
            yield return new WaitForSeconds(delay);
        }
    }
}
