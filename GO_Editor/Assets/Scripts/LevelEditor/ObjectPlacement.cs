using System;
using UniRx;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject stoneIndicatorPref;
    [SerializeField] private GameObject mineIndicatorPref;
    [SerializeField] private GameObject keyIndicatorPref;
    [SerializeField] private GameObject mapIndicatorPref;
    [SerializeField] private GameObject cutterIndicatorPref;
    [SerializeField] private GameObject contextMenuPref;
    [SerializeField] private GameObject keyContextMenuPref;

    private ObjectSelector selector;
    private IDisposable routine;
    #endregion

    private void Awake()
    {
        selector = GetComponent<ObjectSelector>();

        LevelInitializer.StartAddObjEvent.AddListener(() =>
        {
            if (routine != null)
                routine.Dispose();
        });
    }

    public void PlaceObject(int objectType)
    {
        LevelInitializer.StartAddObjEvent.Invoke();

        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select node to place object"))
            .Subscribe(_ =>
            {
                PlaceObject((NodeType)objectType, selector.Nodes[0].transform);
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    public void PlaceObject(NodeType type, Transform parent)
    {
        var indicator = GetIndicator(type, parent);
        if (indicator == null)
        {
            Debug.LogWarning("Indicator pref is lost");
            return;
        }

        GameObject contextMenu = null;

        if (type == NodeType.Key) contextMenu = Instantiate(keyContextMenuPref, parent);
        else contextMenu = Instantiate(contextMenuPref, parent);

        contextMenu.GetComponent<ContextMenu_Obj>().Initialize(indicator);
        parent.GetComponent<Board_Node>().Type = type;
    }

    private GameObject GetIndicator(NodeType type, Transform _parent)
    {
        switch (type)
        {
            case NodeType.Mine:
                 return Instantiate(mineIndicatorPref, _parent);
            case NodeType.Stone:
                return Instantiate(stoneIndicatorPref, _parent);
            case NodeType.Cutter:
                return Instantiate(cutterIndicatorPref, _parent);
            case NodeType.Key:
                return Instantiate(keyIndicatorPref, _parent);
            case NodeType.Map:
                return Instantiate(mineIndicatorPref, _parent);
        }
        return null;
    }

    public void DeleteObject(Board_Node node)
    {
        var indicator = FindObjectIndicator(node.transform);
        if (indicator != null) Destroy(indicator);

        for (int i = 0; i < node.transform.childCount; i++)
        {
            var child = node.transform.GetChild(i);
            if (child.GetComponent<ContextMenu>())
                Destroy(child.gameObject);
        }

        if (node.Type == NodeType.Key) node.GetComponent<KeyIndex>().Index = 0;
        node.Type = NodeType.Default;
    }

    private GameObject FindObjectIndicator(Transform target)
    {
        for (int i = 0; i < target.childCount; i++)
        {
            if (target.GetChild(i).CompareTag("Indicator"))
            {
                var indicator = target.GetChild(i).gameObject;
                return indicator;
            }
        }
        return null;
    }
}
