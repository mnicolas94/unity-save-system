#if ENABLED_ATOMS
using System;
using System.Collections.Generic;
using System.Linq;
using SaveSystem.Attributes;
using SaveSystem.GuidsResolve;
using UnityAtoms;
using UnityEngine;
using Utils.Serializables;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    public abstract class BasePersistentVariablesList<T, P, C, V, E1, E2, F, I, R>
        : ScriptableObject, IPersistentCallbackReceiver, IPersistentResetable
        where P : struct, IPair<T>
        where C : AtomBaseVariable<T>
        where V : AtomVariable<T, P, E1, E2, F>
        where E1 : AtomEvent<T>
        where E2 : AtomEvent<P>
        where F : AtomFunction<T, T>
        where I : AtomVariableInstancer<V, P, T, E1, E2, F>
        where R : AtomReference<T, P, C, V, E1, E2, F, I>
    {
        [SerializeField, DoNotPersist] private SerializableDictionary<V, R> _variables = new();
        [SerializeField, DoNotPersist] private bool _autoSave;
        [SerializeField, HideInInspector] private SerializableDictionary<string, T> _values = new();

        public List<V> Variables => _variables.Keys.ToList();

        private void OnEnable()
        {
            if (_autoSave)
            {
                foreach (var (variable, _) in _variables)
                {
                    variable.Changed.Register(OnVariableChange);
                }
            }
        }

        private void OnDisable()
        {
            if (_autoSave)
            {
                foreach (var (variable, _) in _variables)
                {
                    variable.Changed.Unregister(OnVariableChange);
                }
            }
        }
        
        private async void OnVariableChange()
        {
            if (_autoSave)
            {
                await this.Save();
            }
        }

        public void OnBeforeSave(IGuidResolver guidsDatabase)
        {
            _values.Clear();
            foreach (var (variable, _) in _variables)
            {
                var guid = guidsDatabase.GetGuid(variable);
                _values[guid] = variable.Value;
            }
        }

        public void OnAfterSave()
        {
            
        }

        public void OnBeforeLoad()
        {
            
        }

        public void OnAfterLoad(IGuidResolver guidsDatabase)
        {
            foreach (var (variable, _) in _variables)
            {
                var guid = guidsDatabase.GetGuid(variable);
                if (_values.ContainsKey(guid))
                {
                    variable.Value = _values[guid];
                }
            }
            _values.Clear();
        }

        public void ResetToDefault()
        {
            foreach (var (variable, defaultValue) in _variables)
            {
                variable.Value = defaultValue.Value;
            }
        }
    }

    [Serializable]
    public class VariableDefaultValueTuple<T, P, C, V, E1, E2, F, I, R>
        where P : struct, IPair<T>
        where C : AtomBaseVariable<T>
        where V : AtomVariable<T, P, E1, E2, F>
        where E1 : AtomEvent<T>
        where E2 : AtomEvent<P>
        where F : AtomFunction<T, T>
        where I : AtomVariableInstancer<V, P, T, E1, E2, F>
        where R : AtomReference<T, P, C, V, E1, E2, F, I>
    {
        public AtomVariable<T, P, E1, E2, F> Variable;
        public R DefaultValue;
    }
}
#endif