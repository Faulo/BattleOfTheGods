using System.Linq;
using NUnit.Framework;
using Runtime;
using UnityEngine;

namespace Tests.EditMode {
    public class WorldTests {
        static readonly (int radius, Vector3Int axial, Vector3Int grid)[] axialGridPairs = new[] {
            // radius 0
            (0, new Vector3Int(0, 0, 0), new Vector3Int(0, 0)),
            
            // radius 1
            (1, new Vector3Int(0, -1, 1), new Vector3Int(-1, -1)),
            (1, new Vector3Int(1, -1, 0), new Vector3Int(0, -1)),
            (1, new Vector3Int(1, 0, -1), new Vector3Int(1, 0)),
            (1, new Vector3Int(0, 1, -1), new Vector3Int(0, 1)),
            (1, new Vector3Int(-1, 1, 0), new Vector3Int(-1, 1)),
            (1, new Vector3Int(-1, 0, 1), new Vector3Int(-1, 0)),
        };
        [Test]
        public void T10_AxialToGrid([ValueSource(nameof(axialGridPairs))] (int radius, Vector3Int axial, Vector3Int grid) position) {
            Assert.AreEqual(position.grid, World.AxialToGrid((Vector2Int)position.axial));
        }
        [Test]
        public void T11_GridToAxial([ValueSource(nameof(axialGridPairs))] (int radius, Vector3Int axial, Vector3Int grid) position) {
            Assert.AreEqual((Vector2Int)position.axial, World.GridToAxial(position.grid));
        }
        [Test]
        public void T50_Distance([ValueSource(nameof(axialGridPairs))] (int radius, Vector3Int axial, Vector3Int grid) position) {
            Assert.AreEqual(position.radius, World.Distance(Vector3Int.zero, position.grid));
        }
        [TestCase(0)]
        [TestCase(1)]
        public void T60_GetCircularPositions(int radius) {
            var expected = axialGridPairs
                .Where(pair => pair.radius <= radius)
                .Select(pair => pair.grid)
                .OrderBy(position => (position.x << 8) + (position.y << 4) + position.z);

            var actual = World.GetCircularPositions(Vector3Int.zero, radius)
                .OrderBy(position => (position.x << 8) + (position.y << 4) + position.z);

            CollectionAssert.AreEquivalent(expected, actual);
        }
        [TestCase(0)]
        [TestCase(1)]
        public void T70GetInDistance(int radius) {
            var expected = axialGridPairs
                .Where(pair => pair.radius <= radius)
                .Select(pair => pair.grid)
                .OrderBy(position => (position.x << 8) + (position.y << 4) + position.z);

            var actual = World.GetInDistance(Vector3Int.zero, radius)
                .OrderBy(position => (position.x << 8) + (position.y << 4) + position.z);

            CollectionAssert.AreEquivalent(expected, actual);
        }
        [TestCase(0)]
        [TestCase(1)]
        public void T80_GetRing(int radius) {
            var expected = axialGridPairs
                .Where(pair => pair.radius == radius)
                .Select(pair => pair.grid)
                .OrderBy(position => (position.x << 8) + (position.y << 4) + position.z);

            var actual = World.GetRing(Vector3Int.zero, radius)
                .OrderBy(position => (position.x << 8) + (position.y << 4) + position.z);

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}