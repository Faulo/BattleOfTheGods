using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Runtime {
    public interface ICell {
        Vector3Int gridPosition { get; }
        Vector3 worldPosition { get; }
        TileBase ground { get; }
        List<GameObject> entities { get; }
    }
}