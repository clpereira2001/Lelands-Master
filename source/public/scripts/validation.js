function SetInputsMaxLength(errRules) {
  errorRules.location = 0;
  for (i = 0; i < errRules.keys.length; i++) {
    var rule_key = errRules.getKey();
    var errors = errRules.get(rule_key);
    var el = document.getElementById(rule_key);
    var arr = errors.get("maxlength");
    if (arr != null && el != null) {
      el.maxLength = arr[1];
    }
    errRules.next();
  }
}
function FindControlForValidation(id, submitControl) {
  if (submitControl != null && submitControl.form != null) {
    return submitControl.form.elements[id];
  }
  else {
    return document.getElementById(id); ;
  }
}
function SubmitFormWithClientValidation(errRules, submitControl) {
  var isValid = true;
  errorRules.location = 0;
  for (i = 0; i < errRules.keys.length; i++) {
    var rule_key = errRules.getKey();
    var errors = errRules.get(rule_key);
    errors.location = 0;
    var el = FindControlForValidation(rule_key, submitControl)
    var el_message = document.getElementById("vm_" + rule_key);
    var isError = false;
    if (el_message != null)
      el_message.innerHTML = "";
    if (el != null && el.className != null) {
      if (el.className.indexOf("input-validation-error") != -1) {
        el.className = el.className.replace("input-validation-error", "");
      }
    }
    if (el_message != null && el_message.className != null) {
      if (el_message.className.indexOf("show") != -1) {
        el_message.className = el_message.className.replace("show", "");
      }
    }
    for (j = 0; j < errors.keys.length; j++) {
      var key = errors.getKey();
      var val = '';
      if (el != null && el.disabled == false) {
        if (key == 'required' && trim(el.value) == '') {
          val = errors.get(key)[0];
          isValid = false;
          isError = true;
          break;
        }
      }
      errors.next();
    }
    if (isError) {
      el.className += " input-validation-error";
      if (el_message != null) {
        el_message.className += " show";
        el_message.innerHTML = val;
      }
    }
    errRules.next();
  }
  return isValid;
}
function rtrim(str2trim) {
  if (str2trim == null) return null;
  for (var i = str2trim.length; i > 0; i--) {
    if (str2trim.substr(i - 1, 1) != " ") {
      return str2trim.substr(0, i)
    }
  }
  return ""
}
function ltrim(str2trim) {
  if (str2trim == null) return null;
  for (var i = 0; i < str2trim.length; i++) {
    if (str2trim.substr(i, 1) != " ") {
      return str2trim.slice(i)
    }
  }
  return ""
}
function trim(str2trim) {
  return rtrim(ltrim(str2trim))
}