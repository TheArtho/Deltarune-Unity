using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostTrail : MonoBehaviour
{
    private SpriteRenderer sourceSprite;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private float delay;
    [SerializeField] private Vector3 velocity;

    private void Awake()
    {
        sourceSprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine("UpdateTrail");
    }

    private void OnDisable()
    {
        StopCoroutine("UpdateTrail");
    }

    private IEnumerator UpdateTrail()
    {
        yield return null;
        
        while (true)
        {
            var emitParams = new ParticleSystem.EmitParams
            {
                position = transform.position + velocity.normalized / 10,
                rotation = -1 * transform.eulerAngles.z,
                startColor = new Color(sourceSprite.color.r, sourceSprite.color.g, sourceSprite.color.b, ps.main.startColor.color.a),
                startSize3D = new Vector3(sourceSprite.size.x * sourceSprite.transform.localScale.x, sourceSprite.size.y * sourceSprite.transform.localScale.y, 1 * sourceSprite.transform.localScale.z),
                velocity = this.velocity
            };

            ps.Emit(emitParams, 1);
            ps.textureSheetAnimation.SetSprite(0, sourceSprite.sprite);

            yield return new WaitForSeconds(Mathf.Max(delay, Time.deltaTime));
        }
    }
}
