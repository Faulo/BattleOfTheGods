using System;
using UnityEngine;

namespace Runtime.Level {
    public class TweenEntities : MonoBehaviour {
        [Header("LeanTween Settings")]
        [SerializeField, Range(0, 100000)]
        int maxSimultaneousTweens = 1000;

        [Header("Animation Settings")]
        [SerializeField, Range(0, 60)]
        float spawnDuration = 1;
        [SerializeField]
        LeanTweenType spawnEaseType = LeanTweenType.easeInOutBounce;

        [Space]
        [SerializeField, Range(0, 60)]
        float moveDuration = 1;
        [SerializeField]
        LeanTweenType moveEaseType = LeanTweenType.easeInOutBounce;

        [Space]
        [SerializeField, Range(0, 60)]
        float destroyDuration = 1;
        [SerializeField]
        LeanTweenType destroyEaseType = LeanTweenType.easeInOutBounce;

        protected void Awake() {
            LeanTween.init(maxSimultaneousTweens);
        }
        protected void OnEnable() {
            World.onSpawnEntity += HandleSpawn;
            World.onMoveEntity += HandleMove;
            World.onDestroyEntity += HandleDestroy;
        }
        protected void OnDisable() {
            World.onSpawnEntity -= HandleSpawn;
            World.onMoveEntity -= HandleMove;
            World.onDestroyEntity -= HandleDestroy;
        }

        void HandleSpawn(IEntity entity) {
            entity.gameObject.transform.localScale = Vector3.zero;
            LeanTween
                .scale(entity.gameObject, Vector3.one, spawnDuration)
                .setEase(spawnEaseType);
        }
        void HandleMove(IEntity entity) {
            LeanTween
                .move(entity.gameObject, entity.ownerCell.worldPosition + World.instance.randomDistanceToCenter, moveDuration)
                .setEase(moveEaseType);
        }
        void HandleDestroy(IEntity entity) {
            LeanTween
                .scale(entity.gameObject, Vector3.zero, destroyDuration)
                .setEase(destroyEaseType);
        }
    }
}