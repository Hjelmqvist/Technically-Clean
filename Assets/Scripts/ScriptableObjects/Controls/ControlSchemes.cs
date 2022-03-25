using System;
using UnityEngine;

public class ControlSchemes : ScriptableObject
{
    [Serializable]
    public struct ControlScheme
    {
        public string id;
        public string interact;
        public string throwItem;
    }

    [SerializeField] private ControlScheme[] schemes;

    public ControlScheme GetScheme(string id)
    {
        foreach (ControlScheme scheme in schemes)
        {
            if (scheme.id == id)
                return scheme;
        }

        return schemes[0];
    }

    public string GetId(bool controller)
    {
        int index = controller ? 1 : 0;
        return schemes[index].id;
    }
}
