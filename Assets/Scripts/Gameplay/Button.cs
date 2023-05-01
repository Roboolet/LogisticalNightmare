using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] bool enabledOnStart;
    [SerializeField] bool disableAfterPress;
    bool _pressable;
    public bool pressable
    {
        get { return _pressable; }
        set
        {
            _pressable = value;

            if (value)
            {
                Material[] ms = mesh.materials;
                ms[0] = enabledMat;
                mesh.materials = ms;
            }
            else
            {
                Material[] ms = mesh.materials;
                ms[0] = disabledMat;
                mesh.materials = ms;
            }
        }
    }

    public UnityEvent onPress;

    [SerializeField] MeshRenderer mesh;
    [SerializeField] Material disabledMat;
    Material enabledMat;

    private void Awake()
    {
        enabledMat = mesh.materials[0];
        pressable = enabledOnStart;
    }

    public virtual void Press()
    {
        if (pressable)
        {
            onPress?.Invoke();
            if (disableAfterPress) pressable = false;
        }
    }
}
