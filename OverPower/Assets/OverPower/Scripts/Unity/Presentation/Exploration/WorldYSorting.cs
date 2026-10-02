using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace OverPower.Unity.Presentation.Exploration
{
    /// <summary>
    /// Sorts one composite world visual from its root/foot position.
    /// Lower world Y is visually closer and therefore receives a larger order.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SortingGroup))]
    public sealed class WorldYSorting : MonoBehaviour
    {
        [SerializeField] private int _baseOrder;
        [SerializeField, Min(1)] private int _precision = 100;
        [SerializeField] private bool _refreshWhenPositionChanges = true;

        private SortingGroup _sortingGroup;
        private float _lastWorldY = float.NaN;

        public int BaseOrder => _baseOrder;
        public int Precision => _precision;

        public static int CalculateSortingOrder(float worldY, int baseOrder, int precision)
        {
            if (precision <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(precision), "Sorting precision must be positive.");
            }

            return baseOrder - Mathf.RoundToInt(worldY * precision);
        }

        public void Configure(int baseOrder, int precision, bool refreshWhenPositionChanges = true)
        {
            if (precision <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(precision));
            }

            _baseOrder = baseOrder;
            _precision = precision;
            _refreshWhenPositionChanges = refreshWhenPositionChanges;
            Refresh();
        }

        public void Refresh()
        {
            EnsureSortingGroup();
            _sortingGroup.sortingOrder = CalculateSortingOrder(transform.position.y, _baseOrder, _precision);
            _lastWorldY = transform.position.y;
        }

        private void Awake()
        {
            EnsureSortingGroup();
            Refresh();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void LateUpdate()
        {
            if (_refreshWhenPositionChanges && !Mathf.Approximately(transform.position.y, _lastWorldY))
            {
                Refresh();
            }
        }

        private void OnValidate()
        {
            _precision = Mathf.Max(1, _precision);
            EnsureSortingGroup();
            Refresh();
        }

        private void EnsureSortingGroup()
        {
            if (_sortingGroup == null)
            {
                _sortingGroup = GetComponent<SortingGroup>();
            }
        }
    }
}
