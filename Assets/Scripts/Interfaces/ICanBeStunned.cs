using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanBeStunned
{
    public float TimeTillEndStun { get; }
    public bool IsStunned { get; }

    public void TryStun(float stunDuration);
}
