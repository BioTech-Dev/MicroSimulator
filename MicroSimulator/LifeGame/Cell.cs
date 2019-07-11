using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MicroSimulator
{
    public class Cell
    {
        public UC_LifeGame UC_LifeGame { get; set; }
        public int ColumnNumber { get; set; }  //自分自身の列番号
        public int RowNumber { get; set; }  //自分自身の行番号
        public int CellStatus {
            get
            {
                return _cellStatus;
            }
            set
            {
                if (_cellStatus != value)
                {
                    _cellStatus = value;
                    if (_cellStatus == 0) CellSurface.Fill = White;
                    if (_cellStatus == 1) CellSurface.Fill = Black;
                }
            }
        }  //0：死、1：生
        public int NextCellStatus { get; set; } = 0;
        public Rectangle CellSurface { get; set; }
        SolidColorBrush Black { get; } = new SolidColorBrush(Colors.Black);
        SolidColorBrush White { get; } = new SolidColorBrush(Colors.White);

        int _cellStatus;


        public Cell()
        {
            CellSurface = new Rectangle();
            CellSurface.Fill = White;
            CellSurface.Tapped += new TappedEventHandler(ReverseStatus);
            CellStatus = 0;
        }

        public void ReverseStatus(object sender, TappedRoutedEventArgs e)
        {

            if (CellStatus == 0)
            {
                CellStatus = 1;
            }
            else
            {
                CellStatus = 0;
            }
        }



        int GetSurroundStatus()
        {
            int cellUpperLeft = GetUpperLeftCell().CellStatus;
            int cellUpper = GetUpperCell().CellStatus;
            int cellUpperRight = GetUpperRightCell().CellStatus;
            int cellLeft = GetLeftCell().CellStatus;
            int cellRight = GetRightCell().CellStatus;
            int cellLowerLeft = GetLowerLeftCell().CellStatus;
            int cellLower = GetLowerCell().CellStatus;
            int cellLowerRight = GetLowerRightCell().CellStatus;
            //TB.Text = (cellUpperLeft + cellUpper + cellUpperRight + cellLeft + cellRight + cellLowerLeft + cellLower + cellLowerRight).ToString();
            return cellUpperLeft + cellUpper + cellUpperRight + cellLeft + cellRight + cellLowerLeft + cellLower + cellLowerRight;

        }

        public void SetNextCellStatus()
        {
            if (CellStatus == 0)
            {
                if (GetSurroundStatus() == 3)
                {
                    NextCellStatus = 1;
                }
                else
                {
                    NextCellStatus = 0;
                }
            }
            else
            {
                if (GetSurroundStatus() == 2 || GetSurroundStatus() == 3)
                {
                    NextCellStatus = 1;
                }
                else
                {
                    NextCellStatus = 0;
                }
            }
        }

        public void UpdateCellStatus()
        {
            CellStatus = NextCellStatus;
        }




        Cell GetUpperLeftCell()
        {
            if (ColumnNumber * RowNumber != 0)  //セル座標が0を含まない場合
            {
                return UC_LifeGame.CellList[ColumnNumber - 1, RowNumber - 1];
            }
            else
            {
                return UC_LifeGame.Cell_OutOfField;
            }

        }

        Cell GetUpperCell()
        {
            if (RowNumber != 0)
            {
                return UC_LifeGame.CellList[ColumnNumber, RowNumber - 1];
            }
            else
            {
                return UC_LifeGame.Cell_OutOfField;
            }
        }

        Cell GetUpperRightCell()
        {
            if (ColumnNumber != UC_LifeGame.MaximumColumnNumber - 1 && RowNumber != 0)
            {
                return UC_LifeGame.CellList[ColumnNumber + 1, RowNumber - 1];
            }
            else
            {
                return UC_LifeGame.Cell_OutOfField;
            }
        }

        Cell GetLeftCell()
        {
            if (ColumnNumber * RowNumber != 0)
            {
                return UC_LifeGame.CellList[ColumnNumber - 1, RowNumber];
            }
            else
            {
                return UC_LifeGame.Cell_OutOfField;
            }
        }

        Cell GetRightCell()
        {
            if (ColumnNumber != UC_LifeGame.MaximumColumnNumber - 1)
            {
                return UC_LifeGame.CellList[ColumnNumber + 1, RowNumber];
            }
            else
            {
                return UC_LifeGame.Cell_OutOfField;
            }
        }

        Cell GetLowerLeftCell()
        {
            if (ColumnNumber != 0 && RowNumber != UC_LifeGame.MaximumRowNumber - 1)
            {
                return UC_LifeGame.CellList[ColumnNumber - 1, RowNumber + 1];
            }
            else
            {
                return UC_LifeGame.Cell_OutOfField;
            }
        }

        Cell GetLowerCell()
        {
            if (RowNumber != UC_LifeGame.MaximumRowNumber - 1)
            {
                return UC_LifeGame.CellList[ColumnNumber, RowNumber + 1];
            }
            else
            {
                return UC_LifeGame.Cell_OutOfField;
            }
        }

        Cell GetLowerRightCell()
        {
            if (ColumnNumber != UC_LifeGame.MaximumColumnNumber - 1 && RowNumber != UC_LifeGame.MaximumRowNumber - 1)
            {
                return UC_LifeGame.CellList[ColumnNumber + 1, RowNumber + 1];
            }
            else
            {
                return UC_LifeGame.Cell_OutOfField;
            }
        }
    }

}
