using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This button is to switch between Radar Chart and Detailed View 
public class AbilityDetailsViewButton : UIButton
{
    [SerializeField] GameObject radarChart;
    [SerializeField] GameObject detailedView;
    [SerializeField] Image changeViewButton;
    [SerializeField] Sprite toDetailsIcon;
    [SerializeField] Sprite toRadarChartIcon;

    bool onRadarChart = true;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onRadarChart) {
            radarChart.SetActive(false);
            detailedView.SetActive(true);
            changeViewButton.sprite = toRadarChartIcon;
            onRadarChart = false;
        } else {
            radarChart.SetActive(true);
            detailedView.SetActive(false);
            changeViewButton.sprite = toDetailsIcon;
            onRadarChart = true;           
        }

        clickSfxHandler.PlaySfx();
    }
}
