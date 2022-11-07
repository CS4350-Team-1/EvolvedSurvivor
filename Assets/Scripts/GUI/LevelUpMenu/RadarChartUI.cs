using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TeamOne.EvolvedSurvivor
{
    public class RadarChartUI : MonoBehaviour
    {
        private const string TITLE_TRAIT_DAMAGE = "Damage: ";
        private const string TITLE_TRAIT_UPTIME = "Uptime:\n";
        private const string TITLE_TRAIT_AOE = "AOE:\n";
        private const string TITLE_TRAIT_QUANTITY = "Quantity:\n";
        private const string TITLE_TRAIT_UTILITY = "Utility:\n";

        [SerializeField] private List<CanvasRenderer> radarMeshCanvasRenderers;
        [SerializeField] private Texture2D radarTexture2D;
        [SerializeField] private List<Material> radarMaterials;
        [SerializeField] private GameObject notAvailableBox;

        [SerializeField] protected Text rcDamageText;
        [SerializeField] protected Text rcUptimeText;
        [SerializeField] protected Text rcAoeText;
        [SerializeField] protected Text rcQuantityText;
        [SerializeField] protected Text rcUtilityText;

        [SerializeField] protected Color defaultColor;
        [SerializeField] protected Color statIncreaseColor;
        [SerializeField] protected Color statDecreaseColor;

        public void UpdateVisual(Upgradable upgradable)
        {
            if (upgradable.IsAbility())
            {
                TraitChart traitChart = upgradable.ConvertTo<Ability>().GetTraitChart();
                DrawChart(traitChart, radarMeshCanvasRenderers[0], radarMaterials[0], radarTexture2D);
                UpdateLabels(traitChart);
            } 
            else
            {
                notAvailableBox.SetActive(true);
            }
        }

        public void UpdateVisual(Upgradable original, Upgradable upgraded)
        {
            if (original.IsAbility() && upgraded.IsAbility())
            {
                TraitChart originalTraitChart = original.ConvertTo<Ability>().GetTraitChart();
                TraitChart upgradedTraitChart = upgraded.ConvertTo<Ability>().GetTraitChart();
                DrawChart(originalTraitChart, radarMeshCanvasRenderers[0], radarMaterials[0], radarTexture2D);
                DrawChart(upgradedTraitChart, radarMeshCanvasRenderers[1], radarMaterials[1], radarTexture2D);
                UpdateLabels(originalTraitChart, upgradedTraitChart);

            }
            else
            {
                Debug.LogWarning("The original or upgraded is not an ability");
                notAvailableBox.SetActive(true);
            }
        }

        public void ClearVisual() 
        {
            // This method should remove the mesh from the radarChart 
            radarMeshCanvasRenderers.ForEach(x => x.SetMesh(null));

            notAvailableBox.SetActive(false);
            ClearLabels();
        }

        private void DrawChart(TraitChart traitChart, CanvasRenderer renderer, Material material, Texture2D texture)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[6];
            Vector2[] uv = new Vector2[6];
            int[] triangles = new int[3 * 5]; // 3 vertices for 5 triangles

            float angleIncrement = 360f / 5;
            float radarChartSize = 120f; // Length from origin point to a vertex

            // Set vertex according to their angle, size of the background image, and the value ratio
            Vector3 damageVertex = Quaternion.Euler(0, 0, -angleIncrement * 0) * Vector3.up * radarChartSize * traitChart.DamageRatio;
            Vector3 uptimeVertex = Quaternion.Euler(0, 0, -angleIncrement * 1) * Vector3.up * radarChartSize * traitChart.UptimeRatio;
            Vector3 aoeVertex = Quaternion.Euler(0, 0, -angleIncrement * 2) * Vector3.up * radarChartSize * traitChart.AoeRatio;
            Vector3 quantityVertex = Quaternion.Euler(0, 0, -angleIncrement * 3) * Vector3.up * radarChartSize * traitChart.QuantityRatio;
            Vector3 utilityVertex = Quaternion.Euler(0, 0, -angleIncrement * 4) * Vector3.up * radarChartSize * traitChart.UtilityRatio;

            int damageVertexIndex = 1;
            int uptimeVertexIndex = 2;
            int aoeVertexIndex = 3;
            int quantityVertexIndex = 4;
            int utilityVertexIndex = 5;

            // Vertices for each trait
            vertices[0] = Vector3.zero;
            vertices[damageVertexIndex] = damageVertex;
            vertices[uptimeVertexIndex] = uptimeVertex;
            vertices[aoeVertexIndex] = aoeVertex;
            vertices[quantityVertexIndex] = quantityVertex;
            vertices[utilityVertexIndex] = utilityVertex;

            // Triangle between Damage and Uptime
            triangles[0] = 0;
            triangles[1] = damageVertexIndex;
            triangles[2] = uptimeVertexIndex;

            // Triangle between Uptime and Aoe
            triangles[3] = 0;
            triangles[4] = uptimeVertexIndex;
            triangles[5] = aoeVertexIndex;

            // Triangle between Aoe and Quantity
            triangles[6] = 0;
            triangles[7] = aoeVertexIndex;
            triangles[8] = quantityVertexIndex;

            // Triangle between Quantity and Utility
            triangles[9] = 0;
            triangles[10] = quantityVertexIndex;
            triangles[11] = utilityVertexIndex;

            // Triangle between Utility and Damage
            triangles[12] = 0;
            triangles[13] = utilityVertexIndex;
            triangles[14] = damageVertexIndex;

            // For mesh texture 
            uv[0] = Vector2.zero;
            uv[damageVertexIndex] = Vector2.one;
            uv[uptimeVertexIndex] = Vector2.one;
            uv[aoeVertexIndex] = Vector2.one;
            uv[quantityVertexIndex] = Vector2.one;
            uv[utilityVertexIndex] = Vector2.one;

            // Set mesh
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            renderer.SetMesh(mesh);
            renderer.SetMaterial(material, texture);
        }

        private void UpdateLabels(TraitChart traitChart)
        {
            rcDamageText.text = TITLE_TRAIT_DAMAGE + $"{traitChart.damage:0.0}";
            rcDamageText.color = defaultColor;
            rcUptimeText.text = TITLE_TRAIT_UPTIME + $"{traitChart.uptime:0.0}";
            rcUptimeText.color = defaultColor;
            rcAoeText.text = TITLE_TRAIT_AOE + $"{traitChart.aoe:0.0}";
            rcAoeText.color = defaultColor;
            rcQuantityText.text = TITLE_TRAIT_QUANTITY + $"{traitChart.quantity:0.0}";
            rcQuantityText.color = defaultColor;
            rcUtilityText.text = TITLE_TRAIT_UTILITY + $"{traitChart.utility:0.0}";
            rcUtilityText.color = defaultColor;
        }

        private void UpdateLabels(TraitChart original, TraitChart upgraded)
        {
            UpdateLabels(upgraded);

            // damage
            bool damageChanged = original.damage != upgraded.damage;
            if (damageChanged)
            {
                rcDamageText.color = upgraded.damage > original.damage ? statIncreaseColor : statDecreaseColor;
            }

            // uptime
            bool uptimeChanged = original.uptime != upgraded.uptime;
            if (uptimeChanged)
            {
                rcUptimeText.color = upgraded.uptime > original.uptime ? statIncreaseColor : statDecreaseColor;
            }

            // aoe
            bool aoeChanged = original.aoe != upgraded.aoe;
            if (aoeChanged)
            {
                rcAoeText.color = upgraded.aoe > original.aoe ? statIncreaseColor : statDecreaseColor;
            }

            // quantity
            bool quantityChanged = original.quantity != upgraded.quantity;
            if (quantityChanged)
            {
                rcQuantityText.color = upgraded.quantity > original.quantity ? statIncreaseColor : statDecreaseColor;
            }

            // utility
            bool utilityChanged = original.utility != upgraded.utility;
            if (utilityChanged)
            {
                rcUtilityText.color = upgraded.utility > original.utility ? statIncreaseColor : statDecreaseColor;
            }
        }

        private void ClearLabels()
        {
            rcDamageText.text = TITLE_TRAIT_DAMAGE;
            rcDamageText.color = defaultColor;
            rcUptimeText.text = TITLE_TRAIT_UPTIME;
            rcUptimeText.color = defaultColor;
            rcAoeText.text = TITLE_TRAIT_AOE;
            rcAoeText.color = defaultColor;
            rcQuantityText.text = TITLE_TRAIT_QUANTITY;
            rcQuantityText.color = defaultColor;
            rcUtilityText.text = TITLE_TRAIT_UTILITY;
            rcUtilityText.color = defaultColor;
        }
    }
}
