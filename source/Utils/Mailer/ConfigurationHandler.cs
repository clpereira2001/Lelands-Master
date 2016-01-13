using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace UniMail
{
  public class MailerConfiguration
  {
    public string Host = "quadecco.4check.com.ua";
    public int Port = 25;
    public string User = "no-reply@quadecco.com";
    public string Password = "Q1w2e3r4";
    public int MessagesPerSession = 10;

    public string SenderName = "Mail robot";
    public string SenderEmail = "alexey.bespalko@quadecco.com";

    private XmlAttribute CreateAttribute(XmlDocument doc, string name, string value)
    {
      XmlAttribute attr = doc.CreateAttribute(name);
      attr.Value = value;
      return attr;
    }
    private XmlNode CreateHostNode(XmlDocument doc)
    {
      XmlElement host = doc.CreateElement("host");
      host.Attributes.Append(CreateAttribute(doc, "address", Host));
      host.Attributes.Append(CreateAttribute(doc, "port", Port.ToString()));
      host.Attributes.Append(CreateAttribute(doc, "user", User));
      host.Attributes.Append(CreateAttribute(doc, "password", Password));
      return host;
    }

    private XmlElement CreateTextNode(XmlDocument doc, string name, string text)
    {
      XmlElement node = doc.CreateElement(name);
      node.InnerText = text;
      return node;
    }
    private XmlNode CreateMessageNode(XmlDocument doc)
    {
      XmlNode message = doc.CreateElement("message");
      XmlNode sender = doc.CreateElement("sender");
      sender.AppendChild(CreateTextNode(doc, "name", SenderName));
      sender.AppendChild(CreateTextNode(doc, "email", SenderEmail));
      message.AppendChild(sender);
      return message;
    }
    public XmlNode CreateNode(XmlDocument doc, string name)
    {
      XmlNode retVal = doc.CreateElement(name);
      retVal.AppendChild(CreateHostNode(doc));
      retVal.AppendChild(CreateMessageNode(doc));
      return retVal;
    }
  }

  public class ConfigurationHandler : IConfigurationSectionHandler
  {
    public static MailerConfiguration GetSection(string fileName, string name)
    {
      System.Configuration.ConfigXmlDocument doc = new ConfigXmlDocument();
      try
      {
        doc.Load(fileName);

        XmlNode node = doc.GetElementsByTagName(name)[0];
        IConfigurationSectionHandler handler = new UniMail.ConfigurationHandler();
        return handler.Create(null, null, node) as UniMail.MailerConfiguration;
      }
      catch
      {
        return null;
      }
    }

    public static bool SaveSection(string fileName, string name, MailerConfiguration section)
    {
      System.Configuration.ConfigXmlDocument doc = new ConfigXmlDocument();
      try
      {
        doc.Load(fileName);
        XmlNode configNode = doc.ChildNodes[1];
        foreach (XmlNode node in configNode.ChildNodes)
        {
          if (node.Name == name)
            configNode.RemoveChild(node);
        }

        XmlNode newNode = section.CreateNode(doc, name);
        configNode.AppendChild(newNode);
        doc.Save(fileName);
        return true;
      }
      catch
      {
        return false;
      }
    }

    #region IConfigurationSectionHandler Members

    object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
    {
      MailerConfiguration config = new MailerConfiguration();

      try
      {
        int ConfigMessagesPerSession = Convert.ToInt32(section.Attributes["MessagePerSession"]);
        if (ConfigMessagesPerSession > 0)
        {
          config.MessagesPerSession = ConfigMessagesPerSession;
        }
      }
      catch (System.Exception) { }

      // Gets the child element names and attributes.
      foreach (XmlNode child in section.ChildNodes)
      {
        if (XmlNodeType.Element == child.NodeType)
        {
          if (child.LocalName == "host")
          {
            foreach (XmlAttribute attribute in child.Attributes)
            {
              if (attribute.LocalName == "address")
              {
                config.Host = attribute.Value;
              }
              else
                if (attribute.LocalName == "port")
                {
                  config.Port = Convert.ToInt32(attribute.Value);
                }
                else
                  if (attribute.LocalName == "user")
                  {
                    config.User = attribute.Value;
                  }
              if (attribute.LocalName == "password")
              {
                config.Password = attribute.Value;
              }
            }
          }
          else
            if (child.LocalName == "message")
            {
              foreach (XmlNode node in child.ChildNodes)
              {
                if (node.LocalName == "sender")
                {
                  foreach (XmlNode senderChild in node.ChildNodes)
                  {
                    if (senderChild.LocalName == "name")
                    {
                      config.SenderName = senderChild.InnerText;
                    }
                    else
                      if (senderChild.LocalName == "email")
                      {
                        config.SenderEmail = senderChild.InnerText;
                      }
                  }
                }
              }
            }

        }
      }

      return config;
    }

    #endregion
  }
}
