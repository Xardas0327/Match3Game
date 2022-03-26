using System;
using UnityEngine;

namespace Match3Game.Field
{
    public enum FieldType { None, Red, Blue, Green, Yellow, Purple };

    public abstract class IFieldController: MonoBehaviour
    {
        public event EventHandler<Vector2Int> ArrivedToTarget;

        public abstract int X { get; }
        public abstract int Y { get; }
        public abstract FieldType Type { get; }

        public abstract void Init(int x, int y);

        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void Move(float x, float y);
        public abstract void Move(Vector2 v);

        protected void CallArrivedToTarget()
        {
            ArrivedToTarget?.Invoke(this, new Vector2Int(X, Y));
        }
    }
}