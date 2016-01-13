using System;

namespace Vauction.Models.CustomClasses
{
  public enum JsonExecuteResultTypes : byte { SUCCESS, SUCCESS_INFORMATION, ERROR } ;

  public class JsonExecuteResult
  {
    public JsonExecuteResultTypes Type { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public object Details { get; set; }    

    public JsonExecuteResult(JsonExecuteResultTypes type, string message, object details)
    {
      Type = type;
      Status = type.ToString();
      Message = message;
      Details = details;
    }

    public JsonExecuteResult(JsonExecuteResultTypes type, string message)
      : this(type, message, null)
    {
    }

    public JsonExecuteResult(JsonExecuteResultTypes type)
      : this(type, String.Empty, null)
    {
    }

    public JsonExecuteResult(JsonExecuteResultTypes type, object details)
      : this(type, String.Empty, details)
    {
    }
  }
}
