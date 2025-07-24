using System;
using System.Collections;
using System.Collections.Generic;
using Client.Combat;
using Client.Combat.Events;
using UnityEngine;
using UnityEngine.Playables;

public class BulletHellScene : MonoBehaviour
{
    [SerializeField] SoulController soulController;
    [SerializeField] SpriteRenderer battleArea;
    [SerializeField] Animator battleAreaAnimator;
    [SerializeField] private float margin = 0.1f; // delta/marge
    [SerializeField] private List<PlayableDirector> _directors;

    public void AddDirector(PlayableDirector director)
    {
        _directors.Add(director);
        director.transform.parent = transform;
    }

    public void ClearDirectors()
    {
        foreach (var d in _directors)
        {
            Destroy(d.gameObject);
        }

        _directors.Clear();
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
        foreach (var sprites in BattleScene.Instance.PlayerBattleSprites)
        {
            sprites.Lighten();
        }
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
        double maxDuration = 0;
        if (_directors.Count == 0) yield break;
        foreach (var d in _directors)
        {
            d.Play();
            maxDuration = Math.Max(maxDuration, d.duration);
        }
        yield return new WaitForSeconds((float) maxDuration + 0.2f);
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
