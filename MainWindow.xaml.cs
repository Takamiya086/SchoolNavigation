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
        internal Dictionary<String, String> adminList = new(); // 管理员用户集

        public MainWindow()
        {
            InitializeComponent();
            InitializeAtlas(200, 200);
            SetPosition(49, 49);
        }

        // 地图点击显示信息测试
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

        // 添加地点标记
        private void SetPosition(int row, int column)
        {
            Button button = new Button
            {
                Content = "Damn"
            };
            button.Style = (Style)this.Resources["PositionMark"];
            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            AtlasGrid.Children.Add(button);
            return;
        }

        private void Canvas_MouseDown(object sender, RoutedEventArgs e)
        {
            return;
        }
        private void Canvas_MouseUp(object sender, RoutedEventArgs e)
        {
            Info.Text = "111";
            return;
        }
        private void Position_MouseDown(object sender, RoutedEventArgs e)
        {
            return;
        }
        private void Position_MouseUp(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Info.Text))
            {
                Info.Text = "你说得对，但是西安邮电大学是一款我的问题，这两年来给我带来很多乐趣。\n哦，我的邮宝，爱你哦。";
                return;
            }
            else
            {
                Info.Text = "";
                return;
            }
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

            if (LeftSiderBarLogin.Visibility == Visibility.Visible)
            {
                LeftSiderBarLogin.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                LeftSiderBarLogin.Visibility = Visibility.Visible;
                return;
            }
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
                return;
            }
            else
            {
                RightSiderBarLogin.Visibility = Visibility.Visible;
                return;
            }
        }
    }
}