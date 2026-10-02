using System;
using System.IO;
using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts.Enums;
using OverPower.Domain.Exploration;
using OverPower.Unity.Integration.HeroEditor4D;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace OverPower.Unity.Presentation.Exploration.Editor
{
    public static class ExplorationSceneBuilder
    {
        private const string ScenePath = "Assets/OverPower/Scenes/Exploration.unity";
        private const string HeroPrefabPath = "Assets/OverPower/Prefabs/Exploration/ExplorationHero.prefab";
        private const string SourceHeroPrefabPath = "Assets/HeroEditor4D/FantasyHeroes/Prefabs/Human.prefab";
        private const string ArtFolder = "Assets/OverPower/Art/Exploration";
        private const string TileTexturePath = ArtFolder + "/room_stone_tile.png";
        private const string CircleTexturePath = ArtFolder + "/room_soft_circle.png";

        private const int RoomWidth = 8;
        private const int RoomHeight = 6;
        private const float CellSize = 1f;
        private static readonly Vector2 GridOrigin = new Vector2(-3.5f, -2.5f);

        private static readonly Color VoidColor = Hex("10151F");
        private static readonly Color FoundationColor = Hex("242837");
        private static readonly Color FloorA = Hex("465064");
        private static readonly Color FloorB = Hex("40495C");
        private static readonly Color FloorAccent = Hex("5A6074");
        private static readonly Color WallBase = Hex("252B38");
        private static readonly Color WallFace = Hex("343B4C");
        private static readonly Color Bronze = Hex("9B754B");
        private static readonly Color BronzeLight = Hex("C19A64");
        private static readonly Color Rune = new Color(0.39f, 0.72f, 0.78f, 0.28f);
        private static readonly Color Shadow = new Color(0.03f, 0.04f, 0.07f, 0.62f);

        [MenuItem("OverPower/Exploration/Rebuild Static Room")]
        public static void Rebuild()
        {
            EnsureFolder("Assets/OverPower/Prefabs", "Exploration");
            EnsureFolder("Assets/OverPower/Art", "Exploration");
            CreateTextureAssets();
            var heroPrefab = BuildHeroPrefab();
            BuildScene(heroPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Exploration static room rebuilt.");
        }

        private static GameObject BuildHeroPrefab()
        {
            var sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SourceHeroPrefabPath);

            if (sourcePrefab == null)
            {
                throw new InvalidOperationException($"HeroEditor4D source prefab was not found at {SourceHeroPrefabPath}.");
            }

            var root = new GameObject("ExplorationHero");

            try
            {
                var sortingGroup = root.AddComponent<SortingGroup>();
                sortingGroup.sortingLayerName = "Characters";

                var ySorting = root.AddComponent<WorldYSorting>();
                ySorting.Configure(0, 100);

                var footAnchor = new GameObject("FootAnchor");
                footAnchor.transform.SetParent(root.transform, false);

                var visual = (GameObject)PrefabUtility.InstantiatePrefab(sourcePrefab);
                visual.name = "Visual";
                PrefabUtility.UnpackPrefabInstance(
                    visual,
                    PrefabUnpackMode.Completely,
                    InteractionMode.AutomatedAction);
                visual.transform.SetParent(root.transform, false);
                visual.transform.localPosition = new Vector3(0f, 0.31f, 0f);
                visual.transform.localScale = Vector3.one * 0.34f;

                var character = visual.GetComponent<Character4D>();

                if (character == null)
                {
                    throw new InvalidOperationException("The selected HeroEditor4D prefab has no Character4D root component.");
                }

                var packageSortingGroup = visual.GetComponent<SortingGroup>();

                if (packageSortingGroup != null)
                {
                    UnityEngine.Object.DestroyImmediate(packageSortingGroup);
                }

                // Keep HeroEditor4D's authored internal body-part orders intact while
                // normalizing the imported renderers into the root group's world layer.
                foreach (var spriteRenderer in visual.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    spriteRenderer.sortingLayerName = "Characters";
                }

                character.SetDirection(Vector2.down);
                character.AnimationManager.SetState(CharacterState.Idle);

                var adapter = root.AddComponent<HeroEditor4DCharacterAdapter>();
                adapter.Configure(character, HeroEditor4DDirection.Down);

                var prefab = PrefabUtility.SaveAsPrefabAsset(root, HeroPrefabPath);
                return prefab;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void BuildScene(GameObject heroPrefab)
        {
            if (heroPrefab == null)
            {
                throw new ArgumentNullException(nameof(heroPrefab));
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SceneManager.SetActiveScene(scene);

            var tileSprite = AssetDatabase.LoadAssetAtPath<Sprite>(TileTexturePath);
            var circleSprite = AssetDatabase.LoadAssetAtPath<Sprite>(CircleTexturePath);

            if (tileSprite == null || circleSprite == null)
            {
                throw new InvalidOperationException("Exploration sprite assets failed to import.");
            }

            CreateCamera();

            var world = new GameObject("World");
            var gridView = world.AddComponent<ExplorationGridView>();
            gridView.Configure(RoomWidth, RoomHeight, CellSize, GridOrigin);

            var ground = CreateGroup("Ground", world.transform);
            var environment = CreateGroup("Environment", world.transform);
            var obstacles = CreateGroup("Obstacles", world.transform);
            var characters = CreateGroup("Characters", world.transform);
            var foreground = CreateGroup("Foreground", world.transform);

            CreateSprite("Void", ground, tileSprite, Vector3.zero, new Vector2(18f, 10f), VoidColor, "Ground", -1000);
            CreateSprite("Foundation", environment, tileSprite, Vector3.zero, new Vector2(8.75f, 6.75f), FoundationColor, "Ground", -100);
            CreateSprite("InnerTrim", environment, tileSprite, Vector3.zero, new Vector2(8.25f, 6.25f), Bronze, "Ground", -90);
            CreateFloor(gridView, ground, tileSprite);
            CreateHeroRune(gridView, ground, circleSprite);
            CreateBoundaries(environment, tileSprite, circleSprite);
            CreateObstacles(gridView, obstacles, tileSprite, circleSprite);

            var hero = (GameObject)PrefabUtility.InstantiatePrefab(heroPrefab);
            hero.name = "ExplorationHero";
            hero.transform.SetParent(characters, false);
            hero.transform.position = gridView.GridToWorld(new ExplorationCoordinate(3, 2));
            hero.GetComponent<WorldYSorting>().Refresh();

            CreateSprite(
                "LowerEdgeShade",
                foreground,
                tileSprite,
                new Vector3(0f, -3.58f, 0f),
                new Vector2(9.2f, 0.18f),
                new Color(0.04f, 0.05f, 0.08f, 0.55f),
                "Foreground",
                0);

            new GameObject("UI");
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            var camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 4.7f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = VoidColor;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
        }

        private static void CreateFloor(ExplorationGridView grid, Transform parent, Sprite sprite)
        {
            var floor = CreateGroup("FloorCells", parent);

            for (var y = 0; y < RoomHeight; y++)
            {
                for (var x = 0; x < RoomWidth; x++)
                {
                    var color = (x + y) % 2 == 0 ? FloorA : FloorB;

                    if ((x == 3 || x == 4) && (y == 2 || y == 3))
                    {
                        color = Color.Lerp(color, FloorAccent, 0.35f);
                    }

                    CreateSprite(
                        $"Cell_{x}_{y}",
                        floor,
                        sprite,
                        grid.GridToWorld(new ExplorationCoordinate(x, y)),
                        Vector2.one * 0.94f,
                        color,
                        "Ground",
                        y * RoomWidth + x);
                }
            }
        }

        private static void CreateHeroRune(ExplorationGridView grid, Transform parent, Sprite circleSprite)
        {
            var position = grid.GridToWorld(new ExplorationCoordinate(3, 2));
            CreateSprite("HeroRuneOuter", parent, circleSprite, position, new Vector2(0.86f, 0.38f), Rune, "Environment", 20);
            CreateSprite("HeroRuneInner", parent, circleSprite, position, new Vector2(0.48f, 0.21f), new Color(0.72f, 0.85f, 0.85f, 0.18f), "Environment", 21);
        }

        private static void CreateBoundaries(Transform parent, Sprite tileSprite, Sprite circleSprite)
        {
            var boundaryRoot = CreateGroup("Boundaries", parent);

            for (var x = 0; x < RoomWidth; x++)
            {
                var worldX = GridOrigin.x + x * CellSize;
                CreateWallSegment($"NorthWall_{x}", boundaryRoot, new Vector3(worldX, 3.08f, 0f), tileSprite, circleSprite);
                CreateWallSegment($"SouthWall_{x}", boundaryRoot, new Vector3(worldX, -3.08f, 0f), tileSprite, circleSprite);
            }

            for (var y = 0; y < RoomHeight; y++)
            {
                var worldY = GridOrigin.y + y * CellSize;
                CreateWallSegment($"WestWall_{y}", boundaryRoot, new Vector3(-4.08f, worldY, 0f), tileSprite, circleSprite);
                CreateWallSegment($"EastWall_{y}", boundaryRoot, new Vector3(4.08f, worldY, 0f), tileSprite, circleSprite);
            }

            CreateWallSegment("NorthWestCorner", boundaryRoot, new Vector3(-4.08f, 3.08f, 0f), tileSprite, circleSprite);
            CreateWallSegment("NorthEastCorner", boundaryRoot, new Vector3(4.08f, 3.08f, 0f), tileSprite, circleSprite);
            CreateWallSegment("SouthWestCorner", boundaryRoot, new Vector3(-4.08f, -3.08f, 0f), tileSprite, circleSprite);
            CreateWallSegment("SouthEastCorner", boundaryRoot, new Vector3(4.08f, -3.08f, 0f), tileSprite, circleSprite);
        }

        private static void CreateWallSegment(
            string name,
            Transform parent,
            Vector3 position,
            Sprite tileSprite,
            Sprite circleSprite)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            var group = root.AddComponent<SortingGroup>();
            group.sortingLayerName = "Characters";
            var sorter = root.AddComponent<WorldYSorting>();
            sorter.Configure(0, 100, false);

            CreateSprite("Shadow", root.transform, circleSprite, new Vector3(0f, -0.18f, 0f), new Vector2(0.96f, 0.34f), Shadow, "Characters", 0, true);
            CreateSprite("Face", root.transform, tileSprite, new Vector3(0f, 0.08f, 0f), new Vector2(0.98f, 0.68f), WallBase, "Characters", 1, true);
            CreateSprite("Cap", root.transform, tileSprite, new Vector3(0f, 0.36f, 0f), new Vector2(0.9f, 0.34f), WallFace, "Characters", 2, true);
            CreateSprite("Trim", root.transform, tileSprite, new Vector3(0f, 0.53f, 0f), new Vector2(0.78f, 0.06f), Bronze, "Characters", 3, true);
            sorter.Refresh();
        }

        private static void CreateObstacles(
            ExplorationGridView grid,
            Transform parent,
            Sprite tileSprite,
            Sprite circleSprite)
        {
            CreatePillar("Blocked_2_4", parent, grid.GridToWorld(new ExplorationCoordinate(2, 4)), tileSprite, circleSprite);
            CreatePillar("Blocked_5_1", parent, grid.GridToWorld(new ExplorationCoordinate(5, 1)), tileSprite, circleSprite);
            CreatePillar("Blocked_6_3", parent, grid.GridToWorld(new ExplorationCoordinate(6, 3)), tileSprite, circleSprite);
        }

        private static void CreatePillar(
            string name,
            Transform parent,
            Vector3 position,
            Sprite tileSprite,
            Sprite circleSprite)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            var group = root.AddComponent<SortingGroup>();
            group.sortingLayerName = "Characters";
            var sorter = root.AddComponent<WorldYSorting>();
            sorter.Configure(0, 100, false);

            CreateSprite("GroundShadow", root.transform, circleSprite, new Vector3(0f, 0.03f, 0f), new Vector2(0.9f, 0.36f), Shadow, "Characters", 0, true);
            CreateSprite("Base", root.transform, tileSprite, new Vector3(0f, 0.18f, 0f), new Vector2(0.74f, 0.42f), WallBase, "Characters", 1, true);
            CreateSprite("Shaft", root.transform, tileSprite, new Vector3(0f, 0.58f, 0f), new Vector2(0.56f, 0.82f), WallFace, "Characters", 2, true);
            CreateSprite("Capital", root.transform, tileSprite, new Vector3(0f, 1.02f, 0f), new Vector2(0.84f, 0.26f), Bronze, "Characters", 3, true);
            CreateSprite("Highlight", root.transform, tileSprite, new Vector3(-0.18f, 0.6f, 0f), new Vector2(0.08f, 0.62f), BronzeLight, "Characters", 4, true);
            sorter.Refresh();
        }

        private static SpriteRenderer CreateSprite(
            string name,
            Transform parent,
            Sprite sprite,
            Vector3 position,
            Vector2 size,
            Color color,
            string sortingLayer,
            int sortingOrder,
            bool localPosition = false)
        {
            var gameObject = new GameObject(name, typeof(SpriteRenderer));
            gameObject.transform.SetParent(parent, false);

            if (localPosition)
            {
                gameObject.transform.localPosition = position;
            }
            else
            {
                gameObject.transform.position = position;
            }

            gameObject.transform.localScale = new Vector3(size.x, size.y, 1f);

            var renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingLayerName = sortingLayer;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static Transform CreateGroup(string name, Transform parent)
        {
            var group = new GameObject(name).transform;
            group.SetParent(parent, false);
            return group;
        }

        private static void CreateTextureAssets()
        {
            WriteTexture(TileTexturePath, 32, 32, (x, y) =>
            {
                var edge = Mathf.Min(Mathf.Min(x, 31 - x), Mathf.Min(y, 31 - y));
                var bevel = edge < 2 ? 0.74f : edge < 4 ? 0.9f : 1f;
                var grain = 0.94f + ((x * 17 + y * 31 + x * y * 3) % 13) / 180f;
                var value = Mathf.Clamp01(bevel * grain);
                return new Color(value, value, value, 1f);
            });

            WriteTexture(CircleTexturePath, 64, 64, (x, y) =>
            {
                var nx = (x + 0.5f) / 64f * 2f - 1f;
                var ny = (y + 0.5f) / 64f * 2f - 1f;
                var distance = Mathf.Sqrt(nx * nx + ny * ny);
                var alpha = Mathf.Clamp01((1f - distance) * 2.4f);
                return new Color(1f, 1f, 1f, alpha);
            });

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            ConfigureSpriteImporter(TileTexturePath, FilterMode.Bilinear);
            ConfigureSpriteImporter(CircleTexturePath, FilterMode.Bilinear);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private static void WriteTexture(string assetPath, int width, int height, Func<int, int, Color> pixel)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);

            try
            {
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        texture.SetPixel(x, y, pixel(x, y));
                    }
                }

                texture.Apply(false, false);
                var absolutePath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
                File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        private static void ConfigureSpriteImporter(string path, FilterMode filterMode)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer == null)
            {
                throw new InvalidOperationException($"No texture importer was created for {path}.");
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 32f;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = filterMode;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = parent + "/" + child;

            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static Color Hex(string value)
        {
            if (!ColorUtility.TryParseHtmlString("#" + value, out var color))
            {
                throw new ArgumentException("Invalid color value.", nameof(value));
            }

            return color;
        }
    }
}
