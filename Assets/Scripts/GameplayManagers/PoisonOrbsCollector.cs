using System;
using UnityEngine;

public class PoisonOrbsCollector : MonoBehaviour
{
    public static PoisonOrbsCollector Instance {  get; private set; }

    [Header("Collection Parameters")]
    [SerializeField] private int _poisonUsesPerCollected;
    [SerializeField] private int _maxPoisonUsesCapacity;

    [Header("For Debugging Only")]
    [SerializeField] private int _availablePoisonUses;

    public event EventHandler OnAvailablePoisonUsesIncreased;
    public event EventHandler OnAvailablePoisonUsesDecreased;

    private void Awake() => Instance = this;
    private void Start() => PoisonOrb.OnAnyOrbCollected += PoisonOrb_OnAnyOrbCollected;
    private void OnDestroy() => PoisonOrb.OnAnyOrbCollected -= PoisonOrb_OnAnyOrbCollected;
    private void PoisonOrb_OnAnyOrbCollected(object sender, EventArgs e) => TryAddOrbs(_poisonUsesPerCollected);

    private void TryAddOrbs(int usesToAdd)
    {
        if (_availablePoisonUses >= _maxPoisonUsesCapacity) return;

        int availableUsesIfLessThanCapacity = _availablePoisonUses += usesToAdd;

        _availablePoisonUses = Mathf.Min(availableUsesIfLessThanCapacity, _maxPoisonUsesCapacity);

        OnAvailablePoisonUsesIncreased?.Invoke(this, EventArgs.Empty);
    }
    public void DecreaseOrbs()
    {
        _availablePoisonUses--;
        OnAvailablePoisonUsesDecreased?.Invoke(this, EventArgs.Empty);
    }
    public int GetAvailableUses() => _availablePoisonUses;
}
