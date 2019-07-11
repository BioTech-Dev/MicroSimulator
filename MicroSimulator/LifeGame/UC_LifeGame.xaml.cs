using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace MicroSimulator
{
    public sealed partial class UC_LifeGame : UserControl
    {
        public int MaximumRowNumber { get; set; } = 100;
        public int MaximumColumnNumber { get; set; } = 150;
        public Cell[,] CellList { get; set; }  //セルを表す二次元配列、左上が原点、右/下がプラス
        public Cell Cell_OutOfField { get; set; } = new Cell();

        DispatcherTimer _timer = new DispatcherTimer();

        public UC_LifeGame()
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
                    CellList[m, n].UC_LifeGame = this;


                    CellList[m, n].CellSurface.SetValue(Grid.ColumnProperty, m);
                    CellList[m, n].CellSurface.SetValue(Grid.RowProperty, n);
                    Grid_Simulator.Children.Add(CellList[m, n].CellSurface);
                }
            }
        }


        private void NextStatus(object sender, object e)
        {
            for (int m = 0; m < MaximumColumnNumber; m++)
            {
                for (int n = 0; n < MaximumRowNumber; n++)
                {
                    CellList[m, n].SetNextCellStatus();
                }
            }

            for (int m = 0; m < MaximumColumnNumber; m++)
            {
                for (int n = 0; n < MaximumRowNumber; n++)
                {
                    CellList[m, n].UpdateCellStatus();
                }
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
