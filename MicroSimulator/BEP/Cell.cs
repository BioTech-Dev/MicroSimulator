using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MicroSimulator.BEP
{
    public class Cell
    {
        public UC_BEP UC_BEP { get; set; }
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
        public bool OutOfField { get; set; } = false;

        public int NextCellStatus { get; set; } = 0;
        public Rectangle CellSurface { get; set; }
        SolidColorBrush Black { get; } = new SolidColorBrush(Colors.Black);
        SolidColorBrush White { get; } = new SolidColorBrush(Colors.White);
        SolidColorBrush Red { get; } = new SolidColorBrush(Colors.Red);

        double p0 = 0.01;  //細胞分裂する確率の初期値
        double p;  ////細胞分裂する確率
        double q = 0.01;  //細胞が死ぬ確率
        int n = 50;  //遺伝子の大きさ
        int d = 10;  //ドライバー遺伝子の数
        double r = 0.001;  //遺伝子変異する確率
        double f = 0.1;  //ドライバー遺伝子変異による細胞分裂確率上昇の係数
        public int[] NonDriverGene { get; set; }
        public int[] DriverGene { get; set; }
        
        int _cellStatus;

        bool _isToDevide = false;
        bool IsToDevide
        {
            get { return _isToDevide; }
            set
            {
                _isToDevide = value;
                if (_isToDevide) CellSurface.Fill = Red;
                else CellSurface.Fill = Black;
            }
        }


        



        public Cell()
        {
            CellSurface = new Rectangle();
            CellSurface.Fill = White;
            CellStatus = 0;

            p = p0;

            NonDriverGene = new int[n-d];
            for(int a = 0; a < n - d; a++)
            {
                NonDriverGene[a] = 0;
            }

            DriverGene = new int[d];
            for (int a = 0; a < d; a++)
            {
                DriverGene[a] = 0;
            }
        }




        public void GetNextCellStatus()
        {

            if (CellStatus == 0) return;

            if (IsEventHappen(q)) //qの確率で細胞が死ぬ
            {
                CellStatus = 0;
                return;
            }

            int k = 0;  //ドライバー遺伝子変異の個数
            for (int a = 0; a < d; a++)
            {
                if (DriverGene[a] == 1)
                {
                    k++;
                    continue;
                }


                if (IsEventHappen(r))
                {
                    DriverGene[a] = 1;
                    k++;
                }
            }

            p = p0 * Math.Pow(10, f * k);
            if (!IsEventHappen(p))  //pの確率で細胞分裂する(細胞分裂しない場合はreturn)
            {
                IsToDevide = false;
                return;
            }

            IsToDevide = true;

            for (int a = 0; a < n - d; a++)
            {
                if (NonDriverGene[a] == 1) continue;

                if(IsEventHappen(r))
                {
                    NonDriverGene[a] = 1;
                }
            }



        }




        public void Devide()
        {
            if (CellStatus == 0) return;
            if (!IsToDevide) return;

            
            int newPosition = (int)Math.Floor(UC_BEP.Random.NextDouble() * 8);  //分裂後の細胞の場所を0-7の乱数で指定
            
            UC_BEP.PushSurroundCells(ColumnNumber, RowNumber, newPosition);
            UC_BEP.CellList[ColumnNumber + GetNewXSubtraction(newPosition), RowNumber + GetNewYSubtraction(newPosition)].CopyCellPropertyFrom(this);
            



        }





        bool IsEventHappen(double probability)
        {
            bool result = false;
            if(UC_BEP.Random.NextDouble() < probability)
            {
                result = true;
            }
            return result;
        }

        
        
        public Cell GetSurroundCell(int position)
        {
            Cell cell = null;

            switch (position)
            {
                case 0:  //左上
                    cell = GetUpperLeftCell();
                    break;
                case 1:  //上
                    cell = GetUpperCell();
                    break;
                case 2:  //右上
                    cell = GetUpperRightCell();
                    break;
                case 3:  //左
                    cell = GetLeftCell();
                    break;
                case 4:  //右
                    cell = GetRightCell();
                    break;
                case 5:  //左下
                    cell = GetLowerLeftCell();
                    break;
                case 6:  //下
                    cell = GetLowerCell();
                    break;
                case 7:  //右下
                    cell = GetLowerRightCell();
                    break;
            }

            return cell;
            
        }



        Cell GetUpperLeftCell()
        {
            if (ColumnNumber * RowNumber != 0)  //セル座標が0を含まない場合
            {
                return UC_BEP.CellList[ColumnNumber - 1, RowNumber - 1];
            }
            else
            {
                return UC_BEP.Cell_OutOfField;
            }

        }

        Cell GetUpperCell()
        {
            if (RowNumber != 0)
            {
                return UC_BEP.CellList[ColumnNumber, RowNumber - 1];
            }
            else
            {
                return UC_BEP.Cell_OutOfField;
            }
        }

        Cell GetUpperRightCell()
        {
            if (ColumnNumber != UC_BEP.MaximumColumnNumber - 1 && RowNumber != 0)
            {
                return UC_BEP.CellList[ColumnNumber + 1, RowNumber - 1];
            }
            else
            {
                return UC_BEP.Cell_OutOfField;
            }
        }

        Cell GetLeftCell()
        {
            if (ColumnNumber * RowNumber != 0)
            {
                return UC_BEP.CellList[ColumnNumber - 1, RowNumber];
            }
            else
            {
                return UC_BEP.Cell_OutOfField;
            }
        }

        Cell GetRightCell()
        {
            if (ColumnNumber != UC_BEP.MaximumColumnNumber - 1)
            {
                return UC_BEP.CellList[ColumnNumber + 1, RowNumber];
            }
            else
            {
                return UC_BEP.Cell_OutOfField;
            }
        }

        Cell GetLowerLeftCell()
        {
            if (ColumnNumber != 0 && RowNumber != UC_BEP.MaximumRowNumber - 1)
            {
                return UC_BEP.CellList[ColumnNumber - 1, RowNumber + 1];
            }
            else
            {
                return UC_BEP.Cell_OutOfField;
            }
        }

        Cell GetLowerCell()
        {
            if (RowNumber != UC_BEP.MaximumRowNumber - 1)
            {
                return UC_BEP.CellList[ColumnNumber, RowNumber + 1];
            }
            else
            {
                return UC_BEP.Cell_OutOfField;
            }
        }

        Cell GetLowerRightCell()
        {
            if (ColumnNumber != UC_BEP.MaximumColumnNumber - 1 && RowNumber != UC_BEP.MaximumRowNumber - 1)
            {
                return UC_BEP.CellList[ColumnNumber + 1, RowNumber + 1];
            }
            else
            {
                return UC_BEP.Cell_OutOfField;
            }
        }





        public int GetNewXSubtraction(int newPosition)
        {
            int n = 0;

            switch (newPosition)
            {
                case 0:  //左上
                    n = -1;
                    break;
                case 1:  //上
                    n = 0;
                    break;
                case 2:  //右上
                    n = 1;
                    break;
                case 3:  //左
                    n = -1;
                    break;
                case 4:  //右
                    n = 1;
                    break;
                case 5:  //左下
                    n = -1;
                    break;
                case 6:  //下
                    n = 0;
                    break;
                case 7:  //右下
                    n = 1;
                    break;
            }

            return n;
        }


        public int GetNewYSubtraction(int newPosition)
        {
            int n = 0;

            switch (newPosition)
            {
                case 0:  //左上
                    n = -1;
                    break;
                case 1:  //上
                    n = -1;
                    break;
                case 2:  //右上
                    n = -1;
                    break;
                case 3:  //左
                    n = 0;
                    break;
                case 4:  //右
                    n = 0;
                    break;
                case 5:  //左下
                    n = 1;
                    break;
                case 6:  //下
                    n = 1;
                    break;
                case 7:  //右下
                    n = 1;
                    break;
            }

            return n;
        }


        public void CopyCellPropertyFrom(Cell cell)
        {
            UC_BEP = cell.UC_BEP;
            CellStatus = cell.CellStatus;
            NextCellStatus = cell.NextCellStatus;

            p = cell.p;
            q = cell.q;
            n = cell.n;
            d = cell.d;
            r = cell.r;
            f = cell.f;
            for (int n = 0; n < cell.NonDriverGene.Length; n++)
            {
                NonDriverGene[n] = cell.NonDriverGene[n];
            }
            for (int n = 0; n < cell.DriverGene.Length; n++)
            {
                DriverGene[n] = cell.DriverGene[n];
            }

            _cellStatus = cell._cellStatus;

        }


        


    }

}
