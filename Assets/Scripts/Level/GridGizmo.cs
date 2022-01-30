using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Tiles {
    public class GridGizmo : MonoBehaviour {
        enum Method {
            GetInDistance,
            GetCircularPositions,
            GetNeighbors,
        }
        [SerializeField]
        Color color = Color.cyan;
        [SerializeField, Expandable]
        Method method = default;
        [SerializeField]
        int radius = 1;

        protected void OnDrawGizmos() {
            var gridPosition = World.instance.WorldToGrid(transform.position);
            var positions = method switch {
                Method.GetInDistance => World.GetInDistance(gridPosition, radius),
                Method.GetCircularPositions => World.GetCircularPositions(gridPosition, radius),
                Method.GetNeighbors => World.GetNeighboringPositions(gridPosition),
                _ => throw new System.NotImplementedException(),
            };
            Gizmos.color = color;
            foreach (var position in positions) {
                Gizmos.DrawWireSphere(World.instance.GridToWorld(position), 0.5f);
            }
        }
    }
}