using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Runtime.Tiles {
    [CreateAssetMenu]
    public class ScriptableTile : TileBase {
        [Header("Scriptable Tile")]
        [SerializeField, Expandable]
        Sprite sprite = default;
        [SerializeField, Range(0, 255)]
        public int tileId = 0;
        [SerializeField, Expandable]
        GameObject prefab = default;
        [SerializeField]
        TileFlags tileOptions = TileFlags.None;
        [SerializeField]
        Tile.ColliderType tileCollider = Tile.ColliderType.None;

        [Header("Gameplay")]
        [SerializeField]
        public float movementCost = 1;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = sprite;
            tileData.gameObject = prefab;
            tileData.transform = (tileOptions & TileFlags.LockTransform) == TileFlags.LockTransform
                ? Matrix4x4.identity
                : tilemap.GetTransformMatrix(position);
            tileData.flags = tileOptions;
            tileData.colliderType = tileCollider;
            tileData.color = new Color(tileId / 256f, 0, 0, 0);
        }
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
            base.StartUp(position, tilemap, go);
            if (go) {
                go.transform.rotation = Quaternion.Euler(0, tilemap.GetTransformMatrix(position).rotation.eulerAngles.z, 0);
                return true;
            } else {
                return false;
            }
        }
    }
}