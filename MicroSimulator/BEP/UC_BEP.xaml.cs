using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace MicroSimulator.BEP
{
    public sealed partial class UC_BEP : UserControl
    {
        public int MaximumRowNumber { get; set; } = 100;
        public int MaximumColumnNumber { get; set; } = 150;
        public Cell[,] CellList { get; set; }  //セルを表す二次元配列、左上が原点、右/下がプラス
        public Cell Cell_OutOfField { get; set; } = new Cell { OutOfField = true };
        public int InitialTumorSize { get; set; } = 4;

        DispatcherTimer _timer = new DispatcherTimer();

        public Random Random { get; set; } = new Random();

        public UC_BEP()
        {
            this.InitializeComponent();
            InitializeSimulator();

            _timer.Interval = TimeSpan.FromSeconds(0.1);
            _timer.Tick += NextStatus;
        }

        void InitializeSimulator()
        {
            for (int n = 0; n < MaximumRowNumber; n++)
            {
                Grid_Simulator.RowDefinitions.Add(new RowDefinition());
            }

            for (int n = 0; n < MaximumColumnNumber; n++)
            {
                Grid_Simulator.ColumnDefinitions.Add(new ColumnDefinition());
            }

            CellList = new Cell[MaximumColumnNumber, MaximumRowNumber];
            for (int m = 0; m < MaximumColumnNumber; m++)
            {
                for (int n = 0; n < MaximumRowNumber; n++)
                {
                    CellList[m, n] = new Cell();
                    CellList[m, n].ColumnNumber = m;
                    CellList[m, n].RowNumber = n;
                    CellList[m, n].UC_BEP = this;


                    CellList[m, n].CellSurface.SetValue(Grid.ColumnProperty, m);
                    CellList[m, n].CellSurface.SetValue(Grid.RowProperty, n);
                    Grid_Simulator.Children.Add(CellList[m, n].CellSurface);
                }
            }

            //腫瘍の初期サイズを設定
            int initialTumorX = (int)Math.Ceiling(Math.Sqrt(InitialTumorSize));
            int initialTumorY = (int)Math.Floor(Math.Sqrt(InitialTumorSize));

            for(int n = 0; n < initialTumorX; n++)
            {
                for(int m = 0; m　< initialTumorY; m++)
                {
                    CellList[(int)Math.Floor((double)(MaximumColumnNumber / 2 - initialTumorX / 2)) + n, (int)Math.Floor((double)(MaximumRowNumber / 2 - initialTumorY / 2) ) + m].CellStatus = 1;
                }
            }
            

        }


        private void NextStatus(object sender, object e)
        {
            for (int m = 0; m < MaximumColumnNumber; m++)
            {
                for (int n = 0; n < MaximumRowNumber; n++)
                {
                    CellList[m, n].GetNextCellStatus();
                }
            }


            if (Random.Next() < 0.5)
            {
                for (int m = 0; m < MaximumColumnNumber; m++)
                {
                    for (int n = 0; n < MaximumRowNumber; n++)
                    {
                        CellList[m, n].Devide();
                    }
                }
            }
            else
            {
                for (int m = MaximumColumnNumber - 1; m >= 0; m--)
                {
                    for (int n = MaximumRowNumber - 1; n >= 0; n--)
                    {
                        CellList[m, n].Devide();
                    }
                }
            }
        }


        public void PushSurroundCells(int columnNumber, int rowNumber, int direction)
        {
            List<Cell> pushedCells = new List<Cell>();
            int counter = -1;
            do
            {
                if (counter == -1)
                {
                    pushedCells.Add(CellList[columnNumber, rowNumber].GetSurroundCell(direction));
                }
                else
                {
                    pushedCells.Add(CellList[pushedCells[counter].ColumnNumber, pushedCells[counter].RowNumber].GetSurroundCell(direction));
                }
                counter++;
            }
            while (pushedCells[counter].CellStatus == 1);

            if(pushedCells[counter].OutOfField)
            {
                _timer.Stop();
                return;
            }

            for(int n = counter; n > 0; n--)
            {
                CellList[pushedCells[n].ColumnNumber, pushedCells[n].RowNumber].CopyCellPropertyFrom(pushedCells[n - 1]);



            }
        }









        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NextStatus(sender, e);
        }

        private void Btn_Start_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _timer.Start();
        }

        private void Btn_Stop_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _timer.Stop();
        }
    }
}
