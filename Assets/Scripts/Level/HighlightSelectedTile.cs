using System.Linq;
using Runtime.Extensions;
using Slothsoft.UnityExtensions;
using TMPro;
using UnityEngine;

namespace Runtime.Level {
    public class HighlightSelectedTile : MonoBehaviour {
        [Header("Shading")]
        [SerializeField, Range(0, 255)]
        int defaultEmission = 0;
        [SerializeField, Range(0, 255)]
        int selectedEmission = 1;

        [Header("Tooltip")]
        [SerializeField]
        Vector2 tooltipOffset = Vector2.zero;
        [SerializeField, Expandable]
        CanvasGroup tooltipPanel = default;
        [SerializeField, Expandable]
        TextMeshProUGUI tooltipText = default;

        [SerializeField, Range(0, 10)]
        float tooltipAppearDuration = 1;
        [SerializeField, Range(0, 10)]
        float tooltipDisappearDuration = 1;

        ITile m_currentlySelectedTile;
        ITile currentlySelectedTile {
            get => m_currentlySelectedTile;
            set {
                if (m_currentlySelectedTile != value) {
                    if (m_currentlySelectedTile != null) {
                        m_currentlySelectedTile.emission = defaultEmission;
                    }
                    m_currentlySelectedTile = value;
                    if (m_currentlySelectedTile != null) {
                        m_currentlySelectedTile.emission = selectedEmission;
                    }
                    UpdateTooltip();
                }
            }
        }

        protected void OnValidate() {
            if (!tooltipPanel) {
                transform.TryGetComponentInChildren(out tooltipPanel);
            }
            if (!tooltipText) {
                transform.TryGetComponentInChildren(out tooltipText);
            }
        }
        protected void OnEnable() {
            WorldInput.onSelect += HandleSelect;
        }
        protected void OnDisable() {
            WorldInput.onSelect -= HandleSelect;
        }

        void HandleSelect(Vector3 position) {
            var gridPosition = World.instance.WorldToGrid(position);
            if (World.instance.TryGetTile(gridPosition, out var tile)) {
                currentlySelectedTile = tile;
            } else {
                currentlySelectedTile = null;
            }
        }
        void UpdateTooltip() {
            LeanTween.cancel(tooltipPanel.gameObject);
            if (currentlySelectedTile == null) {
                LeanTween.alphaCanvas(tooltipPanel, 0, tooltipDisappearDuration);
            } else {
                LeanTween.alphaCanvas(tooltipPanel, 1, tooltipAppearDuration);
                tooltipText.text = $"Terrain: <b>{currentlySelectedTile.type.name}</b>\r\n"
                    + $"Owner: <i>{ currentlySelectedTile.ownerCell.owningFaction}</i>\r\n"
                    + $"Entities: {string.Join(", ", currentlySelectedTile.ownerCell.entities.Select(entity => entity.type.name))} \r\n"
                    + $"Influence: <i>{m_currentlySelectedTile.ownerCell.owningFaction} { System.Math.Abs(m_currentlySelectedTile.ownerCell.influence)}</i>";
            }
        }
    }
}