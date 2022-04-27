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
    /// Логика взаимодействия для WindowKlients.xaml
    /// </summary>
    public partial class WindowKlients : Window
    {
        public WindowKlients()
        {
            InitializeComponent();
        }

        RestoranEntities db = new RestoranEntities();

        public string role = "";
        int idKlient = 0;

        void listKlientUpdate()
        {
                var query = from k in db.TbKlients
                            select new
                            {
                                FI = k.lastName + k.firstName,
                                k.phone
                            };
                listKlient.ItemsSource = query.ToList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listKlientUpdate();
            if (role != "admin") btnKlientRedact.IsEnabled = false;
        }

        private void listKlient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listKlient.SelectedItem != null)
            {

                var query = from k in db.TbKlients
                            select new
                            {
                                FI = k.lastName + k.firstName,
                                k.phone
                            };

                foreach (var n in query)
                {
                    if (n.ToString() == listKlient.SelectedItem.ToString()) //Сравниваем, чтобы найти выделенного клиента
                    {
                        txtPhone.Text = n.phone;
                        break;
                    }
                }
                foreach (var n in db.TbKlients) //Отдельно ищем остальные данные, поскольку их нету в query
                {
                    if (n.phone == txtPhone.Text)
                    {
                        idKlient = n.id;
                        txtLastName.Text = n.lastName;
                        txtFirstName.Text = n.firstName;
                        txtOrders.Text = n.orders.ToString();
                        txtDiscount.Text = n.discount.ToString();
                        break;
                    }
                }
            }
            else 
            {
                btnKlientRedact.IsEnabled = false; //Клиент не выбран, выключаем кнопку
            }
        }

        private void Button_Click_Klient_Add(object sender, RoutedEventArgs e)
        {
            if (checkData())
            {
                TbKlients klient = new TbKlients();
                klient.lastName = txtLastName.Text;
                klient.firstName = txtFirstName.Text;
                klient.phone = txtPhone.Text;
                klient.orders = Convert.ToInt32(txtOrders.Text);
                klient.discount = Convert.ToInt32(txtDiscount.Text);

                db.TbKlients.Add(klient);
                db.SaveChanges();
                MessageBox.Show("Клиент добавлен.");
            }
            else MessageBox.Show("Заполните все поля.");
        }
        bool checkData()
        {
            bool success = true;
            if (txtLastName.Text == "" || txtFirstName.Text == "" ||
                txtPhone.Text == "" || txtOrders.Text == "" || txtDiscount.Text == "")
                success = false;
            return success;

        }
        private void btnKlientRedact_Click(object sender, RoutedEventArgs e)
        {
            if (checkData() & listKlient.SelectedItem != null)
            {
                foreach (var n in db.TbKlients)
                {
                    if (n.id == idKlient)
                    {
                        n.lastName = txtLastName.Text;
                        n.firstName = txtFirstName.Text;
                        n.phone = txtPhone.Text;
                        n.orders = Convert.ToInt32(txtOrders.Text);
                        n.discount = Convert.ToInt32(txtDiscount.Text);

                        db.Entry(n).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                        MessageBox.Show("Клиент отредактирован.");
                        break;
                    }
                }
            }
            else MessageBox.Show("Заполните все поля.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listKlientUpdate();
        }
    }
}
