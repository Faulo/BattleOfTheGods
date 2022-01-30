using UnityEngine;

namespace Runtime.Level {
    public class VisualizeCombat : MonoBehaviour {
        [SerializeField]
        GameObject particlesPrefab = default;
        [SerializeField]
        int particleCountPerAttack = 100;

        [Space]
        [SerializeField]
        Color natureColor = Color.green;
        [SerializeField]
        Color civColor = Color.blue;

        protected void OnEnable() {
            World.onAttackByFaction += HandleAttack;
        }
        protected void OnDisable() {
            World.onAttackByFaction -= HandleAttack;
        }
        void HandleAttack(ICell cell, Faction faction, int value) {
            if (faction == Faction.Civilization) {
                SpawnParticles(cell, value, civColor);
            } else {
                SpawnParticles(cell, value, natureColor);
            }
        }
        void SpawnParticles(ICell cell, int value, Color color) {
            var instance = Instantiate(particlesPrefab, cell.worldPosition, Quaternion.identity, cell.tile.gameObject.transform);
            if (instance.TryGetComponent<ParticleSystem>(out var particles)) {
                var main = particles.main;
                main.startColor = color;
                main.maxParticles = value * particleCountPerAttack;
                particles.Emit(value * particleCountPerAttack);
            }
        }
    }
}