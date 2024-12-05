﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace gescom.data.Models
{
    public static class DataHelpers
    {
        public static int BoxItemCount()
        {
            var query = new MssqlQuery("BoxItem");
            return query.CountRecords("Id");
        }

        public static int GetArticleIdByName(string nom)
        {
            var query = new MssqlQuery("ArticleItem");
            int result = query.SelectIdByName(nom);
            return result;
        }

        public static int GetArticleTypeLacedIn(long numero)
        {
            var query = new MssqlQuery("DistItem");
            return query.CountRecordsByNumero(numero);
        }

        public static bool GetConnection()
        {
            var ipep = new IPEndPoint(IPAddress.Parse("172.30.1.2"), 80);
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);
            }
            catch (SocketException)
            {
                Console.WriteLine("Unable to connect to server.");
                return false;
            }
            var conn = new MssqlConnection();
            conn.Open();
            bool result = conn.GetState();
            conn.Close();
            return result;
        }

        public static string GetNewPersonCode(PersonItem item)
        {
            var query = new MssqlQuery("PersonItem");
            query.CountRecords("Groupe");
            string prefix = null;
            long groupe = -1;
            groupe = item.Groupe;
            if (groupe < 0) { return null; }
            switch (groupe)
            {
                case 0: prefix = "F"; break;
                case 1: prefix = "G"; break;
                case 2: prefix = "D"; break;
                case 3: prefix = "S"; break;
                case 4: prefix = "E"; break;
                case 5: prefix = "P"; break;
            }
            string suffix = null;
            if (item.Forme == 1) { suffix = " -F"; }
            if (item.Forme == 0) { suffix = " -I"; }
            string s = query.CountRecordsByGroup(groupe).ToString();
            s += suffix;
            return prefix + s;
        }

        public static bool IsNameDuplicate(string tableName, string nom)
        {
            var query = new MssqlQuery(tableName);
            int count = query.CountByName(nom);
            if (count <= 0)
            {
                return false;
            }
            return true;
        }

        public static void SaveGeneral()
        {
            var conn = ConnForXml();
            if (!conn.GetState()) { return; }
            conn.WriteResearch();
        }

        public static void SaveLocal()
        {
            var conn = ConnForXml();
            if (!conn.GetState()) { return; }
            conn.WriteData();
        }

        public static void WriteBackupXml()
        {
            var conn = ConnForXml();
            if (!conn.GetState()) { return; }
            string hote = Environment.MachineName;
            conn.WriteStock();
            conn.WriteDisponible();
            conn.Close();
        }

        public static void WriteCommand(long id)
        {
            var conn = ConnForXml();
            if (!conn.GetState()) { return; }
            conn.WriteCommande(id);
            conn.Close();
        }

        public static void WriteXmlFile()
        {
            if (!GetConnection()) { return; }
            string hote = Environment.MachineName;
            var conn = new MssqlConnection();
            conn.WriteStock();
            conn.WriteDisponible();
            conn.Close();
        }

        private static MssqlConnection ConnForXml()
        {
            var ipep = new IPEndPoint(IPAddress.Parse("172.30.1.2"), 80);
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);
            }
            catch (SocketException)
            {
                Console.WriteLine("Unable to connect to server.");
            }
            var result = new MssqlConnection();
            result.Open();
            return result;
        }
    }

    public class MontantModel
    {
        public string Percu { get; set; }
        public string Rendu { get; set; }

        private string ValueFormatString(string value)
        {
            if (value != null)
            {
                var x = double.Parse(value);
                var result = StdCalcul.DoubleToSpaceFormat(x);
                return result;
            }
            return "0";
        }

        public MontantModel(string percu, string rendu)
        {
            Percu = ValueFormatString(percu);
            Rendu = ValueFormatString(rendu);
        }
    }

    public class MssqlConnection
    {
        private SqlConnection _conn;

        public MssqlConnection()
        {
            Init();
        }

        public MssqlConnection(string connstring)
        {
            Init();
            _conn.ConnectionString = connstring;
        }

        public MssqlConnection(string host, string database,
            string user, string password)
        {
            Init();
            string cstr =
                string.Format(
                    "server = {0}; User Id = {1};Password = {2} ; Persist Security Info = True; database = {3};", host,
                    user, password, database);
            _conn.ConnectionString = cstr;
        }

        protected string Datasource { get; set; }

        protected string Driver { get; set; }

        // SqlConnection parameters
        protected string Host { get; set; }

        protected string Password { get; set; }

        protected string Path { get; set; }

        protected int Port { get; set; }

        protected string Provider { get; set; }

        // SqlConnection state
        protected bool State { get; set; }

        protected string Uid { get; set; }

        public void Close()
        {
            if (State)
            {
                try
                {
                    _conn.Close();
                    State = false;
                }
                catch (SqlException)
                {
                    State = true;
                }
            }
        }

        public bool EcrireXml(string sqlCommand, string path)
        {
            bool result = Open();
            if (!result)
            {
                return false;
            }
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
                return false;
            }
            // Create SqlDataAdapter
            var da = new SqlDataAdapter(sqlCommand, _conn);
            // Create DataSet
            var ds = new DataSet();
            // Fill DataSet with SqlDataAdapter
            da.Fill(ds, "Data");
            // Write XML file with void WriteXml
            ds.WriteXml(path);
            return true;
        }

        public SqlConnection GetConnection()
        {
            return _conn;
        }

        public bool GetState()
        {
            return State;
        }

        public bool Open()
        {
            /* Pour MySql state = Open pour true, pour ls autres db changer.*/
            if (State == false)
            {
                try
                {
                    _conn.Open();
                    State = true;
                }
                catch (SqlException)
                {
                    State = false;
                }
            }
            return State;
        }

        public void Refresh()
        {
            Close();
            Open();
        }

        public void SetConnection(SqlConnection connection)
        {
            _conn = connection;
        }

        public bool WriteCommande(long id)
        {
            string sqlCommmand = "SELECT * FROM CommandView WHERE Rang = " + id;
            const string path = "c:\\glite\\Commande.xml";
            return EcrireXml(sqlCommmand, path);
        }

        public bool WriteData()
        {
            string sqlCommmand = "SELECT * FROM XMLITEM WHERE QUANTITE > 0";
            string path = "C:\\glite\\Bin\\Dispo.xml";
            EcrireXml(sqlCommmand, path);
            sqlCommmand = "SELECT * FROM XMLITEM";
            path = "C:\\glite\\Bin\\Data.xml";
            return EcrireXml(sqlCommmand, path);
        }

        public bool WriteResearch()
        {
            string sqlCommmand = "SELECT * FROM OperationItem WHERE QStock > 0";
            string path = "C:\\glite\\Bin\\Disponible.xml";
            EcrireXml(sqlCommmand, path);
            sqlCommmand = "SELECT * FROM OperationItem";
            path = "C:\\glite\\Bin\\Tout.xml";
            return EcrireXml(sqlCommmand, path);
        }

        public bool WriteDisponible()
        {
            const string sqlCommmand = "SELECT * FROM XMLITEM WHERE QUANTITE > 0";
            const string path = "D:\\XML\\Dispo.xml";
            return EcrireXml(sqlCommmand, path);
        }

        public bool WriteStock()
        {
            const string sqlCommmand = "SELECT * FROM XMLITEM";
            const string path = "D:\\XML\\Data.xml";
            return EcrireXml(sqlCommmand, path);
        }

        private void Init()
        {
            _conn = new SqlConnection();
            const string connstring = "Data Source=THINK;Initial Catalog=GESCOM;Persist Security Info=True;User ID=sysdba;Password=Pa$$w0rd";
            _conn.ConnectionString = connstring;
        }
    }

    public class MssqlQuery
    {
        private static SqlDataAdapter _adapter;

        // activer enregistrement, index id.***/
        private static DataSet _dataSet;

        private static DataTable _dataTable;

        //insertion
        private string _insParams;

        private string[] _line;
        private int _maxRangGrouped;

        private SqlDataReader _reader;

        // séléction.***/
        private string _selParams;

        private int[] _serie;

        // nom table.
        private string _tableName;

        // mis à jour***/
        private string _updParams;

        // simple constructeur.***/
        public MssqlQuery()
        {
            Init();
        }

        // constructeur avec paramètres database. **/
        public MssqlQuery(Query parameters)
        {
            Init();
            _tableName = parameters.GetTableName();
            _insParams = parameters.GetInsertText();
            _updParams = parameters.GetUpdateText();
        }

        // constructeur avec SqlConnection comme paramètres. **/
        public MssqlQuery(MssqlConnection connection)
        {
            Init();
            Connection = connection;
            Command.Connection = connection.GetConnection();
        }

        public MssqlQuery(MssqlConnection connection, string commande)
        {
            Init();
            Connection = connection;
            Command.Connection = Connection.GetConnection();
            Command.CommandText = commande;
        }

        // constructeur de recopie.***/
        public MssqlQuery(MssqlQuery query)
        {
            Init();
            Copy(query);
        }

        // constructeur avec table **/
        public MssqlQuery(string tableName)
        {
            Init();
            _tableName = tableName;
        }

        // rang dernier à créer **/
        public SqlCommand Command { get; set; }

        public MssqlConnection Connection { get; set; }

        public string SelParams
        {
            get { return _selParams; }
        }

        public void ActivateById(int id)
        {
            string commande = string.Format("UPDATE {0} SET ENABLED = TRUE WHERE ID = {1}"
                , _tableName, id);
            InitCommande(commande);
            ExecuteUpdate();
        }

        // commande générale de réquête avant mis à jour. **/
        public void BeforeUpdate()
        {
            ClearParameters();
            string commande = string.Format("UPDATE {0} SET {1}", _tableName, _updParams);
            InitCommande(commande);
        }

        // effacement du DataSet **/
        public void ClearDataset()
        {
            _dataSet.Clear();
        }

        // effacer les paramètres de réquêtes. **/
        public void ClearParameters()
        {
            Connection.Open();
            Command.Parameters.Clear();
        }

        // effacer toutes les données de la table **/
        public void ClearTable()
        {
            string commande = "DELETE * FROM " + _tableName;
            System.Console.WriteLine(commande);
            InitCommande(commande);
            ExecuteUpdate();
        }

        // ferme la Connection. **/
        public void Close()
        {
            Command.CommandText = "";
            Connection.Close();
        }

        // compter les occurences de code***/
        public int CodeCounter(string code)
        {
            int result = TexteCounter("CODE", code);
            return result;
        }

        // configurer des objets de la base de données.***/
        public void Configurer(string tableName, string insParams, string updParams)
        {
            _tableName = tableName;
            _insParams = insParams;
            _updParams = updParams;
        }

        // compter les enregistrements dans une table avec une commande***/
        public int Count(string commande)
        {
            ExecuteSelect(commande);
            int result = GetTable().Rows.Count;
            return result;
        }

        public int CountByCode(string code)
        {
            string commande = "SELECT IA FROM " + _tableName + " WHERE CODE = '" + code + "'";
            int result = Count(commande);
            return result;
        }

        public int CountByName(string nom)
        {
            string commande = "SELECT COUNT(ID) FROM " + _tableName + " WHERE NOM = '" + nom + "'";
            int result = Count(commande);
            return result;
        }

        public int CountByNameState(string nom)
        {
            string commande = "SELECT COUNT(ID) FROM " + _tableName + " WHERE (NOM = '" + nom + "') AND (STATE = 0)";
            int result = Count(commande);
            return result;
        }

        public int CountRecords()
        {
            string commande = "SELECT COUNT(ID) FROM " + _tableName;
            int result = ExecuteScalar(commande);
            return result;
        }

        // compter occurence table, clé autre que Id.***/
        public int CountRecords(string colName)
        {
            string s = colName.ToUpper();
            string commande = string.Format("SELECT COUNT({0}) FROM {1}", s, _tableName);
            return ExecuteScalar(commande);
        }

        // compter les occurences d' une table, Id entier long paramètre primaire.***/
        public int CountRecordsByGroup(long groupe)
        {
            string commande = "SELECT COUNT(ID) FROM " + _tableName + " WHERE GROUPE = '" + groupe + "'";
            int result = ExecuteScalar(commande);
            return result;
        }

        // compter les occurences d' une table, Id entier court paramètre primaire.***/
        public int CountRecordsById(int id)
        {
            string commande = "SELECT COUNT(ID) FROM " + _tableName + " WHERE ID = '" + id + "'";
            int result = ExecuteScalar(commande);
            return result;
        }

        // compter les occurences d' une table, Id entier long paramètre primaire.***/
        public int CountRecordsById(long id)
        {
            string commande = "SELECT COUNT(ID) FROM " + _tableName + " WHERE ID = '" + id + "'";
            int result = ExecuteScalar(commande);
            return result;
        }

        // compter les occurences d' une table, Id entier long paramètre primaire.***/
        public int CountRecordsByNumero(long numero)
        {
            string commande = "SELECT COUNT(ID) FROM " + _tableName + " WHERE NUMERO = '" + numero + "'";
            int result = ExecuteScalar(commande);
            return result;
        }

        // désactiver enregistrement, index id.***/
        public void DeActivateById(int id)
        {
            string commande = string.Format("UPDATE {0} SET ENABLED = FALSE WHERE ID = {1}"
                , _tableName, id);
            InitCommande(commande);
            ExecuteUpdate();
        }

        public void DeleteAllByName(string nom)
        {
            string commande = "DELETE FROM " + _tableName + " WHERE NOM = '" + nom + "'";
            InitCommande(commande);
            ExecuteUpdate();
        }

        // supprimer enregistrement, index id.***/
        public void DeleteById(int id)
        {
            string commande = string.Format("DELETE FROM {0} WHERE ID = {1}"
                , _tableName, id);
            InitCommande(commande);
            ExecuteUpdate();
        }

        //supprimer enregistrement , index et paramètre entier.***/
        public void DeleteIntBy(string colName, int key)
        {
            colName = colName.ToUpper();
            string commande = String.Format("DELETE FROM {0} WHERE {1} = {2}", _tableName, colName, key);
            InitCommande(commande);
            ExecuteUpdate();
        }

        //supprimer enregistrement , index et paramètre entier.***/
        public void DeleteLongBy(string colName, long key)
        {
            colName = colName.ToUpper();
            string commande = String.Format("DELETE FROM {0} WHERE {1} = {2}", _tableName, colName, key);
            InitCommande(commande);
            ExecuteUpdate();
        }

        //supprimer enregistrement , index et paramètre chaîne.***/
        public void DeleteStringBy(string colName, string key)
        {
            colName = colName.ToUpper();
            string commande = String.Format("DELETE FROM {0} WHERE {1} = '{2}'", _tableName, colName, key);
            InitCommande(commande);
            ExecuteUpdate();
        }

        public void ExecuteProcedure(string commande)
        {
            Connection.Open();
            Command.CommandText = commande;
            Command.CommandType = CommandType.StoredProcedure;
            Command.ExecuteScalar();
            Connection.Close();
        }

        // éxécuter réquete ne retournant pas résultat. ***/
        public int ExecuterQuery()
        {
            int result = 0;
            try
            {
                Connection.Open();
                Command.ExecuteNonQuery();
                Connection.Close();
            }
            catch (Exception)
            {
                result = 1;
                Console.WriteLine(Command.CommandText);
                Command.Cancel();
            }
            finally
            {
                Connection.Close();
            }
            return result;
        }

        // retourne un jeu unique de résultat***/
        public int ExecuteScalar(string commande)
        {
            int result = 0;
            InitCommande(commande);
            try
            {
                result = (int)Command.ExecuteScalar();
            }
            catch (SqlException)
            {
                Console.WriteLine(Command.CommandText);
                Command.Cancel();
            }
            finally
            {
                Connection.Close();
            }
            return result;
        }

        // execution d'une requête Select***/
        public void ExecuteSelect(string commande)
        {
            InitCommande(commande);
            _adapter.SelectCommand = Command;
            _adapter.Fill(_dataSet);
            Connection.Close();
        }

        public void ExecuteSelect()
        {
            string commande = Command.CommandText;
            InitCommande(commande);
            _adapter.SelectCommand = Command;
            _adapter.Fill(_dataSet);
            Connection.Close();
        }

        // execution d'une requête de mise à jour***/
        public void ExecuteUpdate(string commande)
        {
            InitCommande(commande);
            ExecuterQuery();
        }

        // la commande est déjà configurée dans une autre classe.***/
        public int ExecuteUpdate()
        {
            int result = ExecuterQuery();
            return result;
        }

        // remplissement d'un DataSet donnée***/
        public void FillCommander(string tableName, string selectCommand)
        {
            using (_adapter)
            {
                using (Command)
                {
                    Command.CommandText = selectCommand;
                    _adapter.SelectCommand = Command;
                }
                _adapter.Fill(_dataSet, tableName);
            }
        }

        public void FillProcDataSet(DataSet dataSet, string commande, string tableName)
        {
            Command.CommandText = commande;
            Command.CommandType = CommandType.StoredProcedure;
            _adapter.SelectCommand = Command;
            _adapter.Fill(dataSet, tableName);
        }

        public void FillProcedural(string commande, string tableName)
        {
            Command.CommandText = commande;
            Command.CommandType = CommandType.StoredProcedure;
            _adapter.SelectCommand = Command;
            _adapter.Fill(_dataSet, tableName);
        }

        // obtention de données.***/
        public DataSet GetData()
        {
            _adapter.Fill(_dataSet, _tableName);
            return _dataSet;
        }

        // obtenir le DataSet***/
        public DataSet GetDataSet()
        {
            return _dataSet;
        }

        // obtenir une vue composée***/
        public void GetDataSetComposed(string masterCommand, string childCommand)
        {
            _dataSet.Clear();
            var masterAdapter = new SqlDataAdapter();
            var childAdapter = new SqlDataAdapter();
            // la commande en question
            SqlCommand orderCommand;
            using (orderCommand = new SqlCommand { Connection = Connection.GetConnection() })
            {
                orderCommand.CommandText = masterCommand;
                masterAdapter.SelectCommand = orderCommand;
                masterAdapter.Fill(_dataSet, "Masters");
                orderCommand.CommandText = childCommand;
                childAdapter.SelectCommand = orderCommand;
                childAdapter.Fill(_dataSet, "Childs");
                DataColumn masterIndex = _dataSet.Tables["Masters"].Columns[0];
                DataColumn childIndex = _dataSet.Tables["Childs"].Columns[0];
                int rowCount = _dataSet.Tables["Childs"].Rows.Count;
                if (rowCount > 0)
                {
                    var relation = new DataRelation("MTC", masterIndex, childIndex);
                    _dataSet.Relations.Add(relation);
                }
            }
        }

        // lecture de la table, les contrôles détérminants mode de lecture.***/
        public DataView GetDataView(string commande)
        {
            DataView result = GetTableView(commande);
            result.AllowEdit = true;
            result.AllowDelete = true;
            result.AllowNew = true;
            return result;
        }

        // compter les articles déjà existants par famille, et estimer le nouveau rang.***/
        public string GetNewArticleCodeByFamilies(string prefix, string colName, int index)
        {
            string c = colName.ToUpper();
            string commande = String.Format("SELECT COUNT({0}) FROM {1} WHERE {0} = {2}", c, _tableName, index);
            int i = ExecuteScalar(commande);
            i++;
            _maxRangGrouped = i;
            string result = string.Format("{0} {1}", prefix.ToUpper(), i);
            return result;
        }

        // obtenir un nouveau code***/
        public string GetNewCode()
        {
            string result = GetNewKeyCategorized("CODE");
            return result;
        }

        // obtenir nouveau code selon code catégorie***/
        public string GetNewCodeCategorized(string code)
        {
            int id = GetNewKeyRecord("CODE");
            string s = code.ToUpper() + " ";
            string result = s + id;
            return result;
        }

        // nouveau code si Id groupe est Ig***/
        public string GetNewCodeGrouped(string prefix,
            int ig)
        {
            string result = GetNewKeyGrouped(prefix, "ID", ig);
            return result;
        }

        // obtenir id nouvel enregistrement, clé primaire Id.***/
        public int GetNewIdRecord()
        {
            int result = CountRecords();
            result++;
            return result;
        }

        // obtenir nouveau clé selon catégorie***/
        public string GetNewKeyCategorized(string colName)
        {
            int id = GetNewKeyRecord(colName);
            string s = colName.ToUpper();
            string result = s + id;
            return result;
        }

        public string GetNewKeyGrouped(string prefix,
            string colName, int index)
        {
            string commande = String.Format("SELECT COUNT(ID) FROM {0} WHERE {1} = {2}", _tableName, colName, index);
            int i = ExecuteScalar(commande);
            i++;
            string result = string.Format("{0} {1}", prefix.ToUpper(), i);
            return result;
        }

        // obtenir id nouvel enregistrement, clé primaire chaîne.***/
        public int GetNewKeyRecord(string colName)
        {
            int result = CountRecords(colName);
            result++;
            return result;
        }

        // obtenir une nouvelle réference.***/
        public string GetNewRefce(string colName)
        {
            string s = _tableName.Substring(0, 1);
            s = s.ToUpper();
            string result = string.Format("{0}{1}", s, GetNewKeyRecord(colName));
            return result;
        }

        public int GetRangGroup()
        {
            return _maxRangGrouped;
        }

        // obtenir enregistrement(s).***/
        public SqlDataReader GetRecord(string commande)
        {
            InitCommande(commande);
            SqlDataReader reader = Command.ExecuteReader();
            return reader;
        }

        public int[] GetSerie()
        {
            return _serie;
        }

        public DataTable GetTable()
        {
            _dataTable = _dataSet.Tables[0];
            return _dataTable;
        }

        // table sans effacement de nom de table.***/
        public DataView GetTableView(string commande, string tableName)
        {
            InitCommande(commande);
            _adapter.SelectCommand = Command;
            _adapter.Fill(_dataSet);
            _tableName = tableName;
            _dataSet.Tables[0].TableName = _tableName;
            _dataTable = _dataSet.Tables[0];
            var dtView = new DataView(_dataTable);
            return dtView;
        }

        // obtention de vue, d'une seule table.***/
        public DataView GetTableView(string commande)
        {
            InitCommande(commande);
            _adapter.SelectCommand = Command;
            _adapter.Fill(_dataSet);
            _dataSet.Tables[0].TableName = _tableName;
            DataTable dtb = GetTable();
            var dtView = new DataView(dtb);
            return dtView;
        }

        public double GetTaxe()
        {
            return 0.2;
        }

        // initialisation texte de commande.***/
        public void InitCommande(string commande)
        {
            Command.CommandText = commande;
            if (Connection.GetState() == false)
            {
                Connection.Open();
            }
            if (Connection.GetState())
            {
                Connection.Refresh();
            }
        }

        // requête insertion, clé pimaire Id.***/
        public int Insert()
        {
            // récupèrer record Id.***/
            ClearParameters();
            int result = GetNewIdRecord();
            string commande = string.Format("INSERT INTO {0} VALUES {1}", _tableName, _insParams);
            InitCommande(commande);
            SetIdParam(result);
            return result;
        }

        // requete insertion, id n'est pas concerné du tout.***/
        public void InsertOutId()
        {
            ClearParameters();
            string commande = string.Format("INSERT INTO {0} VALUES {1}", _tableName, _insParams);
            InitCommande(commande);
        }

        // le nom du clé primaire n'est pas Id.***/
        public int InsertRecord(string colName)
        {
            ClearParameters();
            int result = GetNewKeyRecord(colName);
            string commande = string.Format("INSERT INTO {0} VALUES {1}", _tableName, _insParams);
            InitCommande(commande);
            SetIntParam(colName, result);
            return result;
        }

        // réquête insertion, paramètre clé pimaire Id.***/
        public void InsertWithoutId(int id)
        {
            ClearParameters();
            string commande = string.Format("INSERT INTO {0} VALUES {1}", _tableName, _insParams);
            InitCommande(commande);
            SetIdParam(id);
        }

        // éviter doublon de code***/
        public bool IsCodeSingle(string code)
        {
            bool result = IsTexteSingle("CODE", code);
            return result;
        }

        // éviter doublons de index.***/
        public bool IsIdSingle(int id)
        {
            string s = id.ToString(CultureInfo.InvariantCulture);
            string commande = string.Format("SELECT COUNT(ID) FROM {0} WHERE ID = {1}", _tableName, s);
            int i = ExecuteScalar(commande);
            bool result = i == 0;
            return result;
        }

        // éviter doublon de noms***/
        public bool IsNameSingle(string nom)
        {
            bool result = IsTexteSingle("NOM", nom);
            return result;
        }

        // éviter doublon de référence***/
        public bool IsRefSingle(string refce)
        {
            bool result = IsTexteSingle("REFCE", refce);
            return result;
        }

        //éviter doublon pour une colonne donnée, valeur textuelle, clé primaire Id.***/
        public bool IsTexteSingle(string colName, string text)
        {
            string commande = string.Format("SELECT COUNT(ID) FROM {0} WHERE {1} = '{2}'", _tableName, colName, text);
            int i = ExecuteScalar(commande);
            bool result = i <= 0;
            return result;
        }

        //éviter doublon pour une colonne donnée, valeur textuelle, clé primaire autre.****/
        public bool IsTexteSingle(string key, string colName, string text)
        {
            string commande = string.Format(string.Format("SELECT COUNT({0}) FROM {{0}} WHERE {{1}} = '{{2}}'", key),
                _tableName, colName, text);
            int i = ExecuteScalar(commande);
            bool result = !(i > 0);
            return result;
        }

        // obtenir la liste.***/
        public DataView Lister()
        {
            DataView result = SelectOrdered();
            return result;
        }

        // liste des clients***/
        public DataView ListerCustomer()
        {
            _tableName = "_CUSTOMER";
            DataView result = SelectOrdered();
            return result;
        }

        /**liste des familles d'articles.***/

        public DataView ListerFamille()
        {
            _tableName = "_FAMILLE";
            DataView result = SelectOrdered();
            return result;
        }

        // séléctionner ordonnée.***/
        public DataView ListerPlace()
        {
            _tableName = "_PLACE";
            const string commande = "SELECT TOP (100) PERCENT dbo._PLACE.ID AS NUMERO, dbo._PLACE.CODE AS CODES, " +
                                    "dbo._PLACE.NOM AS NOMS, dbo._PLACE.T AS LIBERTE, dbo._RESERVATION.T AS UNICITE, " +
                                    "dbo._EMPLACEMENT.T AS OCCUPATION FROM dbo._PLACE INNER JOIN " +
                                    "dbo._RESERVATION ON dbo._PLACE.ID = dbo._RESERVATION.ID LEFT OUTER JOIN " +
                                    "dbo._EMPLACEMENT ON dbo._PLACE.ID = dbo._EMPLACEMENT.ID WHERE (dbo._RESERVATION.T < 0) ORDER BY NOMS ";
            DataView view = GetDataView(commande);
            return view;
        }

        // séléction ordonnée de préference.***/
        public DataView ListerPref()
        {
            _tableName = "_PREFERENCE";
            const string commande = "SELECT DISTINCT dbo._REMARQUE.NOM AS NOMS FROM dbo._REMARQUE ORDER BY NOMS ASC";
            DataView view = GetDataView(commande);
            return view;
        }

        // liste unité article.***/
        public DataView ListerUnite()
        {
            _tableName = "_UNITE";
            DataView result = SelectOrdered();
            return result;
        }

        // liste des fournisseurs***/
        public DataView ListerVendor()
        {
            _tableName = "_VENDOR";
            DataView result = SelectOrdered();
            return result;
        }

        // compter les occurences de nom****/
        public int NameCounter(string nom)
        {
            int result = TexteCounter("NOM", nom);
            return result;
        }

        // préparation réquête avant plusieurs répétitions***/
        public void Prepare()
        {
            Command.Prepare();
        }

        // lecture d' enregistrement.***/
        public string Read(int id, int i)
        {
            string result = null;
            _reader = SelectById(id);
            while (_reader.Read())
            {
                result = _reader[i].ToString();
            }
            return result;
        }

        public string[] Reader(int id, int count)
        {
            var result = new string[count];
            _reader = SelectById(id);
            while (_reader.Read())
            {
                for (int i = 0; i < count; i++)
                {
                    result[i] = _reader[i + 1].ToString();
                }
            }
            Close();
            return result;
        }

        public string[] Reader(long id, int count)
        {
            var result = new string[count];
            _reader = SelectById(id);
            while (_reader.Read())
            {
                for (int i = 0; i < count; i++)
                {
                    result[i] = _reader[i + 1].ToString();
                }
            }
            Close();
            return result;
        }

        public void Reader(string code)
        {
            _reader = SelectByCode(code);
            int count = GetTable().Rows.Count;
            _serie = new int[count];
            int i = 0;
            while (_reader.Read())
            {
                string s = _reader[0].ToString();
                int r = int.Parse(s);
                _serie[i] = r;
                i++;
            }
            Close();
        }

        public void Reader(int id)
        {
            _reader = SelectByIa(id);
            _serie = new int[2];
            int i = 0;
            while (_reader.Read())
            {
                string s = _reader[0].ToString();
                int r = int.Parse(s);
                _serie[i] = r;
                i++;
            }
            Close();
        }

        public string[] Reader(string commande, int count)
        {
            SetReader(commande);
            _line = new string[count];
            while (_reader.Read())
            {
                for (int i = 0; i < count; i++)
                {
                    _line[i] = _reader[i].ToString();
                }
            }
            Close();
            return _line;
        }

        // séléctionne et lit enregistrement, index code. **/
        public SqlDataReader ReaderSelectByName(string nom)
        {
            string commande = "SELECT ID FROM " + _tableName + " WHERE NOM = '" + nom + "'";
            GetTableView(commande);
            SqlDataReader reader = GetRecord(commande);
            return reader;
        }

        public String ReadStringRecord(int id)
        {
            return _line[id];
        }

        // compter les occurences de référence***/
        public int RefCounter(string refce)
        {
            int result = TexteCounter("REFCE", refce);
            return result;
        }

        // mis à jour groupés.***/
        public void SaveData()
        {
            _dataTable.AcceptChanges();
            _adapter.Update(_dataSet, _tableName);
            Connection.Close();
        }

        // séléctionner tout.***/
        public DataView SelectAll()
        {
            string commande = "SELECT * FROM " + _tableName;
            DataView view = GetDataView(commande);
            return view;
        }

        // séléctionne et lit enregistrement, index code. **/
        public SqlDataReader SelectByCode(string code)
        {
            string commande = "SELECT ID FROM " + _tableName + " WHERE CODE = '" + code + "'";
            GetTableView(commande);
            SqlDataReader reader = GetRecord(commande);
            return reader;
        }

        // séléctionner emplacements**/
        public SqlDataReader SelectByIa(int ia)
        {
            string commande = string.Format("SELECT ID, IA, IE, T FROM {0} WHERE IA = '{1}'", _tableName, ia);
            SqlDataReader reader = GetRecord(commande);
            return reader;
        }

        // séléctionne et lit enregistrement, index id. **/
        public SqlDataReader SelectById(int id)
        {
            string commande = string.Format("SELECT * FROM {0} WHERE ID = {1}", _tableName, id);
            SqlDataReader reader = GetRecord(commande);
            return reader;
        }

        // séléctionne et lit enregistrement, index id. **/
        public SqlDataReader SelectById(long id)
        {
            string commande = string.Format("SELECT * FROM {0} WHERE ID = {1}", _tableName, id);
            SqlDataReader reader = GetRecord(commande);
            return reader;
        }

        // séléction par nom.***/
        public DataView SelectByName()
        {
            string commande = string.Format("SELECT * FROM {0} ORDER BY NOM", _tableName);
            DataView view = GetDataView(commande);
            return view;
        }

        public int SelectCode(string code)
        {
            return SelectId("CODE", code);
        }

        public string SelectCodeById(string table, int id)
        {
            string code = null;
            ClearParameters();
            _tableName = table;
            string commande = String.Format("SELECT CODE FROM {0} WHERE ID = {1}", _tableName, id);
            SqlDataReader reader = GetRecord(commande);
            while (reader.Read())
            {
                code = reader[0].ToString();
            }
            Connection.Close();
            return code;
        }

        public string SelectCodeById(int id)
        {
            string code = null;
            ClearParameters();
            string commande = String.Format("SELECT CODE FROM {0} WHERE ID = {1}", _tableName, id);
            SqlDataReader reader = GetRecord(commande);
            while (reader.Read())
            {
                code = reader[0].ToString();
            }
            Connection.Close();
            return code;
        }

        public int SelectId(string colName, string valeur)
        {
            int result = -1;
            int i = valeur.Length;
            ClearParameters();
            string commande = string.Format("SELECT ID FROM {0} WHERE {1} = '{2}'", _tableName, colName, valeur);
            if (i != 0)
            {
                SqlDataReader reader = GetRecord(commande);
                while (reader.Read())
                {
                    string id = reader[0].ToString();
                    result = int.Parse(id);
                }
            }
            Connection.Close();
            return result;
        }

        public int SelectIdByCode(string code)
        {
            return SelectId("NOM", code);
        }

        public int SelectIdByName(string nom)
        {
            return SelectId("NOM", nom);
        }

        public int SelectIdByRef(string refce)
        {
            return SelectId("REFCE", refce);
        }

        public DataTable SelectionnerTable(string procedure, string tableName)
        {
            var query = new MssqlQuery();
            query.FillProcedural(procedure, tableName);
            DataSet dataSet = query.GetDataSet();
            DataTable result = dataSet.Tables[0];
            return result;
        }

        public DataTable SelectionnerTable(string procedure, string tableName, string code)
        {
            var query = new MssqlQuery();
            query.FillProceduralCode(procedure, tableName, code);
            DataSet dataSet = query.GetDataSet();
            DataTable result = dataSet.Tables[0];
            return result;
        }

        public DataTable SelectionnerTable(string procedure, string tableName, int id)
        {
            var query = new MssqlQuery();
            query.FillProceduralId(procedure, tableName, id);
            DataSet dataSet = query.GetDataSet();
            DataTable result = dataSet.Tables[0];
            return result;
        }

        public string SelectNameById(int id)
        {
            string nom = null;
            ClearParameters();
            string commande = String.Format("SELECT NOM FROM {0} WHERE ID = {1}", _tableName, id);
            SqlDataReader reader = GetRecord(commande);
            while (reader.Read())
            {
                nom = reader[0].ToString();
            }
            Connection.Close();
            return nom;
        }

        // séléctionner ordonnée.***/
        public DataView SelectOrdered()
        {
            string commande = string.Format("SELECT * FROM {0} ORDER BY ID ASC", _tableName);
            DataView view = GetDataView(commande);
            return view;
        }

        // séléctionner code user.
        public int SelectUserCode(int ia)
        {
            int result = -1;
            ClearParameters();
            string commande = "SELECT MAX(ID) FROM " + _tableName + " WHERE IA = '" + ia + "'";
            SqlDataReader reader = GetRecord(commande);
            while (reader.Read())
            {
                string id = reader[0].ToString();
                try
                {
                    result = int.Parse(id);
                }
                catch (FormatException)
                {
                    result = 0;
                }
            }
            Connection.Close();
            return result;
        }

        // selectionne avec paramètre nom ou autre***/
        public DataView SelectWhere(string colName, string valeur)
        {
            string commande = string.Format("SELECT * FROM {0} WHERE  {1} = '{2}'", _tableName, colName, valeur);
            DataView view = GetDataView(commande);
            return view;
        }

        // paramètrage booléen.***/
        public void SetBoolParam(string colName, bool valeur)
        {
            int v = valeur ? 1 : 0;
            string s = "@" + colName.ToUpper();
            Command.Parameters.Add(s, SqlDbType.Int);
            Command.Parameters[s].Value = v;
        }

        // ajout avec paramètrage date.***/
        public void SetDateParam(DateTime date)
        {
            Command.Parameters.Add("@DATUM", SqlDbType.DateTime);
            Command.Parameters["@DATUM"].Value = date;
        }

        // date avec autre nom de colonne***/
        public void SetDateParam(string colName, DateTime date)
        {
            string s = "@" + colName.ToUpper();
            Command.Parameters.Add(s, SqlDbType.DateTime);
            Command.Parameters[s].Value = date;
        }

        // paramètrage réel***/
        public void SetDoubleParam(string colName, double valeur)
        {
            string s = "@" + colName.ToUpper();
            Command.Parameters.Add(s, SqlDbType.Float);
            Command.Parameters[s].Value = valeur;
        }

        // ajout avec paramètrage id int.***/
        public void SetIdParam(int id)
        {
            SetIntParam("id", id);
        }

        // ajout avec paramètrage id.***/
        public void SetIdParam(long id)
        {
            SetLongParam("id", id);
        }

        // paramètrage entier.***/
        public void SetIntParam(string colName, int valeur)
        {
            string s = "@" + colName.ToUpper();
            Command.Parameters.Add(s, SqlDbType.Int);
            Command.Parameters[s].Value = valeur;
        }

        // paramètrage entier long.***/
        public void SetLongParam(string colName, long valeur)
        {
            string s = "@" + colName.ToUpper();
            Command.Parameters.Add(s, SqlDbType.BigInt);
            Command.Parameters[s].Value = valeur;
        }

        public void SetReader(string commande)
        {
            Command.CommandText = commande;
            _reader = GetRecord(commande);
        }

        // ajout avec paramètrage activation.***/
        public void SetStateParam(bool state)
        {
            int v = state ? 1 : 0;
            Command.Parameters.Add("@STATE", SqlDbType.Int);
            Command.Parameters["@STATE"].Value = v;
        }

        // paramètrage réquête en tant que variable chaîne.***/
        public void SetStringParam(string colName, string valeur)
        {
            int len = valeur != null ? valeur.Length : 0;
            colName = colName.ToUpper();
            colName = "@" + colName;
            Command.Parameters.Add(colName, SqlDbType.VarChar, 200);
            Command.Parameters[colName].Value = len != 0 ? valeur : "AUCUN";
        }

        // table concernée par les opérations***/
        public void SetTableName(string tableName)
        {
            _tableName = tableName;
        }

        public int TexteCounter(string colName, string text)
        {
            string commande = string.Format("SELECT COUNT(ID) FROM {0} WHERE {1} = '{2}'", _tableName, colName, text);
            int result = ExecuteScalar(commande);
            return result;
        }

        public void Update()
        {
            _dataSet.AcceptChanges();
            _adapter.Update(_dataSet, _tableName);
        }

        /* création réquête ave texte de commande. **/

        internal void FillProceduralCode(string commande, string tableName, string code)
        {
            Command.CommandText = commande;
            SqlParameter inputParameter = Command.Parameters.Add("@CODE", SqlDbType.NChar, 25);
            inputParameter.Value = code;
            Command.CommandType = CommandType.StoredProcedure;
            _adapter.SelectCommand = Command;
            _adapter.Fill(_dataSet, tableName);
        }

        internal void FillProceduralId(string commande, string tableName, int id)
        {
            Command.CommandText = commande;
            SqlParameter inputParameter = Command.Parameters.Add("@ID", SqlDbType.Int);
            inputParameter.Value = id;
            Command.CommandType = CommandType.StoredProcedure;
            _adapter.SelectCommand = Command;
            _adapter.Fill(_dataSet, tableName);
        }

        protected void Copy(MssqlQuery query)
        {
            Connection = query.Connection;
            Command = query.Command;
            _insParams = query._insParams;
            _selParams = query._selParams;
            _tableName = query._tableName;
            _updParams = query._updParams;
        }

        private void Init()
        {
            _maxRangGrouped = 0;
            Connection = new MssqlConnection();
            _adapter = new SqlDataAdapter();
            Command = new SqlCommand(null, Connection.GetConnection());
            _dataSet = new DataSet();
            _dataTable = new DataTable();
        }
    }

    /// <summary>
    ///     Liste de paramètres dans
    ///     les manipulations de tables et autres.
    /// </summary>
    public class Query
    {
        // réquête insertion.
        protected readonly string InsParams;

        // requete selection par colonnes.
        protected readonly string SelParams;

        // nom de la table.
        protected readonly string TableName;

        // requête de mise à jour.
        protected readonly string UpdParams;

        public Query()
        {
        }

        public Query(string tableName, string insParam, string updParam)
        {
            TableName = tableName;
            InsParams = insParam;
            UpdParams = updParam;
            SelParams = "SELECT * FROM " + TableName;
        }

        public Query(string tableName, string insParam, string updParam,
            string selParam)
        {
            TableName = tableName;
            InsParams = insParam;
            UpdParams = updParam;
            SelParams = selParam;
        }

        public Query(string insParam, string updParam)
        {
            InsParams = insParam;
            UpdParams = updParam;
        }

        public string GetInsertText()
        {
            return InsParams;
        }

        public string GetSelectText()
        {
            return SelParams;
        }

        public string GetTableName()
        {
            return TableName;
        }

        public string GetUpdateText()
        {
            return UpdParams;
        }
    }
}