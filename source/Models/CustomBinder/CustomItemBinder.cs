using System;

using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.Routing;
using Vauction.Utils.Validation;
using Vauction.Utils;

namespace Relatives.Models.CustomBinders
{
  /// <summary>
  /// custom binder, parese url string or Request, this dependens from method - post or get
  /// </summary>
  public class CustomItemBinder : IModelBinder
  {
    #region IModelBinder Members
    /// <summary>
    /// bindig logic
    /// </summary>
    /// <param name="controllerContext"></param>
    /// <param name="bindingContext"></param>
    /// <returns>converted object</returns>
    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      NameValueCollection RequestParameters = new NameValueCollection();

      //get parameters from RoutData
      RequestParameters = controllerContext.RouteData.Values.ToNameValueCollection();


      //MErge parameters RoutData with GET || Post parameters
      if (controllerContext.HttpContext.Request.HttpMethod == "POST")
      {
        RequestParameters = RequestParameters.Merge(controllerContext.HttpContext.Request.Form);
      }
      else if (controllerContext.HttpContext.Request.HttpMethod == "GET")
      {
        RequestParameters = RequestParameters.Merge(controllerContext.HttpContext.Request.QueryString);
      }

      //Do binding
      bindingContext = tryToSetValues(RequestParameters, bindingContext);
      ValidateModel(bindingContext);
      return bindingContext.ModelMetadata.Model;
    }

    #endregion

    #region helper protected virtual methods
    /// <summary>
    /// Get value from form elements dictionary
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    protected virtual object getElementValue(NameValueCollection collection, string key)
    {
      return collection.GetValues(key)[0];
    }
    /// <summary>
    /// try to set values to bindingContext.
    /// 
    /// </summary>
    /// <param name="form"></param>
    /// <param name="bindingContext"></param>
    /// <returns></returns>
    protected virtual ModelBindingContext tryToSetValues(NameValueCollection form, ModelBindingContext bindingContext)
    {
      NameValueCollection propertiesForConvert = CreateNotReadOnlyKeysCollection(bindingContext, form);

      PropertyInfo[] infos = bindingContext.ModelType.GetProperties();
      CreateObjectInstance(bindingContext);
      object value = null;

      foreach (PropertyInfo property in infos)
      {
        if (!bindingContext.ModelState.Keys.Contains(property.Name))
        {
          bindingContext.ModelState.Add(property.Name, new ModelState());
        }

        foreach (string key in propertiesForConvert)
        {
          if (string.Compare(property.Name, key, true) == 0)
          {
            value = ObjectConverter(getElementValue(propertiesForConvert, key), property.PropertyType, bindingContext, key);

            if (value != null)
            {
              property.SetValue(bindingContext.ModelMetadata.Model, value, null);
              bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(value, value.ToString(), null));
              break;
            }
          }

        }
        if (value == null)
          AddDefaultValue(property, bindingContext);
        else
          value = null;
      }

      return bindingContext;
    }
    /// <summary>
    /// convert object value to passed type if error occure then write error in state model
    /// </summary>
    /// <param name="value">value for convert</param>
    /// <param name="type">type which you want get</param>
    /// <param name="bindingContex">binding context</param>
    /// <returns>converted object</returns>
    protected virtual object ObjectConverter(object value, Type type, ModelBindingContext bindingContex, string key)
    {
      object retval = null;
      try
      {
        if (value != null && value.ToString() != string.Empty)
        {
          retval = Convert.ChangeType(value, type);

        }
      }
      catch (InvalidCastException)
      {
        try
        {
          retval = TypeDescriptor.GetConverter(type).ConvertFrom(value);
        }
        catch (Exception)
        {
          AddError(bindingContex, key, value);
        }
      }
      catch (FormatException)
      {
        AddError(bindingContex, key, value);
      }

      return retval;
    }

    private void AddError(ModelBindingContext bindingContex, string key, object value)
    {
      if (bindingContex.ModelState.Keys.Contains(key))
      {
        bindingContex.ModelState[key].Errors.Add("Wrong format");
        if (value != null)
          bindingContex.ModelState.SetModelValue(key, new ValueProviderResult(value, value.ToString(), null));


        if (value != null)
        {
          bindingContex.ModelState.SetModelValue(key, new ValueProviderResult(value, value.ToString(), null));
        }

      }
      else
      {
        bindingContex.ModelState.AddModelError(key, "Wrong format");
      }
    }
    private void AddDefaultValue(PropertyInfo property, ModelBindingContext bindingContex)
    {
      object[] defAttrs = property.GetCustomAttributes(typeof(DefaultValueAttribute), true);
      if (defAttrs.Length > 0)
      {
        DefaultValueAttribute defaultValueAttr = (DefaultValueAttribute)defAttrs[0];

        if (defaultValueAttr != null)
        {
          property.SetValue(bindingContex.Model, defaultValueAttr.Value, null);
          bindingContex.ModelState.SetModelValue(property.Name, new ValueProviderResult(defaultValueAttr.Value, defaultValueAttr.Value.ToString(), null));
          return;
        }
      }
      bindingContex.ModelState.SetModelValue(property.Name, new ValueProviderResult(string.Empty, string.Empty, null));
    }
    private void CreateObjectInstance(ModelBindingContext bindingContext)
    {
      if (bindingContext.ModelMetadata.Model == null)
      {
        //create instance of modelType
        ConstructorInfo modelTypeCtor = bindingContext.ModelType.GetConstructor(Type.EmptyTypes);
        if (null == modelTypeCtor)
          throw new ArgumentException("modelType has no default constructor", "modelType");
        bindingContext.ModelMetadata.Model = modelTypeCtor.Invoke(new object[0]);
      }
    }
    private NameValueCollection CreateNotReadOnlyKeysCollection(ModelBindingContext bindingContext, NameValueCollection form)
    {
      NameValueCollection propertiesForConvert = new NameValueCollection();
      foreach (string sKey in form.Keys)
      {
        /*if (!bindingContext.ModelState.Keys.Contains(sKey))
            bindingContext.ModelState.Add(sKey, new ModelState());*/
        propertiesForConvert.Add(sKey, form[sKey]);
      }
      return propertiesForConvert;
    }
    private void ValidateModel(ModelBindingContext bindingContext)
    {
      ValidationCheck.CheckErrors(bindingContext.ModelMetadata.Model, bindingContext.ModelState);

      Type[] interfaces = bindingContext.ModelType.GetInterfaces();
      foreach (Type type in interfaces)
      {
        if (type == typeof(IValidateModel))
        {
          ((IValidateModel)bindingContext.Model).Validate(bindingContext.ModelState);
          break;
        }
      }
    }
    #endregion
  }

  #region Extension

  public static class BinderHelpers
  {
    public static NameValueCollection ToNameValueCollection(this RouteValueDictionary values)
    {
      NameValueCollection result = new NameValueCollection();

      foreach (var item in values)
      {
        result[item.Key.ToString()] = item.Value.ToString();
      }

      return result;
    }

    public static NameValueCollection Merge(this NameValueCollection main, NameValueCollection slave)
    {
      NameValueCollection result = new NameValueCollection();

      foreach (var key in main)
      {
        result.Add(key.ToString(), main.GetValues(key.ToString())[0]);
      }

      foreach (var key in slave)
      {
        if (result[key.ToString()] == null)
        {
          result.Add(key.ToString(), slave.GetValues(key.ToString())[0]);
        }
      }

      return result;
    }

  #endregion
  }
}