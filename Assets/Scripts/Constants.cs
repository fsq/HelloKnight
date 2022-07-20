using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    #region Input
    public static string kHorizontal = "Horizontal";
    public static string kJump = "Jump";
    #endregion

    #region Attacks
    public enum AttackType { None, Blade, Bullet }
    public static string kBladeAttack = "Fire1";
    public static string kBulletAttack = "Fire2";
    #endregion

    public static float kNever = -100;

    #region Tags
    public static string kTagPlayer = "Player";
    public static string kTagGround = "Ground";
    public static string kTagPlayerAttack = "PlayerAttack";
    public static string kTagMonsters = "Monsters";
    #endregion

    #region Scenes
    public static string kSceneDefault = "Level";
    #endregion

    #region Prefabs
    public static string kPrefabBlade = "Blade";
    public static string kPrefabBullet = "Bullet";
    #endregion
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