using System.Linq;
using NUnit.Framework;
using OverPower.Domain.Exploration;
using OverPower.Unity.Integration.HeroEditor4D;
using OverPower.Unity.Presentation.Exploration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace OverPower.Tests.Unity
{
    public sealed class ExplorationPresentationTests
    {
        private const string ScenePath = "Assets/OverPower/Scenes/Exploration.unity";
        private const string HeroPrefabPath = "Assets/OverPower/Prefabs/Exploration/ExplorationHero.prefab";

        [Test]
        public void GridToWorld_ProducesDeterministicCellSizedPositions()
        {
            var origin = new Vector2(-3.5f, -2.5f);
            var start = ExplorationGridView.GridToWorld(new ExplorationCoordinate(0, 0), origin, 1f);
            var right = ExplorationGridView.GridToWorld(new ExplorationCoordinate(1, 0), origin, 1f);
            var up = ExplorationGridView.GridToWorld(new ExplorationCoordinate(0, 1), origin, 1f);

            Assert.That(start, Is.EqualTo(new Vector3(-3.5f, -2.5f, 0f)));
            Assert.That(right.x - start.x, Is.EqualTo(1f));
            Assert.That(up.y - start.y, Is.EqualTo(1f));
            Assert.That(
                ExplorationGridView.GridToWorld(new ExplorationCoordinate(1, 1), origin, 1f),
                Is.EqualTo(new Vector3(-2.5f, -1.5f, 0f)));
        }

        [Test]
        public void YSorting_LowerWorldPositionReceivesLargerFrontOrder()
        {
            var behind = WorldYSorting.CalculateSortingOrder(1.5f, 0, 100);
            var hero = WorldYSorting.CalculateSortingOrder(-0.5f, 0, 100);
            var inFront = WorldYSorting.CalculateSortingOrder(-1.5f, 0, 100);

            Assert.That(hero, Is.GreaterThan(behind));
            Assert.That(inFront, Is.GreaterThan(hero));
            Assert.That(behind, Is.EqualTo(-150));
            Assert.That(hero, Is.EqualTo(50));
            Assert.That(inFront, Is.EqualTo(150));
        }

        [Test]
        public void ExplorationHeroPrefab_HasTypedIntegrationAndGroupedSorting()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HeroPrefabPath);

            Assert.That(prefab, Is.Not.Null);
            Assert.That(prefab.name, Is.EqualTo("ExplorationHero"));

            var sortingGroup = prefab.GetComponent<SortingGroup>();
            var ySorting = prefab.GetComponent<WorldYSorting>();
            var adapter = prefab.GetComponent<HeroEditor4DCharacterAdapter>();

            Assert.That(sortingGroup, Is.Not.Null);
            Assert.That(sortingGroup.sortingLayerName, Is.EqualTo("Characters"));
            Assert.That(ySorting, Is.Not.Null);
            Assert.That(ySorting.Precision, Is.EqualTo(100));
            Assert.That(adapter, Is.Not.Null);
            Assert.That(adapter.InitialDirection, Is.EqualTo(HeroEditor4DDirection.Down));
            Assert.That(adapter.Character, Is.Not.Null);
            Assert.That(prefab.transform.Find("FootAnchor"), Is.Not.Null);
            Assert.That(prefab.transform.Find("Visual"), Is.Not.Null);
        }

        [Test]
        public void ExplorationScene_ContainsAuthoredEightBySixRoomAndStaticHero()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var roots = scene.GetRootGameObjects();
            var world = roots.Single(root => root.name == "World");
            var grid = world.GetComponent<ExplorationGridView>();

            Assert.That(grid, Is.Not.Null);
            Assert.That(grid.Width, Is.EqualTo(8));
            Assert.That(grid.Height, Is.EqualTo(6));
            Assert.That(grid.CellSize, Is.EqualTo(1f));
            Assert.That(world.transform.Find("Ground/FloorCells").childCount, Is.EqualTo(48));
            Assert.That(world.transform.Find("Obstacles").childCount, Is.EqualTo(3));
            Assert.That(world.transform.Find("Environment/Boundaries"), Is.Not.Null);

            var hero = world.transform.Find("Characters/ExplorationHero");
            Assert.That(hero, Is.Not.Null);
            Assert.That(hero.position, Is.EqualTo(grid.GridToWorld(new ExplorationCoordinate(3, 2))));
            Assert.That(hero.GetComponent<HeroEditor4DCharacterAdapter>(), Is.Not.Null);

            var obstacleOrders = world.transform.Find("Obstacles")
                .Cast<Transform>()
                .Select(item => item.GetComponent<SortingGroup>().sortingOrder)
                .ToArray();
            var heroOrder = hero.GetComponent<SortingGroup>().sortingOrder;

            Assert.That(obstacleOrders, Has.Some.LessThan(heroOrder));
            Assert.That(obstacleOrders, Has.Some.GreaterThan(heroOrder));
            Assert.That(roots.Any(root => root.name == "Canvas"), Is.False);
        }
    }
}
