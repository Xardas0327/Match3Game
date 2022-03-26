using System;
using System.Collections.Generic;
using UnityEngine;
using Match3Game.Field;

namespace Match3Game.System
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        [Min(1F)]
        protected float gridFieldsDistance = 1.1F;
        [SerializeField]
        [Min(1F)]
        protected float sumDistance = 5F;
        [SerializeField]
        protected LevelData testLevel;

        public static LevelData level;
        protected List<IFieldController> selectedFields;
        protected IFieldController[,] fieldMatrix;
        protected int currentSteps;
        protected int currentPoint;
        protected float startXCoordinate;
        protected float startYCoordinate;
        protected int waitingPieces = 0;
        protected bool IsWin => Points >= level.RequirementPoints;
        protected bool IsLose => Steps <= 0;

        public event EventHandler<int> RefreshedSteps;
        public event EventHandler<int> RefreshedPoints;
        public event EventHandler Win;
        public event EventHandler Lose;
        public event EventHandler LockMap;
        public event EventHandler UnlockMap;

        public int Steps
        {
            get
            {
                return currentSteps;
            }
            protected set
            {
                if(currentSteps != value)
                {
                    currentSteps = value;
                    RefreshedSteps?.Invoke(this, currentSteps);
                }
            }
        }
        public int Points
        {
            get
            {
                return currentPoint;
            }
            protected set
            {
                if (currentPoint != value)
                {
                    currentPoint = value;
                    RefreshedPoints?.Invoke(this, currentPoint);
                }
            }
        }

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (testLevel != null)
                level = testLevel;

            if (level == null)
                throw new Exception("GameController: The level can't be null");

            if (level.Fields.Length == 0)
                throw new Exception("GameController: The level.Fields' length can't be 0");

            if (level.Fields[0].row.Length == 0)
                throw new Exception("GameController: The first level.Fields.row's length can't be 0");

            if (level.NewItems.Length == 0)
                throw new Exception("GameController: The level.NewItems/ length can't be 0");

            for (int j = 1; j < level.Fields.Length; ++j)
            {
                if (level.Fields[0].row.Length != level.Fields[j].row.Length)
                {
                    throw new Exception(
                        "GameController: The field's rows have to be same lengths and " +
                        j + ". is not same like the first row."
                   );
                }
            }
