using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace Models.DataBase
{

    public partial class Moperson
    {
		public string address { get; set; }
		public Nullable<decimal> age { get; set; }
		public Nullable<DateTime> date_of_birth { get; set; }
		public string first_name { get; set; }
		public long Id { get; set; }
		public string last_name { get; set; }

        public Moperson() { }

        public Moperson(DataRow dr)
        {
			address = Convert.ToString(dr["address"]);
			age = (dr["age"] == System.DBNull.Value) ? (decimal?)null : Convert.ToDecimal(dr["age"]);
			date_of_birth = (dr["date_of_birth"] == System.DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(dr["date_of_birth"]);
			first_name = Convert.ToString(dr["first_name"]);
			Id = Convert.ToInt64(dr["Id"]);
			last_name = Convert.ToString(dr["last_name"]);
        }
    }
}

