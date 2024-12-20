using System.Collections;
using System.Reflection.Metadata;
using System.Text;
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
        } // 定义景点结构体

        #region 全局字段区

        internal Dictionary<String, String> adminList = new(); // 管理员用户集
        internal string[,] attractionInfo = new string[200, 220]; // 拿来存景点信息
        internal List<Attraction> storedAttractions = new(); // 存储已知节点

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

            adminList["114514"] = "1919810"; // 测试用admin

            InitializeComponent();
            InitializeAtlas(200, 220); // 目前测试数据大小，大小差不多，大一点也可以，这个大小感觉稍微有点小，不过也可以用

            // 计算单元格的实际坐标
            double gridWidth = AtlasGrid.ActualWidth;
            double gridHeight = AtlasGrid.ActualHeight;
            InitializeStoredAttractions();

            // 懒得描真实路线了，还是直接画直线吧

            Line line = new Line
            {
                StrokeThickness = 3,
                Stroke = Brushes.CornflowerBlue
            };

            // 连接教学楼和安美公寓的一条线测试
            AtlasGrid.SizeChanged += (sender, e) =>
            {
                // 本来不想用实际坐标的，结果还是要用
                double startX = 150 * (gridWidth / 220);
                double startY = 45 * (gridHeight / 200);

                double endX = 160 * (gridWidth / 220);
                double endY = 60 * (gridHeight / 200);

                line.X1 = startX;
                line.Y1 = startY;
                line.X2 = endX;
                line.Y2 = endY;
            };
            Grid.SetRowSpan(line, 200);
            Grid.SetColumnSpan(line, 220);

            AtlasGrid.Children.Add(line);
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
        // 初始化已经存在的节点 TODO 整合到初始化地图的函数里面
        private void InitializeStoredAttractions()
        {
            foreach (Attraction item in storedAttractions)
            {
                AddAttractionToUI(item.Row, item.Column);
            }
            return;
        }

        // 地图点击事件·用户
        private void AtlasGrid_MouseDown(object sender, RoutedEventArgs e) { return; }
        private void AtlasGrid_MouseUp(object sender, RoutedEventArgs e)
        {
            LeftSiderBar.Visibility = Visibility.Visible;
            Info.Text = "点击一个地点可查看位置信息";
            return;
        }

        // 景点被点击时 替换or添加 信息显示框的文字
        private void Attraction_MouseDown(object sender, RoutedEventArgs e) { return; }
        private void Attraction_MouseUp(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string targetInfo = attractionInfo[Grid.GetRow(button), Grid.GetColumn(button)];
            LeftSiderBar.Visibility = Visibility.Visible;
            Info.Text = targetInfo;
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
                    BeforeLogin.Visibility = Visibility.Collapsed;
                    AfterLogin.Visibility = Visibility.Visible;
                    // 登录之后 为地图订阅高权限的事件
                    AtlasGrid.MouseDown -= AtlasGrid_MouseDown;
                    AtlasGrid.MouseDown += AtlasGrid_Admin_MouseDown;
                    AtlasGrid.MouseUp -= AtlasGrid_MouseUp;
                    AtlasGrid.MouseUp += AtlasGrid_Admin_MouseUp;
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

            // 退出后 订阅一个权限更低的事件
            AtlasGrid.MouseDown -= AtlasGrid_Admin_MouseDown;
            AtlasGrid.MouseDown += AtlasGrid_MouseDown;
            AtlasGrid.MouseUp -= AtlasGrid_Admin_MouseUp;
            AtlasGrid.MouseUp += AtlasGrid_MouseUp;
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

        // 道路点击事件
        private void RoadLine_MouseDown(object sender, RoutedEventArgs e)
        {
            // TODO
            return;
        }
        private void RoadLine_MouseUp(object sender, RoutedEventArgs e)
        {
            // TODO
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
        private void AddRoadToData()
        {
            // TODO 将测试数据封装
            return;
        }
        // 添加道路的函数·UI
        private void AddRoadToUI()
        {
            // TODO 将测试数据封装
            return;
        }

        // 地图点击事件·管理员
        private void AtlasGrid_Admin_MouseDown(object sender, RoutedEventArgs e)
        {
            return;
        }
        private void AtlasGrid_Admin_MouseUp(object sender, RoutedEventArgs e)
        {
            return;
        }

        #endregion

    }
}
