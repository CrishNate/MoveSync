using System;
using System.Collections;
using System.Collections.Generic;
using MoveSync;
using UnityEngine;

public struct BindKey
{
    public KeyCode key;
    public PropertyName objectTag;
    public int layer;
}

public class BindingManager : MonoBehaviour
{
    private List<BindKey> _bind = new List<BindKey>();

    void AddKeyBind(BindKey bind)
    {
        _bind.Add(bind);
    }

    private void Start()
    {
        AddKeyBind(new BindKey
        {
            key = KeyCode.H,
            layer = 1,
            objectTag = "test_item"
        });
        AddKeyBind(new BindKey
        {
            key = KeyCode.K,
            layer = 2,
            objectTag = "test_item"
        });
    }

    private void Update()
    {
        foreach (var bind in _bind)
        {
            if (Input.GetKeyDown(bind.key))
            {
                LevelDataManager.instance.NewBeatObjectAtMarker(bind.objectTag, bind.layer);
            }
        }
    }
}
