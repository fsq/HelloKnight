using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public static string kGround = "Ground";
    public static string kHorizontal = "Horizontal";
    public static string kJump = "Jump";
    public static string kPlayer = "Player";

    public enum AttackType { None, Blade, Bullet }
    public static string kBladeAttack = "Fire1";
    public static string kBulletAttack = "Fire2";

    public static float kNever = -100;
    public static string kTagPlayerAttack = "PlayerAttack";
    public static string kTagMonsters = "Monsters";
}

public class FrameInput
{
    public float X;
    public bool JumpDown;
    public bool JumpHold;
    public bool JumpUp;
    public bool BladeDown;
    public bool BulletDown;

    public bool AttackDown()
    {
        return BladeDown || BulletDown;
    }
}

public struct RayRange
{
    public readonly Vector2 Start, End, Dir;
    public RayRange(float x1, float y1, float x2, float y2, Vector2 dir)
    {
        Start = new Vector2(x1, y1);
        End = new Vector2(x2, y2);
        Dir = dir;
    }
}