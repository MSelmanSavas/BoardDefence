using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainCanvas_Gameplay : MonoBehaviour
{
    [field: SerializeField]
    public List<UIElement_Base> GameplayUIElements { get; private set; } = new();

    private void OnEnable()
    {
        RefBook.Add(this);
    }

    private void OnDisable()
    {
        RefBook.Remove(this);
    }

    public void InitializeGameplayUIElements()
    {
        foreach (var uiElement in GameplayUIElements)
        {
            if (uiElement == null)
                continue;

            uiElement.Initialize();
        }
    }

#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button]
    void FindAllUIElementsOnChildren()
    {
        GameplayUIElements = GetComponentsInChildren<UIElement_Base>().ToList();
    }
#endif
}
