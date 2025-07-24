using System;
using System.Collections;
using Client.Combat;
using Client.Combat.Events;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SoulController : MonoBehaviour
{
    public static SoulController Player;
    
    public float speed = 1;
    
    public SpriteRenderer soulSprite;

    [SerializeField] private ParticleSystem particle;
    [Space] [SerializeField] private float invincibleDelay = 1f;
    [SerializeField] private bool invincible = false;
    [SerializeField] private float grazeDelay = 0.1f;
    [SerializeField] private bool canGraze = true;
    private bool hurt = false;
    
    private PlayerInputAction _inputAction;

    private Vector2 _move;
    
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        if (!Player)
        {
            Player = this;
        }
        
        _rigidbody = GetComponent<Rigidbody2D>();
        _inputAction = new PlayerInputAction();

        _inputAction.Battle.Move.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            float deadzone = 0.25f;

            if (input.magnitude < deadzone)
            {
                _move = Vector2.zero;
                return;
            }

            _move = Get8Direction(input);
        };
        
        _inputAction.Battle.Move.canceled += ctx =>
        {
            _move = Vector2.zero;
        };
    }

    private void ResetProperties()
    {
        invincible = false;
        hurt = false;
        canGraze = true;
    }
    
    private Vector2 Get8Direction(Vector2 input)
    {
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360;

        if (angle >= 337.5f || angle < 22.5f)
            return Vector2.right; // →
        else if (angle < 67.5f)
            return new Vector2(1, 1).normalized; // ↗
        else if (angle < 112.5f)
            return Vector2.up; // ↑
        else if (angle < 157.5f)
            return new Vector2(-1, 1).normalized; // ↖
        else if (angle < 202.5f)
            return Vector2.left; // ←
        else if (angle < 247.5f)
            return new Vector2(-1, -1).normalized; // ↙
        else if (angle < 292.5f)
            return Vector2.down; // ↓
        else
            return new Vector2(1, -1).normalized; // ↘
    }

    public void FixedUpdate()
    {
        Vector2 nextPosition = (Vector2) soulSprite.transform.position + _move * (speed * Time.fixedDeltaTime);
        _rigidbody.position = nextPosition;
    }

    public void Graze()
    {
        if (hurt || !canGraze) return;
        
        particle.Emit(1);
        SfxHandler.Play("graze");
        // Update TP
        BattleScene.Instance.EmitEvent(new GrazeEvent()
        {
            Player = 0
        });
        
        canGraze = false;
        Invoke(nameof(SetCanGraze), grazeDelay);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (invincible) return;
        
        if (other.CompareTag("Projectile"))
        {
            // TODO change this hardcoded value
            int damage = 9999;

            // Make temporarily invincible the player
            invincible = true;
            hurt = true;
            Invoke(nameof(SetVulnerable), invincibleDelay);
            StartCoroutine(nameof(HurtAnimation));
            
            foreach (var i in BattleScene.Instance.Targets)
            {
                BattleScene.Instance.EmitEvent(new PlayerHurtEvent
                {
                    Player = i,
                    Damage = damage
                });
            }
        }
    }

    public void SetVulnerable()
    {
        invincible = false;
        hurt = false;
    }
    
    public void SetCanGraze()
    {
        canGraze = true;
    }

    private IEnumerator HurtAnimation()
    {
        Color baseColor = soulSprite.color;
        Color transitionColor = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a / 10);
        int iterations = 3;

        for (int i = 0; i < iterations; i++)
        {
            LeanTween.value(gameObject, baseColor, transitionColor, invincibleDelay / 6).setOnUpdate(color =>
            {
                soulSprite.color = color;
            });
            yield return new WaitForSeconds(invincibleDelay / 6);
            LeanTween.value(gameObject, transitionColor, baseColor, invincibleDelay / 6).setOnUpdate(color =>
            {
                soulSprite.color = color;
            });
            yield return new WaitForSeconds(invincibleDelay / 6);
        }
    }

    private void OnEnable()
    {
        ResetProperties();
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(HurtAnimation));
    }

    public void EnableInput()
    {
        _inputAction.Enable();
    }
    
    public void DisableInput()
    {
        _inputAction.Disable();
    }
}
