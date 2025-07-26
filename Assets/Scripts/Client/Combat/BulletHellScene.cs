using System;
using System.Collections;
using System.Collections.Generic;
using Client.Combat;
using Client.Combat.Events;
using Core.Combat;
using Scriptables;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public class BulletHellScene : MonoBehaviour
{
    [SerializeField] private PrefabList soulControllerList;
    [SerializeField] private PrefabList patternList;
    
    [Space]
    
    [SerializeField] SpriteRenderer battleArea;
    [SerializeField] Animator battleAreaAnimator;
    [SerializeField] private float margin = 0.1f; // delta/marge
    [SerializeField] private List<PlayableDirector> directors;
    
    [SerializeField] private GameObject soulController;
    [SerializeField] private List<GameObject> patterns = new List<GameObject>();

    public void StartPhase()
    {
        StartCoroutine(nameof(BulletPhase));
    }

    public void OnDisable()
    {
        Stop();
    }
    
    public void Stop()
    {
        StopCoroutine(nameof(BulletPhase));
        foreach (var d in directors)
        {
            d.Stop();
        }
    }

    public void Prepare(int[] attackers, string playerSoulController, string[] attackPatterns)
    {
        try
        {
            soulController = Instantiate(soulControllerList.GetPrefab(playerSoulController), transform);
            if (soulController.TryGetComponent(out SoulController sc))
            {
                sc.SetPlayer();
            }
            soulController.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        
        for (var i = 0; i < attackPatterns.Length; i++)
        {
            try
            {
                GameObject p = Instantiate(patternList.GetPrefab(attackPatterns[i]), transform);
                patterns.Add(p);
                if (p.TryGetComponent(out BulletPattern bp))
                {
                    bp.Initialize(SfxHandler.instance.AudioSource, BattleScene.Instance.EnemyBattleSprites[attackers[i]].Animator, battleAreaAnimator);
                }
                if (p.TryGetComponent(out PlayableDirector director))
                {
                    directors.Add(director);
                }
                p.gameObject.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    private IEnumerator BulletPhase()
    {
        battleArea.gameObject.SetActive(true);
        yield return new WaitForSeconds(battleAreaAnimator.GetCurrentAnimatorStateInfo(0).length);
        SoulController.Player.gameObject.SetActive(true);
        SoulController.Player.EnableInput();
        yield return StartCoroutine(PlayTimeline());
        battleAreaAnimator.Play("Arena_Disappear");
        SoulController.Player.DisableInput();
        SoulController.Player.gameObject.SetActive(false);
        DestroyPrefabs();
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
        // Emit event
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
        if (directors.Count == 0)
        {
            Debug.Log("[BulletHellScene] No directors to play. Skipping.");
            yield break;
        }
        foreach (var d in directors)
        {
            d.gameObject.SetActive(true);
            d.Play();
            maxDuration = Math.Max(maxDuration, d.duration);
        }
        yield return new WaitForSeconds((float) maxDuration + 0.2f);
        foreach (var d in directors)
        {
            d.gameObject.SetActive(false);
        }
    }
    
    private void DestroyPrefabs()
    {
        Destroy(soulController);
        foreach (var pattern in patterns)
        {
            Destroy(pattern);
        }
        patterns.Clear();
        directors.Clear();
    }

    private void Update()
    {
        ApplyBoundaries();
    }

    private void ApplyBoundaries()
    {
        if (!SoulController.Player) return;
        
        Vector2 soulSize = SoulController.Player.soulSprite.bounds.extents;
        Vector3 pos = SoulController.Player.transform.position;

        Bounds bounds = battleArea.bounds;

        float minX = bounds.min.x + soulSize.x + margin;
        float maxX = bounds.max.x - soulSize.x - margin;
        float minY = bounds.min.y + soulSize.y + margin;
        float maxY = bounds.max.y - soulSize.y - margin;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        SoulController.Player.transform.position = pos;
    }
}
