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
using System.Text.RegularExpressions;

namespace Travel_Agent_Ticketing
{
    public partial class BookingForm : Form
    {
        public BookingForm()
        {
            InitializeComponent();
        }

        public string routeId;
        public string fromCityId;
        public string toCityId;
        public string cityCode;
        public string customerId;
        public int numberSeats;
        public int numberSeatsDefault;
        public bool isDepartureTimeSelected = false;
        public string departureDate;
        public double eachSeatsPrice;
        public List<string> selectedSeatNumbers = new List<string>();

        private string database;
        private SqlConnection connection;
        private SqlDataReader dataReader;
        private SqlCommand command;

        public void Connect()
        {
            database = "Data Source = .\\SQLEXPRESS; Initial Catalog = Travel; Integrated Security = True";
            connection = new SqlConnection(database);
        }

        public void StartListBox()
        {
            Connect();
            connection.Open();
            command = new SqlCommand("SELECT * FROM schedule WHERE route_id = '" + routeId + "'", connection);
            dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    listBox1.Items.Add(new ListItem { Name = dataReader["departure_time"].ToString(), Value = int.Parse(dataReader["id"].ToString()) });
                }
            }
            else
            {
                MessageBox.Show("Booking Error", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }

        public class ListItem
        {
            public string Name { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public void StartButtonSeats()
        {
            btnSeat1.BackColor = Color.Silver;
            btnSeat2.BackColor = Color.Silver;
            btnSeat3.BackColor = Color.Silver;
            btnSeat4.BackColor = Color.Silver;
            btnSeat5.BackColor = Color.Silver;
            btnSeat6.BackColor = Color.Silver;
            btnSeat7.BackColor = Color.Silver;
            btnSeat8.BackColor = Color.Silver;
            btnSeat1.Enabled = false;
            btnSeat2.Enabled = false;
            btnSeat3.Enabled = false;
            btnSeat4.Enabled = false;
            btnSeat5.Enabled = false;
            btnSeat6.Enabled = false;
            btnSeat7.Enabled = false;
            btnSeat8.Enabled = false;
        }

        public void StartAvailableSeats()
        {
            btnSeat1.BackColor = Color.Lime;
            btnSeat2.BackColor = Color.Lime;
            btnSeat3.BackColor = Color.Lime;
            btnSeat4.BackColor = Color.Lime;
            btnSeat5.BackColor = Color.Lime;
            btnSeat6.BackColor = Color.Lime;
            btnSeat7.BackColor = Color.Lime;
            btnSeat8.BackColor = Color.Lime;
            btnSeat1.Enabled = true;
            btnSeat2.Enabled = true;
            btnSeat3.Enabled = true;
            btnSeat4.Enabled = true;
            btnSeat5.Enabled = true;
            btnSeat6.Enabled = true;
            btnSeat7.Enabled = true;
            btnSeat8.Enabled = true;
        }

        public void StartCondition()
        {
            StartListBox();
            StartButtonSeats();
            textBox2.ReadOnly = true;
            textBox2.Enabled = false;
            textBox3.ReadOnly = true;
            textBox3.Enabled = false;
            btnSave.Enabled = false;
        }

        private bool EmailAddressCheck(string emailAddress)
        {
            string pattern = @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";
            Match emailAddressMatch = Regex.Match(emailAddress, pattern);
            if (emailAddressMatch.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string CityCode(string fromCityId)
        {
            if (fromCityId == "1")
            {
                cityCode = "JKT";
            }
            if (fromCityId == "2")
            {
                cityCode = "BDG";
            }
            if (fromCityId == "3")
            {
                cityCode = "CBN";
            }
            if (fromCityId == "4")
            {
                cityCode = "TSM";
            }
            if (fromCityId == "5")
            {
                cityCode = "KNG";
            }
            if (fromCityId == "6")
            {
                cityCode = "TGL";
            }
            return cityCode;
        }

        private void BookingForm_Load(object sender, EventArgs e)
        {
            StartCondition();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Fill The Search Box!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {   
                Connect();
                connection.Open();
                command = new SqlCommand("SELECT * FROM customer WHERE phone = '" + textBox1.Text + "'", connection);
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    while (dataReader.Read())
                    {
                        customerId = dataReader["id"].ToString();
                        textBox2.Text = dataReader["customer_name"].ToString();
                        textBox3.Text = dataReader["email"].ToString();
                    }
                }
                else
                {
                    customerId = "";
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    textBox2.ReadOnly = true;
                    textBox3.ReadOnly = true;
                    MessageBox.Show("Customer Phone Not Recorded, Create New!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                connection.Close();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (btnNew.Text == "Cancel")
            {
                customerId = "";
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                btnNew.Text = "New";
                btnSave.Enabled = false;
                btnCheck.Enabled = true;
            }
            else
            {
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = false;
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                btnNew.Text = "Cancel";
                btnSave.Enabled = true;
                btnCheck.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Please Fill All The Customer Text Box With Appropriate Data!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Connect();
                connection.Open();
                command = new SqlCommand("SELECT * FROM customer WHERE phone = '" + textBox1.Text + "'", connection);
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    MessageBox.Show("Phone Number Customer Data Already Exists!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (!double.TryParse(textBox1.Text, out _))
                    {
                        MessageBox.Show("Phone Number Must Number!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (!EmailAddressCheck(textBox3.Text))
                        {
                            MessageBox.Show("Email Is Not Valid!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            Connect();
                            connection.Open();
                            string inputQuery = "INSERT INTO customer (customer_name, email, phone) VALUES ('" + textBox2.Text + "','" + textBox3.Text + "','" + textBox1.Text + "')";
                            command = new SqlCommand(inputQuery, connection);
                            command.ExecuteNonQuery();
                            connection.Close();
                            MessageBox.Show("Insert New Customer Data Success!", "Insert Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Connect();
                            connection.Open();
                            command = new SqlCommand("SELECT * FROM customer WHERE phone = '" + textBox1.Text + "'", connection);
                            dataReader = command.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    customerId = dataReader["id"].ToString();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Booking Error!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            connection.Close();
                            textBox2.ReadOnly = true;
                            textBox3.ReadOnly = true;
                            btnSave.Enabled = false;
                        }
                    }
                }
                connection.Close();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var scheduleId = ((ListItem)listBox1.SelectedItem).Value;
            selectedSeatNumbers.Clear();
            numberSeats = numberSeatsDefault;
            isDepartureTimeSelected = true;
            Connect();
            connection.Open();
            command = new SqlCommand("SELECT * FROM booking JOIN schedule ON booking.schedule_id = schedule.id join booking_detail ON booking.id = booking_detail.id WHERE booking.departure_date = '" + departureDate + "' AND schedule.route_id = '" + routeId + "' AND schedule.id = " + scheduleId, connection);
            dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                StartAvailableSeats();
                while (dataReader.Read())
                {
                    if (dataReader["seat_number"].ToString() == "1")
                    {
                        btnSeat1.BackColor = Color.Silver;
                        btnSeat2.Enabled = false;
                    }
                    if (dataReader["seat_number"].ToString() == "2")
                    {
                        btnSeat2.BackColor = Color.Silver;
                        btnSeat2.Enabled = false;
                    }
                    if (dataReader["seat_number"].ToString() == "3")
                    {
                        btnSeat3.BackColor = Color.Silver;
                        btnSeat3.Enabled = false;
                    }
                    if (dataReader["seat_number"].ToString() == "4")
                    {
                        btnSeat4.BackColor = Color.Silver;
                        btnSeat4.Enabled = false;
                    }
                    if (dataReader["seat_number"].ToString() == "5")
                    {
                        btnSeat5.Enabled = false;
                        btnSeat5.BackColor = Color.Silver;
                    }
                    if (dataReader["seat_number"].ToString() == "6")
                    {
                        btnSeat6.BackColor = Color.Silver;
                        btnSeat6.Enabled = false;
                    }
                    if (dataReader["seat_number"].ToString() == "7")
                    {
                        btnSeat7.BackColor = Color.Silver;
                        btnSeat7.Enabled = false;
                    }
                    if (dataReader["seat_number"].ToString() == "8")
                    {
                        btnSeat8.BackColor = Color.Silver;
                        btnSeat8.Enabled = false;
                    }
                }
            }
            else
            {
                StartAvailableSeats();
            }
            connection.Close();
        }

        private void btnSeat1_Click(object sender, EventArgs e)
        {
            if (btnSeat1.BackColor != Color.Yellow)
            {
                if (numberSeats != 0)
                {
                    btnSeat1.BackColor = Color.Yellow;
                    numberSeats -= 1;
                    selectedSeatNumbers.Add("1");
                }
            }
            else
            {
                btnSeat1.BackColor = Color.Lime;
                numberSeats += 1;
                selectedSeatNumbers.Remove("1");
            }

        }

        private void btnSeat2_Click(object sender, EventArgs e)
        {
            if (btnSeat2.BackColor != Color.Yellow)
            {
                if (numberSeats != 0)
                {
                    btnSeat2.BackColor = Color.Yellow;
                    numberSeats -= 1;
                    selectedSeatNumbers.Add("2");
                }
            }
            else
            {
                btnSeat2.BackColor = Color.Lime;
                numberSeats += 1;
                selectedSeatNumbers.Remove("2");
            }
        }

        private void btnSeat3_Click(object sender, EventArgs e)
        {
            if (btnSeat3.BackColor != Color.Yellow)
            {
                if (numberSeats != 0)
                {
                    btnSeat3.BackColor = Color.Yellow;
                    numberSeats -= 1;
                    selectedSeatNumbers.Add("3");
                }
            }
            else
            {
                btnSeat3.BackColor = Color.Lime;
                numberSeats += 1;
                selectedSeatNumbers.Remove("3");
            }
        }

        private void btnSeat4_Click(object sender, EventArgs e)
        {
            if (btnSeat4.BackColor != Color.Yellow)
            {
                if (numberSeats != 0)
                {
                    btnSeat4.BackColor = Color.Yellow;
                    numberSeats -= 1;
                    selectedSeatNumbers.Add("4");
                }
            }
            else
            {
                btnSeat4.BackColor = Color.Lime;
                numberSeats += 1;
                selectedSeatNumbers.Remove("4");
            }
        }

        private void btnSeat5_Click(object sender, EventArgs e)
        {
            if (btnSeat5.BackColor != Color.Yellow)
            {
                if (numberSeats != 0)
                {
                    btnSeat5.BackColor = Color.Yellow;
                    numberSeats -= 1;
                    selectedSeatNumbers.Add("5");
                }
            }
            else
            {
                btnSeat5.BackColor = Color.Lime;
                numberSeats += 1;
                selectedSeatNumbers.Remove("5");
            }
        }

        private void btnSeat6_Click(object sender, EventArgs e)
        {
            if (btnSeat6.BackColor != Color.Yellow)
            {
                if (numberSeats != 0)
                {
                    btnSeat6.BackColor = Color.Yellow;
                    numberSeats -= 1;
                    selectedSeatNumbers.Add("6");
                }
            }
            else
            {
                btnSeat6.BackColor = Color.Lime;
                numberSeats += 1;
                selectedSeatNumbers.Remove("6");
            }
        }

        private void btnSeat7_Click(object sender, EventArgs e)
        {
            if (btnSeat7.BackColor != Color.Yellow)
            {
                if (numberSeats != 0)
                {
                    btnSeat7.BackColor = Color.Yellow;
                    numberSeats -= 1;
                    selectedSeatNumbers.Add("7");
                }
            }
            else
            {
                btnSeat7.BackColor = Color.Lime;
                numberSeats += 1;
                selectedSeatNumbers.Remove("7");
            }
        }

        private void btnSeat8_Click(object sender, EventArgs e)
        {
            if (btnSeat8.BackColor != Color.Yellow)
            {
                if (numberSeats != 0)
                {
                    btnSeat8.BackColor = Color.Yellow;
                    numberSeats -= 1;
                    selectedSeatNumbers.Add("8");
                }
            }
            else
            {
                btnSeat8.BackColor = Color.Lime;
                numberSeats += 1;
                selectedSeatNumbers.Remove("8");
            }
        }

        private void btnBook_Click(object sender, EventArgs e)
        {
            string bookingCode = "";
            string bookingDate = String.Format("{0:yyyy-MM-dd}", DateTime.Now);
            var scheduleId = ((ListItem)listBox1.SelectedItem).Value;
            string price = (numberSeatsDefault * eachSeatsPrice).ToString();
            string nseats = numberSeatsDefault.ToString();
            string customerId = this.customerId;
            string departureDate = this.departureDate;


            if (customerId == "" || textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Customer Data Cannot Empty!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (!isDepartureTimeSelected)
                {
                    MessageBox.Show("Pick Departure Time!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (numberSeats != 0)
                    {
                        MessageBox.Show("Pick More Seat Based on Inputed Number of Seats!", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        string todayYear = String.Format("{0:yyyy}", DateTime.Now);
                        Int32 increment;
                        Int32 lastestBookingCodeInt = 0;
                        string lastestBookingCodeYear = "";

                        Connect();
                        connection.Open();
                        command = new SqlCommand("SELECT * FROM booking Where booking_code = (SELECT MAX(booking_code)  FROM booking)", connection);
                        dataReader = command.ExecuteReader();

                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                string latestBookingCode = dataReader["booking_code"].ToString();
                                char charIncrement;
                                string stringIncrement = "";
                                char charYear;
                                string stringYear = "";
                                for (int i = 7; i < 11; i++)
                                {
                                    charIncrement = latestBookingCode[i];
                                    stringIncrement += charIncrement.ToString();
                                }
                                for (int i = 3; i < 8 - 1; i++)
                                {
                                    charYear = latestBookingCode[i];
                                    stringYear += charYear.ToString();
                                }
                                lastestBookingCodeInt = Int32.Parse(stringIncrement);
                                lastestBookingCodeYear = stringYear;
                            }

                            if (lastestBookingCodeYear == todayYear)
                            {
                                connection.Close();
                                Connect();
                                connection.Open();
                                increment = lastestBookingCodeInt + 1;
                                bookingCode = CityCode(fromCityId) + todayYear + increment.ToString("D4");
                                command = new SqlCommand("INSERT INTO booking (booking_code, booking_date, schedule_id, price, nseats, customer_id, departure_date) VALUES ('" + bookingCode + "','" + bookingDate + "'," + scheduleId + ", " + price + ", " + nseats + ", " + customerId + ", '" + departureDate + "')", connection);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                connection.Close();
                                Connect();
                                connection.Open();
                                increment = 1;
                                bookingCode = CityCode(fromCityId) + todayYear + increment.ToString("D4");
                                command = new SqlCommand("INSERT INTO booking (booking_code, booking_date, schedule_id, price, nseats, customer_id, departure_date) VALUES ('" + bookingCode + "','" + bookingDate + "'," + scheduleId + ", " + price + ", " + nseats + ", " + customerId + ", '" + departureDate + "')", connection);
                                command.ExecuteNonQuery();
                            }
                            string bookingId = "";
                            connection.Close();
                            Connect();
                            connection.Open();
                            command = new SqlCommand("SELECT * FROM booking Where booking_code = '" + bookingCode + "'", connection);
                            dataReader = command.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    bookingId = dataReader["id"].ToString();
                                }
                                connection.Close();
                                Connect();
                                connection.Open();
                                for (int i = 0; i < selectedSeatNumbers.Count; i++)
                                {
                                    command = new SqlCommand("INSERT INTO booking_detail (id, seat_number) VALUES (" + bookingId + ",'" + selectedSeatNumbers[i].ToString() + "')", connection);
                                    command.ExecuteNonQuery();
                                }

                                MessageBox.Show("Booking Success", "Booking Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                ViewForm viewForm;
                                viewForm = new ViewForm();
                                this.Hide();
                                viewForm.Closed += (s, args) => this.Close();
                                viewForm.StartCondition();
                                viewForm.Show();
                            }
                            else
                            {
                                MessageBox.Show("Booking Error", "Booking Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ViewForm viewForm;
            viewForm = new ViewForm();
            this.Hide();
            viewForm.Closed += (s, args) => this.Close();
            viewForm.StartCondition();
            viewForm.Show();
        }
    }
}
