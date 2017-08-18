using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Models.DataBase
{
    public partial class Moperson
    {
        public class Access
        {
            #region Default Methods
            public static Moperson Get(long Id)
            {
                try
                {
                    SqlDataAdapter SelectAdapter = new SqlDataAdapter();
                    DataTable dt = new DataTable();
                    using(SqlConnection cnn = new SqlConnection(Models.DataBase.Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = "SELECT * FROM Moperson WHERE Id=@Id";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("Id", Id); 

                        SelectAdapter = new SqlDataAdapter(cmd);
                        SelectAdapter.Fill(dt);

                    }

                    if (dt.Rows.Count > 0)
                    {
                        return new Moperson(dt.Rows[0]);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static List<Moperson> Get()
            {
                try
                {         
                    SqlDataAdapter SelectAdapter = new SqlDataAdapter();  
                    DataTable dt = new DataTable();     
                    using(SqlConnection cnn = new SqlConnection(Models.DataBase.Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = "SELECT * FROM Moperson";
                        SqlCommand cmd = new SqlCommand(query, cnn); 

                        SelectAdapter = new SqlDataAdapter(cmd);
                        SelectAdapter.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        return Helpers.GetListFromDataTable(dt);
                    }
                    else
                    {
                        return new List<Moperson>();
                    }
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static List<Moperson> Get(List<long> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        int MAX_Query_Number = Models.DataBase.Config.MAX_BDMS_PARAMS_NUM ; 
                        List<Moperson> Results = null;
                        if(LIds.Count <= MAX_Query_Number)
                        {
                            Results = _Get_(LIds);
                        }else
                        {
                            int batchNumber = LIds.Count / MAX_Query_Number;
                            Results = new List<Moperson>();
                            for(int i=0; i<batchNumber; i++)
                            {
                                Results.AddRange(_Get_(LIds.GetRange(i * MAX_Query_Number, MAX_Query_Number)));
                            }
                            Results.AddRange(_Get_(LIds.GetRange(batchNumber * MAX_Query_Number, LIds.Count-batchNumber * MAX_Query_Number)));
                        }
                        return Results;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return new List<Moperson>();
            }
            private static List<Moperson> _Get_(List<long> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        SqlDataAdapter SelectAdapter = new SqlDataAdapter();
                        DataTable dt = new DataTable();
                        using(SqlConnection cnn = new SqlConnection(Models.DataBase.Config.GetConnectionString()))
                        {
                            cnn.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = cnn;

                            string queryIds = string.Empty;
                            for(int i=0; i<LIds.Count; i++)
                            {
                                queryIds += "@Id"+i+",";
                                cmd.Parameters.AddWithValue("Id" + i, LIds[i]);
                            }
                            queryIds = queryIds.TrimEnd(',');

                            cmd.CommandText = "SELECT * FROM Moperson WHERE Id IN ("+ queryIds +")";                    
                        SelectAdapter = new SqlDataAdapter(cmd);
                            SelectAdapter.Fill(dt);
                        }

                        if (dt.Rows.Count > 0)
                        {
                            return Helpers.GetListFromDataTable(dt);
                        }
                        else
                        {
                            return new List<Moperson>();
                        }
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return new List<Moperson>();
            }

            public static int Add(Moperson T)
            {
                try
                {
                    int InsertedID = -1;
                    using(SqlConnection cnn = new SqlConnection(Models.DataBase.Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = "INSERT INTO Moperson(address,age,date_of_birth,first_name,last_name)  VALUES (@address,@age,@date_of_birth,@first_name,@last_name)";

                        SqlCommand cmd = new SqlCommand(query, cnn);
						cmd.Parameters.AddWithValue("address", T.address);
						cmd.Parameters.AddWithValue("age", T.age == null ? (object)DBNull.Value  : T.age);
						cmd.Parameters.AddWithValue("date_of_birth", T.date_of_birth == null ? (object)DBNull.Value  : T.date_of_birth);
						cmd.Parameters.AddWithValue("first_name", T.first_name);
						cmd.Parameters.AddWithValue("last_name", T.last_name);

                        InsertedID = cmd.ExecuteNonQuery();
                
                    }

                    return InsertedID;
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static int Add(List<Moperson> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int MAX_Params_Number = Models.DataBase.Config.MAX_BDMS_PARAMS_NUM / 6; // Nb params per query
                        int Results=0;
                        if(Lt.Count <= MAX_Params_Number)
                        {
                            Results = _Add_(Lt);
                        }else
                        {
                            int batchNumber = Lt.Count / MAX_Params_Number;
                            for(int i=0; i<batchNumber; i++)
                            {
                                Results += _Add_(Lt.GetRange(i * MAX_Params_Number, MAX_Params_Number));
                            }
                            Results += _Add_(Lt.GetRange(batchNumber * MAX_Params_Number,Lt.Count-batchNumber * MAX_Params_Number));
                        }
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }
            private static int _Add_(List<Moperson> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int r = -1;
                        using(SqlConnection cnn = new SqlConnection(Models.DataBase.Config.GetConnectionString()))
                        {
                            cnn.Open();
                            string query = "";
                            SqlCommand cmd = new SqlCommand(query, cnn);

                            int i = 0;
                            foreach (Moperson t in Lt)
                            {
                                i++;
                                query += " INSERT INTO Moperson(address,age,date_of_birth,first_name,last_name) VALUES( "

									+ "@address"+ i +","
									+ "@age"+ i +","
									+ "@date_of_birth"+ i +","
									+ "@first_name"+ i +","
									+ "@last_name"+ i 
                                     + "); ";

                                    
									cmd.Parameters.AddWithValue("address" + i, t.address);
									cmd.Parameters.AddWithValue("age" + i, t.age == null ? (object)DBNull.Value  : t.age);
									cmd.Parameters.AddWithValue("date_of_birth" + i, t.date_of_birth == null ? (object)DBNull.Value  : t.date_of_birth);
									cmd.Parameters.AddWithValue("first_name" + i, t.first_name);
									cmd.Parameters.AddWithValue("last_name" + i, t.last_name);
                            }

                            cmd.CommandText = query;

                            r = cmd.ExecuteNonQuery();
                        }

                        return r;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }

            public static int Edit(Moperson T)
            {
                try
                {        
                    int r = -1;
                    using(SqlConnection cnn = new SqlConnection(Models.DataBase.Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = "UPDATE Moperson SET address=@address,age=@age,date_of_birth=@date_of_birth,first_name=@first_name,last_name=@last_name WHERE Id=@Id";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                    
                        cmd.Parameters.AddWithValue("Id", T.Id);
						cmd.Parameters.AddWithValue("address", T.address);
						cmd.Parameters.AddWithValue("age", T.age == null ? (object)DBNull.Value  : T.age);
						cmd.Parameters.AddWithValue("date_of_birth", T.date_of_birth == null ? (object)DBNull.Value  : T.date_of_birth);
						cmd.Parameters.AddWithValue("first_name", T.first_name);
						cmd.Parameters.AddWithValue("last_name", T.last_name);
                        
                        r = cmd.ExecuteNonQuery();
                    }
                
                    return r;
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static int Edit(List<Moperson> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int MAX_Params_Number = Models.DataBase.Config.MAX_BDMS_PARAMS_NUM / 6; // Nb params per query
                        int Results=0;
                        if(Lt.Count <= MAX_Params_Number)
                        {
                            Results = _Edit_(Lt);
                        }else
                        {
                            int batchNumber = Lt.Count / MAX_Params_Number;
                            for(int i=0; i<batchNumber; i++)
                            {
                                Results += _Edit_(Lt.GetRange(i * MAX_Params_Number, MAX_Params_Number));
                            }
                            Results += _Edit_(Lt.GetRange(batchNumber * MAX_Params_Number,Lt.Count-batchNumber * MAX_Params_Number));
                        }
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }
            private static int _Edit_(List<Moperson> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int r = -1;
                        using(SqlConnection cnn = new SqlConnection(Models.DataBase.Config.GetConnectionString()))
                        {
                            cnn.Open();
                            string query = "";
                            SqlCommand cmd = new SqlCommand(query, cnn);

                            int i = 0;
                            foreach (Moperson t in Lt)
                            {
                                i++;
                                query += " UPDATE Moperson SET "

									+ "address=@address"+ i +","
									+ "age=@age"+ i +","
									+ "date_of_birth=@date_of_birth"+ i +","
									+ "first_name=@first_name"+ i +","
									+ "last_name=@last_name"+ i +" WHERE Id=@Id" + i 
                                     + "; ";

                                    cmd.Parameters.AddWithValue("Id" + i, t.Id);
									cmd.Parameters.AddWithValue("address" + i, t.address);
									cmd.Parameters.AddWithValue("age" + i, t.age == null ? (object)DBNull.Value  : t.age);
									cmd.Parameters.AddWithValue("date_of_birth" + i, t.date_of_birth == null ? (object)DBNull.Value  : t.date_of_birth);
									cmd.Parameters.AddWithValue("first_name" + i, t.first_name);
									cmd.Parameters.AddWithValue("last_name" + i, t.last_name);
                            }

                            cmd.CommandText = query;

                            r = cmd.ExecuteNonQuery();
                        }

                        return r;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }

            public static int Delete(long Id)
            {
                try
                {
                    int r = -1;
                    using(SqlConnection cnn = new SqlConnection(Models.DataBase.Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = "DELETE FROM Moperson WHERE Id=@Id";
                        SqlCommand cmd = new SqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("Id", Id);

                        r = cmd.ExecuteNonQuery();
                    }

                    return r;
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static int Delete(List<long> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        int MAX_Params_Number = Models.DataBase.Config.MAX_BDMS_PARAMS_NUM; 
                        int Results=0;
                        if(LIds.Count <= MAX_Params_Number)
                        {
                            Results = _Delete_(LIds);
                        }else
                        {
                            int batchNumber = LIds.Count / MAX_Params_Number;
                            for(int i=0; i<batchNumber; i++)
                            {
                                Results += _Delete_(LIds.GetRange(i * MAX_Params_Number, MAX_Params_Number));
                            }
                            Results += _Delete_(LIds.GetRange(batchNumber * MAX_Params_Number,LIds.Count-batchNumber * MAX_Params_Number));
                        }
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return -1;
            }
            private static int _Delete_(List<long> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        int r = -1;
                        using(SqlConnection cnn = new SqlConnection(Models.DataBase.Config.GetConnectionString()))
                        {
                            cnn.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = cnn;

                            string queryIds = string.Empty;
                            for(int i=0; i<LIds.Count; i++)
                            {
                                queryIds += "@Id"+i+",";
                                cmd.Parameters.AddWithValue("Id" + i, LIds[i]);
                            }
                            queryIds = queryIds.TrimEnd(',');

                            string query = "DELETE FROM Moperson WHERE Id IN ("+ queryIds +")";                    
                            cmd.CommandText = query;
                        
                            r = cmd.ExecuteNonQuery();
                        }

                        return r;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return -1;
            }
            #endregion

            #region Custom Methods


            #endregion

            #region Helpers
            public class Helpers
            {
                public static List<Moperson> GetListFromDataTable(DataTable dt)
                {
                    List<Moperson> L = new List<Moperson>(dt.Rows.Count);
                    foreach (DataRow dr in dt.Rows)
                    { L.Add(new Moperson(dr)); }
                    return L;
                }
            }
            #endregion
        }
    }
}
