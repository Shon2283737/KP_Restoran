using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Restoran.Windows;

namespace Restoran
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RestoranEntities db = new RestoranEntities();
        public MainWindow()
        {
            InitializeComponent();
        }

        

        private void Button_Click_Authorization(object sender, RoutedEventArgs e)
        {
            foreach (var n in db.TbUsers)
            {
                if(n.login == txtLogin.Text && n.password == txtPassword.Text)
                {
                    WindowMenu w = new WindowMenu();
                    w.userLastName = n.lastName;
                    w.userFirstName = n.firstName;
                    w.userPatronymic = n.patronymic;
                    w.userRole = n.role;
                    w.userNumberTable = n.numberTable;
                    w.Show();
                    this.Close();
                    break;
                }
                
            }
            MessageBox.Show("Неправильный логин или пароль.");
        }
    }
}
