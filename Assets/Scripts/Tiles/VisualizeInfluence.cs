using UnityEngine;

namespace Runtime.Tiles {
    public class VisualizeInfluence : MonoBehaviour {
        [SerializeField]
        ParticleSystem particlesPrefab = default;
        ParticleSystem particlesInstance;
        [SerializeField]
        int particleCountPerInfluence = 100;

        [Space]
        [SerializeField]
        Color natureColor = Color.green;
        [SerializeField]
        Color civColor = Color.blue;

        ICell cell;
        protected void Start() {
            if (particlesPrefab) {
                cell = World.instance.GetTileByTileObject(gameObject).ownerCell;
                cell.onGainInfluence += HandleInfluence;
                particlesInstance = Instantiate(particlesPrefab, cell.worldPosition, Quaternion.identity, transform);
            }
        }
        protected void OnDestroy() {
            if (cell != null) {
                cell.onGainInfluence -= HandleInfluence;
            }
        }
        void HandleInfluence(int change) {
            if (change > 0) {
                SpawnParticles(change, civColor);
            }
            if (change < 0) {
                SpawnParticles(change, natureColor);
            }
        }
        void SpawnParticles(int value, Color color) {
            var main = particlesInstance.main;
            main.startColor = color;
            particlesInstance.Emit(value * particleCountPerInfluence);
        }
    }
}