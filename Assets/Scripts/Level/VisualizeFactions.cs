using UnityEngine;

namespace Runtime.Level {
    public class VisualizeFactions : MonoBehaviour {
        protected void OnEnable() {
            World.onChangeFaction += HandleChange;
        }
        protected void OnDisable() {
            World.onChangeFaction -= HandleChange;
        }
        protected void Start() {
            foreach (var cell in World.instance.cellValues) {
                HandleChange(cell);
            }
        }
        void HandleChange(ICell cell) {
            cell.tile.border = (int)cell.owningFaction;
        }
    }
}