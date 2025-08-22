using MedicalBookingApp.DataCs;
using System;
using System.Data;
using System.Windows.Forms;

namespace MedicalBookingApp.Forms
{
    public partial class AppointmentForm : Form
    {
        private ComboBox cboDoctor;
        private ComboBox cboPatient;

        private DateTimePicker dtpDate;

        private TextBox txtNotes;
        private Button btnBook;

        public  AppointmentForm()
        {
            InitializeComponent();
            LoadDoctors();

            LoadPatients();
        }

        private void InitializeComponent()
        {
            this.Text = "Book Appointment";
            this.Width = 520; this.Height = 320; this.StartPosition = FormStartPosition.CenterParent;

            var lblDoctor = new Label { Text = "Doctor:", Left = 30, Top = 30, Width = 80 };
            var lblPatient = new Label { Text = "Patient:", Left = 30, Top = 70, Width = 80 };

            var lblDate = new Label { Text = "Date/Time:", Left = 30, Top = 110, Width = 80 };
            var lblNotes = new Label { Text = "Notes:", Left = 30, Top = 150, Width = 80 };

            cboDoctor = new ComboBox { Left = 120, Top = 26, Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPatient = new ComboBox { Left = 120, Top = 66, Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };

            dtpDate = new DateTimePicker { Left = 120, Top = 106, Width = 200, Format = DateTimePickerFormat.Custom, CustomFormat = "dd-MMM-yyyy HH:mm" };
            txtNotes = new TextBox { Left = 120, Top = 146, Width = 340, Height = 60, Multiline = true };

            btnBook = new Button { Left = 120, Top = 220, Width = 120, Text = "Book" };

            btnBook.Click += BtnBook_Click;

            Controls.AddRange(new Control[] { lblDoctor, lblPatient, lblDate, lblNotes, cboDoctor, cboPatient, dtpDate, txtNotes, btnBook });
        }

        private void LoadDoctors()
        {
            try
            {
                using (var conn = Db.GetOpenConnection())
                using (var cmd = Db.CreateCommand(conn, "SELECT DoctorID, FullName + ' (' + Specialty + ')' AS DisplayName FROM dbo.Doctors WHERE Availability = 1 ORDER BY FullName"))

                using (var rdr = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(rdr);
                    cboDoctor.DataSource = dt;

                    cboDoctor.DisplayMember = "DisplayName";

                    cboDoctor.ValueMember = "DoctorID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading doctors: {ex.Message}");
            }
        }

        private void LoadPatients()
        {
            try
            {
                using (var conn = Db.GetOpenConnection())
                using (var cmd = Db.CreateCommand(conn, "SELECT PatientID, FullName FROM dbo.Patients ORDER BY FullName"))

                using (var rdr = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(rdr);
                    cboPatient.DataSource = dt;
                    cboPatient.DisplayMember = "FullName";

                    cboPatient.ValueMember = "PatientID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading patients: {ex.Message}");
            }
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            if (cboDoctor.SelectedValue == null || cboPatient.SelectedValue == null)
            {
                MessageBox.Show("Please select both a doctor and a patient.");
                return;
            }

            var doctorId = Convert.ToInt32(cboDoctor.SelectedValue);
            var patientId = Convert.ToInt32(cboPatient.SelectedValue);

            var apptDate = dtpDate.Value;

            var notes = txtNotes.Text?.Trim();

            if (apptDate < DateTime.Now)
            {
                MessageBox.Show("Appointment date must be in the future.");
                return;
            }

            try
            {
                using (var conn = Db.GetOpenConnection())
                {
                    using (var cmdAvail = Db.CreateCommand(conn, "SELECT Availability FROM dbo.Doctors WHERE DoctorID = @DoctorID"))
                    {
                        cmdAvail.Parameters.Add(Db.In("@DoctorID", SqlDbType.Int, doctorId));

                        var isAvailable = Convert.ToBoolean(cmdAvail.ExecuteScalar() ?? false);

                        if (!isAvailable)
                        {
                            MessageBox.Show("Selected doctor is not accepting bookings.");
                            return;
                        }
                    }

                    using (var cmdConf = Db.CreateCommand(conn, @"
                        SELECT COUNT(*) FROM dbo.Appointments
                        WHERE DoctorID = @DoctorID
                          AND ABS(DATEDIFF(MINUTE, AppointmentDate, @Date)) < 30;"))
                    {
                        cmdConf.Parameters.Add(Db.In("@DoctorID", SqlDbType.Int, doctorId));
                        cmdConf.Parameters.Add(Db.In("@Date", SqlDbType.DateTime, apptDate));
                        int cnt = Convert.ToInt32(cmdConf.ExecuteScalar());
                        if (cnt > 0)
                        {
                            MessageBox.Show("This time slot is already booked for the selected doctor.");
                            return;
                        }
                    }

                    using (var cmd = Db.CreateCommand(conn, @"
                        INSERT INTO dbo.Appointments (DoctorID, PatientID, AppointmentDate, Notes)
                        VALUES (@DoctorID, @PatientID, @Date, @Notes);"))
                    {
                        cmd.Parameters.Add(Db.In("@DoctorID", SqlDbType.Int, doctorId));
                        cmd.Parameters.Add(Db.In("@PatientID", SqlDbType.Int, patientId));
                        cmd.Parameters.Add(Db.In("@Date", SqlDbType.DateTime, apptDate));
                        cmd.Parameters.Add(Db.In("@Notes", SqlDbType.VarChar, (object)notes ?? DBNull.Value));

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Appointment booked successfully.");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("No rows inserted. Please try again.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error booking appointment: {ex.Message}");
            }
        }
    }
}