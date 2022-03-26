using System;
using UnityEngine;
using Match3Game.Field;

namespace Match3Game.System
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        [SerializeField]
        [Min(1)]
        protected int maxSteps = 1;
        [SerializeField]
        [Min(1)]
        protected int requirementPoints = 1;
        [SerializeField]
        [Min(1F)]
        protected float cameraSize = 5F;
        [SerializeField]
        protected LevelRow[] fields;
        [SerializeField]
        protected IFieldController[] newItems;

        public int MaxSteps => maxSteps;
        public int RequirementPoints => requirementPoints;
        public float CameraSize => cameraSize;
        public LevelRow[] Fields => fields;
        public IFieldController[] NewItems => newItems;
    }

    [Serializable]
    public class LevelRow
    {
        public IFieldController[] row;
    }

}