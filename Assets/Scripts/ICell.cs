using System.Collections.Generic;
using UnityEngine;

namespace Runtime {
    public interface ICell {
        Vector3Int gridPosition { get; }
        Vector3 worldPosition { get; }

        ITile tile { get; }
        IEnumerable<IEntity> entities { get; }
        Faction owningFaction { get; }
    }
}