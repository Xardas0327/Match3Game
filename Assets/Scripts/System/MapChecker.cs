using System.Collections.Generic;
using UnityEngine;
using Match3Game.Field;

namespace Match3Game.System
{
    public static class MapChecker
    {
        private class QueueItem
        {
            public IFieldController Item { get; private set; }
            public int Length { get; private set; }

            public QueueItem(IFieldController item, int length)
            {
                Item = item;
                Length = length;
            }
        }

        public static bool isPlayable(IFieldController[,] fieldMatrix)
        {
            var queue = new Queue<QueueItem>();

            for (int i = 0; i < fieldMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < fieldMatrix.GetLength(1); ++j)
                {
                    queue.Enqueue(new QueueItem(fieldMatrix[i, j], 1));
                    var checkedFields = new bool[fieldMatrix.GetLength(0), fieldMatrix.GetLength(1)];
                    while (queue.Count > 0)
                    {
                        var field = queue.Dequeue();
                        checkedFields[field.Item.X, field.Item.Y] = true;
                        for (int k = field.Item.X - 1; k <= field.Item.X + 1; ++k)
                        {
                            for (int l = field.Item.Y - 1; l <= field.Item.Y + 1; ++l)
                            {
                                if (k >= 0 && k < fieldMatrix.GetLength(0) &&
                                   l >= 0 && l < fieldMatrix.GetLength(1) &&
                                   !checkedFields[k, l] &&
                                   fieldMatrix[k, l].Type == field.Item.Type)
                                {
                                    if (field.Length == 2)
                                        return true;

                                    queue.Enqueue(new QueueItem(fieldMatrix[k, l], field.Length + 1));
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}