﻿using UnityEngine;

namespace PolymindGames.WieldableSystem
{
    public abstract class WieldableBehaviour : MonoBehaviour
    {
        public IWieldable Wieldable { get; private set; }
        protected ICharacter Character => Wieldable.Character;

        
        protected virtual void Awake()
        {
            Wieldable = GetComponentInParent<IWieldable>();
            Wieldable.EquippingStarted += OnEquippingStarted;
            Wieldable.HolsteringEnded += OnHolsteringEnded;
        }

        protected virtual void OnEquippingStarted() { }
        protected virtual void OnHolsteringEnded() { }
    }
}