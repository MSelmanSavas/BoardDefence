using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIElement_DefenceItemSelectionButton : UIElement_Base
{
    [SerializeField]
    TypeReferenceInheritedFrom<DefenceItemBase> _defenceItemTypeToShow;

    [SerializeField]
    Image _defenceItemVisual;

    [SerializeField]
    TextMeshProUGUI _defenceItemCountText;

    [SerializeField]
    Image _defenceItemSelectedIndicator;

    [SerializeField]
    Button _selectionButton;

    DefenceItemPlacementSystem _defenceItemPlacementSystem;
    ConfigUnityEntitiesContainer _entityContainer;

    public override void Initialize()
    {
        base.Initialize();

        if (!RefBook.TryGet(out _defenceItemPlacementSystem))
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize : {nameof(UIElement_DefenceItemSelectionButton)}! There is no : {nameof(DefenceItemPlacementSystem)} found!", this);
            return;
        }

        if (!RefBook.TryGet(out Configurer configurer))
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize : {nameof(UIElement_DefenceItemSelectionButton)}! There is no : {nameof(Configurer)} found!", this);
            return;
        }

        if (!configurer.TryGetConfig(out _entityContainer))
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize : {nameof(UIElement_DefenceItemSelectionButton)}! There is no : {nameof(ConfigUnityEntitiesContainer)} found!", this);
            return;
        }

        InitializeVisualsAndCounts(_entityContainer, _defenceItemPlacementSystem);
        InitializeButtons(_defenceItemPlacementSystem);
    }

    void InitializeVisualsAndCounts(ConfigUnityEntitiesContainer entitiesContainer, DefenceItemPlacementSystem defenceItemPlacementSystem)
    {
        if (!entitiesContainer.TryGetUnityEntityData(_defenceItemTypeToShow, out UnityEntityData data))
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize : {nameof(UIElement_DefenceItemSelectionButton)}! There is no : {_defenceItemTypeToShow} type data found in entity container!", this);
            return;
        }

        int leftOverCount = defenceItemPlacementSystem.GetDefenceItemLeftCountByType(_defenceItemTypeToShow.Type);

        _defenceItemVisual.sprite = data.BasicVisual;
        _defenceItemCountText?.SetText(leftOverCount.ToString());
        _defenceItemSelectedIndicator.gameObject.SetActive(false);
    }

    void InitializeButtons(DefenceItemPlacementSystem defenceItemPlacementSystem)
    {
        if (_selectionButton == null)
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize : {nameof(UIElement_DefenceItemSelectionButton)}! There is no button assigned to connect selection functions!", this);
            return;
        }

        _selectionButton.onClick.AddListener(() => TrySelectDefenceItem(defenceItemPlacementSystem));
        defenceItemPlacementSystem.OnDefenceItemSelectionChange += OnDefenceItemSelectionChange;
        defenceItemPlacementSystem.OnDefenceItemSpawned += OnDefenceItemSpawned;
    }

    public override void DeInitialize()
    {
        base.DeInitialize();
        
        if (_defenceItemPlacementSystem != null)
        {
            _defenceItemPlacementSystem.OnDefenceItemSelectionChange -= OnDefenceItemSelectionChange;
            _defenceItemPlacementSystem.OnDefenceItemSpawned -= OnDefenceItemSpawned;
        }
    }

    protected override void OnDisableInternal()
    {
        DeInitialize();
    }

    void TrySelectDefenceItem(DefenceItemPlacementSystem defenceItemPlacementSystem)
    {
        if (defenceItemPlacementSystem.GetCurrentSelectedDefenceItemType() == _defenceItemTypeToShow)
        {
            UnSelectDefenceItem(defenceItemPlacementSystem);
            return;
        }

        if (!defenceItemPlacementSystem.TrySelectDefenceItemToPlaceByType(_defenceItemTypeToShow.Type))
            return;

        _defenceItemSelectedIndicator.gameObject.SetActive(true);
    }

    void UnSelectDefenceItem(DefenceItemPlacementSystem defenceItemPlacementSystem)
    {
        defenceItemPlacementSystem.DeSelectDefenceItemToPlace();
        _defenceItemSelectedIndicator.gameObject.SetActive(false);
    }

    void OnDefenceItemSelectionChange(DefenceItemChangeData defenceItemChangeData)
    {
        if (defenceItemChangeData.CurrentType == _defenceItemTypeToShow)
            return;

        _defenceItemSelectedIndicator.gameObject.SetActive(false);
    }

    void OnDefenceItemSpawned(DefenceItemSpawnData defenceItemSpawnData)
    {
        if (defenceItemSpawnData.ItemType != _defenceItemTypeToShow)
            return;

        _defenceItemCountText?.SetText(defenceItemSpawnData.CurrentCount.ToString());

        if (defenceItemSpawnData.CurrentCount > 0)
            return;

        UnSelectDefenceItem(_defenceItemPlacementSystem);
    }
}
