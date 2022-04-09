using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Travel_Agent_Ticketing
{
    public partial class ViewForm : Form
    {
        public ViewForm()
        {
            InitializeComponent();
        }

        private string database;
        private SqlConnection connection;
        private SqlDataAdapter dataAdapter;
        private DataSet dataSet;
        private SqlDataReader dataReader;
        private SqlCommand command;

        private string pickRouteId;
        private string price;
        private string pickdepartureDate;

        public void Connect()
        {
            database = "Data Source = .\\SQLEXPRESS; Initial Catalog = Travel; Integrated Security = True";
            connection = new SqlConnection(database);
        }

        public void StartFromComboBox()
        {
            Connect();
            connection.Open();
            dataAdapter = new SqlDataAdapter("SELECT * FROM city", connection);
            dataSet = new DataSet();
            dataSet.Clear();
            dataAdapter.Fill(dataSet, "city");
            comboBox1.DisplayMember = "city_name";
            comboBox1.ValueMember = "id";
            comboBox1.DataSource = dataSet.Tables["city"];
            connection.Close();
        }

        public void StartToComboBox(string selectedFromCity)
        {
            Connect();
            connection.Open();
            dataAdapter = new SqlDataAdapter("SELECT  city.id, city.city_name FROM route JOIN city ON route.to_city_id = city.id WHERE from_city_id = '" + selectedFromCity + "'", connection);
            dataSet = new DataSet();
            dataSet.Clear();
            dataAdapter.Fill(dataSet, "city");
            comboBox2.DisplayMember = "city_name";
            comboBox2.ValueMember = "id";
            comboBox2.DataSource = dataSet.Tables["city"];
            connection.Close();
        }

        public void StartDataGridView()
        {
            dataGridView1.Rows.Clear();
            Connect();
            connection.Open();
            command = new SqlCommand("SELECT booking.id, DATEPART(dd, booking.booking_date) AS booking_day, DATEPART(mm, booking.booking_date) AS booking_date, DATEPART(yy, booking.booking_date) AS booking_year, city.city_name AS fromCity, c.city_name AS toCity, customer.customer_name, customer.phone, DATEPART(dd, booking.departure_date) AS departure_day, DATEPART(mm, booking.departure_date) AS departure_date, DATEPART(yyyy, booking.departure_date) AS departure_year, booking.booking_code, booking.price, schedule.id As scheduleId FROM booking JOIN schedule ON booking.schedule_id = schedule.id JOIN route ON schedule.route_id = route.id JOIN city ON route.from_city_id = city.id JOIN city c ON route.to_city_id = c.id JOIN customer ON booking.customer_id = customer.id ORDER BY booking.id DESC", connection);
            dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    string bookingDay = dataReader["booking_day"].ToString();
                    if (bookingDay.Length != 2)
                    {
                        bookingDay = "0" + bookingDay;
                    }
                    string bookingDate = dataReader["booking_date"].ToString();
                    if (bookingDate.Length != 2)
                    {
                        bookingDate = "0" + bookingDate;
                    }
                    string bookingYear = dataReader["booking_year"].ToString();

                    string bookingDateFinal = bookingDay + "/" + bookingDate + "/" + bookingYear;

                    string departureDay = dataReader["departure_day"].ToString();
                    if (departureDay.Length != 2)
                    {
                        departureDay = "0" + departureDay;
                    }
                    string departureDate = dataReader["departure_date"].ToString();
                    if (departureDate.Length != 2)
                    {
                        departureDate = "0" + departureDate;
                    }
                    string departureYear = dataReader["booking_year"].ToString();

                    string departureDateFinal = departureDay + "/" + departureDate + "/" + departureYear;

                    string[] row = new string[] { dataReader["id"].ToString(), bookingDateFinal, dataReader["fromCity"].ToString(), dataReader["toCity"].ToString(), dataReader["customer_name"].ToString(), dataReader["phone"].ToString(), departureDateFinal, dataReader["booking_code"].ToString(), dataReader["price"].ToString(), dataReader["scheduleId"].ToString() };
                    dataGridView1.Rows.Add(row);
                }
            }
            else
            {
                MessageBox.Show("Booking Error!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }

        public void StartCondition()
        {
            dataGridView1.ColumnCount = 10;
            dataGridView1.Columns[0].Name = "Id";
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Name = "Date";
            dataGridView1.Columns[2].Name = "From";
            dataGridView1.Columns[3].Name = "To";
            dataGridView1.Columns[4].Name = "Name";
            dataGridView1.Columns[5].Name = "Phone Number";
            dataGridView1.Columns[6].Name = "Departure Date";
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[7].Name = "Booking Code";
            dataGridView1.Columns[7].Visible = false;
            dataGridView1.Columns[8].Name = "Total Price";
            dataGridView1.Columns[8].Visible = false;
            dataGridView1.Columns[9].Name = "Route Id";
            dataGridView1.Columns[9].Visible = false;
            StartDataGridView();

            DataGridViewButtonColumn buttonDataGridView = new DataGridViewButtonColumn();
            dataGridView1.Columns.Add(buttonDataGridView);
            buttonDataGridView.HeaderText = "Print";
            buttonDataGridView.Text = "Print";
            buttonDataGridView.Name = "Print";
            buttonDataGridView.UseColumnTextForButtonValue = true;

            dateTimePicker1.MinDate = DateTime.Now;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            label5.Visible = false;
            label6.Visible = false;
            textBox1.Enabled = false;
            button1.Enabled = false;

            LinkLabel lbl = new LinkLabel();
            lbl.Text = "Hello World!";
            lbl.Location = new Point(100, 25);
            this.Controls.Add(lbl);
        }

        private void ViewForm_Load(object sender, EventArgs e)
        {
            StartCondition();
        }

        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            this.pickdepartureDate = String.Format("{0:yyyy-MM-dd}", dateTimePicker1.Value);
            StartFromComboBox();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            StartToComboBox(comboBox1.SelectedValue.ToString());
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int fillSeats = 0;

            Connect();
            connection.Open();
            command = new SqlCommand("SELECT  * FROM route JOIN city ON route.to_city_id = city.id WHERE from_city_id = " + comboBox1.SelectedValue.ToString() + " AND to_city_id = " + comboBox2.SelectedValue.ToString(), connection);
            dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    string routeId = dataReader["id"].ToString();
                    this.pickRouteId = routeId;
                    this.price = dataReader["price"].ToString();
                    connection.Close();
                    Connect();
                    connection.Open();
                    command = new SqlCommand("SELECT * FROM booking JOIN schedule ON booking.schedule_id = schedule.id WHERE booking.departure_date = '" + String.Format("{0:yyyy-MM-dd}", dateTimePicker1.Value) + "' AND schedule.route_id = '" + routeId + "'", connection);
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        int fillSeatsTemp = 0;
                        while (dataReader.Read())
                        {
                            fillSeatsTemp += int.Parse(dataReader["nseats"].ToString());
                        }

                        fillSeats = fillSeatsTemp;

                        int maxSeats = 0;
                        connection.Close();
                        Connect();
                        connection.Open();
                        command = new SqlCommand("SELECT COUNT(*) * 8 AS count FROM schedule WHERE route_id = '" + routeId + "'", connection);
                        dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            int maxSeatsTemp = 0;
                            while (dataReader.Read())
                            {
                                maxSeatsTemp += int.Parse(dataReader["count"].ToString());
                            }
                            maxSeats = maxSeatsTemp;
                        }
                        else
                        {
                            MessageBox.Show("Error", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (maxSeats - fillSeats == 0)
                        {
                            label5.ForeColor = Color.Red;
                            label5.Visible = true;
                            label6.ForeColor = Color.Red;
                            label6.Text = (maxSeats - fillSeats).ToString();
                            label6.Visible = true;
                        }
                        else
                        {
                            label5.Visible = true;
                            label6.Text = (maxSeats - fillSeats).ToString();
                            label6.Visible = true;
                            textBox1.Enabled = true;
                            button1.Enabled = true;
                        }
                    }
                    else
                    {
                        int maxSeats = 0;
                        connection.Close();
                        Connect();
                        connection.Open();
                        command = new SqlCommand("SELECT COUNT(*) * 8 AS count FROM schedule WHERE route_id = '" + routeId + "'", connection);
                        dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            int maxSeatsTemp = 0;
                            while (dataReader.Read())
                            {
                                maxSeatsTemp += int.Parse(dataReader["count"].ToString());
                            }
                            maxSeats = maxSeatsTemp;
                        }
                        else
                        {
                            MessageBox.Show("Error", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        label5.Visible = true;
                        label6.Text = maxSeats.ToString();
                        label6.Visible = true;
                        textBox1.Enabled = true;
                        button1.Enabled = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Error", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar > (char)Keys.D8 || e.KeyChar < (char)Keys.D1) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Number of Seats Cannot Be Empty!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (textBox1.Text == "0" || textBox1.Text == "9")
                {
                    MessageBox.Show("Number Enterd Not Valid!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    BookingForm bookingForm;
                    bookingForm = new BookingForm();
                    bookingForm.routeId = this.pickRouteId;
                    bookingForm.fromCityId = comboBox1.SelectedValue.ToString();
                    bookingForm.toCityId = comboBox2.SelectedValue.ToString();
                    bookingForm.numberSeats = int.Parse(textBox1.Text);
                    bookingForm.numberSeatsDefault = int.Parse(textBox1.Text);
                    bookingForm.departureDate = this.pickdepartureDate;
                    bookingForm.eachSeatsPrice = double.Parse(this.price);
                    this.Hide();
                    bookingForm.Closed += (s, args) => this.Close();
                    bookingForm.Show();

                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Connect();
            connection.Open();
            command = new SqlCommand("SELECT booking.id, DATEPART(dd, booking.booking_date) AS booking_day, DATEPART(mm, booking.booking_date) AS booking_date, DATEPART(yy, booking.booking_date) AS booking_year, city.city_name AS fromCity, c.city_name AS toCity, customer.customer_name, customer.phone, DATEPART(dd, booking.departure_date) AS departure_day, DATEPART(mm, booking.departure_date) AS departure_date, DATEPART(yyyy, booking.departure_date) AS departure_year, booking.booking_code, booking.price, schedule.id As scheduleId FROM booking JOIN schedule ON booking.schedule_id = schedule.id JOIN route ON schedule.route_id = route.id JOIN city ON route.from_city_id = city.id JOIN city c ON route.to_city_id = c.id JOIN customer ON booking.customer_id = customer.id WHERE booking.booking_date = '" + String.Format("{0:yyyy-MM-dd}", dateTimePicker2.Value) + "' OR booking.booking_date = '" + String.Format("{0:yyyy-MM-dd}", dateTimePicker2.Value) + "' OR city.city_name = '%"+ textBox2.Text + "%' OR customer.customer_name = '%" + textBox2.Text + "%' OR  customer.phone = '%" + textBox2.Text + "%' OR booking.booking_code = '%" + textBox2.Text + "%' ORDER BY booking.id DESC", connection);
            dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                dataGridView1.Rows.Clear();
                button3.Visible = true;
                while (dataReader.Read())
                {
                    string bookingDay = dataReader["booking_day"].ToString();
                    if (bookingDay.Length != 2)
                    {
                        bookingDay = "0" + bookingDay;
                    }
                    string bookingDate = dataReader["booking_date"].ToString();
                    if (bookingDate.Length != 2)
                    {
                        bookingDate = "0" + bookingDate;
                    }
                    string bookingYear = dataReader["booking_year"].ToString();

                    string bookingDateFinal = bookingDay + "/" + bookingDate + "/" + bookingYear;

                    string departureDay = dataReader["departure_day"].ToString();
                    if (departureDay.Length != 2)
                    {
                        departureDay = "0" + departureDay;
                    }
                    string departureDate = dataReader["departure_date"].ToString();
                    if (departureDate.Length != 2)
                    {
                        departureDate = "0" + departureDate;
                    }
                    string departureYear = dataReader["booking_year"].ToString();

                    string departureDateFinal = departureDay + "/" + departureDate + "/" + departureYear;

                    string[] row = new string[] { dataReader["id"].ToString(), bookingDateFinal, dataReader["fromCity"].ToString(), dataReader["toCity"].ToString(), dataReader["customer_name"].ToString(), dataReader["phone"].ToString(), departureDateFinal, dataReader["booking_code"].ToString(), dataReader["price"].ToString(), dataReader["scheduleId"].ToString() };
                    dataGridView1.Rows.Add(row);
                }
            }
            else
            {
                MessageBox.Show("Data Not Found!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StartDataGridView();
            textBox2.Text = "";
            dateTimePicker2.Value = DateTime.Now;
            button3.Visible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            connection.Close();

            if (e.ColumnIndex == 10)
            {
                int i = dataGridView1.SelectedCells[0].RowIndex;

                string bookingCode = dataGridView1.Rows[i].Cells[7].Value.ToString();
                string departureDate = dataGridView1.Rows[i].Cells[6].Value.ToString();
                string departureTime = "";
                string fromCity = dataGridView1.Rows[i].Cells[2].Value.ToString();
                string toCity = dataGridView1.Rows[i].Cells[3].Value.ToString();
                string seatNumbers = "";
                string totalPrice = dataGridView1.Rows[i].Cells[8].Value.ToString();

                Connect();
                connection.Open();
                command = new SqlCommand("SELECT * FROM booking_detail WHERE id = " + dataGridView1.Rows[i].Cells[0].Value.ToString(), connection);
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        seatNumbers += dataReader["seat_number"].ToString();
                        seatNumbers += "  ";
                    }
                }

                connection.Close();
                Connect();
                connection.Open();
                command = new SqlCommand("SELECT * FROM schedule WHERE id = "+ dataGridView1.Rows[i].Cells[9].Value.ToString(), connection);
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        departureTime = dataReader["departure_time"].ToString();
                    }
                }

                PopupForm popupForm;
                popupForm = new PopupForm();
                popupForm.bookingCode = bookingCode;
                popupForm.departureDate = departureDate;
                popupForm.departureTime = departureTime;
                popupForm.fromCity = fromCity;
                popupForm.toCity = toCity;
                popupForm.seatNumber = seatNumbers;
                popupForm.totalPrice = totalPrice;
                popupForm.Show();
            }
        }
    }
}
