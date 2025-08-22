using MedicalBookingApp.DataCs;
using System;
using System.Data;
using System.Windows.Forms;

namespace MedicalBookingApp.Forms
{
    public partial class DoctorListForm : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;

        private ComboBox cboAvailability;

        public DoctorListForm()
        {
            InitializeComponent();
            LoadDoctors();
        }

        private void InitializeComponent()
        {
            this.Text = "Doctors";
            this.Width = 800; this.Height = 500; this.StartPosition = FormStartPosition.CenterParent;

            txtSearch = new TextBox { Left = 20, Top = 15, Width = 300 };

            txtSearch.PlaceholderText = "Search name or specialty...";
            cboAvailability = new ComboBox { Left = 340, Top = 15, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };

            cboAvailability.Items.AddRange(new object[] { "All", "Available Only" });
            cboAvailability.SelectedIndex = 0;

            dgv = new DataGridView { Left = 20, Top = 50, Width = 740, Height = 380, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            txtSearch.TextChanged += (s, e) => LoadDoctors();

            cboAvailability.SelectedIndexChanged += (s, e) => LoadDoctors();

            Controls.AddRange(new Control[] { txtSearch, cboAvailability, dgv });
        }

        private void LoadDoctors()
        {
            string search = txtSearch?.Text?.Trim() ?? string.Empty;
            bool onlyAvailable = cboAvailability?.SelectedIndex == 1;

            try
            {
                using (var conn = Db.GetOpenConnection())
                using (var cmd = Db.CreateCommand(conn, @"
                    SELECT DoctorID, FullName, Specialty, Availability
                    FROM dbo.Doctors
                    WHERE (@q = '' OR FullName LIKE '%' + @q + '%' OR Specialty LIKE '%' + @q + '%')
                      AND (@avail = 0 OR Availability = 1)
                    ORDER BY FullName;"))
                {
                    cmd.Parameters.Add(Db.In("@q", SqlDbType.VarChar, search));
                    cmd.Parameters.Add(Db.In("@avail", SqlDbType.Bit, onlyAvailable ? 1 : 0));

                    using (var rdr = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();

                        dt.Load(rdr);

                        dgv.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading doctors: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}