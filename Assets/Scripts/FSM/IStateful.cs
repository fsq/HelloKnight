using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateful
{
    public abstract StateParam StateParam { get; set; }
}

