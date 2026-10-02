using System;
using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts.Enums;
using UnityEngine;

namespace OverPower.Unity.Integration.HeroEditor4D
{
    public enum HeroEditor4DDirection
    {
        Down,
        Up,
        Left,
        Right
    }

    /// <summary>
    /// Typed boundary around the installed HeroEditor4D presentation API.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HeroEditor4DCharacterAdapter : MonoBehaviour
    {
        [SerializeField] private Character4D _character;
        [SerializeField] private HeroEditor4DDirection _initialDirection = HeroEditor4DDirection.Down;

        public Character4D Character => RequireCharacter();
        public HeroEditor4DDirection InitialDirection => _initialDirection;

        public void Configure(Character4D character, HeroEditor4DDirection initialDirection)
        {
            _character = character != null
                ? character
                : throw new ArgumentNullException(nameof(character));
            _initialDirection = initialDirection;
        }

        public void InitializePresentation()
        {
            SetDirection(_initialDirection);
            PlayIdle();
        }

        public void SetDirection(HeroEditor4DDirection direction)
        {
            RequireCharacter().SetDirection(ToVector(direction));
        }

        public void PlayIdle()
        {
            SetState(CharacterState.Idle);
        }

        public void PlayWalk()
        {
            SetState(CharacterState.Walk);
        }

        private void Start()
        {
            InitializePresentation();
        }

        private void Reset()
        {
            _character = GetComponentInChildren<Character4D>(true);
        }

        private void SetState(CharacterState state)
        {
            var character = RequireCharacter();

            if (character.AnimationManager == null)
            {
                throw new InvalidOperationException(
                    "HeroEditor4D Character4D is missing its AnimationManager reference.");
            }

            character.AnimationManager.SetState(state);
        }

        private Character4D RequireCharacter()
        {
            if (_character == null)
            {
                throw new InvalidOperationException(
                    "HeroEditor4DCharacterAdapter requires a Character4D reference.");
            }

            return _character;
        }

        private static Vector2 ToVector(HeroEditor4DDirection direction)
        {
            switch (direction)
            {
                case HeroEditor4DDirection.Down:
                    return Vector2.down;
                case HeroEditor4DDirection.Up:
                    return Vector2.up;
                case HeroEditor4DDirection.Left:
                    return Vector2.left;
                case HeroEditor4DDirection.Right:
                    return Vector2.right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}
