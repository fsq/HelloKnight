using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Centralized place to dispatch events.
public static class EventManager
{
    public static Action<Monsters> onMonsterDie;
}
