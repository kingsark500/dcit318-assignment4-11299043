using System.Windows.Forms;

namespace MedicalBookingApp.Forms
{
    public partial class MainForm : Form
    {
        private Button btnDoctors;
        private Button btnBook;
        private Button btnManage;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Medical Booking — Main";
            this.Width = 420; this.Height = 220; this.StartPosition = FormStartPosition.CenterScreen;

            btnDoctors = new Button { Text = "View Doctors", Left = 40, Top = 40, Width = 120 };

            btnBook = new Button { Text = "Book Appointment", Left = 200, Top = 40, Width = 150 };

            btnManage = new Button { Text = "Manage Appointments", Left = 120, Top = 100, Width = 170 };

            btnDoctors.Click += (s, e) => new DoctorListForm().ShowDialog();
            btnBook.Click += (s, e) => new AppointmentForm().ShowDialog();

            btnManage.Click += (s, e) => new ManageAppointmentsForm().ShowDialog();

            Controls.AddRange(new Control[] { btnDoctors, btnBook, btnManage });
        }
    }
}