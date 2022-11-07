using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUpHeaderButton : MMTouchButton
{
    [SerializeField] private bool startsSelected;

    [SerializeField] private Color selectedButtonColor;
    [SerializeField] private Color selectedTextColor;
    [SerializeField] private Color unselectedTextColor;

    [Header("Button Text")]
    [SerializeField] private Text buttonText;

    [Header("Header buttons")]
    [SerializeField] private LevelUpHeaderButton[] headerButtons;

    private bool isInitialised = false;

    protected override void Initialization()
    {
        base.Initialization();
        ReturnToInitialSpriteAutomatically = false;
        isInitialised = true;
    }

    private void Start()
    {
        if (startsSelected)
        {
            SetSelected();

        }
    }

    public override void OnPointerDown(PointerEventData data)
    {
        base.OnPointerDown(data);

        SetSelected();

        foreach (LevelUpHeaderButton headerButton in headerButtons)
        {
            headerButton.ResetAppearance();
        }
    }

    public void ResetAppearance()
    {
        if (isInitialised)
        {
            _image.color = _initialColor;
            buttonText.color = unselectedTextColor;
        }
    }

    public void SetSelected()
    {
        if (isInitialised)
        {
            _image.color = selectedButtonColor;
            buttonText.color = selectedTextColor;
        }        
    }
}
