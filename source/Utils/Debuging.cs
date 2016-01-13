using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.SessionState;

namespace Vauction.Utils
{
  public static class Debuging
  {
     private static string GetSessionID(string login, long user_id)
    {
      StringBuilder sb = new StringBuilder();
      foreach (char c in login) sb.Insert(0, (byte)c);
      int l = sb.Length;
      if (l > 14) sb.Remove(14, l - 14);
      DateTime dt = DateTime.Now;
      sb.Insert(0, String.Format("{0}{1}{2}{3}", user_id, dt.Month, dt.DayOfYear, dt.Day));
      return sb.ToString();
    }

     public static void Insert(byte method, HttpSessionState session, string login, long user_id, string url, string appsession)
     {
       SqlConnection conn =
         new SqlConnection(ConfigurationManager.ConnectionStrings["LelandsConnectionString"].ConnectionString);
       try
       {
         SqlCommand cmd = new SqlCommand("exec spDebuging_Insert @u, @s, @g, @m, @url, @cid, @gcid", conn);
         cmd.Parameters.Add("@u", SqlDbType.BigInt).Value = user_id;
         cmd.Parameters.Add("@s", SqlDbType.VarChar, 255).Value = session.SessionID;
         cmd.Parameters.Add("@g", SqlDbType.VarChar, 255).Value = user_id == -1? String.Empty : GetSessionID(login,user_id);
         cmd.Parameters.Add("@m", SqlDbType.TinyInt).Value = method;
         cmd.Parameters.Add("@url", SqlDbType.VarChar, 1024).Value = url;
         cmd.Parameters.Add("@cid", SqlDbType.BigInt).Value = -1;
         cmd.Parameters.Add("@gcid", SqlDbType.VarChar, 255).Value = appsession;
         conn.Open();
         cmd.ExecuteNonQuery();
       }
       catch (Exception)
       {
       }
       finally
       {
         if (conn.State == ConnectionState.Open)
           conn.Close();
       }
     }
  }
}