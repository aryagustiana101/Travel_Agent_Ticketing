using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Travel_Agent_Ticketing
{
    public partial class PopupForm : Form
    {
        public PopupForm()
        {
            InitializeComponent();
        }

        public string bookingCode;
        public string departureDate;
        public string departureTime;
        public string fromCity;
        public string toCity;
        public string seatNumber;
        public string totalPrice;

        public void StartCondition()
        {
            labelBookingCode.Text = ": " + bookingCode;
            labelDepartureDate.Text = ": " + departureDate + " " + departureTime;
            labelFrom.Text = ": " + fromCity;
            labelTo.Text = ": " + toCity;
            labelSeatNumber.Text = ": " + seatNumber;
            labelTotalPrice.Text = ": " + totalPrice;
        }

        private void PopupForm_Load(object sender, EventArgs e)
        {
            StartCondition();
        }
    }
}
