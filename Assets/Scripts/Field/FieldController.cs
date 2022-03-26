using System;
using UnityEngine;

namespace Match3Game.Field
{
    public class FieldController : IFieldController
    {
        [SerializeField]
        protected FieldType type;
        [SerializeField]
        [Tooltip("This is the deactive color.")]
        protected Color defaultColor;
        [SerializeField]
        protected Color activeColor;
        [SerializeField]
        [Min(1F)]
        protected float moveSpeed = 5;

        protected SpriteRenderer spriteRenderer;
        protected Vector2 target;
        protected bool isMoving;

        //Coordinates
        protected int x;
        protected int y;

        public override int X => x;
        public override int Y => y;
        public override FieldType Type => type;

        protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
#if UNITY_EDITOR
            if (type == FieldType.None)
                throw new Exception("FieldController(" + name + "): The type can't be null.");

            if (spriteRenderer == null)
                throw new Exception("FieldController(" + name + "): spriteRenderer can't be null.");
#endif
        }

        protected void Start()
        {
            //It should have the default color
            Deactivate();
        }

        protected void Update()
        {
            MoveToTarget();
        }

        public override void Init(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override void Activate()
        {
            spriteRenderer.color = activeColor;
        }

        public override void Deactivate()
        {
            spriteRenderer.color = defaultColor;
        }

        public override void Move(float x, float y)
        {
            Move(new Vector2(x, y));
        }

        public override void Move(Vector2 v)
        {
            if (transform.position.x == v.x && transform.position.y == v.y)
                return;

            target = v;
            isMoving = true;
        }

        private void MoveToTarget()
        {
            if (!isMoving)
                return;

            transform.position = new Vector3(
                    Mathf.MoveTowards(transform.position.x, target.x, moveSpeed * Time.deltaTime),
                    Mathf.MoveTowards(transform.position.y, target.y, moveSpeed * Time.deltaTime),
                    transform.position.z
                );

            if (transform.position.x == target.x && transform.position.y == target.y)
            {
                isMoving = false;
                CallArrivedToTarget();
            }
        }
    }
}