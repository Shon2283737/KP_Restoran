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
    /// Логика взаимодействия для WindowOrders.xaml
    /// </summary>
    public partial class WindowOrders : Window
    {
        public WindowOrders()
        {
            InitializeComponent();
        }

        RestoranEntities db = new RestoranEntities();

        public string role = "";
        int idOrder = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listOrdersUpdate();
            cmbDishUpdate();
            cmbKlientUpdate();
            cmbWaiterUpdate();
            if (role != "admin") btnRedact.IsEnabled = false;
        }

        void listOrdersUpdate()
        {
                var query = from o in db.TbOrders
                            from d in db.TbDishes
                            where (o.idDish == d.id)
                            select new
                            {
                                d.name,
                                o.data,
                                sum = d.price + " р."
                            };
                listOrders.ItemsSource = query.ToList();
        }

        void cmbDishUpdate()
        {
            cmbNameDish.Items.Clear();
            foreach (var n in db.TbDishes)
            {
                cmbNameDish.Items.Add(n.name);
            }
        }
        void cmbKlientUpdate()
        {
            cmbKlient.Items.Clear();
            foreach (var n in db.TbKlients)
            {
                cmbKlient.Items.Add(n.lastName);
            }
        }
        void cmbWaiterUpdate()
        {
            cmbWaiter.Items.Clear();
            foreach (var n in db.TbUsers)
            {
                if (n.role == "waiter")
                    cmbWaiter.Items.Add(n.lastName);
            }
        }

        private void listOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOrders.SelectedItem != null)
            {
                var query = from o in db.TbOrders
                            from d in db.TbDishes
                            where (o.idDish == d.id)
                            select new
                            {
                                d.name,
                                o.data,
                                sum = d.price + " р."
                            };

                foreach (var n in query)
                {
                    if (n.ToString() == listOrders.SelectedItem.ToString()) //Сраниваем, чтобы найти дату и блюдо заказа
                    {
                        txtDate.Text = n.data;
                        cmbNameDish.SelectedItem = n.name;
                        break;
                    }
                }
                foreach (var n in db.TbOrders) //В query не все данные, поэтому при помощи даты и блюда ищем остальные данные в базе
                {
                    if (n.data == txtDate.Text)
                    {
                        txtSum.Text = n.sum.ToString(); ;
                        idOrder = n.id;

                        //нужные данные заносим в комбобоксы
                        foreach(var k in db.TbKlients)
                        {
                            if (k.id == n.idKlient) cmbKlient.SelectedItem = k.lastName;
                        }

                        foreach (var u in db.TbUsers)
                        {
                            if (u.id == n.idUser) cmbWaiter.SelectedItem = u.lastName;
                        }
                        break;
                    }
                }
            }
        }

        private void Button_Click_Order_Add(object sender, RoutedEventArgs e)
        {
            if (checkData())
            {
                int idDish = 0;
                int idUser = 0;
                int idKlient = 0;
                foreach (var n in db.TbUsers) //узнаём айди выбранных вещей в комбобоксах
                {
                    if (n.lastName == cmbWaiter.SelectedItem.ToString())
                        idUser = n.id;
                }
                foreach (var n in db.TbKlients)
                {
                    if (n.lastName == cmbKlient.SelectedItem.ToString())
                        idKlient = n.id;
                }
                foreach (var n in db.TbDishes)
                {
                    if (n.name == cmbNameDish.SelectedItem.ToString())
                        idDish = n.id;
                }
                TbOrders order = new TbOrders();
                order.idDish = idDish;
                order.data = txtDate.Text;
                order.sum = Convert.ToInt32(txtSum.Text);
                order.idKlient = idKlient;
                order.idUser = idUser;


                db.TbOrders.Add(order);
                db.SaveChanges();
                MessageBox.Show("Заказ добавлен.");
            }
            else MessageBox.Show("Заполните все поля.");
        }

        private void Button_Click_listOrdersUpdate(object sender, RoutedEventArgs e)
        {
            listOrdersUpdate();
        }
        bool checkData()
        {
            bool success = true;
            if (cmbNameDish.SelectedItem == null || cmbWaiter.SelectedItem == null || cmbKlient.SelectedItem == null ||
                txtDate.Text == "" || txtSum.Text == "" )
                success = false;
            return success;

        }

        private void btnRedact_Click(object sender, RoutedEventArgs e)
        {
            if (checkData() & (listOrders.SelectedItem != null))
            {
                int idDish = 0;
                int idUser = 0;
                int idKlient = 0;
                foreach (var n in db.TbUsers) //узнаём айди выбранных вещей в комбобоксах
                {
                    if (n.lastName == cmbWaiter.SelectedItem.ToString())
                        idUser = n.id;
                }
                foreach (var n in db.TbKlients)
                {
                    if (n.lastName == cmbKlient.SelectedItem.ToString())
                        idKlient = n.id;
                }
                foreach (var n in db.TbDishes)
                {
                    if (n.name == cmbNameDish.SelectedItem.ToString())
                        idDish = n.id;
                }

                foreach (var n in db.TbOrders)
                {
                    if (n.id == idOrder)
                    {
                        n.idDish = idDish;
                        n.data = txtDate.Text;
                        n.sum = Convert.ToInt32(txtSum.Text);
                        n.idKlient = idKlient;
                        n.idUser = idUser;

                        db.Entry(n).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                        MessageBox.Show("Заказ отредактирован.");
                        break;
                    }
                }
            }
            else MessageBox.Show("Заполните все поля и выберите заказ из списка.");
        }
    }
}
