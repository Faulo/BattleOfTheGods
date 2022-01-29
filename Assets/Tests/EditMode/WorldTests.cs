using System.Linq;
using NUnit.Framework;
using Runtime;
using UnityEngine;

namespace Tests.EditMode {
    public class WorldTests {
        static readonly (int radius, Vector2Int axial, Vector3Int grid)[] axialGridPairs = new[] {
            // radius 0
            (0, new Vector2Int(0, 0), new Vector3Int(0, 0)),
            
            // radius 1
            (1, new Vector2Int(0, -1), new Vector3Int(0, -1)),
            (1, new Vector2Int(1, -1), new Vector3Int(1, 0)),
            (1, new Vector2Int(1, 0), new Vector3Int(1, 1)),
            (1, new Vector2Int(0, 1), new Vector3Int(0, 1)),
            (1, new Vector2Int(-1, 1), new Vector3Int(-1, 1)),
            (1, new Vector2Int(-1, 0), new Vector3Int(-1, 0)),
            
            // radius 2
            (2, new Vector2Int(0, -2), new Vector3Int(0, -2)),
            (2, new Vector2Int(1, -2), new Vector3Int(1, -1)),
            (2, new Vector2Int(2, -2), new Vector3Int(2, -1)),
            (2, new Vector2Int(2, -1), new Vector3Int(2, 0)),
            (2, new Vector2Int(2, 0), new Vector3Int(2, 1)),
            (2, new Vector2Int(1, 1), new Vector3Int(1, 2)),
            (2, new Vector2Int(0, 2), new Vector3Int(0, 2)),
            (2, new Vector2Int(-1, 2), new Vector3Int(-1, 2)),
            (2, new Vector2Int(-2, 2), new Vector3Int(-2, 1)),
            (2, new Vector2Int(-2, 1), new Vector3Int(-2, 0)),
            (2, new Vector2Int(-2, 0), new Vector3Int(-2, -1)),
            (2, new Vector2Int(-1, -1), new Vector3Int(-1, -1)),
        };
        [Test]
        public void TestAxialToGrid([ValueSource(nameof(axialGridPairs))] (int radius, Vector2Int axial, Vector3Int grid) position) {
            Assert.AreEqual(position.grid, World.AxialToGrid(position.axial));
        }
        [Test]
        public void TestGridToAxial([ValueSource(nameof(axialGridPairs))] (int radius, Vector2Int axial, Vector3Int grid) position) {
            Assert.AreEqual(position.axial, World.GridToAxial(position.grid));
        }
        [Test]
        public void TestDistance([ValueSource(nameof(axialGridPairs))] (int radius, Vector2Int axial, Vector3Int grid) position) {
            Assert.AreEqual(position.radius, World.Distance(Vector3Int.zero, position.grid));
        }
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void TestGetCircularPositions(int radius) {
            var expected = axialGridPairs
                .Where(pair => pair.radius <= radius)
                .Select(pair => pair.grid)
                .OrderBy(position => (position.x << 8) + (position.y << 4) + position.z);

            var actual = World.GetCircularPositions(Vector3Int.zero, radius)
                .OrderBy(position => (position.x << 8) + (position.y << 4) + position.z);

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}