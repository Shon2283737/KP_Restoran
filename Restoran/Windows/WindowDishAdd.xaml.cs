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
    /// Логика взаимодействия для WindowDishAdd.xaml
    /// </summary>
    public partial class WindowDishAdd : Window
    {
        public WindowDishAdd()
        {
            InitializeComponent();
        }

        RestoranEntities db = new RestoranEntities();

        public int idDish = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmbMenusUpdate();
            if (idDish == 0) btnDishRedact.IsEnabled = false;
        }

        public void cmbMenusUpdate()
        {
            foreach (var n in db.TbMenus)
            {
                cmbMenus.Items.Add(n.name);
            }
        }

        bool checkData()
        {
            bool success = true;
            if (cmbMenus.SelectedItem == null ||
                txtIngredients.Text == "" || txtName.Text == "" || txtPrice.Text == "")
                success = false;
            return success;

        }

        private void Button_Click_Dish_Add(object sender, RoutedEventArgs e)
        {
            if (checkData())
            {
                TbDishes dish = new TbDishes();
                dish.idMenu = idMenu();
                dish.name = txtName.Text;
                dish.price = Convert.ToInt32(txtPrice.Text);
                dish.ingredients = txtIngredients.Text;

                db.TbDishes.Add(dish);
                db.SaveChanges();
                MessageBox.Show("Блюдо добавлено.");
            }
            else MessageBox.Show("Заполните все поля.");
        }

        private void btnDishRedact_Click(object sender, RoutedEventArgs e)
        {
            if (checkData())
            {
                foreach (var n in db.TbDishes)
                {
                    if (n.id == idDish)
                    {
                        n.name = txtName.Text;
                        n.ingredients = txtIngredients.Text;
                        n.price = Convert.ToInt32(txtPrice.Text);

                        n.idMenu = idMenu();

                        db.Entry(n).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                        MessageBox.Show("Блюдо отредактировано.");
                        break;
                    }
                }
            }
            else MessageBox.Show("Заполните все поля.");
        }

        int idMenu() //Метод возвращает айди выбранного меню
        {
            int idMenu = 0;
            foreach (var n in db.TbMenus)
            {
                if (n.name == cmbMenus.SelectedItem.ToString())
                {
                    idMenu = n.id;
                    break;
                }
            }
            return idMenu;
        }
    }
}
