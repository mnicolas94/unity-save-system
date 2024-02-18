using System.Collections.Generic;
using SaveSystem.Attributes;
using SaveSystem.GuidsResolve;
using UnityAtoms;
using UnityEngine;
using Utils.Serializables;

namespace SaveSystem.Runtime.Integrations.UnityAtoms
{
    public abstract class BasePersistentVariablesList<T, P, E1, E2, F> : ScriptableObject, IPersistentCallbackReceiver
        where P : struct, IPair<T>
        where E1 : AtomEvent<T>
        where E2 : AtomEvent<P>
        where F : AtomFunction<T, T>
    {
        [SerializeField, DoNotPersist] private List<AtomVariable<T, P, E1, E2, F>> _variables;
        [SerializeField, DoNotPersist] private bool _autoSave;
        [SerializeField] private SerializableDictionary<string, T> _values;
        

        private void OnEnable()
        {
            if (_autoSave)
            {
                _variables.ForEach(variable => variable.Changed.Register(OnVariableChange));
            }
        }

        private void OnDisable()
        {
            if (_autoSave)
            {
                _variables.ForEach(variable => variable.Changed.Unregister(OnVariableChange));
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
            foreach (var variable in _variables)
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
            foreach (var variable in _variables)
            {
                var guid = guidsDatabase.GetGuid(variable);
                if (_values.ContainsKey(guid))
                {
                    variable.Value = _values[guid];
                }
            }
            _values.Clear();
        }
    }
}