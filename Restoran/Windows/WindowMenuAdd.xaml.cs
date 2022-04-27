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
using System.Windows.Shapes;

namespace Restoran.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowMenuAdd.xaml
    /// </summary>
    public partial class WindowMenuAdd : Window
    {
        RestoranEntities db = new RestoranEntities();
        public WindowMenuAdd()
        {
            InitializeComponent();
        }

        public int idMenu = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (idMenu == 0) btnMenuRedact.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text != "")
            {
                TbMenus menu = new TbMenus();
                menu.name = txtName.Text;

                db.TbMenus.Add(menu);
                db.SaveChanges();
                MessageBox.Show("Меню добавлено.");
            }
            else MessageBox.Show("Заполните все поля.");
        }

        private void btnMenuRedact_Click(object sender, RoutedEventArgs e)
        {
            foreach (var n in db.TbMenus)
            {
                if (n.id == idMenu)
                {
                    n.name= txtName.Text;

                    db.Entry(n).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                    break;
                }
            }
        }
    }
}
