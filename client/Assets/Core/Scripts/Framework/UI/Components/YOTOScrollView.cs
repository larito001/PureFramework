using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class YOTOScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum CenterMode
    {
        None,
        Horizontal,
        Vertical,
        Both
    }

    public enum LayoutType
    {
        Vertical,
        Horizontal
    }

    [Header("Layout Settings")] [SerializeField]
    private LayoutType layout = LayoutType.Vertical;

    [SerializeField] private int columns = 5; // Used when layout is Vertical
    [SerializeField] private int rows = 1; // Used when layout is Horizontal

    [Header("Center Settings")] [SerializeField]
    private CenterMode centerMode = CenterMode.None;
    [Header("Layout Padding")]
    [SerializeField] private RectOffset padding ;

    [Header("References")] [SerializeField]
    private RectTransform content;

    [SerializeField] private RectTransform viewport;
    [SerializeField] private GameObject itemPrefab;

    [Header("Item Settings")] private float itemWidth = 100f;
    private float itemHeight = 100f;
    [SerializeField] private int spacing = 5;

    private List<YOTOScrollViewItem> itemPool;

    // private IList rawDataList;
    private HashSet<int> visibleIndices;
    private int dataCount = 0;
    private float contentWidth;
    private float contentHeight;
    private int poolSize;
    private bool isStatic = false;

    // 泛型回调（不再使用 DynamicInvoke）
    private Action<YOTOScrollViewItem, int> renderAction;

    // 缓存 Dictionary，避免 GC
    private Dictionary<int, YOTOScrollViewItem> currentVisible;

    // For vertical
    private int startRow, endRow;

    // For horizontal
    private int startCol, endCol;

    private Vector2 lastDragPosition;
    private bool isDragging;
    private Vector2 lastContentPosition;

    private void Awake()
    {
        if (padding == null)
            padding = new RectOffset(0, 0, 0, 0);
        itemPool = new List<YOTOScrollViewItem>();
        visibleIndices = new HashSet<int>();
        currentVisible = new Dictionary<int, YOTOScrollViewItem>();

        if (!TryGetComponent<Image>(out var img))
        {
            img = gameObject.AddComponent<Image>();
            img.color = Color.clear;
        }

        img.raycastTarget = true;

        if (content == null || viewport == null)
        {
            Debug.LogError("Content or Viewport not assigned!");
            enabled = false;
            return;
        }

        // Anchor content top-left
        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = new Vector2(0, 1);
        content.pivot = new Vector2(0, 1);
        content.anchoredPosition = Vector2.zero;

        // Stretch viewport
        viewport.anchorMin = Vector2.zero;
        viewport.anchorMax = Vector2.one;
    }

    private void Start() => Canvas.ForceUpdateCanvases();

    public void Initialize(int poolSize = 10, bool isStatic = false)
    {
        this.isStatic = isStatic;
        this.poolSize = poolSize;
        foreach (var it in itemPool)
            if (it)
                Destroy(it.gameObject);
        itemPool.Clear();

        itemWidth = itemPrefab.GetComponent<RectTransform>().rect.width;
        itemHeight = itemPrefab.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < poolSize; i++)
        {
            var go = Instantiate(itemPrefab, content);
            var item = go.GetComponent<YOTOScrollViewItem>();
            if (!item)
            {
                Debug.LogError("Prefab needs YOTOScrollViewItem component!");
                return;
            }

            item.gameObject.SetActive(false);
            itemPool.Add(item);
        }
    }

    public void SetRenderer(Action<YOTOScrollViewItem, int> onRender)
    {
        renderAction = (item, obj) => onRender(item, obj);
    }

    public void SetData(int count)
    {
        dataCount = Mathf.Max(0, count);
        if (content == null || viewport == null) return;
        if (itemPool == null || itemPool.Count == 0)
        {
            Debug.LogError("Initialize pool first.");
            return;
        }

        foreach (var it in itemPool)
        {
            if (it.gameObject.activeSelf)
            {
                it.OnHidItem();
                it.gameObject.SetActive(false);
            }
        }
        visibleIndices.Clear();
        currentVisible.Clear();

        columns = Mathf.Max(1, columns);
        rows = Mathf.Max(1, rows);

        if (dataCount == 0)
        {
            content.sizeDelta = Vector2.zero;
            content.anchoredPosition = Vector2.zero;
            startRow = endRow = startCol = endCol = 0;
            return;
        }

        int totalRows, totalCols;
        if (layout == LayoutType.Vertical)
        {
            totalCols = columns;
            totalRows = Mathf.CeilToInt((float)dataCount / totalCols);
        }
        else
        {
            totalRows = rows;
            totalCols = Mathf.CeilToInt((float)dataCount / totalRows);
        }

        // 加上 padding
        contentWidth  = totalCols > 0 ? totalCols * itemWidth  + (totalCols - 1) * spacing + padding.left + padding.right : 0f;
        contentHeight = totalRows > 0 ? totalRows * itemHeight + (totalRows - 1) * spacing + padding.top  + padding.bottom : 0f;

        content.sizeDelta = new Vector2(contentWidth, contentHeight);

        ClampContentPosition();
        RefreshItems();
    }


    private void RefreshItems()
{
    if (renderAction == null) return;

    if (dataCount <= 0)
    {
        foreach (var it in itemPool)
        {
            if (it.gameObject.activeSelf)
            {
                it.OnHidItem();
                it.gameObject.SetActive(false);
            }
        }
        visibleIndices.Clear();
        currentVisible.Clear();
        return;
    }

    columns = Mathf.Max(1, columns);
    rows = Mathf.Max(1, rows);

    currentVisible.Clear();

    if (layout == LayoutType.Vertical)
    {
        float scrollY = content.anchoredPosition.y;
        float viewH = viewport.rect.height;
        float rowH = itemHeight + spacing;

        startRow = Mathf.FloorToInt(scrollY / rowH);
        int visRows = Mathf.CeilToInt(viewH / rowH) + 1;
        endRow = startRow + visRows;

        int maxRow = Mathf.CeilToInt((float)dataCount / columns) - 1;
        maxRow = Mathf.Max(-1, maxRow);
        startRow = Mathf.Clamp(startRow, 0, Mathf.Max(0, maxRow));
        endRow   = Mathf.Clamp(endRow,   0, Mathf.Max(0, maxRow));

        foreach (var it in itemPool)
        {
            if (it.gameObject.activeSelf)
            {
                int idx = it.DataIndex;
                int r = idx / columns;
                if (r < startRow || r > endRow)
                {
                    it.OnHidItem();
                    it.gameObject.SetActive(false);
                    visibleIndices.Remove(idx);
                }
                else currentVisible[idx] = it;
            }
        }

        for (int r = startRow; r <= endRow; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int idx = r * columns + c;
                if (idx >= dataCount) break;

                // 加上 padding
                float x = padding.left + c * (itemWidth + spacing) + itemWidth * 0.5f;
                float y = -padding.top - r * (itemHeight + spacing) - itemHeight * 0.5f;

                if (currentVisible.TryGetValue(idx, out var exist))
                {
                    exist.transform.localPosition = new Vector3(x, y, 0);
                    continue;
                }

                var ni = GetFreeItem();
                if (ni == null) continue;
                ni.transform.localPosition = new Vector3(x, y, 0);
                ni.DataIndex = idx;
                ni.gameObject.SetActive(true);
                ni.OnRenderItem();
                renderAction(ni, idx);
                visibleIndices.Add(idx);
            }
        }
    }
    else // Horizontal
    {
        float scrollX = -content.anchoredPosition.x;
        float viewW = viewport.rect.width;
        float colW = itemWidth + spacing;

        startCol = Mathf.FloorToInt(scrollX / colW);
        int visCols = Mathf.CeilToInt(viewW / colW) + 1;
        endCol = startCol + visCols;

        int maxCol = Mathf.CeilToInt((float)dataCount / rows) - 1;
        maxCol = Mathf.Max(-1, maxCol);
        startCol = Mathf.Clamp(startCol, 0, Mathf.Max(0, maxCol));
        endCol   = Mathf.Clamp(endCol,   0, Mathf.Max(0, maxCol));

        foreach (var it in itemPool)
        {
            if (it.gameObject.activeSelf)
            {
                int idx = it.DataIndex;
                int c = idx / rows;
                if (c < startCol || c > endCol)
                {
                    it.OnHidItem();
                    it.gameObject.SetActive(false);
                    visibleIndices.Remove(idx);
                }
                else currentVisible[idx] = it;
            }
        }

        for (int c = startCol; c <= endCol; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                int idx = c * rows + r;
                if (idx >= dataCount) break;

                // 加上 padding
                float x = padding.left + c * (itemWidth + spacing) + itemWidth * 0.5f;
                float y = -padding.top - r * (itemHeight + spacing) - itemHeight * 0.5f;

                if (currentVisible.TryGetValue(idx, out var exist))
                {
                    exist.transform.localPosition = new Vector3(x, y, 0);
                    continue;
                }

                var ni = GetFreeItem();
                if (ni == null) continue;
                ni.transform.localPosition = new Vector3(x, y, 0);
                ni.DataIndex = idx;
                ni.gameObject.SetActive(true);
                ni.OnRenderItem();
                renderAction(ni, idx);
                visibleIndices.Add(idx);
            }
        }
    }
}



    private YOTOScrollViewItem GetFreeItem()
    {
        foreach (var it in itemPool)
            if (!it.gameObject.activeSelf)
                return it;
        Debug.LogWarning($"Pool shortage (size={itemPool.Count})");
        return null;
    }

    public void OnBeginDrag(PointerEventData ev)
    {
        if (isStatic) return;
        isDragging = true;
        lastDragPosition = ev.position;
    }

    public void OnDrag(PointerEventData ev)
    {
        if (isStatic) return;
        if (!isDragging) return;
        Vector2 delta = ev.position - lastDragPosition;
        if (layout == LayoutType.Vertical)
            content.anchoredPosition += new Vector2(0, delta.y);
        else
            content.anchoredPosition += new Vector2(delta.x, 0);

        ClampContentPosition();
        RefreshItems();
        lastDragPosition = ev.position;
    }

    public void OnEndDrag(PointerEventData ev)
    {
        if (isStatic) return;
        isDragging = false;
    }

    private void ClampContentPosition()
    {
        Vector2 pos = content.anchoredPosition;

        if (layout == LayoutType.Vertical)
        {
            float maxY = Mathf.Max(0, contentHeight - viewport.rect.height);
            pos.y = Mathf.Clamp(pos.y, 0, maxY);

            // 纵向居中
            if ((centerMode == CenterMode.Vertical || centerMode == CenterMode.Both)
                && contentHeight < viewport.rect.height)
            {
                pos.y = -(viewport.rect.height - contentHeight) * 0.5f;
            }

            // 横向居中
            if ((centerMode == CenterMode.Horizontal || centerMode == CenterMode.Both)
                && contentWidth < viewport.rect.width)
            {
                pos.x = (viewport.rect.width - contentWidth) * 0.5f;
            }
        }
        else // Horizontal
        {
            float maxX = Mathf.Max(0, contentWidth - viewport.rect.width);
            pos.x = Mathf.Clamp(pos.x, -maxX, 0);

            // 横向居中
            if ((centerMode == CenterMode.Horizontal || centerMode == CenterMode.Both)
                && contentWidth < viewport.rect.width)
            {
                pos.x = (viewport.rect.width - contentWidth) * 0.5f;
            }

            // 纵向居中
            if ((centerMode == CenterMode.Vertical || centerMode == CenterMode.Both)
                && contentHeight < viewport.rect.height)
            {
                pos.y = -(viewport.rect.height - contentHeight) * 0.5f;
            }
        }

        content.anchoredPosition = pos;
    }


    private void Update()
    {
        if (!isStatic && content.anchoredPosition != lastContentPosition)
        {
            lastContentPosition = content.anchoredPosition;
            RefreshItems();
        }
    }
}