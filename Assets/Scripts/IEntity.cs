using Runtime.Entities;
using UnityEngine;

namespace Runtime {
    public interface IEntity {
        GameObject gameObject { get; }
        EntityData type { get; }
        Vector3Int gridPosition { get; }
        Vector3 worldPosition { get; }
        ICell ownerCell { get; }
    }
}