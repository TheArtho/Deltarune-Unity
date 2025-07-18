using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SoulController : MonoBehaviour
{
    public float speed = 1;
    
    public SpriteRenderer soulSprite;
    public SpriteRenderer battleArea;

    [SerializeField] private ParticleSystem particle;
    
    private PlayerInputAction _inputAction;

    private Vector2 _move;
    
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
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
        particle.Emit(1);
        SfxHandler.Play("graze");
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
