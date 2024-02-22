using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrollController : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private float scrollAmount = 0.1f;

    private void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    public void ArrowLeft()
    {
        ScrollHorizontally(-scrollAmount);
    }

    public void ArrowRight()
    {
        ScrollHorizontally(scrollAmount);
    }

    void ScrollHorizontally(float amount)
    {
        _scrollRect.horizontalNormalizedPosition += amount;

        _scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(_scrollRect.horizontalNormalizedPosition);
    }
}
