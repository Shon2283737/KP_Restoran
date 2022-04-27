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
    /// Логика взаимодействия для WindowUsers.xaml
    /// </summary>
    public partial class WindowUsers : Window
    {
        public WindowUsers()
        {
            InitializeComponent();
        }

        RestoranEntities db = new RestoranEntities();

        int idUser = 0;
        public string role = "";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listUsersUpdate();
            if (role != "admin")
            {
                txtLastName.IsEnabled = false;
                txtfirstName.IsEnabled = false;
                txtPatronymic.IsEnabled = false;
                txtLogin.IsEnabled = false;
                txtPassword.IsEnabled = false;
                txtPhone.IsEnabled = false;
                txtRole.IsEnabled = false;
            }
        }

        void listUsersUpdate()
        {
            var query = from u in db.TbUsers
                        select new
                        {
                            u.id,
                            FIO = u.lastName + " " + u.firstName + " " + u.patronymic,
                            u.phone,
                            u.role
                        };
            listUsers.ItemsSource = query.ToList();
        }

        private void Button_Click_UpdateList(object sender, RoutedEventArgs e)
        {
            listUsersUpdate();
        }

        private void listUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listUsers.SelectedItem != null)
            {
                var query = from u in db.TbUsers
                            select new
                            {
                                u.id,
                                FIO = u.lastName + " " + u.firstName + " " + u.patronymic,
                                u.phone,
                                u.role
                            };

                foreach (var n in query)
                {
                    if (n.ToString() == listUsers.SelectedItem.ToString()) //Сраниваем, чтобы найти выбранного сотрудника
                    {
                        idUser = n.id;
                        foreach(var u in db.TbUsers)
                        {
                            if(n.id == u.id)
                            {
                                txtLastName.Text = u.lastName;
                                txtfirstName.Text = u.firstName;
                                txtPatronymic.Text = u.patronymic;
                                txtLogin.Text = u.login;
                                txtPassword.Text = u.password;
                                txtPhone.Text = u.phone;
                                txtRole.Text = u.role;
                                txtNumberTable.Text = u.numberTable;
                            }
                            break;
                        }
                        break;
                    }
                }
            }
        }

        private void Button_Click_User_Add(object sender, RoutedEventArgs e)
        {
            if (checkData())
            {
                TbUsers user = new TbUsers();
                user.lastName = txtLastName.Text;
                user.firstName = txtfirstName.Text;
                user.patronymic = txtPatronymic.Text;
                user.phone = txtPhone.Text;
                user.role = txtRole.Text;
                user.login = txtLogin.Text;
                user.password = txtPassword.Text;
                user.numberTable = txtNumberTable.Text;


                db.TbUsers.Add(user);
                db.SaveChanges();
                MessageBox.Show("Сотрудник добавлен.");
            }
            else MessageBox.Show("Заполните все поля.");
        }
        bool checkData()
        {
            bool success = true;
            if (txtLastName.Text == "" || txtfirstName.Text == "" || txtPatronymic.Text == "" || txtPhone.Text == ""||
                txtLogin.Text == "" || txtPassword.Text == "" || txtRole.Text == "")
                success = false;
            return success;

        }

        private void btnRedact_Click(object sender, RoutedEventArgs e)
        {
            if (checkData() & (listUsers.SelectedItem != null))
            {
                foreach (var n in db.TbUsers)
                {
                    if (n.id == idUser)
                    {
                        TbUsers u = new TbUsers();
                        u.lastName = txtLastName.Text;
                        u.firstName = txtfirstName.Text;
                        u.patronymic = txtPatronymic.Text;
                        u.phone = txtPhone.Text;
                        u.role = txtRole.Text;
                        u.login = txtLogin.Text;
                        u.password = txtPassword.Text;
                        u.numberTable = txtNumberTable.Text;

                        db.Entry(n).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                        MessageBox.Show("Сотрудник отредактирован.");
                        break;
                    }
                }
            }
            else MessageBox.Show("Заполните все поля и выберите сотрудника из списка.");
        }
    }
}
