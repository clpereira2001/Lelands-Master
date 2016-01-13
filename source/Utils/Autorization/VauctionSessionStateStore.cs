using System;
using System.Web;
using System.Web.Configuration;
using System.Configuration;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Vauction.Utils.Lib;

namespace Vauction.Utils.Autorization
{
  public sealed class VauctionSessionStateStore : SessionStateStoreProviderBase
  {

    private SessionStateSection pConfig = null;
    private string connectionString;
    private ConnectionStringSettings pConnectionStringSettings;
    private string eventSource = "VauctionSessionStateStore";
    private string eventLog = "Application";
    //private string exceptionMessage = "An exception occurred. Please contact your administrator.";
    private string pApplicationName;

    // If false, exceptions are thrown to the caller. If true, exceptions are written to the event log.
    private bool pWriteExceptionsToEventLog = false;

    #region properties
    public bool WriteExceptionsToEventLog
    {
      get { return pWriteExceptionsToEventLog; }
      set { pWriteExceptionsToEventLog = value; }
    }

    // The ApplicationName property is used to differentiate sessions in the data source by application.
    public string ApplicationName
    {
      get { return pApplicationName; }
    }
    #endregion

    public override void Initialize(string name, NameValueCollection config)
    {
      // Initialize values from web.config.
      if (config == null) throw new ArgumentNullException("config");
      if (name == null || name.Length == 0) name = "VauctionSessionStateStore";
      if (String.IsNullOrEmpty(config["description"]))
      {
        config.Remove("description");
        config.Add("description", "Vauction Session State Store provider");
      }
      // Initialize the abstract base class.
      base.Initialize(name, config);
      // Initialize the ApplicationName property.

      // Get <sessionState> configuration element.
      System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(ApplicationName);
      pConfig = (SessionStateSection)cfg.GetSection("system.web/sessionState");

      // Initialize connection string.
      pConnectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

      if (pConnectionStringSettings == null || pConnectionStringSettings.ConnectionString.Trim() == "")
        throw new ProviderException("Connection string cannot be blank.");
      connectionString = pConnectionStringSettings.ConnectionString;

      // Initialize WriteExceptionsToEventLog
      pWriteExceptionsToEventLog = false;
      if (config["writeExceptionsToEventLog"] != null)
      {
        if (config["writeExceptionsToEventLog"].ToUpper() == "TRUE")
          pWriteExceptionsToEventLog = true;
      }
      pApplicationName = (config["application"] != null) ? config["application"] : System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
      eventSource = (!String.IsNullOrEmpty(config["eventLogSource"])) ? config["eventLogSource"] : "VauctionSessionStateStore";
    }

    #region SessionStateStoreProviderBase members
    //Dispose
    public override void Dispose()
    {
    }

    //SetItemExpireCallback
    public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
    {
      return false;
    }

