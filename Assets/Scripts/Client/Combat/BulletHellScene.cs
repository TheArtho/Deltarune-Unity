using System;
using System.Collections;
using Client.Combat;
using Client.Combat.Events;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class BulletHellScene : MonoBehaviour
{
    [SerializeField] SoulController soulController;
    [SerializeField] SpriteRenderer battleArea;
    [SerializeField] Animator battleAreaAnimator;
    [SerializeField] private float margin = 0.1f; // delta/marge
    private PlayableDirector _director;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }

    public void StartPhase()
    {
        StartCoroutine(nameof(BulletPhase));
    }

    public void OnDisable()
    {
        StopCoroutine(nameof(BulletPhase));
    }

    private IEnumerator BulletPhase()
    {
        LeanTween.value(gameObject, Color.white, Color.gray, 0.8f).setOnUpdate(color =>
        {
            if (BattleScene.Instance.Background)
            {
                BattleScene.Instance.Background.color = color;
            }
        }).setEaseOutQuad();
        battleArea.gameObject.SetActive(true);
        yield return new WaitForSeconds(battleAreaAnimator.GetCurrentAnimatorStateInfo(0).length);
        soulController.gameObject.SetActive(true);
        soulController.EnableInput();
        yield return StartCoroutine(PlayTimeline());
        battleAreaAnimator.Play("Arena_Disappear");
        soulController.DisableInput();
        soulController.gameObject.SetActive(false);
        LeanTween.value(gameObject, Color.gray, Color.white, 0.8f).setOnUpdate(color =>
        {
            if (BattleScene.Instance.Background)
            {
                BattleScene.Instance.Background.color = color;
            }
        }).setEaseOutQuad();
        yield return null;
        yield return new WaitForSeconds(battleAreaAnimator.GetCurrentAnimatorStateInfo(0).length);
        battleArea.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
        // Emit event
        // TODO Change this hardcoded part
        BattleScene.Instance.EmitEvent(new BulletHellEndedEvent()
        {
            Player = 0,
        });
        BattleScene.Instance.EmitEvent(new BulletHellEndedEvent()
        {
            Player = 1,
        });
        BattleScene.Instance.EmitEvent(new BulletHellEndedEvent()
        {
            Player = 2,
        });
    }

    private IEnumerator PlayTimeline()
    {
        _director.Play();
        yield return new WaitForSeconds((float) _director.duration + 0.2f);
    }

    private void Update()
    {
        ApplyBoundaries();
    }

    private void ApplyBoundaries()
    {
        Vector2 soulSize = soulController.soulSprite.bounds.extents;
        Vector3 pos = soulController.transform.position;

        Bounds bounds = battleArea.bounds;

        float minX = bounds.min.x + soulSize.x + margin;
        float maxX = bounds.max.x - soulSize.x - margin;
        float minY = bounds.min.y + soulSize.y + margin;
        float maxY = bounds.max.y - soulSize.y - margin;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        soulController.transform.position = pos;
    }
}
