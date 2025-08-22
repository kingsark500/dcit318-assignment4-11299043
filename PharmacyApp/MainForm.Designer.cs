using System;
using System.Data;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PharmacyApp
{
    public partial class MainForm : Form
    {
        DatabaseHelper db = new DatabaseHelper();

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAddMedicine_Click(object sender, EventArgs e)
        {
            db.AddMedicine(txtName.Text, txtCategory.Text,
                decimal.Parse(txtPrice.Text), int.Parse(txtQuantity.Text));
            LoadMedicines();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dgvMedicines.DataSource = db.SearchMedicine(txtSearch.Text);
        }

        private void btnUpdateStock_Click(object sender, EventArgs e)
        {
            if (dgvMedicines.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvMedicines.CurrentRow.Cells["MedicineID"].Value);
                db.UpdateStock(id, int.Parse(txtQuantity.Text));
                LoadMedicines();
            }
        }

        private void btnRecordSale_Click(object sender, EventArgs e)
        {
            if (dgvMedicines.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvMedicines.CurrentRow.Cells["MedicineID"].Value);
                db.RecordSale(id, int.Parse(txtQuantity.Text));
                LoadMedicines();
            }
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadMedicines();
        }

        private void LoadMedicines()
        {
            dgvMedicines.DataSource = db.GetAllMedicines();
        }
    }
}