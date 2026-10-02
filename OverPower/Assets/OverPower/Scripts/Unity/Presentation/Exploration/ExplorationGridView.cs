using System;
using OverPower.Domain.Exploration;
using UnityEngine;

namespace OverPower.Unity.Presentation.Exploration
{
    /// <summary>
    /// Owns the presentation-space mapping for an exploration grid.
    /// Authoritative walkability and movement remain in Domain.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ExplorationGridView : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _width = 8;
        [SerializeField, Min(1)] private int _height = 6;
        [SerializeField, Min(0.01f)] private float _cellSize = 1f;
        [SerializeField] private Vector2 _origin = new Vector2(-3.5f, -2.5f);

        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;
        public Vector2 Origin => _origin;

        public Vector3 GridToWorld(ExplorationCoordinate coordinate)
        {
            return GridToWorld(coordinate, _origin, _cellSize);
        }

        public static Vector3 GridToWorld(
            ExplorationCoordinate coordinate,
            Vector2 origin,
            float cellSize)
        {
            if (cellSize <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(cellSize), "Cell size must be positive.");
            }

            return new Vector3(
                origin.x + coordinate.X * cellSize,
                origin.y + coordinate.Y * cellSize,
                0f);
        }

        public void Configure(int width, int height, float cellSize, Vector2 origin)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (cellSize <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(cellSize));
            }

            _width = width;
            _height = height;
            _cellSize = cellSize;
            _origin = origin;
        }

        private void OnValidate()
        {
            _width = Mathf.Max(1, _width);
            _height = Mathf.Max(1, _height);
            _cellSize = Mathf.Max(0.01f, _cellSize);
        }
    }
}
