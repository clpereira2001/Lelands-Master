using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models.CustomClasses.JsTree
{
  public class JsTreeNode
  {
    public JsTreeNode()
    {
    }    
    public Attributes attributes {get; set;}
    public Data data {get; set;}    
    public string state {get; set;}        
    public List<JsTreeNode> children {get; set;}    
  }
  public class Attributes
  {
    public string id {get; set;}
    public string rel {get;set;}
    public string mdata {get;set;}    
  }
  public class Data
  {
    public string title {get; set;}
    public string icon {get; set;}
    public Attributes attributes { get; set; }
  }
}
