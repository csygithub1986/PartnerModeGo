using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PartnerModeGo
{
    /// <summary>
    /// 棋盘控件
    /// 自己维护StepNum
    /// </summary>
    public partial class Board : UserControl
    {
        public Board()
        {
            InitializeComponent();
        }

        #region 字段
        //Rectangle m_CurrentRect = new Rectangle();          // 当前标志
        //Rectangle[] m_DIanMuRects;   // 点目标志
        //Rectangle[] m_AffectRects;   // 点目标志

        //Step[,] m_Steps = new Step[19, 19];                 // 棋步，逻辑棋子
        //Step m_CurrentStep = null;

        //Stone[,] m_Stones = new Stone[19, 19];              // 棋子，含步数

        // 棋盘纵横线和星
        Line[] m_LinesH;
        Line[] m_LinesV;
        Ellipse[] m_Stars;
        Ellipse[,] m_Stones;
        Rectangle m_LastPlayStone;
        Rectangle[,] m_Affects;
        //Rectangle[,] m_PointChoises;
        //Image m_ImageAffects;//不用层绘制，反而没有空间效率高，不知道为什么


        double m_GridSize;//
        double m_StoneSize;// 棋子尺寸，根据棋盘计算
        double m_Offset;
        BoardState m_BoardState;

        IRule rule = new RuleDemo();//规则类

        /// <summary>
        /// 显示地域分析
        /// </summary>
        /// <param name="territory"></param>
        internal void ShowAffects(int[] territory)
        {
            //不用层绘制，反而没有空间效率高，不知道为什么
            //DrawingGroup dGroup = (m_ImageAffects.Source as DrawingImage).Drawing as DrawingGroup;
            //using (DrawingContext dc = dGroup.Open())
            //{
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    byte color = (byte)(255 - (territory[i + j * BoardSize] + 1000) / 2000.0 * 255);
                    byte alpha = (byte)(Math.Abs(color - 127.5) / 127.5 * (255 - 100) + 100);
                    m_Affects[i, j].Fill = new SolidColorBrush(Color.FromArgb(alpha, color, color, color));
                    //SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(alpha, color, color, color));
                    //Rect rect = new Rect(m_Offset + (i - 0.5) * m_GridSize, m_Offset + (j - 0.5) * m_GridSize, m_GridSize, m_GridSize);
                    //dc.DrawRectangle(brush, null, rect);

                }
            }
            //}
        }

        #endregion

        #region 事件
        public event Action<int, int, int, int> MousePlayEvent;
        #endregion

        #region 属性
        /// <summary>
        /// 棋盘大小 9~19
        /// </summary>
        public int BoardSize { get; set; }

        public BoardMode BoardMode { get; set; }
        public bool IsHostTurn;   //BoardMode是Playing时，判断是否是鼠标的轮子
        #endregion

        #region 公用方法

        public void SetStones(int[] state)
        {
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    m_Stones[x, y].Visibility = state[x * BoardSize + y] == 0 ? Visibility.Hidden : Visibility.Visible;
                    m_Stones[x, y].Fill = m_BoardState.Turn == 2 ? Brushes.Black : Brushes.White;
                }
            }

        }


        /// <summary>
        /// 添加棋子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">1为白，2为黑</param>
        public void AddStone(int x, int y, int color)
        {

        }

        /// <summary>
        /// 落子，和AddStone区别为，这里要判断打劫，提子，禁入点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color">1为白，2为黑</param>
        public void Play(int x, int y)
        {
            if (x < BoardSize && x >= 0 && y < BoardSize && y >= 0 && m_BoardState.State[x, y] == 0)
            {
                List<Position> list = null;
                //判断是否能落字
                if (BoardMode == BoardMode.Playing || BoardMode == BoardMode.AutoPlaying)
                {
                    list = rule.GetMoveResult(m_BoardState, new Position(x, y));
                    if (list == null)
                    {
                        return;
                    }
                }

                //处理界面
                m_Stones[x, y].Fill = m_BoardState.Turn == 2 ? Brushes.Black : Brushes.White;
                m_Stones[x, y].Visibility = Visibility.Visible;
                //highlightImage.Margin = new Thickness(m_Offset + x * m_GridSize - m_GridSize / 2, m_Offset + y * m_GridSize - m_GridSize / 2, 0, 0);
                //highlightImage.Visibility = Visibility.Visible;
                //更改游戏状态
                m_BoardState.State[x, y] = m_BoardState.Turn;
                m_BoardState.LastMove = new Position(x, y);
                m_BoardState.LastEatCount = list == null ? 0 : list.Count;
                m_BoardState.LastSetpNum++;

                Canvas.SetLeft(m_LastPlayStone, m_Offset + x * m_GridSize - m_LastPlayStone.Width / 2);
                Canvas.SetTop(m_LastPlayStone, m_Offset + y * m_GridSize - m_LastPlayStone.Height / 2);
                m_LastPlayStone.Visibility = Visibility.Visible;

                m_BoardState.Turn = 3 - m_BoardState.Turn;
                //吃子
                if (BoardMode == BoardMode.Playing || BoardMode == BoardMode.AutoPlaying)
                {
                    foreach (var p in list)
                    {
                        m_BoardState.State[p.X, p.Y] = 0;
                        m_Stones[p.X, p.Y].Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        #endregion

        public void Pass()
        {
            //更改游戏状态
            m_BoardState.LastMove = new Position(-1, -1);
            m_BoardState.LastEatCount = 0;
            m_BoardState.LastSetpNum++;
            m_BoardState.Turn = 3 - m_BoardState.Turn;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (BoardMode != BoardMode.Playing && BoardMode != BoardMode.Edit)
            {
                return;
            }
            if (BoardMode == BoardMode.Playing && IsHostTurn == false)
            {
                return;
            }
            //把屏幕坐标转化为落子坐标
            Point pos = e.GetPosition(m_Canvas);
            int x = (int)((pos.X - m_Offset + m_GridSize / 2) / m_GridSize);
            int y = (int)((pos.Y - m_Offset + m_GridSize / 2) / m_GridSize);

            if (x < BoardSize && x >= 0 && y < BoardSize && y >= 0 && m_BoardState.State[x, y] == 0)
            {
                List<Position> list = null;
                //判断是否能落字
                if (BoardMode == BoardMode.Playing)
                {
                    list = rule.GetMoveResult(m_BoardState, new Position(x, y));
                    if (list == null)
                    {
                        return;
                    }
                }

                //处理界面
                m_Stones[x, y].Fill = m_BoardState.Turn == 2 ? Brushes.Black : Brushes.White;
                m_Stones[x, y].Visibility = Visibility.Visible;
                //highlightImage.Margin = new Thickness(m_Offset + x * m_GridSize - m_GridSize / 2, m_Offset + y * m_GridSize - m_GridSize / 2, 0, 0);
                //highlightImage.Visibility = Visibility.Visible;
                //更改游戏状态
                m_BoardState.State[x, y] = m_BoardState.Turn;
                m_BoardState.LastMove = new Position(x, y);
                m_BoardState.LastEatCount = list == null ? 0 : list.Count;
                m_BoardState.LastSetpNum++;

                Canvas.SetLeft(m_LastPlayStone, m_Offset + x * m_GridSize - m_LastPlayStone.Width / 2);
                Canvas.SetTop(m_LastPlayStone, m_Offset + y * m_GridSize - m_LastPlayStone.Height / 2);
                m_LastPlayStone.Visibility = Visibility.Visible;

                //检查吃子
                if (BoardMode == BoardMode.Playing)
                {
                    foreach (var p in list)
                    {
                        m_BoardState.State[p.X, p.Y] = 0;
                        m_Stones[p.X, p.Y].Visibility = Visibility.Hidden;
                    }
                    IsHostTurn = false;
                    MousePlayEvent?.Invoke(m_BoardState.LastSetpNum, x, y, m_BoardState.Turn);
                }

                m_BoardState.Turn = 3 - m_BoardState.Turn;
            }
            else if (m_BoardState.State[x, y] != 0)
            {
                //ShowConnected(new Position(x, y));
            }
            e.Handled = true;
        }

        public void InitGame()
        {
            m_BoardState = new BoardState(BoardSize);
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    m_Stones[i, j].Visibility = Visibility.Hidden;
                }
            }
        }

        private void InitBoard()
        {
            m_GridSize = m_Canvas.ActualWidth / (BoardSize + 1);
            m_Offset = m_GridSize; ;
            m_StoneSize = m_GridSize * 0.95;

            m_BoardState = new BoardState(BoardSize);

            //线
            m_LinesH = new Line[BoardSize];
            m_LinesV = new Line[BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                m_LinesH[i] = new Line();
                m_LinesH[i].Stroke = Brushes.Black;
                m_Canvas.Children.Add(m_LinesH[i]);

                m_LinesV[i] = new Line();
                m_LinesV[i].Stroke = Brushes.Black;
                m_Canvas.Children.Add(m_LinesV[i]);
            }

            // 星
            //m_Stars = new Ellipse[9];
            //for (int i = 0; i < 9; i++)
            //{
            //    m_Stars[i] = new Ellipse();
            //    m_Stars[i].Fill = Brushes.Black;
            //    m_Canvas.Children.Add(m_Stars[i]);
            //}

            //棋子
            m_Stones = new Ellipse[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    m_Stones[i, j] = new Ellipse();
                    m_Stones[i, j].Width = m_StoneSize;
                    m_Stones[i, j].Height = m_StoneSize;

                    m_Stones[i, j].Visibility = Visibility.Hidden;
                    Canvas.SetTop(m_Stones[i, j], m_Offset + j * m_GridSize - m_StoneSize / 2);
                    Canvas.SetLeft(m_Stones[i, j], m_Offset + i * m_GridSize - m_StoneSize / 2);
                    m_Stones[i, j].StrokeThickness = 1;
                    m_Stones[i, j].Stroke = Brushes.Black;
                    //m_Stones[i].Fill = Brushes.Black;
                    m_Canvas.Children.Add(m_Stones[i, j]);
                }
            }

            //最后落子标记
            m_LastPlayStone = new Rectangle();
            m_LastPlayStone.Width = m_GridSize / 3;
            m_LastPlayStone.Height = m_GridSize / 3;
            m_LastPlayStone.Visibility = Visibility.Hidden;
            m_LastPlayStone.Fill = Brushes.Red;
            m_Canvas.Children.Add(m_LastPlayStone);
            //棋子
            //pieceImages = new Image[BoardSize, BoardSize];
            //for (int i = 0; i < BoardSize; i++)
            //{
            //    for (int j = 0; j < BoardSize; j++)
            //    {
            //        pieceImages[i, j] = new Image();
            //        pieceImages[i, j].Visibility = Visibility.Visible;
            //        pieceImages[i, j].Stretch = Stretch.Fill;
            //        pieceImages[i, j].Width = m_StoneSize;
            //        pieceImages[i, j].Height = m_StoneSize;
            //        Canvas.SetTop(pieceImages[i, j], m_Offset + j * m_GridSize - m_StoneSize / 2);
            //        Canvas.SetLeft(pieceImages[i, j], m_Offset + i * m_GridSize - m_StoneSize / 2);
            //        m_Canvas.Children.Add(pieceImages[i, j]);
            //    }
            //}

            //影响力区域
            m_Affects = new Rectangle[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    m_Affects[i, j] = new Rectangle();
                    m_Affects[i, j].Width = m_GridSize;
                    m_Affects[i, j].Height = m_GridSize;
                    Canvas.SetLeft(m_Affects[i, j], m_Offset + (i - 0.5) * m_GridSize);
                    Canvas.SetTop(m_Affects[i, j], m_Offset + (j - 0.5) * m_GridSize);
                    m_Affects[i, j].Fill = Brushes.Transparent;
                    m_Canvas.Children.Add(m_Affects[i, j]);
                }
            }

            //DrawingGroup dGroup = new DrawingGroup();
            //DrawingImage dImageSource = new DrawingImage(dGroup);
            //m_ImageAffects = new Image();
            //m_ImageAffects.Source = dImageSource;
            //m_Canvas.Children.Add(m_ImageAffects);
        }

        //画棋盘
        private void DrawBoard()
        {
            //线
            for (int i = 0; i < BoardSize; i++)
            {
                Line l = m_LinesH[i];
                double y = m_Offset + i * m_GridSize;
                l.X1 = m_Offset;
                l.Y1 = y;
                l.X2 = m_Offset + (BoardSize - 1) * m_GridSize;
                l.Y2 = y;

                l = m_LinesV[i];
                double x = m_Offset + i * m_GridSize;
                l.X1 = x;
                l.Y1 = m_Offset;
                l.X2 = x;
                l.Y2 = m_Offset + (BoardSize - 1) * m_GridSize;
            }

            // 画星
            //for (int j = 0; j < 3; j++)
            //{
            //    for (int i = 0; i < 3; i++)
            //    {
            //        Ellipse e = m_Stars[i+3*j];
            //        e.Width = e.Height = m_GridSize / 5;
            //        double left = 4 * m_GridSize + j * 6 * m_GridSize - m_GridSize / 2 - e.Width / 2;
            //        double top = 4 * m_GridSize + i * 6 * m_GridSize - m_GridSize / 2 - e.Height / 2;
            //        Canvas.SetLeft(e, left);
            //        Canvas.SetTop(e, top);
            //    }
            //}

            //画棋子
            //for (int i = 0; i < m_Stones.Length; i++)
            //{
            //    Canvas.SetLeft(m_Stones[i], (i % BoardSize + 1) * m_GridSize);
            //    Canvas.SetTop(m_Stones[i], (i / BoardSize + 1) * m_GridSize);
            //}
        }

        /// <summary>
        /// 自适应大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (double.IsNaN(ActualWidth) || ActualWidth == 0 || double.IsNaN(ActualHeight) || ActualHeight == 0)
            //{
            //    return;
            //}
            //if (ActualWidth > ActualHeight)
            //{
            //    m_ViewBox.Width = ActualHeight;
            //    m_ViewBox.Height = ActualHeight;
            //}
            //else
            //{
            //    m_ViewBox.Width = ActualWidth;
            //    m_ViewBox.Height = ActualWidth;
            //}
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitBoard();
            DrawBoard();
        }
    }

}
