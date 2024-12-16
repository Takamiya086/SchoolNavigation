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
        // TODO 之后要设置文件来对数据进行存储
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

        internal Dictionary<String, String> adminList = new(); // 管理员用户集
        internal string[,] attractionInfo = new string[200, 220]; // 拿来存景点信息
        internal List<Attraction> storedAttractions = new(); // 存储已知节点

        // 窗体构造函数
        public MainWindow()
        {
            AddAttractionToData(45, 150, "邮电大学东区教学楼"); // TODO Pin图片还得扣一下
            AddAttractionToData(60, 160, "安美公寓楼");
            AddAttractionToData(70, 160, "安悦公寓楼");
            AddAttractionToData(100, 140, "东升食堂");
            AddAttractionToData(135, 130, "东区家属楼");

            InitializeComponent();
            InitializeAtlas(200, 220); // 目前测试数据大小，大小差不多，大一点也可以，这个大小感觉稍微有点小，不过也可以用
            InitializeStoredAttractions();
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

        // 登录按钮事件
        // TODO 可选，封装到按钮事件里面，除了按钮函数以外都不用这个函数。或者可以直接不清除，让用户自己删
        private void ResetLoginTextBox()
        {
            LoginUserName.Text = string.Empty;
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
                return;
            }

            if (adminList.ContainsKey(LoginUserName.Text))
            {
                if (adminList[LoginUserName.Text] == LoginPassword.Password)
                {
                    // TODO登录成功 重新加载界面
                    return;
                }
                else
                {
                    LoginMessage.Content = "密码错误";
                    ResetLoginTextBox();
                    return;
                }
            }
            else
            {
                LoginMessage.Content = "用户名不存在";
                ResetLoginTextBox();
                return;
            }
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

        // 地图点击事件·管理员
        private void AtlasGrid_Admin_MouseDown()
        {
            return;
        }
        private void AtlasGrid_Admin_MouseUp()
        {
            return;
        }
        #endregion
    }
}