    //SetAndReleaseItemExclusive
    public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
    {
      // Serialize the SessionStateItemCollection as a string.
      string sessItems = Serialize((SessionStateItemCollection)item.Items);
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd;
      SqlCommand deleteCmd = null;
      if (newItem)
      {
        // OdbcCommand to clear an existing expired session if it exists.
        deleteCmd = new SqlCommand("DELETE FROM dbo.[UserSessions] WHERE SessionId = @Id AND ApplicationName = @app AND Expires < @exp", conn);
        deleteCmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
        deleteCmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
        deleteCmd.Parameters.Add("@exp", SqlDbType.DateTime).Value = DateTime.Now;
        // OdbcCommand to insert the new session item.
        cmd = new SqlCommand("INSERT INTO dbo.[UserSessions](SessionId, ApplicationName, Created, Expires, LockDate, LockId, Timeout, Locked, SessionItems, Flags) Values(@Id, @app, @cr, @exp, @lDate, @lId, @tout, @l, @sesItems, @flgs)", conn);
        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
        cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
        cmd.Parameters.Add("@cr", SqlDbType.DateTime).Value = DateTime.Now;
        cmd.Parameters.Add("@exp", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes((Double)item.Timeout);
        cmd.Parameters.Add("@lDate", SqlDbType.DateTime).Value = DateTime.Now;
        cmd.Parameters.Add("@lId", SqlDbType.Int).Value = 0;
        cmd.Parameters.Add("@tout", SqlDbType.Int).Value = item.Timeout;
        cmd.Parameters.Add("@l", SqlDbType.Bit).Value = false;
        cmd.Parameters.Add("@sesItems", SqlDbType.Text).Value = sessItems;
        cmd.Parameters.Add("@flgs", SqlDbType.Int).Value = 0;
      }
      else
      {
        // OdbcCommand to update the existing session item.
        cmd = new SqlCommand("UPDATE dbo.[UserSessions] SET Expires = @exp, SessionItems = @sItems, Locked = @l WHERE SessionId = @Id AND ApplicationName = @app AND LockId = @lId", conn);
        cmd.Parameters.Add("@exp", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes((Double)item.Timeout);
        cmd.Parameters.Add("@sItems", SqlDbType.Text).Value = sessItems;
        cmd.Parameters.Add("@l", SqlDbType.Bit).Value = false;
        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
        cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
        cmd.Parameters.Add("@lId", SqlDbType.Int).Value = lockId;
      }

      try
      {
        conn.Open();

        if (deleteCmd != null)
          deleteCmd.ExecuteNonQuery();

        cmd.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        Logger.LogException("SetAndReleaseItemExclusive", e);
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "SetAndReleaseItemExclusive");
          //throw new ProviderException(exceptionMessage);
        }
        //else
        //  throw e;
      }
      finally
      {
        conn.Close();
      }
    }

    //GetItem
    public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
    {
      return GetSessionStoreItem(false, context, id, out locked, out lockAge, out lockId, out actionFlags);
    }

    // GetItemExclusive
    public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
    {
      return GetSessionStoreItem(true, context, id, out locked, out lockAge, out lockId, out actionFlags);
    }

    //
    // GetSessionStoreItem is called by both the GetItem and 
    // GetItemExclusive methods. GetSessionStoreItem retrieves the 
    // session data from the data source. If the lockRecord parameter
    // is true (in the case of GetItemExclusive), then GetSessionStoreItem
    // locks the record and sets a new LockId and LockDate.
    //
    private SessionStateStoreData GetSessionStoreItem(bool lockRecord, HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
    {
      // Initial values for return value and out parameters.
      SessionStateStoreData item = null;
      lockAge = TimeSpan.Zero;
      lockId = null;
      locked = false;
      actionFlags = 0;

      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = null;
      SqlDataReader reader = null;
      DateTime expires;
      string serializedItems = "";
      bool foundRecord = false;
      bool deleteData = false;
      int timeout = 0;

      try
      {
        conn.Open();

        // lockRecord is true when called from GetItemExclusive and false when called from GetItem. Obtain a lock if possible. Ignore the record if it is expired.
        if (lockRecord)
        {
          cmd = new SqlCommand("UPDATE dbo.[UserSessions] SET Locked = @l1, LockDate = @lDate WHERE SessionId = @Id AND ApplicationName = @app AND Locked = @l2 AND Expires > @exp", conn);
          cmd.Parameters.Add("@l1", SqlDbType.Bit).Value = true;
          cmd.Parameters.Add("@lDate", SqlDbType.DateTime).Value = DateTime.Now;
          cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
          cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
          cmd.Parameters.Add("@l2", SqlDbType.Int).Value = false;
          cmd.Parameters.Add("@exp", SqlDbType.DateTime).Value = DateTime.Now;
          locked = (cmd.ExecuteNonQuery() == 0);// No record was updated because the record was locked or not found.
        }
        // Retrieve the current session item information.
        cmd = new SqlCommand("SELECT Expires, SessionItems, LockId, LockDate, Flags, Timeout FROM dbo.[UserSessions] WHERE SessionId = @Id AND ApplicationName = @app", conn);
        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
        cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
        // Retrieve session item data from the data source.
        reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
        while (reader.Read())
        {
          expires = reader.GetDateTime(0);
          if (expires < DateTime.Now)
          {
            // The record was expired. Mark it as not locked.
            locked = false;
            // The session was expired. Mark the data for deletion.
            deleteData = true;
          }
          else
            foundRecord = true;

          serializedItems = reader.GetString(1);
          lockId = reader.GetInt32(2);
          lockAge = DateTime.Now.Subtract(reader.GetDateTime(3));
          actionFlags = (SessionStateActions)reader.GetInt32(4);
          timeout = reader.GetInt32(5);
        }
        reader.Close();
        // If the returned session item is expired, 
        // delete the record from the data source.
        if (deleteData)
        {
          cmd = new SqlCommand("DELETE FROM dbo.[UserSessions] WHERE SessionId = @Id AND ApplicationName = @app", conn);
          cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
          cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
          cmd.ExecuteNonQuery();
        }
        // The record was not found. Ensure that locked is false.
        if (!foundRecord) locked = false;

        // If the record was found and you obtained a lock, then set the lockId, clear the actionFlags, and create the SessionStateStoreItem to return.
        if (foundRecord && !locked)
        {
          lockId = (int)lockId + 1;
          cmd = new SqlCommand("UPDATE dbo.[UserSessions] SET LockId = @lId, Flags = 0 WHERE SessionId = @Id AND ApplicationName = @app", conn);
          cmd.Parameters.Add("@lId", SqlDbType.Int).Value = lockId;
          cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
          cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
          cmd.ExecuteNonQuery();
          // If the actionFlags parameter is not InitializeItem, deserialize the stored SessionStateItemCollection.
          if (actionFlags == SessionStateActions.InitializeItem)
            item = CreateNewStoreData(context, (int)pConfig.Timeout.TotalMinutes);
          else
            item = Deserialize(context, serializedItems, timeout);
        }
      }
      catch (SqlException e)
      {
        Logger.LogException("GetSessionStoreItem", e);
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "GetSessionStoreItem");
          //throw new ProviderException(exceptionMessage);
        }
        //else
        //  throw e;
      }
      finally
      {
        if (reader != null) { reader.Close(); }
        conn.Close();
      }
      return item;
    }

