Hashtable.prototype.hash = null;
Hashtable.prototype.keys = null;
Hashtable.prototype.location = null;
function Hashtable() {
  this.hash = new Array();
  this.keys = new Array();
  this.location = 0;
}
Hashtable.prototype.put = function (key, value) {
  if (value == null) return;
  if (this.hash[key] == null)
    this.keys[this.keys.length] = key;
  this.hash[key] = value;
}
Hashtable.prototype.get = function (key) {
  return this.hash[key];
}
Hashtable.prototype.remove = function (key) {
  for (var i = 0; i < this.keys.length; i++) {
    if (key == this.keys[i]) {
      this.hash[this.keys[i]] = null;
      this.keys.splice(i, 1);
      return;
    }
  }
}
Hashtable.prototype.size = function () {
  return this.keys.length;
}
Hashtable.prototype.populateItems = function () { }
Hashtable.prototype.next = function () {
  return (++this.location < this.keys.length);
}
Hashtable.prototype.moveFirst = function () {
  try {
    this.location = -1;
  } catch (e) {}
}
Hashtable.prototype.moveLast = function () {
  try {
    this.location = this.keys.length - 1;
  } catch (e) {}
}
Hashtable.prototype.getKey = function () {
  try {
    return this.keys[this.location];
  } catch (e) {
    return null;
  }
}
Hashtable.prototype.getValue = function () {
  try {
    return this.hash[this.keys[this.location]];
  } catch (e) {
    return null;
  }
}
Hashtable.prototype.getKeyOfValue = function (value) {
  for (var i = 0; i < this.keys.length; i++)
    if (this.hash[this.keys[i]] == value)
      return this.keys[i]
    return null;
  }
Hashtable.prototype.toString = function () {
  try {
    var s = new Array(this.keys.length);
    s[s.length] = "{";
    for (var i = 0; i < this.keys.length; i++) {
      s[s.length] = this.keys[i];
      s[s.length] = "=";
      var v = this.hash[this.keys[i]];
      if (v)
        s[s.length] = v.toString();
      else
        s[s.length] = "null";
      if (i != this.keys.length - 1)
        s[s.length] = ", ";
    }
  } catch (e) {    
  } finally {
    s[s.length] = "}";
  }
  return s.join("");
}
Hashtable.prototype.add = function (ht) {
  try {
    ht.moveFirst();
    while (ht.next()) {
      var key = ht.getKey();
      this.hash[key] = ht.getValue();
      if (this.get(key) != null) {
        this.keys[this.keys.length] = key;
      }
    }
  } catch (e) {    
  } finally {
    return this;
  }
};