using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    class Cell
    {
        public CellStates cellState;
        public float x;
        public float y;
        public int id;
        public Cell() { }
        public Cell(CellStates cellState)
        {
            this.cellState = cellState;
        }
        public Cell(CellStates cellState,float x,float y)
        {
            this.cellState = cellState;
            this.x = x;
            this.y = y;
        }
        public Cell(CellStates cellState, float x, float y,int id)
        {
            this.cellState = cellState;
            this.x = x;
            this.y = y;
            this.id = id;
        }
    }
}