    //
    // Serialize is called by the SetAndReleaseItemExclusive method to 
    // convert the SessionStateItemCollection into a Base64 string to    
    // be stored in an Access Memo field.
    //
    private string Serialize(SessionStateItemCollection items)
    {
      MemoryStream ms = new MemoryStream();
      BinaryWriter writer = new BinaryWriter(ms);
      if (items != null)
        items.Serialize(writer);
      writer.Close();
      return Convert.ToBase64String(ms.ToArray());
    }

    // DeSerialize is called by the GetSessionStoreItem method to 
    // convert the Base64 string stored in the Access Memo field to a 
    // SessionStateItemCollection.
    private SessionStateStoreData Deserialize(HttpContext context, string serializedItems, int timeout)
    {
      MemoryStream ms = new MemoryStream(Convert.FromBase64String(serializedItems));
      SessionStateItemCollection sessionItems = new SessionStateItemCollection();
      if (ms.Length > 0)
      {
        BinaryReader reader = new BinaryReader(ms);
        sessionItems = SessionStateItemCollection.Deserialize(reader);
      }
      return new SessionStateStoreData(sessionItems, SessionStateUtility.GetSessionStaticObjects(context), timeout);
    }

    // SessionStateProviderBase.ReleaseItemExclusive
    public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("UPDATE dbo.[UserSessions] SET Locked = 0, Expires = @exp WHERE SessionId = @Id AND ApplicationName = @app AND LockId = @lId", conn);
      cmd.Parameters.Add("@exp", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes(pConfig.Timeout.TotalMinutes);
      cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
      cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
      cmd.Parameters.Add("@lId", SqlDbType.Int).Value = lockId;
      try
      {
        conn.Open();
        cmd.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        Logger.LogException("ReleaseItemExclusive", e);
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "ReleaseItemExclusive");
          //throw new ProviderException(exceptionMessage);
        }
        //else
        //  throw e;
      }
      finally
      {
        conn.Close();
      }
    }

    // RemoveItem    
    public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("DELETE * FROM dbo.[UserSessions] WHERE SessionId = @Id AND ApplicationName = @app AND LockId = @lId", conn);
      cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
      cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
      cmd.Parameters.Add("@lId", SqlDbType.Int).Value = lockId;
      try
      {
        conn.Open();
        cmd.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        Logger.LogException("RemoveItem", e);
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "RemoveItem");
          //throw new ProviderException(exceptionMessage);
        }
        //else
        //  throw e;
      }
      finally
      {
        conn.Close();
      }
    }

    // CreateUninitializedItem
    public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      try
      {
        SqlCommand cmd = new SqlCommand("SELECT count(*) as C FROM dbo.[UserSessions] WHERE SessionId = @Id AND ApplicationName = @app", conn);
        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
        cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
        conn.Open();

        SqlDataReader reader = cmd.ExecuteReader();        
        reader.Read();
        if (Convert.ToInt32(reader["C"]) > 0) return;

        cmd = new SqlCommand("INSERT INTO dbo.[UserSessions] (SessionId, ApplicationName, Created, Expires, LockDate, LockId, Timeout, Locked, SessionItems, Flags) Values(@Id, @app, @c, @exp, @lDate, @lId, @t, @l, @sItems, @flgs)", conn);
        cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
        cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
        cmd.Parameters.Add("@c", SqlDbType.DateTime).Value = DateTime.Now;
        cmd.Parameters.Add("@exp", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes((Double)timeout);
        cmd.Parameters.Add("@lDate", SqlDbType.DateTime).Value = DateTime.Now;
        cmd.Parameters.Add("@lId", SqlDbType.Int).Value = 0;
        cmd.Parameters.Add("@t", SqlDbType.Int).Value = timeout;
        cmd.Parameters.Add("@l", SqlDbType.Bit).Value = false;
        cmd.Parameters.Add("@sItems", SqlDbType.Text).Value = "";
        cmd.Parameters.Add("@flgs", SqlDbType.Int).Value = 1;
        cmd.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        //Logger.LogException("CreateUninitializedItem", e);
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "CreateUninitializedItem");
          //throw new ProviderException(exceptionMessage);
        }
        //else
        //  throw e;
      }
      finally
      {
        conn.Close();
      }
    }

    // CreateNewStoreData
    public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
    {
      return new SessionStateStoreData(new SessionStateItemCollection(), SessionStateUtility.GetSessionStaticObjects(context), timeout);
    }

    // ResetItemTimeout
    public override void ResetItemTimeout(HttpContext context, string id)
    {
      SqlConnection conn = new SqlConnection(connectionString);
      SqlCommand cmd = new SqlCommand("UPDATE dbo.[UserSessions] SET Expires = @exp WHERE SessionId = @Id AND ApplicationName = @app", conn);
      cmd.Parameters.Add("@exp", SqlDbType.DateTime).Value = DateTime.Now.AddMinutes(pConfig.Timeout.TotalMinutes);
      cmd.Parameters.Add("@Id", SqlDbType.VarChar, 80).Value = id;
      cmd.Parameters.Add("@app", SqlDbType.VarChar, 255).Value = ApplicationName;
      try
      {
        conn.Open();
        cmd.ExecuteNonQuery();
      }
      catch (SqlException e)
      {
        Logger.LogException("[ResetItemTimeout]", e);
        if (WriteExceptionsToEventLog)
        {
          WriteToEventLog(e, "ResetItemTimeout");
          //throw new ProviderException(exceptionMessage);
        }
        //else
        //  throw e;
      }
      finally
      {
        conn.Close();
      }
    }

    // InitializeRequest
    public override void InitializeRequest(HttpContext context)
    {
    }

    // EndRequest
    public override void EndRequest(HttpContext context)
    {
    }

    #endregion

    //
    // WriteToEventLog
    // This is a helper function that writes exception detail to the 
    // event log. Exceptions are written to the event log as a security
    // measure to ensure private database details are not returned to 
    // browser. If a method does not return a status or Boolean
    // indicating the action succeeded or failed, the caller also 
    // throws a generic exception.
    //


    private void WriteToEventLog(Exception e, string action)
    {
      EventLog log = new EventLog();
      log.Source = eventSource;
      log.Log = eventLog;
      string message = "An exception occurred communicating with the data source.\n\n";
      message += "Action: " + action + "\n\n";
      message += "Exception: " + e.ToString();
      try
      {
        log.WriteEntry(message);
      }
      catch
      {
      }
    }
  }
}