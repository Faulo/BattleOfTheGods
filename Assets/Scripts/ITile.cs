using Runtime.Tiles;
using UnityEngine;

namespace Runtime {
    public interface ITile {
        GameObject gameObject { get; }
        ScriptableTile type { get; }
        Vector3Int gridPosition { get; }
        Vector3 worldPosition { get; }
        ICell ownerCell { get; }

        int border { get; set; }
        int emission { get; set; }
    }
}