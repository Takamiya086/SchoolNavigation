using System.Collections;
using System.Reflection.Metadata;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Globalization;

namespace SchoolNavigationSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 测试用数据
        public struct Attraction
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public Attraction(int x, int y)
            {
                Row = x;
                Column = y;
            }

            // 重载运算符
            public static bool operator ==(Attraction a, Attraction b)
            {
                if (a.Row == b.Row && a.Column == b.Column) return true;
                else return false;
            }
            public static bool operator !=(Attraction a, Attraction b)
            {
                return !(a == b);
            }
        } // 定义景点结构体

        #region 全局字段区

        internal Dictionary<String, String> adminList = new(); // 管理员用户集
        internal string[,] attractionInfo = new string[200, 220]; // 拿来存景点信息
        internal List<Attraction> storedAttractions = new(); // 存储已知节点
        internal Dictionary<Attraction, HashSet<Attraction>> storedRoads = new(); // 存储已知道路
        internal Dictionary<(int, int, int, int), string> roadInfo = new(); // 道路信息
        #endregion

        // 窗体构造函数
        public MainWindow()
        {
            // 测试用数据
            AddAttractionToData(45, 150, "邮电大学东区教学楼");
            AddAttractionToData(60, 160, "安美公寓楼");
            AddAttractionToData(70, 160, "安悦公寓楼");
            AddAttractionToData(100, 140, "东升食堂");
            AddAttractionToData(135, 130, "东区家属楼");
            AddAttractionToData(50, 50, "西区");

            AddRoadToData(45, 150, 60, 160, "安美-逸夫");
            AddRoadToData(60, 160, 70, 160, "安美-安悦");
            AddRoadToData(70, 160, 100, 140, "安悦-东升");
            AddRoadToData(100, 140, 135, 130, "东升-家属楼");
            AddRoadToData(45, 150, 100, 140, "东升-逸夫");
            AddRoadToData(60, 160, 100, 140, "安美-东升");
            AddRoadToData(70, 160, 135, 130, "安悦-家属楼");

            adminList["114514"] = "1919810"; // 测试用admin

            InitializeComponent();
            InitializeAtlas(200, 220); // 目前测试数据大小，大小差不多，大一点也可以，这个大小感觉稍微有点小，不过也可以用

            InitializeStoredAttractions();
            InitializeStoredRoads();

        }

        // 初始化地图函数 重复调用的情况出现于转换身份（可能）
        private void InitializeAtlas(int row, int column)
        {
            // 先清理网格，防止重复添加
            AtlasGrid.RowDefinitions.Clear();
            AtlasGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < row; i++)
            {
                AtlasGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < column; i++)
            {
                AtlasGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            return;
        }
        // 初始化已经存在的节点
        private void InitializeStoredAttractions()
        {
            foreach (Attraction item in storedAttractions)
            {
                AddAttractionToUI(item.Row, item.Column);
            }
            return;
        }
        // 初始化已经存在的道路
        private void InitializeStoredRoads()
        {
            foreach (var item in storedRoads)
            {
                foreach (var childItem in item.Value)
                {
                    AddRoadToUI(item.Key.Row, item.Key.Column, childItem.Row, childItem.Column);
                }
            }
            return;
        }

        // 地图点击事件·复合
        private void AtlasGrid_MouseDown(object sender, RoutedEventArgs e) { return; }
        private void AtlasGrid_MouseUp(object sender, RoutedEventArgs e)
        {
            LeftSiderBar.Visibility = Visibility.Visible;
            Info.Text = "点击一个地点可查看位置信息";
            return;
        }

        // 景点点击事件·复合 替换or添加 信息显示框的文字
        private void Attraction_MouseDown(object sender, RoutedEventArgs e) { return; }
        private void Attraction_MouseUp(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string targetInfo = attractionInfo[Grid.GetRow(button), Grid.GetColumn(button)];
            if (isAdmin)
            {
                LeftSiderBar.Visibility = Visibility.Visible;
                ResetAttraction.Visibility = Visibility.Visible;
                AddAttraction.Visibility = Visibility.Collapsed;
                LeftInfo.Visibility = Visibility.Collapsed;
                ResetRoad.Visibility = Visibility.Collapsed;
                AddRoad.Visibility = Visibility.Collapsed;

                RowPresent.Text = Grid.GetRow(button).ToString();
                ColumnPresent.Text = Grid.GetColumn(button).ToString();
            }
            else
            {
                LeftSiderBar.Visibility = Visibility.Visible;
                Info.Text = targetInfo;
            }
            return;
        }

        // 道路点击事件·复合 显示道路信息
        string focusLineTag;
        private void RoadLine_MouseDown(object sender, RoutedEventArgs e) { return; }
        private void RoadLine_MouseUp(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Line line = (Line)sender;
            if (isAdmin)
            {
                focusLineTag = line.Tag.ToString();
                AddX1.Text = line.X1.ToString();
                AddY1.Text = line.Y1.ToString();
                AddX2.Text = line.X2.ToString();
                AddY2.Text = line.Y2.ToString();
                ResetRoad.Visibility = Visibility.Visible;
                ResetAttraction.Visibility = Visibility.Collapsed;
                AddRoad.Visibility = Visibility.Collapsed;
                AddAttraction.Visibility = Visibility.Collapsed;
            }
            else
            {
                Info.Text = line.Tag.ToString();
            }
            return;
        }

        // 道路右键点击 右键点击两个点 会显示两个点之间的路径
        private bool _isSecondAttraction = false;
        private Button AttractionStart = new();
        private void Attraction_RightMouseDown(object sneder, RoutedEventArgs e)
        {
            e.Handled = true;

            if (isAdmin)
            {
                AddRoad.Visibility = Visibility.Visible;
                AddAttraction.Visibility = Visibility.Collapsed;
                ResetAttraction.Visibility = Visibility.Collapsed;
                ResetRoad.Visibility = Visibility.Collapsed;
            }
            if (!_isSecondAttraction)
            {
                AttractionStart = (Button)sneder;
                _isSecondAttraction = true;
            }
            else
            {
                _isSecondAttraction = false;
                Button AttractionEnd = (Button)sneder;

                // 搜索两点间的路径以及信息
                Attraction Start = new Attraction(Grid.GetRow(AttractionStart), Grid.GetColumn(AttractionStart));
                Attraction End = new Attraction(Grid.GetRow(AttractionEnd), Grid.GetColumn(AttractionEnd));

                if (isAdmin)
                {
                    AddX1.Text = Start.Column.ToString();
                    AddX2.Text = End.Column.ToString();
                    AddY1.Text = Start.Row.ToString();
                    AddY2.Text = End.Row.ToString();
                    return;
                }

                // 查找最短路----Dijkstra
                Dictionary<Attraction, int> distances = new Dictionary<Attraction, int>(); // Start到各节点的最短路
                Dictionary<Attraction, Attraction> previous = new Dictionary<Attraction, Attraction>(); // 存储前驱
                PriorityQueue<HashSet<Attraction>, int> priorityQueue = new();

                foreach (var node in storedRoads.Keys)
                {
                    distances[node] = int.MaxValue;
                }
                distances[Start] = 0;

                priorityQueue.Enqueue(new HashSet<Attraction> { Start }, 0);

                while (priorityQueue.Count > 0)
                {
                    HashSet<Attraction> currentNodes = priorityQueue.Peek();
                    Attraction currentNode = currentNodes.First();
                    currentNodes.Remove(currentNode);
                    if (currentNodes.Count == 0) priorityQueue.Dequeue();

                    // 如果当前节点已经是目标节点，停止搜索
                    if (currentNode == End)
                        break;
                    // 没有从这个点出发的路
                    if (!storedRoads.ContainsKey(currentNode))
                        break;

                    // 更新相邻节点的最短距离
                    foreach (var neighbor in storedRoads[currentNode])
                    {
                        int newDist = distances[currentNode] + 1;
                        if (newDist < distances[neighbor])
                        {
                            distances[neighbor] = newDist;
                            previous[neighbor] = currentNode;

                            // 将新的最短路径的节点添加到优先队列中
                            priorityQueue.Enqueue(new HashSet<Attraction>() { neighbor }, newDist);
                        }
                    }
                }

                // 构造路径
                var path = new List<Attraction>();
                var currentPathNode = End;
                while (currentPathNode != Start && previous.Count != 0)
                {
                    path.Insert(0, currentPathNode);
                    currentPathNode = previous.ContainsKey(currentPathNode) ? previous[currentPathNode] : default;
                }
                if (currentPathNode == Start)
                {
                    path.Insert(0, Start);
                }

                // 查找最短路的路径信息并且输出
                int cameRow = 0, cameCol = 0;
                Info.Text = string.Empty; // 先清空一下
                foreach (var item in path)
                {
                    if (item == path.First())
                    {
                        cameRow = item.Row;
                        cameCol = item.Column;
                        continue;
                    }
                    Info.Text += roadInfo[(cameRow, cameCol, item.Row, item.Column)] + "----->";
                    cameRow = item.Row;
                    cameCol = item.Column;
                }
                Info.Text += "End";
            }

            return;
        }

        // 用户名结束转换光标事件·Enter
        private void Username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginPassword.Focus();
            }
        }
        // 密码结束的登录事件·Enter 
        private void Password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 模拟鼠标点击事件（触发 PreviewMouseDown 和 PreviewMouseUp）
                var mouseDownEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                {
                    RoutedEvent = UIElement.PreviewMouseDownEvent
                };
                var mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                {
                    RoutedEvent = UIElement.PreviewMouseUpEvent
                };

                // 触发 PreviewMouseDown 和 PreviewMouseUp 事件
                LoginButton.RaiseEvent(mouseDownEvent);
                LoginButton.RaiseEvent(mouseUpEvent);
            }
        }

        // 登录按钮事件
        private void ResetLoginTextBox()
        {
            LoginPassword.Password = string.Empty;
            return;
        }
        private void LoginButton_MouseDown(object sender, RoutedEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#224989");
            LoginButton.Background = new SolidColorBrush(color);
            return;
        }
        private void LoginButton_MouseUp(object sender, RoutedEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#4583eb");
            LoginButton.Background = new SolidColorBrush(color);
            if (string.IsNullOrEmpty(LoginUserName.Text) || string.IsNullOrEmpty(LoginUserName.Text))
            {
                LoginMessage.Content = "必填项不能为空";
                ResetLoginTextBox();
            }
            if (adminList.ContainsKey(LoginUserName.Text))
            {
                if (adminList[LoginUserName.Text] == LoginPassword.Password)
                {
                    ResetLoginTextBox();
                    isAdmin = true;
                    BeforeLogin.Visibility = Visibility.Collapsed;
                    AfterLogin.Visibility = Visibility.Visible;
                    LeftInfo.Visibility = Visibility.Collapsed;
                    AddAttraction.Visibility = Visibility.Collapsed;
                    AddRoad.Visibility = Visibility.Collapsed;
                    // 登录之后 为地图订阅高权限的事件
                    AtlasGrid.MouseLeftButtonDown -= AtlasGrid_MouseDown;
                    AtlasGrid.MouseLeftButtonDown += AtlasGrid_Admin_MouseDown;
                    AtlasGrid.MouseLeftButtonUp -= AtlasGrid_MouseUp;
                    AtlasGrid.MouseLeftButtonUp += AtlasGrid_Admin_MouseUp;
                }
                else
                {
                    LoginMessage.Content = "密码错误";
                    ResetLoginTextBox();
                }
            }
            else
            {
                LoginMessage.Content = "用户名不存在";
                ResetLoginTextBox();

            }
            return;
        }

        // 注册按钮事件
        private void RegisterButton_MouseDown(object sender, RoutedEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#C4C5C5");
            RegisterButton.Background = new SolidColorBrush(color);
            return;
        }
        private void RegisterButton_MouseUp(object sender, RoutedEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#F6F9FB");
            RegisterButton.Background = new SolidColorBrush(color);

            LoginMessage.Content = "你没有权限";
            return;
        }

        // 登录后 退出按钮事件
        private void ExitButton_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#224989");
            ExitButton.Background = new SolidColorBrush(color);
            return;
        }
        private void ExitButton_MouseUp(Object sender, MouseButtonEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#4583eb");
            LoginButton.Background = new SolidColorBrush(color);
            AfterLogin.Visibility = Visibility.Collapsed;
            BeforeLogin.Visibility = Visibility.Visible;
            AddAttraction.Visibility = Visibility.Collapsed;
            LeftInfo.Visibility = Visibility.Visible;
            ResetAttraction.Visibility = Visibility.Collapsed;
            AddRoad.Visibility = Visibility.Collapsed;
            ResetRoad.Visibility = Visibility.Collapsed;
            AddRoad.Visibility = Visibility.Collapsed;

            // 退出后 订阅一个权限更低的事件
            isAdmin = false;
            AtlasGrid.MouseLeftButtonDown -= AtlasGrid_Admin_MouseDown;
            AtlasGrid.MouseLeftButtonDown += AtlasGrid_MouseDown;
            AtlasGrid.MouseLeftButtonUp -= AtlasGrid_Admin_MouseUp;
            AtlasGrid.MouseLeftButtonUp += AtlasGrid_MouseUp;
            return;
        }

        // 左边收起按钮
        private void LeftTrigger_MouseDown(object sender, RoutedEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#C84B4C");
            LeftTrigger.Background = new SolidColorBrush(color);
            return;
        }
        private void LeftTrigger_MouseUp(object sender, RoutedEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#E75759");
            LeftTrigger.Background = new SolidColorBrush(color);

            if (LeftSiderBar.Visibility == Visibility.Visible)
            {
                LeftSiderBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                LeftSiderBar.Visibility = Visibility.Visible;
            }
            return;
        }

        // 右边收起按钮
        private void RightTrigger_MouseDown(object sender, RoutedEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#56A536");
            RightTrigger.Background = new SolidColorBrush(color);
            return;
        }
        private void RightTrigger_MouseUp(object sender, RoutedEventArgs e)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#6ECE48");
            RightTrigger.Background = new SolidColorBrush(color);
            if (RightSiderBarLogin.Visibility == Visibility.Visible)
            {
                RightSiderBarLogin.Visibility = Visibility.Collapsed;
            }
            else
            {
                RightSiderBarLogin.Visibility = Visibility.Visible;
            }
            return;
        }

        #region Methods Only for Administrator

        private bool isAdmin = false; // 识别管理员身份 目前没什么用，感觉可能有用

        // 添加景点的函数·数据
        private void AddAttractionToData(int row, int col, string info)
        {
            storedAttractions.Add(new Attraction(row, col)); // 添加景点
            attractionInfo[row, col] = info; // 存储节点信息
        }
        // 添加景点的函数·UI
        private void AddAttractionToUI(int row, int col)
        {
            Button button = new Button { Content = "" };
            button.Style = (Style)this.Resources["Attraction"];
            Grid.SetRow(button, row);
            Grid.SetColumn(button, col);
            AtlasGrid.Children.Add(button);
            return;
        }

        // 添加道路的函数·数据
        private void AddRoadToData(int rowStart, int colStart, int rowEnd, int colEnd, string info)
        {
            Attraction one = new Attraction(rowStart, colStart);
            Attraction other = new Attraction(rowEnd, colEnd);
            // 检查路是不是已经存在
            if (storedRoads.ContainsKey(one))
            {
                if (storedRoads[one].Contains(other))
                {
                    // 道路是存在的
                    return;
                }
                else
                {
                    storedRoads[one].Add(other);
                }
            }
            else
            {
                storedRoads[one] = new HashSet<Attraction>();
                storedRoads[one].Add(other);
            }
            // 处理反向，保证无向
            if (storedRoads.ContainsKey(other))
            {
                storedRoads[other].Add(one);
            }
            else
            {
                storedRoads[other] = new HashSet<Attraction>();
                storedRoads[other].Add(one);
            }
            roadInfo[(one.Row, one.Column, other.Row, other.Column)] = info;
            roadInfo[(other.Row, other.Column, one.Row, one.Column)] = info;
            return;
        }

        // 添加道路的函数·UI
        private void AddRoadToUI(int rowStart, int colStart, int rowEnd, int colEnd)
        {
            Line line = new Line();
            line.Tag = roadInfo[(rowStart, colStart, rowEnd, colEnd)];
            line.Style = (Style)this.Resources["RoadLine"];
            AtlasGrid.SizeChanged += (sender, e) =>
            {
                double gridWidth = AtlasGrid.ActualWidth;
                double gridHeight = AtlasGrid.ActualHeight;
                // 本来不想用实际坐标的，结果还是要用
                line.X1 = colStart * (gridWidth / 220);
                line.Y1 = rowStart * (gridHeight / 200);

                line.X2 = colEnd * (gridWidth / 220);
                line.Y2 = rowEnd * (gridHeight / 200);
            };
            Grid.SetRowSpan(line, 200);
            Grid.SetColumnSpan(line, 220);
            AtlasGrid.Children.Add(line);
            return;
        }
        private void AddRoadToUI(int rowStart, int colStart, int rowEnd, int colEnd, bool isAfter)
        {
            Line line = new Line();
            line.Tag = roadInfo[(rowStart, colStart, rowEnd, colEnd)];
            line.Style = (Style)this.Resources["RoadLine"];

            double gridWidth = AtlasGrid.ActualWidth;
            double gridHeight = AtlasGrid.ActualHeight;
            // 本来不想用实际坐标的，结果还是要用
            line.X1 = colStart * (gridWidth / 220);
            line.Y1 = rowStart * (gridHeight / 200);

            line.X2 = colEnd * (gridWidth / 220);
            line.Y2 = rowEnd * (gridHeight / 200);
            Grid.SetRowSpan(line, 200);
            Grid.SetColumnSpan(line, 220);
            AtlasGrid.Children.Add(line);
            return;
        }
        // 地图点击事件·管理员
        private void AtlasGrid_Admin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LeftSiderBar.Visibility = Visibility.Visible;
            AddAttraction.Visibility = Visibility.Visible;
            return;
        }
        private void AtlasGrid_Admin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(AtlasGrid);
            int row = GetRowAtPosition(clickPosition);
            int column = GetColumnAtPosition(clickPosition);
            RowPresent.Text = row.ToString();
            ColumnPresent.Text = column.ToString();
            return;
        }
        // 地图点击辅助函数
        // 根据点击位置获取行
        private int GetRowAtPosition(Point position)
        {
            foreach (RowDefinition row in AtlasGrid.RowDefinitions)
            {
                // 获取行的区域
                double rowTop = AtlasGrid.RowDefinitions.TakeWhile(r => r != row).Sum(r => r.ActualHeight);
                double rowBottom = rowTop + row.ActualHeight;

                // 判断点击位置是否在该行区域内
                if (position.Y >= rowTop && position.Y <= rowBottom)
                {
                    return AtlasGrid.RowDefinitions.IndexOf(row);
                }
            }
            return -1;
        }
        // 根据点击位置获取列
        private int GetColumnAtPosition(Point position)
        {
            foreach (ColumnDefinition column in AtlasGrid.ColumnDefinitions)
            {
                // 获取列的区域
                double columnLeft = AtlasGrid.ColumnDefinitions.TakeWhile(c => c != column).Sum(c => c.ActualWidth);
                double columnRight = columnLeft + column.ActualWidth;

                // 判断点击位置是否在该列区域内
                if (position.X >= columnLeft && position.X <= columnRight)
                {
                    return AtlasGrid.ColumnDefinitions.IndexOf(column);
                }
            }
            return -1;
        }

        // 添加地点的确定按钮
        private void AddEnsureButton_Click(object sender, RoutedEventArgs e)
        {
            // 添加地点的信息不能为空
            if (AddInfo.Text == string.Empty)
            {
                return;
            }

            AddAttractionToData(int.Parse(RowPresent.Text), int.Parse(ColumnPresent.Text), AddInfo.Text);
            AddAttractionToUI(int.Parse(RowPresent.Text), int.Parse(ColumnPresent.Text));
            RowPresent.Text = string.Empty;
            ColumnPresent.Text = string.Empty;
            AddInfo.Text = string.Empty;
            return;
        }
        // 添加地点的取消按钮
        private void AddRejectButton_Click(object sender, RoutedEventArgs e)
        {
            RowPresent.Text = string.Empty;
            ColumnPresent.Text = string.Empty;
            AddInfo.Text = string.Empty;
            return;
        }
        // 修改地点的按钮
        private void ResetAttractionButton_Click(object sender, RoutedEventArgs e)
        {
            int row = int.Parse(RowPresent.Text);
            int column = int.Parse(ColumnPresent.Text);

            attractionInfo[row, column] = ResetInfo.Text;

            ResetInfo.Text = string.Empty;
            RowPresent.Text = string.Empty;
            ColumnPresent.Text = string.Empty;
            return;
        }
        // 删除地点的按钮
        private void DeleteAttractionButton_Click(object sender, RoutedEventArgs e)
        {
            int row = int.Parse(RowPresent.Text);
            int column = int.Parse(ColumnPresent.Text);

            attractionInfo[row, column] = string.Empty;

            foreach (var child in AtlasGrid.Children)
            {
                // 检查子控件是否是控件（例如Button），并且它的位置在指定的行列上
                if (child is UIElement element)
                {
                    int tarRow = Grid.GetRow(element);  // 获取元素的行
                    int tarColumn = Grid.GetColumn(element);  // 获取元素的列

                    // 如果行列匹配，移除该元素
                    if (tarRow == row && tarColumn == column)
                    {
                        AtlasGrid.Children.Remove(element);
                        break;
                    }
                }
            }
            return;
        }
        // 添加道路的确定按钮
        private void RoadEnsureButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddRoadInfo.Text == string.Empty)
            {
                AddX1.Text = string.Empty;
                AddY1.Text = string.Empty;
                AddX2.Text = string.Empty;
                AddY2.Text = string.Empty;
                AddRoadInfo.Text = string.Empty;
                return;
            }
            else
            {
                AddRoadToData(int.Parse(AddY1.Text), int.Parse(AddX1.Text), int.Parse(AddY2.Text), int.Parse(AddX2.Text), AddRoadInfo.Text);
                AddRoadToUI(int.Parse(AddY1.Text), int.Parse(AddX1.Text), int.Parse(AddY2.Text), int.Parse(AddX2.Text), true);
                return;
            }
        }
        // 添加道路的取消按钮
        private void RoadRejectButton_Click(object sender, RoutedEventArgs e)
        {
            AddX1.Text = string.Empty;
            AddY1.Text = string.Empty;
            AddX2.Text = string.Empty;
            AddY2.Text = string.Empty;
            return;
        }
        // 修改道路按钮
        private void ResetRoadButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in AtlasGrid.Children)
            {
                if (child is Line element && element.Tag.ToString() == focusLineTag && ResetRoadInfo.Text != string.Empty)
                {
                    element.Tag = ResetRoadInfo.Text;
                    break;
                }
            }
        }
        // 删除道路按钮
        private void DeleteRoadButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in AtlasGrid.Children)
            {
                if (child is Line element && element.Tag.ToString() == focusLineTag)
                {
                    AtlasGrid.Children.Remove(element);
                    break;
                }
            }
            return;
        }

        #endregion
    }
}
