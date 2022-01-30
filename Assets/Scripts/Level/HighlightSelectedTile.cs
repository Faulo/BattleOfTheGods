using UnityEngine;

namespace Runtime.Level {
    public class HighlightSelectedTile : MonoBehaviour {
        protected void OnEnable() {
            WorldInput.onSelect += HandleSelect;
        }
        protected void OnDisable() {
            WorldInput.onSelect -= HandleSelect;
        }

        [SerializeField, Range(0, 255)]
        int defaultEmission = 0;
        [SerializeField, Range(0, 255)]
        int selectedEmission = 1;

        ITile m_currentlySelectedTile;
        public ITile currentlySelectedTile {
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
                }
            }
        }

        void HandleSelect(Vector3 position) {
            var gridPosition = World.instance.WorldToGrid(position);
            if (World.instance.TryGetTile(gridPosition, out var tile)) {
                currentlySelectedTile = tile;
            } else {
                currentlySelectedTile = null;
            }
        }
    }
}