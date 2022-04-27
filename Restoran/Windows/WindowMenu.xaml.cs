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
    /// Логика взаимодействия для WindowMenu.xaml
    /// </summary>
    public partial class WindowMenu : Window
    {
        public WindowMenu()
        {
            InitializeComponent();
        }

        RestoranEntities db = new RestoranEntities();

        public string userRole = "";
        public string userNumberTable = "";
        public string userLastName = "";
        public string userFirstName = "";
        public string userPatronymic = "";

        int idDish = 0;
        int dishPrice = 0;
        string dishIngredients = "";
        int dishIdMenu = 0;
        int sum = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblUserFIO.Content = userLastName + " " + userFirstName + " " + userPatronymic;
            lblUserNumberTable.Content = "Номер столика официанта: "+userNumberTable;
            cmbMenuUpdate();
            cmbWaiterUpdate();
            listDishesUpdate();

            if(userRole == "waiter")
            {
                btnWindowDishesAdd.IsEnabled = false;
                btnWindowDishesAdd.Visibility = Visibility.Hidden;

                btnWindowKlients.IsEnabled = false;
                btnWindowKlients.Visibility = Visibility.Hidden;

                btnWindowMenuAdd.IsEnabled = false;
                btnWindowMenuAdd.Visibility = Visibility.Hidden;

                btnWindowOrders.IsEnabled = false;
                btnWindowOrders.Visibility = Visibility.Hidden;

                btnWindowUsers.IsEnabled = false;
                btnWindowUsers.Visibility = Visibility.Hidden;
            }
        }

        void cmbMenuUpdate()
        {
            cmbMenu.Items.Clear();
            cmbMenu.Items.Add("Все блюда");
            cmbMenu.SelectedItem = "Все блюда";
            foreach (var n in db.TbMenus)
            {
                cmbMenu.Items.Add(n.name);
            }
        }

        void cmbWaiterUpdate()
        {
            cmbWaiter.Items.Clear();
            foreach (var n in db.TbUsers) //Заполняем комбобокс офицантами
            {
                if(n.role == "waiter")
                cmbWaiter.Items.Add(n.lastName+" "+n.firstName);
            }
        }

        void SumOrder()
        {
            double sum = dishPrice;
            foreach (var n in db.TbKlients) //Считаем сумму заказа, учитывая скидку
            {
                if (txtKlientPhone.Text == n.phone)
                {
                    sum = Convert.ToDouble(dishPrice * ( (100 - n.discount) / 100 ));
                    break;
                }
            }
            lblPrice.Content = "Сумма заказа: " + sum + " р.";
        }

        void listDishesUpdate()
        {
            if (cmbMenu.SelectedItem.ToString() == "Все блюда") //Вывод всех блюд в список
            {
                var query = from d in db.TbDishes
                            select new
                            {
                                d.name,
                                price = d.price + "р.",
                                d.ingredients
                            };
                listDishes.ItemsSource = query.ToList();
            }
            else //Вывод блюд, входящих в меню
            {
                var query = from d in db.TbDishes
                            from m in db.TbMenus
                            where (m.name == cmbMenu.SelectedItem.ToString() && d.idMenu == m.id)
                            select new
                            {
                                d.name,
                                price = d.price + "р.",
                                d.ingredients
                            };
                listDishes.ItemsSource = query.ToList();
            }
        }

        private void listDishes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listDishes.SelectedItem != null)
            {
                var query = from d in db.TbDishes
                            select new
                            {
                                d.name,
                                price = d.price + "р.",
                                d.ingredients
                            };
            
                foreach (var n in query)
                {
                    if (n.ToString() == listDishes.SelectedItem.ToString()) //Сраниваем, чтобы найти выбранное блюдо
                    {
                        lblDishName.Content = n.name;
                        dishIngredients = n.ingredients;
                        break;
                    }
                }
                foreach (var n in db.TbDishes) //В query нету id и price, поэтому отдельно ищем их в базе
                {
                    if (n.name == lblDishName.Content.ToString())
                    {
                        idDish = n.id;
                        dishPrice = n.price;
                        SumOrder(); //Считаем сумму заказа
                        break;
                    }
                }
            }
        }

        private void Button_Click_Order_Add(object sender, RoutedEventArgs e)
        {
            if (checkData())
            {
                checkAddNewKlient(); //Проверка на нового пользователя
                int idUser = 0;
                foreach (var n in db.TbUsers) //Узнаём айди офицанта
                {
                    string userFI = n.lastName + " " + n.firstName;
                    if (userFI == cmbWaiter.SelectedItem.ToString())
                        idUser = n.id;
                }
                TbOrders order = new TbOrders();
                order.idDish = idDish;
                order.data = DateTime.Now.ToString();
                order.sum = sum;
                order.idKlient = idKlient();
                order.idUser = idUser;

                CountOrdersKlient(); //Обновляем кол-во заказов у клиента

                db.TbOrders.Add(order);
                db.SaveChanges();
                MessageBox.Show("Заказ добавлен.");
            }
            else MessageBox.Show("Заполните все поля.");
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            SumOrder();
        }

        private void cmbMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listDishesUpdate();
        }

        private void Button_Click_WindowUsers(object sender, RoutedEventArgs e)
        {
            WindowUsers w = new WindowUsers();
            w.role = userRole;
            w.ShowDialog();
        }

        private void Button_Click_WindowKlients(object sender, RoutedEventArgs e)
        {
            WindowKlients w = new WindowKlients();
            w.role = userRole;
            w.ShowDialog();
        }

        private void Button_Click_WindowOrders(object sender, RoutedEventArgs e)
        {
            WindowOrders w = new WindowOrders();
            w.role = userRole;
            w.ShowDialog();
        }

        bool checkData()
        {
            bool success = true;
            if (lblDishName == null || cmbWaiter.SelectedItem == null || txtKlientLastName.Text == "" ||
                txtKlientFirstName.Text == "" || txtKlientPhone.Text == "")
                success = false;
            return success;
            
        }

        private void btnWindowMenuAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowMenuAdd w = new WindowMenuAdd();
            if (cmbMenu.SelectedItem.ToString() != "Все блюда")
                w.idMenu = idMenu();
            w.ShowDialog();
        }

        private void btnWindowDishesAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowDishAdd w = new WindowDishAdd();
            if (listDishes.SelectedItem != null) //Есил выбрано блюдо, переносим его данные на окно редактирования
            {
                w.idDish = idDish;
                w.txtName.Text = lblDishName.Content.ToString();
                w.txtIngredients.Text = dishIngredients;
                w.cmbMenusUpdate(); //Обновляем комбобокс в новом окне, чтобы выбрать там нужное меню
                w.cmbMenus.SelectedItem = this.cmbMenu.SelectedItem;
                w.txtPrice.Text = dishPrice.ToString();
            }
            w.ShowDialog();
        }

        private void btnUpdateList_Click(object sender, RoutedEventArgs e)
        {
            listDishesUpdate();
        }

        void checkAddNewKlient() //Метод проверяет является ли клиент новым и добавляет его
        {
            bool newKlient = true;
            foreach (var n in db.TbKlients)
            {
                if (n.phone == txtKlientPhone.Text) newKlient = false; //ПРоверка на нового клиента
            }
            if(newKlient) //Добавление
            {
                TbKlients klient = new TbKlients();
                klient.lastName = txtKlientLastName.Text;
                klient.firstName = txtKlientFirstName.Text;
                klient.phone = txtKlientPhone.Text;

                db.TbKlients.Add(klient);
                db.SaveChanges();
                MessageBox.Show("Клиент добавлен.");
            }
        }

        int idKlient() //Метод вовзращает айди клиента, который указан в заказе
        {
            int idKlient = 0;
            foreach (var n in db.TbKlients.Where(i => i.phone == txtKlientPhone.Text))
            {
                idKlient = n.id;
            }
            return idKlient;
        }

        void CountOrdersKlient() //Метод обновляет кол-во заказов у клиента
        {
            int klient = idKlient();
            foreach (var n in db.TbKlients.Where(j => j.id == klient))
            {
                
                if (n.orders >= 10) n.discount = 10;

                db.Entry(n).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges();
        }

        int idMenu() //Метод возвращает айди выбранного меню
        {
            int idMenu = 0;
            foreach(var n in db.TbMenus)
            {
                if (cmbMenu.SelectedItem.ToString() == n.name) idMenu = n.id;
            }
            return idMenu;
        }
    }
}
