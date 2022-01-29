using UnityEngine;
using UnityEngine.Tilemaps;

namespace Runtime {
    public interface ITile {
        GameObject gameObject { get; }
        TileBase type { get; }
        Vector3Int gridPosition { get; }
        Vector3 worldPosition { get; }
        ICell ownerCell { get; }
    }
}