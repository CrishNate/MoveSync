using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class EditorGrid : MonoBehaviour
{
    [SerializeField] private RectTransform _viewport;
    [SerializeField] private RectTransform _timeline;
    [SerializeField] private GameObject _UIGrid;

    private float _offset;
    private List<GameObject> _UIGrids = new List<GameObject>();
    private float _startOffset = 0.0f;
    
    public void UpdateOffsetGrid()
    {
        _startOffset = -_timeline.localPosition.x;
        
        UpdateGrid();
    }

    void UpdateGrid()
    {
        float offsetFromStart = Mathf.Floor(_startOffset / _offset) * _offset;

        for (int i = 0; i < _UIGrids.Count; i++)
        {
            GameObject grid = _UIGrids[i];
            RectTransform rectTransform = grid.GetComponent<RectTransform>();

            Vector3 position = rectTransform.localPosition;
            position.x = i * _offset + offsetFromStart;
            rectTransform.localPosition = position;

            foreach (var text in grid.GetComponentsInChildren<Text>())
            {
                // some gay shit
                text.text = (i + Mathf.Floor(_startOffset / _offset)).ToString();
            }
        }
    }
    
    public void SetGridParams(float offset)
    {
        _offset = offset;

        int requireCount = Mathf.FloorToInt(_viewport.rect.width / _offset) + 1;
        int count = Mathf.Max(_UIGrids.Count, requireCount);
        
        for (int i = 0; i < count; i++)
        {
            if (_UIGrids.Count <= i)
            {
                GameObject grid = Instantiate(_UIGrid, gameObject.transform);
                _UIGrids.Add(grid);
            }
            else if (i >= requireCount)
            {
                Destroy(_UIGrids[i]);
            }
        }

        int diff = (_UIGrids.Count - requireCount);
        if (diff > 0) _UIGrids.RemoveRange(requireCount, diff);

        UpdateGrid();
    }
}
