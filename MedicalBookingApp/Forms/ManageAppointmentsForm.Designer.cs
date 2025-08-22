using MedicalBookingApp.DataCs;
using System;
using System.Data;

using System.Data.SqlClient;

using System.Windows.Forms;

namespace MedicalBookingApp.Forms
{
    public partial class ManageAppointmentsForm : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;

        private Button btnRefresh;
        private Button btnUpdateDate;

        private Button btnDelete;

        private DateTimePicker dtpNewDate;

        private SqlDataAdapter adapter;

        private DataSet ds;

        public ManageAppointmentsForm()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Appointments";
            this.Width = 900; this.Height = 560; this.StartPosition = FormStartPosition.CenterParent;

            txtSearch = new TextBox { Left = 20, Top = 15, Width = 300 };
            txtSearch.PlaceholderText = "Search by patient/doctor...";


            btnRefresh = new Button { Left = 340, Top = 12, Width = 90, Text = "Search" };
            btnRefresh.Click += (s, e) => LoadAppointments();

            dgv = new DataGridView { Left = 20, Top = 50, Width = 840, Height = 380, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            dtpNewDate = new DateTimePicker { Left = 20, Top = 450, Width = 220, Format = DateTimePickerFormat.Custom, CustomFormat = "dd-MMM-yyyy HH:mm" };

            btnUpdateDate = new Button { Left = 250, Top = 450, Width = 150, Text = "Update Date" };

            btnDelete = new Button { Left = 420, Top = 450, Width = 120, Text = "Delete" };

            btnUpdateDate.Click += UpdateAppointment;

            btnDelete.Click += DeleteAppointment;

            Controls.AddRange(new Control[] { txtSearch, btnRefresh, dgv, dtpNewDate, btnUpdateDate, btnDelete });
        }

        private void LoadAppointments()
        {
            try
            {
                using (SqlConnection conn = Db.GetOpenConnection())
                {
                    string sql = @"
                        SELECT a.AppointmentID, a.AppointmentDate, a.Notes,
                               d.DoctorID, d.FullName AS DoctorName, d.Specialty,
                               p.PatientID, p.FullName AS PatientName, p.Email
                        FROM dbo.Appointments a
                        JOIN dbo.Doctors d ON a.DoctorID = d.DoctorID
                        JOIN dbo.Patients p ON a.PatientID = p.PatientID
                        WHERE (@q = '' OR d.FullName LIKE '%' + @q + '%' OR p.FullName LIKE '%' + @q + '%')
                        ORDER BY a.AppointmentDate DESC;";

                    adapter = new SqlDataAdapter(sql, conn);
                    adapter.SelectCommand.Parameters.Add(Db.In("@q", SqlDbType.VarChar, txtSearch.Text?.Trim() ?? string.Empty));

                    ds = new DataSet();
                    adapter.Fill(ds, "AppointmentsView");
                    dgv.DataSource = ds.Tables["AppointmentsView"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading appointments: " + ex.Message);
            }
        }

        private int? GetSelectedAppointmentId()
        {
            if (dgv.CurrentRow == null) return null;
            return Convert.ToInt32(dgv.CurrentRow.Cells["AppointmentID"].Value);
        }

        private void UpdateAppointment(object sender, EventArgs e)
        {
            var id = GetSelectedAppointmentId();
            if (id == null) { MessageBox.Show("Please select a row."); return; }

            if (dtpNewDate.Value < DateTime.Now)
            {
                MessageBox.Show("New date must be in the future."); return;
            }

            try
            {
                using (var conn = Db.GetOpenConnection())
                using (var cmd = Db.CreateCommand(conn, "UPDATE dbo.Appointments SET AppointmentDate=@d WHERE AppointmentID=@id"))
                {
                    cmd.Parameters.Add(Db.In("@d", SqlDbType.DateTime, dtpNewDate.Value));
                    cmd.Parameters.Add(Db.In("@id", SqlDbType.Int, id.Value));
                    int rows = cmd.ExecuteNonQuery();
                    MessageBox.Show(rows > 0 ? "Updated." : "No change.");
                }
                LoadAppointments();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating: " + ex.Message);
            }
        }

        private void DeleteAppointment(object sender, EventArgs e)
        {
            var id = GetSelectedAppointmentId();
            if (id == null) { MessageBox.Show("Please select a row."); return; }

            if (MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            try
            {
                using (var conn = Db.GetOpenConnection())
                using (var cmd = Db.CreateCommand(conn, "DELETE FROM dbo.Appointments WHERE AppointmentID=@id"))
                {
                    cmd.Parameters.Add(Db.In("@id", SqlDbType.Int, id.Value));
                    int rows = cmd.ExecuteNonQuery();
                    MessageBox.Show(rows > 0 ? "Deleted." : "Not found.");
                }
                LoadAppointments();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting: " + ex.Message);
            }
        }
    }
}