using MuonhoryoLibrary.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servant
{
    /// <summary>
    /// Default value changed by add's and multiply's modifiers
    /// </summary>
    public sealed class CompositeParameter
    {
        public interface ICharacterConstModifier
        {
            public event Action<float> UpdateValueEvent;
            public void RemoveModifier();
            public float Modifier_ { get; }
            public void UpdateModifier(float newValue);
        }
        private sealed class ModifierHandler : ICharacterConstModifier
        {
            public ModifierHandler(float Modifier, SingleLinkedList<ModifierHandler> list, CompositeParameter owner)
            {
                void RemoveFromListAction()
                {
                    list.Remove(this);
                    owner.RecalculateSpeed();
                }
                RemoveHandlerAction = RemoveFromListAction;
                UpdateValueEvent += (i) => owner.RecalculateSpeed();
                this.Modifier = Modifier;
            }
            private float Modifier;
            public event Action<float> UpdateValueEvent = delegate { };
            private readonly Action RemoveHandlerAction;
            public float Modifier_ => Modifier;
            void ICharacterConstModifier.RemoveModifier() => RemoveHandlerAction();
            public void UpdateModifier(float newValue)
            {
                Modifier = newValue;
                UpdateValueEvent(Modifier);
            }
        }
        private CompositeParameter() { }
        public CompositeParameter(float DefaultValue)
        {
            this.DefaultValue = DefaultValue;
            RecalculateSpeed();
        }
        public readonly float DefaultValue;
        public float CurrentValue_ { get; private set; }
        public event Action<ICharacterConstModifier> AddingAddModifierEvent;
        public event Action<ICharacterConstModifier> AddingMultiplyModifierEvent;
        public event Action<float> ValueHasBeenRecalculatedEvent;
        private readonly SingleLinkedList<ModifierHandler> AddersList = new();
        private readonly SingleLinkedList<ModifierHandler> MultipliesList = new();
        private void RecalculateSpeed()
        {
            CurrentValue_ = DefaultValue;
            foreach (var item in MultipliesList)
            {
                CurrentValue_ *= item.Modifier_;
            }
            foreach (var item in AddersList)
            {
                CurrentValue_ += item.Modifier_;
            }
            ValueHasBeenRecalculatedEvent?.Invoke(CurrentValue_);
        }
        private ICharacterConstModifier AddModifier
            (float modifierValue, SingleLinkedList<ModifierHandler> list,
            Action<ICharacterConstModifier> runningEventAction)
        {
            ModifierHandler modifier = new(modifierValue, AddersList, this);
            list.AddLast(modifier);
            runningEventAction(modifier);
            RecalculateSpeed();
            return modifier;
        }
        public ICharacterConstModifier AddModifier_Add(float speed)
        {
            return AddModifier(speed, AddersList, (item) => AddingAddModifierEvent?.Invoke(item));
        }
        public ICharacterConstModifier AddModifier_Multiply(float speed)
        {
            return AddModifier(speed, MultipliesList, (item) => AddingMultiplyModifierEvent?.Invoke(item));
        }
        public static explicit operator float(CompositeParameter i) => i.CurrentValue_;
    }
}
