using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalBookingApp.Models
{
    public class Patient

    {
        public int PatientID { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public override string ToString() => FullName;
    }
}
