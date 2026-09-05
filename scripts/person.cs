using Godot;
using System;

public partial class person : CharacterBody2D
{
    private const float Speed = 300.0f;
    private const float Gravity = 980.0f;
    private const float JumpVelocity = -500.0f;

    //Coeficientes de atrito
    private const float NormalFriction = 0.6f;
    private const float IceFriction = 0.05f;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        //Gravidade
        if (!IsOnFloor())
        {
            velocity.Y += Gravity * (float)delta;
        }

        //Pulo
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        //Verifica se o personagem encontra-se no gelo
        bool isOnIce = false;

        if (IsOnFloor())
        {
            for (int i = 0; i < GetSlideCollisionCount(); i++)
            {
                var collision = GetSlideCollision(i);
                var collider = collision.GetCollider() as Node;

                if (collider != null && collider.IsInGroup("gelo"))
                {
                    isOnIce = true;
                    break;
                }
            }
        }

        //Movimento horizontal
        float direction = Input.GetAxis("ui_left", "ui_right");

        if (direction != 0)
        {
            //Aceleração para atingir a velocidade desejada
            velocity.X = Mathf.MoveToward(
                velocity.X,
                direction * Speed,
                1000.0f * (float)delta
            );
        }
        else
        {
            //Define o coeficiente de atrito
            float friction = isOnIce
                ? IceFriction
                : NormalFriction;

            //Força de atrito simulada através da desaceleração
            float frictionAcceleration = friction * Gravity;

            //Desacelera o personagem até parar
            velocity.X = Mathf.MoveToward(
                velocity.X,
                0,
                frictionAcceleration * (float)delta
            );
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}