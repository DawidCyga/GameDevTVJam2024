using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanBeSlowedDown
{
    public float TimeTillEndSlowDown { get; set; }
    public bool IsSlowedDown { get; set; }
    public void TrySlowDown(float slowdownDuration, float slowdownMultiplier);
    public void UpdateSlowDown();
}