#endif
            Camera mainCamera = Camera.main;
            mainCamera.orthographicSize = level.CameraSize;
            mainCamera.orthographic = true;

            selectedFields = new List<IFieldController>();
            currentSteps = level.MaxSteps;
            currentPoint = 0;

            CreateFields();
        }

        protected void CreateFields()
        {
            fieldMatrix = new IFieldController[level.Fields[0].row.Length, level.Fields.Length];

            startXCoordinate = -(gridFieldsDistance * level.Fields[0].row.Length / 2);
            startYCoordinate = gridFieldsDistance * level.Fields.Length / 2;

            for (int j = 0; j < level.Fields.Length; ++j)
            {
                for (int i = 0; i < level.Fields[0].row.Length; ++i)
                {
                    CreateField(level.Fields[j].row[i], i, j);
                }
            }
        }

        protected void CreateField(IFieldController field, int x, int y, float distance = 0)
        {
            IFieldController f = Instantiate(
                                       field,
                                       new Vector3(
                                               startXCoordinate + gridFieldsDistance * x,
                                               startYCoordinate - gridFieldsDistance * y + distance,
                                       0),
                                       Quaternion.identity,
                                       transform);
            f.name = "(" +x + ", " + y + ")";
            f.Init(x, y);
            fieldMatrix[x, y] = f;

            if(distance > 0)
            {
                f.ArrivedToTarget += ArrivedField;
                f.Move(startXCoordinate + gridFieldsDistance * x,
                    startYCoordinate - gridFieldsDistance * y);
            }
        }

        public int GetMaxSteps()
        {
            if (level != null)
                return level.MaxSteps;
            else
                return -1;
        }

        public int GetRequirementPoints()
        {
            if (level != null)
                return level.RequirementPoints;
            else
                return -1;
        }

        public void SelectField(IFieldController field)
        {
            if(field != null && 
                (selectedFields.Count == 0  ||
                (IsNextField(field) && !isPreviousField(field))))
            {
                field.Activate();
                selectedFields.Add(field);
            }
        }

        public void DoAction()
        {
            if (selectedFields.Count > 0)
            {
                if(selectedFields.Count > 2)
                {
                    AddPoints();
                    --Steps;

                    if (IsWin)
                        Win(this, EventArgs.Empty);
                    else if (IsLose)
                        Lose(this, EventArgs.Empty);
                    else
                        FixMatrix();
                }
                else
                {
                    foreach (var field in selectedFields)
                        field.Deactivate();
                }

                selectedFields.Clear();
            }
        }
        
        // Here I check the color too.
        protected bool IsNextField(IFieldController field)
        {
            //If a team want to be really secure,
            //here they should check the selectedFields list's Count is not 0.
            var lastField = selectedFields[selectedFields.Count - 1];

            if (lastField == field)
                return false;

            return lastField.Type == field.Type &&
                Mathf.Abs(lastField.X - field.X) <= 1 && Mathf.Abs(lastField.Y - field.Y) <= 1;
        }

        protected bool isPreviousField(IFieldController field)
        {
            if (selectedFields.Count < 2)
                return false;

            var previousField = selectedFields[selectedFields.Count - 2];

            if (field == previousField)
            {
                //I remove the last field from selected
                selectedFields[selectedFields.Count - 1].Deactivate();
                selectedFields.RemoveAt(selectedFields.Count - 1);
                return true;
            }
            // we can't add it again, if it is in the last
            else if(selectedFields.Contains(field))
                return true;

            return false;
        }

        //Point calculation
        protected void AddPoints()
        {
            Points += selectedFields.Count - 2;
        }

        protected void FixMatrix()
        {
            LockMap?.Invoke(this, EventArgs.Empty);

            foreach (var field in selectedFields)
            {
                fieldMatrix[field.X, field.Y] = null;
                GameObject.Destroy(field.gameObject);
            }

            for (int i = 0; i < fieldMatrix.GetLength(0); ++i)
            {
                int j = fieldMatrix.GetLength(1) - 1;
                //k is a j index, when there aren't field, the code use it,
                //that the higher fields downward to fill empty space and create new fields.
                int k = -1;
                while (j >= 0)
                {
                    //find first empty place
                    if (fieldMatrix[i, j] == null && k < 0)
                    {
                        k = j;
                    }
                    // the higher fields downward to fill empty space
                    else if (fieldMatrix[i, j] != null && k > -1)
                    {
                        ++waitingPieces;
                        fieldMatrix[i, k] = fieldMatrix[i, j];
                        fieldMatrix[i, k].ArrivedToTarget += ArrivedField;
                        fieldMatrix[i, k].Init(i, k);
                        fieldMatrix[i, k].Move(startXCoordinate + gridFieldsDistance * i,
                            startYCoordinate - gridFieldsDistance * k);

                        fieldMatrix[i, k].name = "(" + i + ", " + k + ")";
                        fieldMatrix[i, j] = null;
                        --k;
                    }
                    --j;
                }

                //create new fields
                while (k > -1)
                {
                    ++waitingPieces;
                    int randomIndex = UnityEngine.Random.Range(0, level.NewItems.Length);
                    CreateField(level.NewItems[randomIndex], i, k, sumDistance);
                    --k;
                }
            }

            //this never should be run, but I have to be sure.
            if(waitingPieces <= 0)
                UnlockMap?.Invoke(this, EventArgs.Empty);

            CheckMapPlayable();
        }

        protected void ArrivedField(object sender, Vector2Int itemCoordinate)
        {
            --waitingPieces;

            fieldMatrix[itemCoordinate.x, itemCoordinate.y].ArrivedToTarget -= ArrivedField;

            if (waitingPieces <= 0)
                UnlockMap?.Invoke(this, EventArgs.Empty);
        }

        protected void CheckMapPlayable()
        {

            while (!MapChecker.isPlayable(fieldMatrix))
            {
                LockMap?.Invoke(this, EventArgs.Empty);
                for (int i = 0; i < fieldMatrix.GetLength(0); ++i)
                {
                    for (int j = 0; j < fieldMatrix.GetLength(1); ++j)
                    {
                        GameObject.Destroy(fieldMatrix[i, j].gameObject);

                        int randomIndex = UnityEngine.Random.Range(0, level.NewItems.Length);
                        CreateField(level.NewItems[randomIndex], i, j, 0);
                    }
                }
                //if the new fields is falling, but the system deleted them.
                waitingPieces = 0;
                UnlockMap?.Invoke(this, EventArgs.Empty);
            }

        }
    } 
}
