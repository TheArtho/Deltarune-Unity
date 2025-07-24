using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private Image soul;
    [SerializeField] private ParticleSystem soulParticles;
    [SerializeField] private Image title;
    [SerializeField] private DialogBox dialogBox;
    [Space]
    [SerializeField] private float timePerChar = 0.1f;
    [SerializeField] private string sound = "text";
    
    private void OnEnable()
    {
        // Play soul break animation
        soul.gameObject.SetActive(true);
        if (SoulController.Player)
        {
            soul.rectTransform.position = new Vector3(SoulController.Player.transform.position.x, SoulController.Player.transform.position.y, soul.rectTransform.position.z) / 2;
            soulParticles.GetComponent<RectTransform>().anchoredPosition = soul.rectTransform.anchoredPosition;
        }
        else
        {
            soul.rectTransform.anchoredPosition = Vector2.zero;
            soulParticles.transform.position = soul.transform.position;
        }
        title.color = Color.clear;
        dialogBox.Clear();
        BgmHandler.Stop();
        StartCoroutine("AnimationIE");
    }

    private IEnumerator AnimationIE()
    {
        SfxHandler.Play("break_1");
        
        yield return new WaitForSeconds(1f);
        
        soul.gameObject.SetActive(false);
        SfxHandler.Play("break_2");
        soulParticles.Play();
        
        yield return new WaitForSeconds(1.3f);
        
        BgmHandler.PlayMain("game_over");
        
        LeanTween.value(gameObject, Color.clear, Color.white, 1.3f).setOnUpdate(color =>
        {
            title.color = color;
        }).setEaseOutQuad();
        
        yield return new WaitForSeconds(1.3f);
        
        dialogBox.SetTypingSound(sound);
        dialogBox.Clear();
        yield return StartCoroutine(dialogBox.DrawText("This is not\nyour fate...!", timePerChar));
        yield return new WaitForSeconds(2f);
        dialogBox.Clear();
        yield return StartCoroutine(dialogBox.DrawText("Please,", timePerChar));
        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(dialogBox.DrawText("\ndont give up!", timePerChar));
    }
}
