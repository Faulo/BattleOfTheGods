using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Runtime {
    public readonly struct WorldCell : ICell {
        public WorldCell(Vector3Int gridPosition, Vector3 worldPosition, TileBase ground) {
            this.gridPosition = gridPosition;
            this.worldPosition = worldPosition;
            this.ground = ground;
            entities = new List<GameObject>();
        }

        public Vector3Int gridPosition { get; }

        public Vector3 worldPosition { get; }

        public TileBase ground { get; }

        public List<GameObject> entities { get; }

        IReadOnlyList<GameObject> ICell.entities => entities;
    }
}