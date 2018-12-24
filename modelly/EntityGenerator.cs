using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace ErnstUtils.Helpers
{
    public class EntityGenerator
    {
        public enum DatabaseType : int
        {
            SqlServer = 0,
            MySql = 1,
            PostgreSql = 2
        }


        public static string folderPath = @"GeneratedCode/";
        static string accessFolder;
        static string configFolder;
        static string HelpersFolder;


        public static string GenerateFiles(string key, string ProjectName, string ClassName, int IdIdx, List<string> ClassAttributes,List<string> ClassAttributesType,List<string> ClassAttributesNullable,List<List<string>> ClassEnums,string strConn, DatabaseType MyDBType, bool IsDotNetCore, bool configFiles, bool helpersFiles,string MyDBServerIp = "localhost", string MyDBPort = "3306", string MyDBName = "test", string MyDBUsername = "root", string MyDBUserpasswd = "NewRam5Sab")
        {
            try
            {
                //Create directories
                ////accessFolder = folderPath + @"\Models\Database\Access";
                ////if (!Directory.Exists(accessFolder)) { Directory.CreateDirectory(accessFolder); }
                ////string dbPath = folderPath + @"\Models\Database\" + ClassName + ".cs";
                ////string accPath = folderPath + @"\Models\Database\Access\" + ClassName + ".cs";

                ////writeFile(dbPath, getDatabaseFile(ProjectName, ClassName, ClassAttributes, ClassAttributesType, ClassAttributesNullable, ClassEnums, IsDotNetCore));
                ////writeFile(accPath, getAccessFile(ProjectName, ClassName, IdIdx, ClassAttributes, ClassAttributesType, ClassAttributesNullable, MyDBType, IsDotNetCore));
                ////if (configFiles)
                ////{
                ////    configFolder = folderPath + @"\Models\Database\Config";
                ////    if (!Directory.Exists(configFolder)) { Directory.CreateDirectory(configFolder); }
                ////    string cfgPath = folderPath + @"\Models\Database\Config\Config.cs";
                ////    writeFile(cfgPath, getConfigFile(ProjectName, strConn, MyDBType, MyDBServerIp, MyDBPort, MyDBName, MyDBUsername, MyDBUserpasswd));
                ////}
                ////if (helpersFiles)
                ////{
                ////    HelpersFolder = folderPath + @"\Models\Database\Helpers";
                ////    if (!Directory.Exists(HelpersFolder)) { Directory.CreateDirectory(HelpersFolder); }
                ////    string hlpPath = folderPath + @"\Models\Database\Helpers\Helpers.cs";
                ////    writeFile(hlpPath, getHelpersFile(ProjectName));
                ////}

                return key;
            }
            catch
            {
                return null;
            }
        }
        public static string GenerateFiles(string key, string ProjectName, string ClassName, int IdIdx, List<string> ClassAttributes, List<string> ClassAttributesType, List<string> ClassAttributesNullable, List<List<string>> ClassEnums, string projectConnectionString, DatabaseType MyDBType, bool IsDotNetCore, string ConnectionString, bool configFiles, bool helpersFiles, string CfgNameSpace = "", string HlpNameSpace = "")
        {
            try
            {
                //Create directories
                accessFolder = folderPath + @"\Models\Database\Access";
                if (!Directory.Exists(accessFolder)) { Directory.CreateDirectory(accessFolder); }
                string dbPath = folderPath + @"\Models\Database\" + ClassName + ".cs";
                string accPath = folderPath + @"\Models\Database\Access\" + ClassName + ".cs";

                writeFile(dbPath, getDatabaseFile(ProjectName, ClassName, ClassAttributes, ClassAttributesType, ClassAttributesNullable, ClassEnums, IsDotNetCore));
                writeFile(accPath, getAccessFile(ProjectName, ClassName, IdIdx, ClassAttributes, ClassAttributesType, ClassAttributesNullable, MyDBType, IsDotNetCore,CfgNameSpace,HlpNameSpace));
                if (configFiles)
                {
                    configFolder = folderPath + @"\Models\Database\Config";
                    if (!Directory.Exists(configFolder)) { Directory.CreateDirectory(configFolder); }
                    string cfgPath = folderPath + @"\Models\Database\Config\Config.cs";
                    writeFile(cfgPath, getConfigFile(ProjectName, projectConnectionString, MyDBType, ConnectionString, CfgNameSpace));
                }
                if (helpersFiles)
                {
                    HelpersFolder = folderPath + @"\Models\Database\Helpers";
                    if (!Directory.Exists(HelpersFolder)) { Directory.CreateDirectory(HelpersFolder); }
                    string hlpPath = folderPath + @"\Models\Database\Helpers\Helpers.cs";
                    writeFile(hlpPath, getHelpersFile(ProjectName,HlpNameSpace));
                }

                return Path.GetFullPath(folderPath);
            }
            catch
            {
                throw;
            }
        }

        #region FileCreation
        static string getDatabaseFile(string ProjectName, string ClassName, List<string> ClassAttributes, List<string> ClassAttributesType, List<string> ClassAttributesNullable, List<List<string>> ClassEnums, bool IsDotNetCore)
        {
            string usings = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


";
            if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrWhiteSpace(ProjectName))
            {
                ProjectName += ".";
            }
            string corpus = @"namespace " + ProjectName + @"Models.DataBase
{
";
            if (IsDotNetCore) { corpus += @"using System.Data.SqlClient;"; }
            corpus += @"
    public partial class " + ClassName + @"
    {";
            if (ClassEnums != null && ClassEnums.Count > 0)
            {
                corpus += @"
        #region Enums";
                for (int i = 0; i < ClassEnums.Count; i++)
                {
                    corpus += @"
            public enum enum" + ClassEnums[i][0] + @" {";
                    for (int j = 1; j < ClassEnums[i].Count; j++)
                    {
                        corpus += ClassEnums[i][j] + ", ";
                    }

                    corpus = corpus.TrimEnd(',');
                    corpus += @"}";
                }

                corpus += @"
        #endregion
";
            }

            for (int i = 0; i < ClassAttributes.Count; i++)
            {
                if (ClassAttributesNullable[i].ToLower() == "yes")
                {
                    switch (ClassAttributesType[i].ToLower())
                    {
                        case "int":
                        case "smallint":
                        case "mediumint":
                            corpus += "\n\t\tpublic Nullable<int> " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "bigint":
                            corpus += "\n\t\tpublic Nullable<long> " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "real":
                            corpus += "\n\t\tpublic Nullable<float> " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        //case "decimal":
                        //case "double":
                        //case "float":
                        //    corpus += "\t\t\t" + ClassAttributes[i] + " = " + ClassAttributesType[i] + @".Parse(dr[""" + ClassAttributes[i] + "\"].ToString());\n";
                        //    break;
                        case "datetime":
                        case "datetime2":
                        case "date":
                        case "time":
                            corpus += "\n\t\tpublic Nullable<DateTime> " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "tinyint":
                        case "bool":
                        case "bit":
                            corpus += "\n\t\tpublic Nullable<bool> " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "string":
                        case "text":
                        case "tinytext":
                        case "ntext":
                        case "longtext":
                        case "mediumtext":
                        case "uniqueidentifier":
                            corpus += "\n\t\tpublic string " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "enum":
                            corpus += "\n\t\tpublic Nullable<enum" + ClassAttributes[i] + "> " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "blob":
                        case "longblob":
                        case "binary":
                        case "varbinary":
                            corpus += "\n\t\tpublic byte[] " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        default:
                            corpus += "\n\t\tpublic Nullable<" + ClassAttributesType[i] + "> " + ClassAttributes[i] + @" { get; set; }";
                            break;
                    }
                }
                else
                {
                    switch (ClassAttributesType[i].ToLower())
                    {
                        case "int":
                        case "smallint":
                        case "mediumint":
                            corpus += "\n\t\tpublic int " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "bigint":
                            corpus += "\n\t\tpublic long " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "real":
                            corpus += "\n\t\tpublic float " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "numeric":
                            corpus += "\t\t\tpublic Decimal " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "datetime2":
                        case "datetime":
                        case "date":
                        case "time":
                            corpus += "\n\t\tpublic DateTime " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "tinyint":
                        case "bool":
                        case "bit":
                            corpus += "\n\t\tpublic bool " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "string":
                        case "text":
                        case "tinytext":
                        case "ntext":
                        case "longtext":
                        case "mediumtext":
                        case "uniqueidentifier":
                            corpus += "\n\t\tpublic string " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "enum":
                            corpus += "\n\t\tpublic enum" + ClassAttributes[i] + " " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        case "blob":
                        case "longblob":
                        case "binary":
                        case "varbinary":
                            corpus += "\n\t\tpublic byte[] " + ClassAttributes[i] + @" { get; set; }";
                            break;
                        default:
                            corpus += "\n\t\tpublic " + ClassAttributesType[i] + " " + ClassAttributes[i] + @" { get; set; }";
                            break;
                    }
                }
            }

            corpus += @"

        public " + ClassName + @"() { }
";
            if (IsDotNetCore)
            {
                corpus += @"
        public " + ClassName + @"(SqlDataReader dr)
        {
";
            }
            else
            {
                corpus += @"
        public " + ClassName + @"(DataRow dr)
        {
";
            }

            for (int i = 0; i < ClassAttributes.Count; i++)
            {
                if (ClassAttributesNullable[i].ToLower() == "yes")
                {
                    switch (ClassAttributesType[i].ToLower())
                    {
                        case "int":
                        case "smallint":
                        case "mediumint":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (" + ClassAttributesType[i] + "?)null : Convert.ToInt32(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "bigint":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (long?)null : Convert.ToInt64(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "real":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (" + ClassAttributesType[i] + "?)null : Convert.ToSingle(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "decimal":
                        case "numeric":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (" + ClassAttributesType[i] + "?)null : Convert.ToDecimal(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "double":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (" + ClassAttributesType[i] + "?)null : Convert.ToDouble(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "float":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (" + ClassAttributesType[i] + "?)null : Convert.ToSingle(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "datetime2":
                        case "datetime":
                        case "date":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "time":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (TimeSpan?)null : TimeSpan.Parse(dr[\"" + ClassAttributes[i] + "\"].ToString());\n";
                            break;
                        case "tinyint":
                        case "bool":
                        case "bit":
                        case "boolean":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (bool?)null : Convert.ToBoolean(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "string":
                        case "text":
                        case "tinytext":
                        case "ntext":
                        case "longtext":
                        case "mediumtext":
                        case "uniqueidentifier":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? \"\" : Convert.ToString(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "char":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (" + ClassAttributesType[i] + "?)null : Convert.ToChar(dr[\"" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "enum":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? (enum" + ClassAttributes[i] + "?)null : (enum" + ClassAttributes[i] + ")Enum.Parse(typeof(enum" + ClassAttributes[i] + "), dr[\"" + ClassAttributes[i] + "\"].ToString());\n";
                            break;
                        case "blob":
                        case "longblob":
                        case "binary":
                        case "varbinary":
                            corpus += "\t\t\t" + ClassAttributes[i] + " = (dr[\"" + ClassAttributes[i] + "\"] == System.DBNull.Value) ? new byte[0] : (byte[])dr[\"" + ClassAttributes[i] + "\"];\n";
                            break;
                        default:
                            System.Windows.MessageBox.Show("Unsupported DB type, nullable : " + ClassAttributesType[i]);
                            break;
                    }
                }
                else
                {
                    switch (ClassAttributesType[i].ToLower())
                    {
                        case "int":
                        case "smallint":
                        case "mediumint":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = Convert.ToInt32(dr[""" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "bigint":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = Convert.ToInt64(dr[""" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "real":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = Convert.ToSingle(dr[""" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "decimal":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = Convert.ToDecimal(dr[""" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "double":
                        case "float":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = Convert.ToDouble(dr[""" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "datetime2":
                        case "datetime":
                        case "date":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = Convert.ToDateTime(dr[""" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "time":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = TimeSpan.Pase(dr[""" + ClassAttributes[i] + "\"].ToString());\n";
                            break;
                        case "string":
                        case "text":
                        case "tinytext":
                        case "ntext":
                        case "longtext":
                        case "mediumtext":
                        case "uniqueidentifier":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = Convert.ToString(dr[""" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "tinyint":
                        case "bit":
                        case "bool":
                        case "boolean":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = Convert.ToBoolean(dr[""" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "char":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = Convert.ToChar(dr[""" + ClassAttributes[i] + "\"]);\n";
                            break;
                        case "enum":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = (enum" + ClassAttributes[i] + ")Enum.Parse(typeof(enum" + ClassAttributes[i] + "), dr[\"" + ClassAttributes[i] + "\"].ToString());\n";
                            break;
                        case "blob":
                        case "longblob":
                        case "binary":
                        case "varbinary":
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = (byte[])dr[""" + ClassAttributes[i] + "\"];\n";
                            break;
                        default:
                            corpus += "\t\t\t" + ClassAttributes[i] + @" = (" + ClassAttributesType[i] + @")dr[""" + ClassAttributes[i] + "\"];\n";
                            break;
                    }
                }
            }
            corpus += @"        }
    }
}
";

            return usings + corpus;
        }
        static string getAccessFile(string ProjectName, string ClassName, int IdIdx, List<string> ClassAttributes, List<string> ClassAttributesType, List<string> ClassAttributesNullable, DatabaseType MyDBType, bool IsDotNetCore, string CfgNameSpace = "", string HlpNameSpace = "")
        {
            string usingType = string.Empty,
                ConnectionType = string.Empty,
                CommandType = string.Empty,
                DataAdapterType = string.Empty;
            switch (MyDBType)
            {
                case DatabaseType.SqlServer:
                    {
                        usingType = "using System.Data.SqlClient;";
                        ConnectionType = "SqlConnection";
                        CommandType = "SqlCommand";
                        DataAdapterType = "SqlDataAdapter";
                        break;
                    }
                case DatabaseType.MySql:
                    {
                        usingType = "using MySql.Data.MySqlClient;";
                        ConnectionType = "MySqlConnection";
                        CommandType = "MySqlCommand";
                        DataAdapterType = "MySqlDataAdapter";
                        break;
                    }
            }
            string usings = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
" + usingType;
            string IdxType = "int";
            if (ClassAttributesType[IdIdx].ToLower() == "bigint") { IdxType = "long"; } else { IdxType = ClassAttributesType[IdIdx]; }
            string corpus = "";
            if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrWhiteSpace(ProjectName))
            {
                ProjectName += ".";
            }
            if (string.IsNullOrEmpty(CfgNameSpace) || string.IsNullOrWhiteSpace(CfgNameSpace))
            {
                CfgNameSpace = ProjectName + "Models.DataBase";
            }
            if (string.IsNullOrEmpty(HlpNameSpace) || string.IsNullOrWhiteSpace(HlpNameSpace))
            {
                HlpNameSpace = ProjectName + "Models.DataBase";
            }

            corpus = "\n\n" + @"namespace " + ProjectName + @"Models.DataBase";


            string gets = getGetMethods(ClassName, ClassAttributes, IdxType, IdIdx, DataAdapterType, ConnectionType, CommandType, CfgNameSpace, IsDotNetCore, MyDBType);
            string adds = getAddMethods(ClassAttributesNullable, ClassAttributesType, ClassName, ClassAttributes, IdxType, IdIdx, DataAdapterType, ConnectionType, CommandType, CfgNameSpace, IsDotNetCore, MyDBType);
            string edits = getEditMethods(ClassAttributesNullable, ClassAttributesType, ClassName, ClassAttributes, IdxType, IdIdx, DataAdapterType, ConnectionType, CommandType, CfgNameSpace, IsDotNetCore, MyDBType);
            string deletes = getDeleteMethods(ClassName, ClassAttributes, IdxType, IdIdx, ConnectionType, CommandType, CfgNameSpace);

            corpus += @"
{
    public partial class " + ClassName + @"
    {
        public class Access
        {
            #region Default Methods
            " + gets + @"

            " + adds + @"

            " + edits + @"

            " + deletes + @"
            #endregion

            #region Custom Methods


            #endregion

            #region Helpers
            public class Helpers
            {";
            if (MyDBType == DatabaseType.SqlServer && IsDotNetCore)
            {

                corpus += @"

                public static List<" + ClassName + @"> GetListFromDataTable(SqlDataReader dr)
                {
                    List<" + ClassName + @"> L = new List<" + ClassName + @">();
                    while (dr.Read())
                    { L.Add(new " + ClassName + @"(dr)); }
                    return L;
                }";
            }
            else
            {
                corpus += @"
                public static List<" + ClassName + @"> GetListFromDataTable(DataTable dt)
                {
                    List<" + ClassName + @"> L = new List<" + ClassName + @">(dt.Rows.Count);
                    foreach (DataRow dr in dt.Rows)
                    { L.Add(new " + ClassName + @"(dr)); }
                    return L;
                }";
            }
            corpus += @"
            }
            #endregion
        }
    }
}";

            return usings + corpus;
        }
        static string getGetMethods(string ClassName, List<string> ClassAttributes, string IdxType, int IdIdx, string DataAdapterType, string ConnectionType, string CommandType, string CfgNameSpace, bool IsDotNetCore, DatabaseType MyDBType)
        {
            #region Gets

            string get_methods = @"public static " + ClassName + @" Get(" + IdxType + @" " + ClassAttributes[IdIdx] + @")
            {
                try
                {
                    " + DataAdapterType + @" SelectAdapter = new " + DataAdapterType + @"();
                    DataTable dt = new DataTable();
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""SELECT * FROM " + ClassName + @" WHERE " + ClassAttributes[IdIdx] + @"=@Id"";
                        " + CommandType + " cmd = new " + CommandType + @"(query, cnn);
                        cmd.Parameters.AddWithValue(""Id"", " + ClassAttributes[IdIdx] + @");";
            if (MyDBType == DatabaseType.SqlServer && IsDotNetCore)
            {
                get_methods += @" 

                        List<" + ClassName + @"> Lt = new List<" + ClassName + @">();
                        using (var reader = cmd.ExecuteReader())
                        {
                            Lt = Helpers.GetListFromDataTable(reader);
                        }
                    }
                    return Lt[0];";
            }
            else
            {
                get_methods += @" 

                        SelectAdapter = new " + DataAdapterType + @"(cmd);
                        SelectAdapter.Fill(dt);

                    }

                    if (dt.Rows.Count > 0)
                    {
                        return new " + ClassName + @"(dt.Rows[0]);
                    }
                    else
                    {
                        return null;
                    }";
            }

            get_methods += @"
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static List<" + ClassName + @"> Get()
            {
                try
                {         
                    " + DataAdapterType + @" SelectAdapter = new " + DataAdapterType + @"();  
                    DataTable dt = new DataTable();     
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""SELECT * FROM " + ClassName + @""";
                        " + CommandType + " cmd = new " + CommandType + @"(query, cnn);";
            if (MyDBType == DatabaseType.SqlServer && IsDotNetCore)
            {
                get_methods += @" 

                        List<" + ClassName + @"> Lt = new List<" + ClassName + @">();
                        using (var reader = cmd.ExecuteReader())
                        {
                            Lt = Helpers.GetListFromDataTable(reader);
                        }
                    }
                    return Lt;";
            }
            else
            {
                get_methods += @" 

                        SelectAdapter = new " + DataAdapterType + @"(cmd);
                        SelectAdapter.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        return Helpers.GetListFromDataTable(dt);
                    }
                    else
                    {
                        return new List<" + ClassName + @">();
                    }";
            }

            get_methods += @"
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static List<" + ClassName + @"> Get(List<" + IdxType + @"> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        int MAX_QUERY_SIZE = " + CfgNameSpace + @".Config.MAX_BDMS_PARAMS_NUM ; 
                        List<" + ClassName + @"> Results = null;
                        if(LIds.Count <= MAX_QUERY_SIZE)
                        {
                            Results = get(LIds);
                        }else
                        {
                            int batchSize = LIds.Count / MAX_QUERY_SIZE;
                            Results = new List<" + ClassName + @">();
                            for(int i=0; i<batchSize; i++)
                            {
                                Results.AddRange(get(LIds.GetRange(i * MAX_QUERY_SIZE, MAX_QUERY_SIZE)));
                            }
                            Results.AddRange(get(LIds.GetRange(batchSize * MAX_QUERY_SIZE, LIds.Count-batchSize * MAX_QUERY_SIZE)));
                        }
                        return Results;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return new List<" + ClassName + @">();
            }
            private static List<" + ClassName + @"> get(List<" + IdxType + @"> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        " + DataAdapterType + @" SelectAdapter = new " + DataAdapterType + @"();
                        DataTable dt = new DataTable();
                        using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                        {
                            cnn.Open();
                            " + CommandType + " cmd = new " + CommandType + @"();
                            cmd.Connection = cnn;

                            string queryIds = string.Empty;
                            for(int i=0; i<LIds.Count; i++)
                            {
                                queryIds += ""@Id""+i+"","";
                                cmd.Parameters.AddWithValue(""Id"" + i, LIds[i]);
                            }
                            queryIds = queryIds.TrimEnd(',');

                            cmd.CommandText = ""SELECT * FROM " + ClassName + @" WHERE " + ClassAttributes[IdIdx] + @" IN (""+ queryIds +"")"";                    
                        ";
            if (MyDBType == DatabaseType.SqlServer && IsDotNetCore)
            {
                get_methods += @" 
                            List<" + ClassName + @"> Lt = new List<" + ClassName + @">();
                            using (var reader = cmd.ExecuteReader())
                            {
                                Lt = Helpers.GetListFromDataTable(reader);
                            }
                        }
                    return Lt;";
            }
            else
            {
                get_methods += @"SelectAdapter = new " + DataAdapterType + @"(cmd);
                            SelectAdapter.Fill(dt);
                        }

                        if (dt.Rows.Count > 0)
                        {
                            return Helpers.GetListFromDataTable(dt);
                        }
                        else
                        {
                            return new List<" + ClassName + @">();
                        }";
            }

            get_methods += @"
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return new List<" + ClassName + @">();
            }";
            #endregion Gets

            #region Gets_async

            string get_methods_async = @"public static async Task<" + ClassName + @"> GetAsync(" + IdxType + @" " + ClassAttributes[IdIdx] + @")
            {
                try
                {
                    " + DataAdapterType + @" SelectAdapter = new " + DataAdapterType + @"();
                    DataTable dt = new DataTable();
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""SELECT * FROM " + ClassName + @" WHERE " + ClassAttributes[IdIdx] + @"=@Id"";
                        " + CommandType + " cmd = new " + CommandType + @"(query, cnn);
                        cmd.Parameters.AddWithValue(""Id"", " + ClassAttributes[IdIdx] + @");";
            if (MyDBType == DatabaseType.SqlServer && IsDotNetCore)
            {
                get_methods_async += @" 

                        List<" + ClassName + @"> Lt = new List<" + ClassName + @">();
                        using (var reader = cmd.ExecuteReader())
                        {
                            Lt = Helpers.GetListFromDataTable(reader);
                        }
                    }
                    return Lt[0];";
            }
            else
            {
                get_methods_async += @" 

                        SelectAdapter = new " + DataAdapterType + @"(cmd);
                        await SelectAdapter.FillAsync(dt);

                    }

                    if (dt.Rows.Count > 0)
                    {
                        return new " + ClassName + @"(dt.Rows[0]);
                    }
                    else
                    {
                        return null;
                    }";
            }

            get_methods_async += @"
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static async Task<List<" + ClassName + @">> GetAsync()
            {
                try
                {         
                    " + DataAdapterType + @" SelectAdapter = new " + DataAdapterType + @"();  
                    DataTable dt = new DataTable();     
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""SELECT * FROM " + ClassName + @""";
                        " + CommandType + " cmd = new " + CommandType + @"(query, cnn);";
            if (MyDBType == DatabaseType.SqlServer && IsDotNetCore)
            {
                get_methods_async += @" 

                        List<" + ClassName + @"> Lt = new List<" + ClassName + @">();
                        using (var reader = cmd.ExecuteReader())
                        {
                            Lt = Helpers.GetListFromDataTable(reader);
                        }
                    }
                    return Lt;";
            }
            else
            {
                get_methods_async += @" 

                        SelectAdapter = new " + DataAdapterType + @"(cmd);
                        await SelectAdapter.FillAsync(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        return Helpers.GetListFromDataTable(dt);
                    }
                    else
                    {
                        return new List<" + ClassName + @">();
                    }";
            }

            get_methods_async += @"
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static async Task<List<" + ClassName + @">> GetAsync(List<" + IdxType + @"> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        int MAX_QUERY_SIZE = " + CfgNameSpace + @".Config.MAX_BDMS_PARAMS_NUM ; 
                        List<" + ClassName + @"> Results = null;
                        if(LIds.Count <= MAX_QUERY_SIZE)
                        {
                            Results = await getAsync(LIds);
                        }else
                        {
                            int batchSize = LIds.Count / MAX_QUERY_SIZE;
                            Results = new List<" + ClassName + @">();
                            for(int i=0; i<batchSize; i++)
                            {
                                Results.AddRange(await getAsync(LIds.GetRange(i * MAX_QUERY_SIZE, MAX_QUERY_SIZE)));
                            }
                            Results.AddRange(await getAsync(LIds.GetRange(batchSize * MAX_QUERY_SIZE, LIds.Count-batchSize * MAX_QUERY_SIZE)));
                        }
                        return Results;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return new List<" + ClassName + @">();
            }
            private static async Task<List<" + ClassName + @">> getAsync(List<" + IdxType + @"> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        " + DataAdapterType + @" SelectAdapter = new " + DataAdapterType + @"();
                        DataTable dt = new DataTable();
                        using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                        {
                            cnn.Open();
                            " + CommandType + " cmd = new " + CommandType + @"();
                            cmd.Connection = cnn;

                            string queryIds = string.Empty;
                            for(int i=0; i<LIds.Count; i++)
                            {
                                queryIds += ""@Id""+i+"","";
                                cmd.Parameters.AddWithValue(""Id"" + i, LIds[i]);
                            }
                            queryIds = queryIds.TrimEnd(',');

                            cmd.CommandText = ""SELECT * FROM " + ClassName + @" WHERE " + ClassAttributes[IdIdx] + @" IN (""+ queryIds +"")"";                    
                        ";
            if (MyDBType == DatabaseType.SqlServer && IsDotNetCore)
            {
                get_methods_async += @" 
                            List<" + ClassName + @"> Lt = new List<" + ClassName + @">();
                            using (var reader = cmd.ExecuteReader())
                            {
                                Lt = Helpers.GetListFromDataTable(reader);
                            }
                        }
                    return Lt;";
            }
            else
            {
                get_methods_async += @"SelectAdapter = new " + DataAdapterType + @"(cmd);
                            await SelectAdapter.FillAsync(dt);
                        }

                        if (dt.Rows.Count > 0)
                        {
                            return Helpers.GetListFromDataTable(dt);
                        }
                        else
                        {
                            return new List<" + ClassName + @">();
                        }";
            }

            get_methods_async += @"
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return new List<" + ClassName + @">();
            }";
            #endregion Gets

            return get_methods +"\n\t\t\t"+ get_methods_async;
        }
        static string getAddMethods(List<string> ClassAttributesNullable, List<string> ClassAttributesType, string ClassName, List<string> ClassAttributes, string IdxType, int IdIdx, string DataAdapterType, string ConnectionType, string CommandType, string CfgNameSpace, bool IsDotNetCore, DatabaseType MyDBType)
        {
            #region Adds
            if (ClassAttributesType == null || ClassAttributesType.Count <= 1) { return string.Empty; } // not enough attributes to add methods

            string add_methods = @"public static int Add(" + ClassName + @" T)
            {
                try
                {
                    int InsertedID = -1;
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""INSERT INTO " + ClassName + @"";
            string atts = "";
            string vals = "";
            string parms = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++)
                {
                    if (i != IdIdx)
                    {
                        atts += ClassAttributes[i] + ",";
                        vals += "@" + ClassAttributes[i] + ",";

                        if (ClassAttributesNullable[i].ToLower() == "yes")
                        {
                            switch (ClassAttributesType[i].ToLower())
                            {
                                case "int":
                                case "smallint":
                                case "mediumint":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "bigint":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "real":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "decimal":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "double":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "float":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "datetime2":
                                case "datetime":
                                case "date":
                                case "time":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "tinyint":
                                case "bool":
                                case "bit":
                                case "boolean":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "string":
                                case "text":
                                case "tinytext":
                                case "ntext":
                                case "longtext":
                                case "mediumtext":
                                case "uniqueidentifier":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "char":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "enum":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (enum" + ClassAttributes[i] + "?)null  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "blob":
                                case "longblob":
                                case "binary":
                                case "varbinary":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? null  : T." + ClassAttributes[i] + ");";
                                    break;
                                default:
                                    System.Windows.MessageBox.Show("Unsupported DB type, nullable : " + ClassAttributesType[i]);
                                    break;
                            }
                        }
                        else
                        {
                            parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + ");";
                        }
                    }
                }
                if (atts != string.Empty) { atts = atts.Substring(0, atts.Length - 1); }
                if (vals != string.Empty) { vals = vals.Substring(0, vals.Length - 1); }
            }

            add_methods += "(" + atts + @")  VALUES (" + vals + @")"";

                        " + CommandType + " cmd = new " + CommandType + "(query, cnn);"

            + parms + @"

                        InsertedID = cmd.ExecuteNonQuery();
                ";
            #region return LastInserted ID ** FOR INTEGER IDS ONLY **
            if (ClassAttributesType[IdIdx].ToLower() == "int")
            {
                add_methods += @"
                    if (InsertedID > 0)
                    {";
                switch (MyDBType)
                {

                    case DatabaseType.MySql:
                        {
                            add_methods += @"
                        query = ""SELECT last_insert_id()"";";
                            break;
                        }
                    case DatabaseType.SqlServer:
                        {
                            add_methods += @"query = ""SELECT " + ClassAttributes[IdIdx] + @" FROM " + ClassName + @" WHERE Id = @@IDENTITY"";";
                            break;
                        }
                }

                add_methods += @"
                        cmd = new " + CommandType + @"(query, cnn);
                        object _InsertedID = cmd.ExecuteScalar();

                        if (_InsertedID != null)
                        {
                            InsertedID = Convert.ToInt32(_InsertedID.ToString());
                        }
                        else
                        {
                            InsertedID = -1;
                        }
                    }
                    else
                    {
                        InsertedID = -1;
                    }";
            }
            #endregion

            add_methods += @"
                    }

                    return InsertedID;
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static int Add(List<" + ClassName + @"> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int MAX_Params_Number = " + CfgNameSpace + @".Config.MAX_BDMS_PARAMS_NUM / " + ClassAttributes.Count + @"; // Nb params per query
                        int Results=0;
                        if(Lt.Count <= MAX_Params_Number)
                        {
                            Results = add(Lt);
                        }else
                        {
                            int batchSize = Lt.Count / MAX_Params_Number;
                            for(int i=0; i<batchSize; i++)
                            {
                                Results += add(Lt.GetRange(i * MAX_Params_Number, MAX_Params_Number));
                            }
                            Results += add(Lt.GetRange(batchSize * MAX_Params_Number,Lt.Count-batchSize * MAX_Params_Number));
                        }
                        return Results;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }
            private static int add(List<" + ClassName + @"> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int r = -1;
                        using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                        {
                            cnn.Open();
                            string query = """";
                            " + CommandType + " cmd = new " + CommandType + @"(query, cnn);

                            int i = 0;
                            foreach (" + ClassName + @" t in Lt)
                            {
                                i++;
                                query += "" INSERT INTO " + ClassName + "(" + atts + ") VALUES( \"\n";
            string attsmu = "";
            string valsmu = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++)
                {
                    if (i != IdIdx)
                    {
                        attsmu += "\n\t\t\t\t\t\t\t\t\t+ \"@" + ClassAttributes[i] + "\"+ i +\",\"";
                        if (ClassAttributesNullable[i].ToLower() == "yes")
                        {
                            switch (ClassAttributesType[i].ToLower())
                            {
                                case "int":
                                case "smallint":
                                case "mediumint":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "bigint":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value : t." + ClassAttributes[i] + ");";
                                    break;
                                case "real":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "decimal":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "double":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "float":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "datetime2":
                                case "datetime":
                                case "date":
                                case "time":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "tinyint":
                                case "bool":
                                case "bit":
                                case "boolean":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "string":
                                case "text":
                                case "tinytext":
                                case "ntext":
                                case "longtext":
                                case "mediumtext":
                                case "uniqueidentifier":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "char":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "enum":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (enum" + ClassAttributes[i] + "?)null  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "blob":
                                case "longblob":
                                case "binary":
                                case "varbinary":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? null  : t." + ClassAttributes[i] + ");";
                                    break;
                                default:
                                    System.Windows.MessageBox.Show("Unsupported DB type, nullable : " + ClassAttributesType[i]);
                                    break;
                            }
                        }
                        else
                        {
                            valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + ");";
                        }
                    }
                }
            }

            add_methods += attsmu.Substring(0,attsmu.Length-4) + @"
                                     + ""); "";

                                    ";
            add_methods += valsmu +
    @"
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
            }";
            #endregion Adds

            #region Adds_async
            if (ClassAttributesType == null || ClassAttributesType.Count <= 1) { return string.Empty; } // not enough attributes to add methods

            string add_methods_async = @"public static async Task<int> AddAsync(" + ClassName + @" T)
            {
                try
                {
                    int InsertedID = -1;
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""INSERT INTO " + ClassName + @"";
            atts = "";
            vals = "";
            parms = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++)
                {
                    if (i != IdIdx)
                    {
                        atts += ClassAttributes[i] + ",";
                        vals += "@" + ClassAttributes[i] + ",";

                        if (ClassAttributesNullable[i].ToLower() == "yes")
                        {
                            switch (ClassAttributesType[i].ToLower())
                            {
                                case "int":
                                case "smallint":
                                case "mediumint":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "bigint":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "real":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "decimal":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "double":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "float":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "datetime2":
                                case "datetime":
                                case "date":
                                case "time":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "tinyint":
                                case "bool":
                                case "bit":
                                case "boolean":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "string":
                                case "text":
                                case "tinytext":
                                case "ntext":
                                case "longtext":
                                case "mediumtext":
                                case "uniqueidentifier":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "char":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "enum":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (enum" + ClassAttributes[i] + "?)null  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "blob":
                                case "longblob":
                                case "binary":
                                case "varbinary":
                                    parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? null  : T." + ClassAttributes[i] + ");";
                                    break;
                                default:
                                    System.Windows.MessageBox.Show("Unsupported DB type, nullable : " + ClassAttributesType[i]);
                                    break;
                            }
                        }
                        else
                        {
                            parms += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + ");";
                        }
                    }
                }
                if (atts != string.Empty) { atts = atts.Substring(0, atts.Length - 1); }
                if (vals != string.Empty) { vals = vals.Substring(0, vals.Length - 1); }
            }

            add_methods_async += "(" + atts + @")  VALUES (" + vals + @")"";

                        " + CommandType + " cmd = new " + CommandType + "(query, cnn);"

            + parms + @"

                        InsertedID = await cmd.ExecuteNonQueryAsync();
                ";
            #region return LastInserted ID ** FOR INTEGER IDS ONLY **
            if (ClassAttributesType[IdIdx].ToLower() == "int")
            {
                add_methods_async += @"
                    if (InsertedID > 0)
                    {";
                switch (MyDBType)
                {

                    case DatabaseType.MySql:
                        {
                            add_methods_async += @"
                        query = ""SELECT last_insert_id()"";";
                            break;
                        }
                    case DatabaseType.SqlServer:
                        {
                            add_methods_async += @"query = ""SELECT " + ClassAttributes[IdIdx] + @" FROM " + ClassName + @" WHERE Id = @@IDENTITY"";";
                            break;
                        }
                }

                add_methods_async += @"
                        cmd = new " + CommandType + @"(query, cnn);
                        object _InsertedID = await cmd.ExecuteScalarAsync();

                        if (_InsertedID != null)
                        {
                            InsertedID = Convert.ToInt32(_InsertedID.ToString());
                        }
                        else
                        {
                            InsertedID = -1;
                        }
                    }
                    else
                    {
                        InsertedID = -1;
                    }";
            }
            #endregion

            add_methods_async += @"
                    }

                    return InsertedID;
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static async Task<int> AddAsync(List<" + ClassName + @"> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int MAX_Params_Number = " + CfgNameSpace + @".Config.MAX_BDMS_PARAMS_NUM / " + ClassAttributes.Count + @"; // Nb params per query
                        int Results=0;
                        if(Lt.Count <= MAX_Params_Number)
                        {
                            Results = await addAsync(Lt);
                        }else
                        {
                            int batchSize = Lt.Count / MAX_Params_Number;
                            for(int i=0; i<batchSize; i++)
                            {
                                Results += await addAsync(Lt.GetRange(i * MAX_Params_Number, MAX_Params_Number));
                            }
                            Results += await addAsync(Lt.GetRange(batchSize * MAX_Params_Number,Lt.Count-batchSize * MAX_Params_Number));
                        }
                        return Results;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }
            private static async Task<int> addAsync(List<" + ClassName + @"> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int r = -1;
                        using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                        {
                            cnn.Open();
                            string query = """";
                            " + CommandType + " cmd = new " + CommandType + @"(query, cnn);

                            int i = 0;
                            foreach (" + ClassName + @" t in Lt)
                            {
                                i++;
                                query += "" INSERT INTO " + ClassName + "(" + atts + ") VALUES( \"\n";
            attsmu = "";
            valsmu = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++)
                {
                    if (i != IdIdx)
                    {
                        attsmu += "\n\t\t\t\t\t\t\t\t\t+ \"@" + ClassAttributes[i] + "\"+ i +\",\"";
                        if (ClassAttributesNullable[i].ToLower() == "yes")
                        {
                            switch (ClassAttributesType[i].ToLower())
                            {
                                case "int":
                                case "smallint":
                                case "mediumint":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "bigint":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value : t." + ClassAttributes[i] + ");";
                                    break;
                                case "real":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "decimal":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "double":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "float":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "datetime2":
                                case "datetime":
                                case "date":
                                case "time":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "tinyint":
                                case "bool":
                                case "bit":
                                case "boolean":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "string":
                                case "text":
                                case "tinytext":
                                case "ntext":
                                case "longtext":
                                case "mediumtext":
                                case "uniqueidentifier":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "char":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "enum":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (enum" + ClassAttributes[i] + "?)null  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "blob":
                                case "longblob":
                                case "binary":
                                case "varbinary":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? null  : t." + ClassAttributes[i] + ");";
                                    break;
                                default:
                                    System.Windows.MessageBox.Show("Unsupported DB type, nullable : " + ClassAttributesType[i]);
                                    break;
                            }
                        }
                        else
                        {
                            valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + ");";
                        }
                    }
                }
            }

            add_methods_async += attsmu.Substring(0, attsmu.Length - 4) + @"
                                     + ""); "";

                                    ";
            add_methods_async += valsmu +
    @"
                            }

                            cmd.CommandText = query;

                            r = await cmd.ExecuteNonQueryAsync();
                        }

                        return r;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }";
            #endregion Adds

            return add_methods+ "\n\t\t\t"+ add_methods_async;
        }
        static string getEditMethods(List<string> ClassAttributesNullable, List<string> ClassAttributesType, string ClassName, List<string> ClassAttributes, string IdxType, int IdIdx, string DataAdapterType, string ConnectionType, string CommandType, string CfgNameSpace, bool IsDotNetCore, DatabaseType MyDBType)
        {
            #region Edits
            if (ClassAttributesType == null || ClassAttributesType.Count <= 1) { return string.Empty; } // not enough attributes to add methods


            string edit_methods = @"public static int Edit(" + ClassName + @" T)
            {
                try
                {        
                    int r = -1;
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""UPDATE " + ClassName + @" SET ";
            string attsu = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++) { if (i != IdIdx) { attsu += ClassAttributes[i] + "=@" + ClassAttributes[i] + ","; } }
            }
            if (attsu != "")
            {
                attsu = attsu.Substring(0, attsu.Length - 1);
            }
            string parms1 = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++)
                {
                    if (i != IdIdx)
                    {
                        if (ClassAttributesNullable[i].ToLower() == "yes")
                        {
                            switch (ClassAttributesType[i].ToLower())
                            {
                                case "int":
                                case "smallint":
                                case "mediumint":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "bigint":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "real":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "decimal":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "double":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "float":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "datetime2":
                                case "datetime":
                                case "date":
                                case "time":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "tinyint":
                                case "bool":
                                case "bit":
                                case "boolean":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "string":
                                case "text":
                                case "tinytext":
                                case "ntext":
                                case "longtext":
                                case "mediumtext":
                                case "uniqueidentifier":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "char":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "enum":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (enum" + ClassAttributes[i] + "?)null  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "blob":
                                case "longblob":
                                case "binary":
                                case "varbinary":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? null  : T." + ClassAttributes[i] + ");";
                                    break;
                                default:
                                    System.Windows.MessageBox.Show("Unsupported DB type, nullable : " + ClassAttributesType[i]);
                                    break;
                            }
                        }
                        else
                        {
                            parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + ");";
                        }
                    }
                }
            }

            edit_methods += attsu + " WHERE " + ClassAttributes[IdIdx] + "=@" + ClassAttributes[IdIdx] + @""";
                        " + CommandType + " cmd = new " + CommandType + @"(query, cnn);
                    
                        cmd.Parameters.AddWithValue(""" + ClassAttributes[IdIdx] + @""", T." + ClassAttributes[IdIdx] + ");" + parms1 +
                    @"
                        
                        r = cmd.ExecuteNonQuery();
                    }
                
                    return r;
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static int Edit(List<" + ClassName + @"> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int MAX_Params_Number = " + CfgNameSpace + @".Config.MAX_BDMS_PARAMS_NUM / " + ClassAttributes.Count + @"; // Nb params per query
                        int Results=0;
                        if(Lt.Count <= MAX_Params_Number)
                        {
                            Results = edit(Lt);
                        }else
                        {
                            int batchSize = Lt.Count / MAX_Params_Number;
                            for(int i=0; i<batchSize; i++)
                            {
                                Results += edit(Lt.GetRange(i * MAX_Params_Number, MAX_Params_Number));
                            }
                            Results += edit(Lt.GetRange(batchSize * MAX_Params_Number,Lt.Count-batchSize * MAX_Params_Number));
                        }
                        return Results;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }
            private static int edit(List<" + ClassName + @"> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int r = -1;
                        using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                        {
                            cnn.Open();
                            string query = """";
                            " + CommandType + " cmd = new " + CommandType + @"(query, cnn);

                            int i = 0;
                            foreach (" + ClassName + @" t in Lt)
                            {
                                i++;
                                query += "" UPDATE " + ClassName + " SET \"\n";
            string attsmu = "";
            string valsmu = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++)
                {
                    if (i != IdIdx)
                    {
                        attsmu += "\n\t\t\t\t\t\t\t\t\t+ \"" + ClassAttributes[i] + "=@" + ClassAttributes[i] + "\"+ i +\",\"";
                        if (ClassAttributesNullable[i].ToLower() == "yes")
                        {
                            switch (ClassAttributesType[i].ToLower())
                            {
                                case "int":
                                case "smallint":
                                case "mediumint":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "bigint":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value : t." + ClassAttributes[i] + ");";
                                    break;
                                case "real":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "decimal":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "double":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "float":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "datetime2":
                                case "datetime":
                                case "date":
                                case "time":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "tinyint":
                                case "bool":
                                case "bit":
                                case "boolean":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "string":
                                case "text":
                                case "tinytext":
                                case "ntext":
                                case "longtext":
                                case "mediumtext":
                                case "uniqueidentifier":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "char":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "enum":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (enum" + ClassAttributes[i] + "?)null  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "blob":
                                case "longblob":
                                case "binary":
                                case "varbinary":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? null  : t." + ClassAttributes[i] + ");";
                                    break;
                                default:
                                    System.Windows.MessageBox.Show("Unsupported DB type, nullable : " + ClassAttributesType[i]);
                                    break;
                            }
                        }
                        else
                        {
                            valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + ");";
                        }
                    }
                }
            }

            edit_methods += attsmu.Substring(0,attsmu.Length-4) + @"+"" WHERE " + ClassAttributes[IdIdx] + "=@" + ClassAttributes[IdIdx] + @""" + i 
                                     + ""; "";

                                    cmd.Parameters.AddWithValue(""" + ClassAttributes[IdIdx] + @""" + i, t." + ClassAttributes[IdIdx] + @");" +
        valsmu +
    @"
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
            }";
            #endregion Edits

            #region Edits_async
            if (ClassAttributesType == null || ClassAttributesType.Count <= 1) { return string.Empty; } // not enough attributes to add methods


            string edit_methods_async = @"public static async Task<int> EditAsync(" + ClassName + @" T)
            {
                try
                {        
                    int r = -1;
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""UPDATE " + ClassName + @" SET ";
            attsu = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++) { if (i != IdIdx) { attsu += ClassAttributes[i] + "=@" + ClassAttributes[i] + ","; } }
            }
            if (attsu != "")
            {
                attsu = attsu.Substring(0, attsu.Length - 1);
            }
            parms1 = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++)
                {
                    if (i != IdIdx)
                    {
                        if (ClassAttributesNullable[i].ToLower() == "yes")
                        {
                            switch (ClassAttributesType[i].ToLower())
                            {
                                case "int":
                                case "smallint":
                                case "mediumint":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "bigint":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "real":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "decimal":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "double":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "float":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "datetime2":
                                case "datetime":
                                case "date":
                                case "time":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "tinyint":
                                case "bool":
                                case "bit":
                                case "boolean":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "string":
                                case "text":
                                case "tinytext":
                                case "ntext":
                                case "longtext":
                                case "mediumtext":
                                case "uniqueidentifier":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "char":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "enum":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? (enum" + ClassAttributes[i] + "?)null  : T." + ClassAttributes[i] + ");";
                                    break;
                                case "blob":
                                case "longblob":
                                case "binary":
                                case "varbinary":
                                    parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + " == null ? null  : T." + ClassAttributes[i] + ");";
                                    break;
                                default:
                                    System.Windows.MessageBox.Show("Unsupported DB type, nullable : " + ClassAttributesType[i]);
                                    break;
                            }
                        }
                        else
                        {
                            parms1 += "\n\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\", T." + ClassAttributes[i] + ");";
                        }
                    }
                }
            }

            edit_methods_async += attsu + " WHERE " + ClassAttributes[IdIdx] + "=@" + ClassAttributes[IdIdx] + @""";
                        " + CommandType + " cmd = new " + CommandType + @"(query, cnn);
                    
                        cmd.Parameters.AddWithValue(""" + ClassAttributes[IdIdx] + @""", T." + ClassAttributes[IdIdx] + ");" + parms1 +
                    @"
                        
                        r = await cmd.ExecuteNonQueryAsync();
                    }
                
                    return r;
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static async Task<int> EditAsync(List<" + ClassName + @"> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int MAX_Params_Number = " + CfgNameSpace + @".Config.MAX_BDMS_PARAMS_NUM / " + ClassAttributes.Count + @"; // Nb params per query
                        int Results=0;
                        if(Lt.Count <= MAX_Params_Number)
                        {
                            Results = await editAsync(Lt);
                        }else
                        {
                            int batchSize = Lt.Count / MAX_Params_Number;
                            for(int i=0; i<batchSize; i++)
                            {
                                Results += await editAsync(Lt.GetRange(i * MAX_Params_Number, MAX_Params_Number));
                            }
                            Results += await editAsync(Lt.GetRange(batchSize * MAX_Params_Number,Lt.Count-batchSize * MAX_Params_Number));
                        }
                        return Results;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }
            private static async Task<int> editAsync(List<" + ClassName + @"> Lt)
            {
                if (Lt != null && Lt.Count > 0)
                {
                    try
                    {
                        int r = -1;
                        using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                        {
                            cnn.Open();
                            string query = """";
                            " + CommandType + " cmd = new " + CommandType + @"(query, cnn);

                            int i = 0;
                            foreach (" + ClassName + @" t in Lt)
                            {
                                i++;
                                query += "" UPDATE " + ClassName + " SET \"\n";
            attsmu = "";
            valsmu = "";
            if (ClassAttributes.Count > 1)
            {
                for (int i = 0; i < ClassAttributes.Count; i++)
                {
                    if (i != IdIdx)
                    {
                        attsmu += "\n\t\t\t\t\t\t\t\t\t+ \"" + ClassAttributes[i] + "=@" + ClassAttributes[i] + "\"+ i +\",\"";
                        if (ClassAttributesNullable[i].ToLower() == "yes")
                        {
                            switch (ClassAttributesType[i].ToLower())
                            {
                                case "int":
                                case "smallint":
                                case "mediumint":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "bigint":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value : t." + ClassAttributes[i] + ");";
                                    break;
                                case "real":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "decimal":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "double":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "float":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "datetime2":
                                case "datetime":
                                case "date":
                                case "time":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "tinyint":
                                case "bool":
                                case "bit":
                                case "boolean":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "string":
                                case "text":
                                case "tinytext":
                                case "ntext":
                                case "longtext":
                                case "mediumtext":
                                case "uniqueidentifier":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "char":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (object)DBNull.Value  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "enum":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? (enum" + ClassAttributes[i] + "?)null  : t." + ClassAttributes[i] + ");";
                                    break;
                                case "blob":
                                case "longblob":
                                case "binary":
                                case "varbinary":
                                    valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + " == null ? null  : t." + ClassAttributes[i] + ");";
                                    break;
                                default:
                                    System.Windows.MessageBox.Show("Unsupported DB type, nullable : " + ClassAttributesType[i]);
                                    break;
                            }
                        }
                        else
                        {
                            valsmu += "\n\t\t\t\t\t\t\t\t\tcmd.Parameters.AddWithValue(\"" + ClassAttributes[i] + "\" + i, t." + ClassAttributes[i] + ");";
                        }
                    }
                }
            }

            edit_methods_async += attsmu.Substring(0, attsmu.Length - 4) + @"+"" WHERE " + ClassAttributes[IdIdx] + "=@" + ClassAttributes[IdIdx] + @""" + i 
                                     + ""; "";

                                    cmd.Parameters.AddWithValue(""" + ClassAttributes[IdIdx] + @""" + i, t." + ClassAttributes[IdIdx] + @");" +
        valsmu +
    @"
                            }

                            cmd.CommandText = query;

                            r = await cmd.ExecuteNonQueryAsync();
                        }

                        return r;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }

                return -1;
            }";
            #endregion Edits_async

            return edit_methods + "\n\t\t\t"+ edit_methods_async;
        }
        static string getDeleteMethods(string ClassName, List<string> ClassAttributes, string IdxType, int IdIdx, string ConnectionType, string CommandType, string CfgNameSpace)
        {
            #region Deletes
            string delete_methods = @"public static int Delete(" + IdxType + " " + ClassAttributes[IdIdx] + @")
            {
                try
                {
                    int r = -1;
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""DELETE FROM " + ClassName + @" WHERE " + ClassAttributes[IdIdx] + "=@" + ClassAttributes[IdIdx] + @""";
                        " + CommandType + " cmd = new " + CommandType + @"(query, cnn);
                        cmd.Parameters.AddWithValue(""" + ClassAttributes[IdIdx] + @""", " + ClassAttributes[IdIdx] + @");

                        r = cmd.ExecuteNonQuery();
                    }

                    return r;
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static int Delete(List<" + IdxType + @"> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        int MAX_Params_Number = " + CfgNameSpace + @".Config.MAX_BDMS_PARAMS_NUM; 
                        int Results=0;
                        if(LIds.Count <= MAX_Params_Number)
                        {
                            Results = delete(LIds);
                        }else
                        {
                            int batchSize = LIds.Count / MAX_Params_Number;
                            for(int i=0; i<batchSize; i++)
                            {
                                Results += delete(LIds.GetRange(i * MAX_Params_Number, MAX_Params_Number));
                            }
                            Results += delete(LIds.GetRange(batchSize * MAX_Params_Number,LIds.Count-batchSize * MAX_Params_Number));
                        }
                        return Results;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return -1;
            }
            private static int delete(List<" + IdxType + @"> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        int r = -1;
                        using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                        {
                            cnn.Open();
                            " + CommandType + " cmd = new " + CommandType + @"();
                            cmd.Connection = cnn;

                            string queryIds = string.Empty;
                            for(int i=0; i<LIds.Count; i++)
                            {
                                queryIds += ""@Id""+i+"","";
                                cmd.Parameters.AddWithValue(""Id"" + i, LIds[i]);
                            }
                            queryIds = queryIds.TrimEnd(',');

                            string query = ""DELETE FROM " + ClassName + @" WHERE " + ClassAttributes[IdIdx] + @" IN (""+ queryIds +"")"";                    
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
            }";
            #endregion Deletes

            #region Deletes_async

            string delete_methods_aync = @"public static async Task<int> DeleteAsync(" + IdxType + " " + ClassAttributes[IdIdx] + @")
            {
                try
                {
                    int r = -1;
                    using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                    {
                        cnn.Open();
                        string query = ""DELETE FROM " + ClassName + @" WHERE " + ClassAttributes[IdIdx] + "=@" + ClassAttributes[IdIdx] + @""";
                        " + CommandType + " cmd = new " + CommandType + @"(query, cnn);
                        cmd.Parameters.AddWithValue(""" + ClassAttributes[IdIdx] + @""", " + ClassAttributes[IdIdx] + @");

                        r = await cmd.ExecuteNonQueryAsync();
                    }

                    return r;
                }
                catch(Exception Ex)
                {
                    throw Ex;
                }
            }
            public static async Task<int> DeleteAsync(List<" + IdxType + @"> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        int MAX_Params_Number = " + CfgNameSpace + @".Config.MAX_BDMS_PARAMS_NUM; 
                        int Results=0;
                        if(LIds.Count <= MAX_Params_Number)
                        {
                            Results = await deleteAsync(LIds);
                        }else
                        {
                            int batchSize = LIds.Count / MAX_Params_Number;
                            for(int i=0; i<batchSize; i++)
                            {
                                Results += await deleteAsync(LIds.GetRange(i * MAX_Params_Number, MAX_Params_Number));
                            }
                            Results += await deleteAsync(LIds.GetRange(batchSize * MAX_Params_Number,LIds.Count-batchSize * MAX_Params_Number));
                        }
                        return Results;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return -1;
            }
            private static async Task<int> deleteAsync(List<" + IdxType + @"> LIds)
            {
                if(LIds != null && LIds.Count > 0)
                {
                    try
                    {
                        int r = -1;
                        using(" + ConnectionType + " cnn = new " + ConnectionType + "(" + CfgNameSpace + @".Config.GetConnectionString()))
                        {
                            cnn.Open();
                            " + CommandType + " cmd = new " + CommandType + @"();
                            cmd.Connection = cnn;

                            string queryIds = string.Empty;
                            for(int i=0; i<LIds.Count; i++)
                            {
                                queryIds += ""@Id""+i+"","";
                                cmd.Parameters.AddWithValue(""Id"" + i, LIds[i]);
                            }
                            queryIds = queryIds.TrimEnd(',');

                            string query = ""DELETE FROM " + ClassName + @" WHERE " + ClassAttributes[IdIdx] + @" IN (""+ queryIds +"")"";                    
                            cmd.CommandText = query;
                        
                            r = await cmd.ExecuteNonQueryAsync();
                        }

                        return r;
                    }
                    catch(Exception Ex)
                    {
                        throw Ex;
                    }
                }
                return -1;
            }";
            #endregion Deletes

            return delete_methods + "\n\t\t\t"+ delete_methods_aync;
        }
        
        
        static string getConfigFile2(string ProjectName, string connectionString, DatabaseType MyDBType, string MyDBServerIp = "localhost", string MyDBPort = "3306", string MyDBName = "test", string MyDBUsername = "root", string MyDBUserpasswd = "NewRam5Sab")
        {
            string ConnectionValue;

            switch (MyDBType)
            {
                case DatabaseType.MySql:
                    {
                        ConnectionValue = "Server = " + MyDBServerIp + "; Port = " + MyDBPort + "; Database = " + MyDBName + "; Uid = " + MyDBUsername + "; Pwd = " + MyDBUserpasswd + ";";
                        break;
                    }
                case DatabaseType.SqlServer:
                    {
                        ConnectionValue = connectionString;
                        break;
                    }
            }
            string header = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Text;";
            if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrWhiteSpace(ProjectName))
            {
                ProjectName += ".";
            }
            string corpus = "\n\n" + @"namespace " + ProjectName + @"Models.DataBase";

            corpus += @"
{
    public class Config 
    {
        public const int MAX_BDMS_PARAMS_NUM = 100;
        public static string ServerIP { get; set; }
        private static bool DebugMode = true;
        
        public static string GetConnectionString()
        {
            // Add appSettings entry key=""ConnectionString"", value=""" + connectionString + @"""
            return System.Configuration.ConfigurationManager.AppSettings[""ConnectionString""].ToString();
        }

        public static bool GetDebugMode()
        {
            return DebugMode;
        }

    }
}";

            return header + corpus;
        }
        static string getConfigFile(string NamespaceProjectName, string projectConnectionString, DatabaseType MyDBType, string ConnectionString, string Namespace = "")
        {
            string ConnectionValue;

            switch (MyDBType)
            {
                case DatabaseType.MySql:
                    {
                        ConnectionValue = @ConnectionString;
                        break;
                    }
                case DatabaseType.SqlServer:
                    {
                        ConnectionValue = @ConnectionString;
                        break;
                    }
            }
            string header = @"using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
";
            string corpus = "";
            if (string.IsNullOrEmpty(Namespace) || string.IsNullOrWhiteSpace(Namespace))
            {
                if (!string.IsNullOrEmpty(NamespaceProjectName) && !string.IsNullOrWhiteSpace(NamespaceProjectName))
                {
                    NamespaceProjectName  += ".";
                }
                corpus = "\n" + @"namespace " + NamespaceProjectName + @"Models.DataBase";
            }
            else
            {
                corpus = "\n" + @"namespace " + Namespace + @"";
            }
            corpus += @"
{
    public class Config 
    {
        public const int MAX_BDMS_PARAMS_NUM = 100;
        public static string ServerIP { get; set; }
        private static bool DebugMode = true;
        
        public static string GetConnectionString()
        {
            // Add appSettings entry key=""ConnectionString"", value=""" + projectConnectionString + @"""
            return System.Configuration.ConfigurationManager.AppSettings[""ConnectionString""].ToString();
        }

        public static bool GetDebugMode()
        {
            return DebugMode;
        }

    }
}";

            return header + corpus;

        }
        static string getHelpersFile(string ProjectName,string Namespace="")
        {
            string corpus = "";
            if (string.IsNullOrWhiteSpace(Namespace) || string.IsNullOrEmpty(Namespace))
            {

                if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrWhiteSpace(ProjectName))
                {
                    ProjectName += ".";
                }
                corpus = @"namespace " + ProjectName + @"Models.DataBase";
            }
            else
            {
                corpus = @"namespace " + Namespace + @"";
            }

            corpus += @"
{
    public class Helpers 
    {
        public static void BugReport(Exception ex)
        {
            try
            {
                //string errorText = Ernst.Security.Crypting.GenerateErrorLine(ex);
                string errorText = ex.Source + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;
                if (!System.IO.File.Exists(""Reports.err""))
                {
                    using (System.IO.StreamWriter sw = System.IO.File.CreateText(""Reports.err""))
                    {
                        sw.Write(errorText);
                        sw.Close();
                    }
                }

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(""Reports.err"", true))
                {
                    sw.Write(errorText);
                    sw.Close();
                }
            }
            catch { }
        }
    }
}";
            return "using System;\n\n" + corpus;
        }
        #endregion

        static void writeFile(string ClassPath, string ClassContent)
        {
            try
            {
                if (File.Exists(ClassPath))
                {
                    File.Delete(ClassPath);
                }

                using (StreamWriter fs = new StreamWriter(ClassPath))
                {
                    fs.WriteLine(ClassContent);
                }
            }
            catch { throw; }
        }

        #region Get DB Tables Infos
        public static List<string> GetTables(string connectionString, DatabaseType DBType)
        {
            switch (DBType)
            {
                #region SqlServer
                case DatabaseType.SqlServer:
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            DataTable schema = connection.GetSchema("Tables");
                            List<string> TableNames = new List<string>();
                            foreach (DataRow row in schema.Rows)
                            {
                                TableNames.Add(FirstCharToUpper(row[2].ToString()));
                            }
                            return TableNames;
                        }
                        break;
                    }
                #endregion

                #region MySql
                case DatabaseType.MySql:
                    {
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();
                            DataTable schema = connection.GetSchema("Tables");
                            List<string> TableNames = new List<string>();
                            foreach (DataRow row in schema.Rows)
                            {
                                TableNames.Add(FirstCharToUpper(row[2].ToString()));
                            }
                            return TableNames;
                        }
                        break;
                    }
                #endregion
            }
            return null;
        }
        public static List<List<string>> GetColumns(string connectionString, string tableName, DatabaseType DBType)
        {
            List<string> listacolumnas = new List<string>(),
                listacolumntypeas = new List<string>(),
                listacolumnNullableas = new List<string>();
            //string DBName = GetDataBaseName(connectionString,  DBType);

            switch (DBType)
            {
                #region SqlServer
                case DatabaseType.SqlServer:
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "select distinct(column_name),data_type,is_nullable from information_schema.columns where table_catalog='" + connection.Database + "' AND table_name = '" + tableName + "'";
                            connection.Open();
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //for (int i = 0; i < reader.FieldCount; i++)
                                    //{
                                    //    if (!reader.IsDBNull(i))
                                    //    { System.Windows.Forms.MessageBox.Show(reader.GetString(i).ToString() + " " + reader.GetProviderSpecificFieldType(i)); }
                                    //}

                                    listacolumnas.Add(reader.GetString(0));
                                    listacolumntypeas.Add(reader.GetString(1));
                                    listacolumnNullableas.Add(reader.GetString(2));
                                }
                            }
                        }
                        break;
                    }
                #endregion

                #region MySql
                case DatabaseType.MySql:
                    {
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        using (MySqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "select distinct(column_name),data_type,is_nullable from information_schema.columns where table_schema='" + connection.Database + "' AND table_name='" + tableName + "'";
                            connection.Open();
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //for (int i = 0; i < reader.FieldCount; i++)
                                    //{
                                    //    if (!reader.IsDBNull(i))
                                    //    { System.Windows.Forms.MessageBox.Show(reader.GetString(i).ToString() + " " + reader.GetProviderSpecificFieldType(i)); }
                                    //}

                                    listacolumnas.Add(reader.GetString(0));
                                    listacolumntypeas.Add(reader.GetString(1));
                                    listacolumnNullableas.Add(reader.GetString(2));
                                }
                            }
                        }
                        break;
                    }
                #endregion
            }

            #region Property type conversion
            for (int i = 0; i < listacolumntypeas.Count; i++)
            {
                if (listacolumntypeas[i] == "bit")
                {
                    listacolumntypeas[i] = "bool";
                }
                else
                {
                    if (listacolumntypeas[i] == "datetime")
                    {
                        listacolumntypeas[i] = "DateTime";
                    }
                    else
                    {
                        if (listacolumntypeas[i] == "nvarchar" || listacolumntypeas[i] == "varchar")
                        {
                            listacolumntypeas[i] = "string";
                        }
                        else
                        {
                            if (listacolumntypeas[i] == "nchar" || listacolumntypeas[i] == "char")
                            {
                                listacolumntypeas[i] = "char";
                            }
                        }
                    }
                }
            }
            #endregion

            return new List<List<string>>() { listacolumnas, listacolumntypeas, listacolumnNullableas };
        }
        public static string GetPrimaryColumn(string connectionString, string tableName, DatabaseType DBType)
        {
            try
            {
                switch (DBType)
                {
                    #region SqlServer
                    case DatabaseType.SqlServer:
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            using (SqlCommand command = connection.CreateCommand())
                            {
                                command.CommandText = "SELECT column_name "
                                    + "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE "
                                    + "WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 "
                                    + "AND TABLE_NAME = '" + tableName + "' AND TABLE_CATALOG = '" + connection.Database + "'";
                                //command.Parameters.AddWithValue("tableName", tableName);
                                connection.Open();
                                return (string)command.ExecuteScalar();
                            }
                            break;
                        }
                    #endregion

                    #region MySql
                    case DatabaseType.MySql:
                        {

                            using (MySqlConnection connection = new MySqlConnection(connectionString))
                            using (MySqlCommand command = connection.CreateCommand())
                            {
                                command.CommandText = "SELECT COLUMN_NAME  FROM INFORMATION_SCHEMA.INDEXES  WHERE PRIMARY_KEY = 1  AND TABLE_NAME = '" + tableName + "'";
                                //command.Parameters.AddWithValue("tableName", tableName);
                                connection.Open();
                                return (string)command.ExecuteScalar();
                            }
                            break;
                        }
                    #endregion
                }
                return string.Empty;
            }
            catch (Exception esx)
            {
                return string.Empty;
            }
        }
        public static List<List<string>> GetEnums(string connectionString, string tableName, DatabaseType DBType)
        {
            List<List<string>> r = new List<List<string>>();
            string temp = "";

            switch (DBType)
            {
                #region SqlServer
                case DatabaseType.SqlServer:
                    {
                        List<string> listacolumnas = new List<string>(),
                            listacolumntypeas = new List<string>(),
                            listacolumnNullableas = new List<string>();

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "select column_name, data_type,IS_NULLABLE from information_schema.columns where table_name = '" + tableName + "'";
                            connection.Open();
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (!reader.IsDBNull(i))
                                        {
                                            // System.Windows.Forms.MessageBox.Show(reader.GetString(i).ToString() + " " + reader.GetProviderSpecificFieldType(i)); 
                                        }
                                    }

                                    listacolumnas.Add(reader.GetString(0));
                                    listacolumntypeas.Add(reader.GetString(1));
                                    listacolumnNullableas.Add(reader.GetString(2));
                                }
                            }
                        }
                        break;
                    }
                #endregion

                #region MySql
                case DatabaseType.MySql:
                    {
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        using (MySqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "select column_name, data_type, column_type from information_schema.columns where table_name = '" + tableName + "'";
                            connection.Open();
                            using (var reader = command.ExecuteReader())
                            {
                                List<string> col = new List<string>();
                                while (reader.Read())
                                {
                                    if (reader.GetString(1).ToLower() == "enum")
                                    {
                                        col = new List<string>();
                                        col.Add(reader.GetString(0));
                                        temp = reader.GetString(2);
                                        temp = temp.Substring(temp.IndexOf('(') + 1, temp.IndexOf(')') - temp.IndexOf('(') - 1);
                                        temp = temp.Replace("'", string.Empty);
                                        col.AddRange(temp.Split(','));

                                        //System.Windows.Forms.MessageBox.Show(col.Count+""+col[0]);
                                        r.Add(col);
                                    }
                                }
                            }
                        }
                        break;
                    }
                #endregion
            }

            return r;
        }
        #endregion

        public static string FirstCharToUpper(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                return input.First().ToString().ToUpper() + input.Substring(1);
            }
            else
                return input;
        }

        public static string GetDataBaseName(string cnxString, DatabaseType DBType)
        {
            string DBName = string.Empty;
            switch (DBType)
            {
                case DatabaseType.SqlServer:
                    break;
                case DatabaseType.MySql:
                    var s = cnxString.ToLower().IndexOf("database=");

                    if (s > -1)
                    {
                        int i = s;
                        while (cnxString[i] != ';')
                        {
                            if (cnxString[i] == '=') { s = i + 1; }
                            i++;
                        }
                        DBName = cnxString.Substring(s, i - s);
                    }
                    break;
            }
            return DBName;
        }
        public static string GenerateFiles(string cnxString, List<string> tableNamesList, string projectName, DatabaseType MyDBType, bool IsDotNetCore, bool configFiles = true, bool helpersFiles = true, string CfgNameSpace = "", string HlpNameSpace = "")
        {
            string destFolder = "";
            if (tableNamesList != null && tableNamesList.Count > 0)
            {
                if (!configFiles || (configFiles && CfgNameSpace.Contains(' ')))
                    CfgNameSpace = string.Empty;
                if (!helpersFiles || (helpersFiles && HlpNameSpace.Contains(' ')))
                    HlpNameSpace = string.Empty;
                foreach (var tblname in tableNamesList)
                {
                    destFolder = GenerateFiles(cnxString, tblname, projectName, configFiles, helpersFiles, MyDBType, IsDotNetCore,CfgNameSpace,HlpNameSpace);
                }
            }
            if (!string.IsNullOrEmpty(destFolder) && !string.IsNullOrWhiteSpace(destFolder))
            {
                return destFolder;
            }
            else
            {
                throw new Exception();
            }
        }
        public static string GenerateFiles(string cnxString, string T, string NameSpace, bool configFiles, bool helpersFiles, DatabaseType MyDBType, bool IsDotNetCore, string CfgNameSpace = "", string HlpNameSpace = "")
        {
            //DatabaseType MyDBType = DatabaseType.SqlServer;
            string key = "LOP";
            string _key = "";

            List<List<string>> MyTablesCols = new List<List<string>>();
            List<List<string>> classEnums = new List<List<string>>();
            string PrimaryKeyColumn = string.Empty;
            int PrimaryKeyId;

            MyTablesCols = Helpers.EntityGenerator.GetColumns(cnxString, T, MyDBType);
            PrimaryKeyColumn = Helpers.EntityGenerator.GetPrimaryColumn(cnxString, T, MyDBType);
            classEnums = Helpers.EntityGenerator.GetEnums(cnxString, T, MyDBType);
            if (!string.IsNullOrWhiteSpace(PrimaryKeyColumn) && !string.IsNullOrEmpty(PrimaryKeyColumn))
            {
                PrimaryKeyId = MyTablesCols[0].FindIndex(C => C == PrimaryKeyColumn);
                if (PrimaryKeyId > -1)
                {
                    _key = Helpers.EntityGenerator.GenerateFiles(key, FirstCharToUpper(NameSpace), T,
                        PrimaryKeyId, MyTablesCols[0], MyTablesCols[1], MyTablesCols[2],
                        classEnums, cnxString, MyDBType, IsDotNetCore, cnxString, configFiles, helpersFiles,CfgNameSpace,HlpNameSpace);
                }
            }
            else
            {
                _key = Helpers.EntityGenerator.GenerateFiles(key, FirstCharToUpper(NameSpace), T,
                            0, MyTablesCols[0], MyTablesCols[1], MyTablesCols[2],
                            classEnums, cnxString, MyDBType, IsDotNetCore, cnxString, configFiles, helpersFiles,CfgNameSpace,HlpNameSpace);
                //Helpers.EntityGenerator.GenerateFiles(key, FirstCharToUpper(NameSpace), T, 0, MyTablesCols[0], MyTablesCols[1], MyTablesCols[2], classEnums, cnxString, MyDBType, configFiles, helpersFiles);
            }

            return _key;
        }
    }
}