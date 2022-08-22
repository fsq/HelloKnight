using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffManager
{
    public enum BuffType
    {
        // Increase melee attack range
        LongBlade
    }

    [SerializeField] public static float kLongBladeMultiplier = 1.2f;

    // Buff/Debuff type and current stack size.
    private Dictionary<BuffType, int> _buffs = new Dictionary<BuffType, int>();

    public void Apply(BuffType type, int stack)
    {
        // "dict[type]+=" won't work here, bruh.
        if (_buffs.ContainsKey(type))
        {
            _buffs[type] += stack;
        }
        else
        {
            _buffs[type] = stack;
        }
    }

    public int GetStackCount(BuffType type)
    {
        return _buffs.ContainsKey(type) ? _buffs[type] : 0;
    }
}
