
namespace Vauction.Utils.Validation
{
  public class ErrorMessages
  {
    public const string Required = @"'{0}' is required";
    public const string Invalid = @"Field '{0}' is invalid";
    public const string InvalidEmail = @"Field '{0}' is invalid.";
    public const string MinLength = @"Field '{0}' mast be at least {1} character(s)";
    public const string MaxLength = @"Field '{0}' mast be maximum {1} character(s)";
    public const string YearWrong = @"Year must be 1900 - 2078.";
    public const string AlphaWrong = @"Field should contain letters only";
    public const string NumericWrong = @"Field should contain digits only";
    public const string DecimalWrong = @"Field should contain decimal value only";
    public const string AlphaNumericWrong = @"Field should contain alphanumeric symbols only";
    public const string AlphaNumericExtWrong = @"Field should contain alphanumeric and . , - _ symbols only";
    public const string DateTimeWrong = @"Field should contain date and/or time";
    public const string FieldNonSpacedWrong = @"Field cannot contain spaces";
    public const string AmericanPhoneWrong = @"'{0}' should be like: XXX-XXX-XXXX or XXX-XXX-XXXX EXTXXXX";
    public const string PhoneWrong = @"'{0}' should be like: XXXXXXXXXXXXXXXX";
    public const string AmericanOrNotPhoneWrong =  @"'{0}' should be like: XXX-XXX-XXXX or XXX-XXX-XXXX EXTXXXX for the US phone numbers and XXXXXXXXXXXXXXXX for the other countries";    

    public static string GetRequired(string field) { return string.Format(Required, field); }
    public static string GetInvalid(string field) { return string.Format(Invalid, field); }
    public static string GetInvalidEmail(string field) { return string.Format(InvalidEmail, field); }
    public static string GetMinLength(string field, int length) { return string.Format(MinLength, field, length); }
    public static string GetMaxLength(string field, int length) { return string.Format(MaxLength, field, length); }
    public static string GetYearWrong(string field) { return string.Format(YearWrong, field); }
    public static string GetDateTimeWrong(string field) { return string.Format(DateTimeWrong, field); }
    public static string GetAlphanumericWrong(string title) { return string.Format(AlphaNumericWrong, title); }
    public static string GetAlphaWrong(string title){ return string.Format(AlphaWrong, title);}
    public static string GetFieldNumericWrong(string title){ return string.Format(NumericWrong, title);}
    public static string GetFieldDecimalWrong(string title) { return string.Format(DecimalWrong, title); }
    public static string GetFieldAmericanPhoneWrong(string title) {return string.Format(AmericanPhoneWrong, title); }
    public static string GetFieldPhoneWrong(string title) { return string.Format(PhoneWrong, title); }
    public static string GetFieldAmericanOrNotPhoneWrong(string title) { return string.Format(AmericanOrNotPhoneWrong, title); }
    public static string GetFieldNonSpaced(string title){return string.Format(FieldNonSpacedWrong, title);}
    public static string GetAlphanumericExtWrong(string title) { return string.Format(AlphaNumericExtWrong, title);}    
  }
}