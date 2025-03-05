using UnityEngine;

namespace SnakePowerByte.Level
{
    public class LevelGrid : MonoBehaviour
    {
        private int _width;
        private int _height;


        public LevelGrid(int width, int height)
        {
            this._width = width;
            this._height = height;
        }


        public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
        {
            if (gridPosition.x < 0)
            {
                gridPosition.x = _width;
            }
            if (gridPosition.x > _width)
            {
                gridPosition.x = 0;
            }
            if (gridPosition.y < 0)
            {
                gridPosition.y = _height;
            }
            if (gridPosition.y > _height)
            {
                gridPosition.y = 0;
            }
            return gridPosition;
        }
    }
}
