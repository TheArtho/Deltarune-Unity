using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private Sprite soulBroken;
    [SerializeField] private Image title;
    [SerializeField] private DialogBox dialogBox;
    [Space]
    [SerializeField] private float timePerChar = 0.1f;
    [SerializeField] private string sound = "text";
    
    public void Play()
    {
        // Play soul break animation
        SoulController.Player.brokenSoul.gameObject.SetActive(true);
        title.color = Color.clear;
        dialogBox.Clear();
        BgmHandler.Stop();
        StartCoroutine("AnimationIE");
    }

    private IEnumerator AnimationIE()
    {
        SoulController.Player.soulSprite.color = Color.white;
        SfxHandler.Play("break_1");
        
        yield return new WaitForSeconds(1f);

        SoulController.Player.brokenSoul.gameObject.SetActive(false);
        SfxHandler.Play("break_2");
        SoulController.Player.shatterParticles.Play();
        
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
