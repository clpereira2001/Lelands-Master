using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using ExcelLibrary.SpreadSheet;
using Vauction.Models.CustomClasses;
using Vauction.Utils;
using Vauction.Utils.Helper;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
    public class GeneralRepository : IGeneralRepository
    {
        private ICacheDataProvider CacheRepository;
        private VauctionDataContext dataContext;

        //GeneralRepository
        public GeneralRepository(VauctionDataContext dataContext, ICacheDataProvider cacheRepository)
        {
            this.dataContext = dataContext;
            CacheRepository = cacheRepository;
        }

        //SubmitChanges

        // NOT DONE


        //GetCommissionRatesForSelect
        public string GetCommissionRatesForSelect()
        {
            var pt = (from CR in dataContext.CommissionRates
                      orderby CR.MaxPercent
                      select CR).ToList<CommissionRate>();
            var sb = new StringBuilder();
            foreach (var p in pt)
                sb.AppendFormat("{0}:{1};", p.RateID, p.Description);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        //GetCountryList
        public object GetCountryList(string sidx, string sord, int page, int rows, bool _search, string Id, string Title,
            string Code)
        {
            var countries = new List<Country>((from C in dataContext.Countries
                                               select C).OrderBy(sidx + " " + sord));

            if (_search)
            {
                if (countries.Count > 0 && !String.IsNullOrEmpty(Id))
                    countries = new List<Country>(countries.Where(A => A.ID == Convert.ToInt64(Id)));
                if (countries.Count > 0 && !String.IsNullOrEmpty(Title))
                    countries = new List<Country>(countries.Where(A => A.Title.ToLower().Contains(Title.ToLower())));
                if (countries.Count > 0 && !String.IsNullOrEmpty(Code))
                    countries = new List<Country>(countries.Where(A => A.Code.ToLower().Contains(Code.ToLower())));
            }

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = countries.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            countries = new List<Country>(countries.Skip(pageIndex * pageSize).Take(pageSize));

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in countries
                    select new
                    {
                        i = query.ID,
                        cell = new string[] { query.ID.ToString(), query.Title, query.Code }
                    }).ToArray()
            };

            countries = null;

            return jsonData;
        }


        //AddEditCountry
        public bool AddEditCountry(long? ID, string Title, string Code, bool IsAddin)
        {
            var country = (IsAddin) ? new Country() : dataContext.Countries.SingleOrDefault(C => C.ID == ID);
            if (country == null) return false;

            country.Title = Title;
            country.Code = Code;

            if (IsAddin) dataContext.Countries.InsertOnSubmit(country);
            return SubmitChanges(dataContext);
        }

        //DeleteCountry
        public bool DeleteCountry(long ID)
        {
            var country = dataContext.Countries.SingleOrDefault(C => C.ID == ID);
            if (country == null) return false;
            dataContext.Countries.DeleteOnSubmit(country);
            return SubmitChanges(dataContext);
        }

        //GetCommissionRateList
        public object GetCommissionRateList(string sidx, string sord, int page, int rows, bool _search, string RateID,
            string Description, string MaxPercent)
        {
            var comrate = new List<CommissionRate>((from C in dataContext.CommissionRates
                                                    select C).OrderBy(sidx + " " + sord));

            if (_search)
            {
                if (comrate.Count > 0 && !String.IsNullOrEmpty(RateID))
                    comrate = new List<CommissionRate>(comrate.Where(A => A.RateID == Convert.ToInt64(RateID)));
                if (comrate.Count > 0 && !String.IsNullOrEmpty(Description))
                    comrate =
                        new List<CommissionRate>(
                            comrate.Where(A => A.Description.ToLower().Contains(Description.ToLower())));
                if (comrate.Count > 0 && !String.IsNullOrEmpty(MaxPercent))
                    comrate = new List<CommissionRate>(comrate.Where(A => A.MaxPercent == Convert.ToDecimal(MaxPercent)));
            }

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = comrate.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            comrate = new List<CommissionRate>(comrate.Skip(pageIndex * pageSize).Take(pageSize));

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in comrate
                    select new
                    {
                        i = query.RateID,
                        cell =
                            new string[]
                            {
                                query.RateID.ToString(), query.Description, query.MaxPercent.ToString(),
                                query.LongDescription
                            }
                    }).ToArray()
            };

            comrate = null;

            return jsonData;
        }

        //AddEditCommissionRate
        public bool AddEditCommissionRate(long id, string description, decimal maxPercent, bool isAddin,
            string longDescription)
        {
            var cr = (isAddin) ? new CommissionRate() : dataContext.CommissionRates.SingleOrDefault(t => t.RateID == id);
            if (cr == null) return false;
            cr.Description = description;
            cr.MaxPercent = maxPercent;
            cr.LongDescription = longDescription;
            if (isAddin) dataContext.CommissionRates.InsertOnSubmit(cr);
            return SubmitChanges(dataContext);
        }

        //DeleteCommissionRate
        public bool DeleteCommissionRate(long ID)
        {
            var cr = dataContext.CommissionRates.SingleOrDefault(A => A.RateID == ID);
            if (cr == null) return false;
            dataContext.CommissionRates.DeleteOnSubmit(cr);
            return SubmitChanges(dataContext);
        }

        //GetVariableList
        public object GetVariableList(string sidx, string sord, int page, int rows, bool _search, string Id, string Name,
            string Value)
        {
            var variable = new List<Variable>((from C in dataContext.Variables
                                               select C).OrderBy(sidx + " " + sord));

            if (_search)
            {
                if (variable.Count > 0 && !String.IsNullOrEmpty(Id))
                    variable = new List<Variable>(variable.Where(A => A.ID == Convert.ToInt64(Id)));
                if (variable.Count > 0 && !String.IsNullOrEmpty(Name))
                    variable = new List<Variable>(variable.Where(A => A.Name.ToLower().Contains(Name.ToLower())));
                if (variable.Count > 0 && !String.IsNullOrEmpty(Value))
                    variable = new List<Variable>(variable.Where(A => A.Value == Convert.ToDecimal(Value)));
            }

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = variable.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            variable = new List<Variable>(variable.Skip(pageIndex * pageSize).Take(pageSize));

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in variable
                    select new
                    {
                        i = query.ID,
                        cell = new string[] { query.ID.ToString(), query.Name, query.Value.ToString() }
                    }).ToArray()
            };

            variable = null;

            return jsonData;
        }

        //AddEditVariable
        public bool AddEditVariable(long ID, string Name, decimal Value, bool IsAddin)
        {
            var vr = (IsAddin) ? new Variable() : dataContext.Variables.SingleOrDefault(C => C.ID == ID);
            if (vr == null) return false;

            vr.Name = Name;
            vr.Value = Value;

            if (IsAddin) dataContext.Variables.InsertOnSubmit(vr);
            return SubmitChanges(dataContext);
        }

        //DeleteVariable
        public bool DeleteVariable(long ID)
        {
            var vr = dataContext.Variables.SingleOrDefault(A => A.ID == ID);
            if (vr == null) return false;
            dataContext.Variables.DeleteOnSubmit(vr);
            return SubmitChanges(dataContext);
        }

        //GetStateList
        public object GetStateList(string sidx, string sord, int page, int rows, bool _search, string Id, string Title,
            string Code, string Country_ID)
        {
            var st = new List<State>((from C in dataContext.States
                                      select C).OrderBy(sidx + " " + sord));

            if (_search)
            {
                if (st.Count > 0 && !String.IsNullOrEmpty(Id))
                    st = new List<State>(st.Where(A => A.ID == Convert.ToInt64(Id)));
                if (st.Count > 0 && !String.IsNullOrEmpty(Title))
                    st = new List<State>(st.Where(A => A.Title.ToLower().Contains(Title.ToLower())));
                if (st.Count > 0 && !String.IsNullOrEmpty(Code))
                    st = new List<State>(st.Where(A => A.Code.ToLower().Contains(Code.ToLower())));
                if (st.Count > 0 && !String.IsNullOrEmpty(Country_ID))
                    st = new List<State>(st.Where(A => A.Country_ID == Convert.ToInt64(Country_ID)));
            }

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = st.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            st = new List<State>(st.Skip(pageIndex * pageSize).Take(pageSize));

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in st
                    select new
                    {
                        i = query.ID,
                        cell = new string[] { query.ID.ToString(), query.Title, query.Code, query.Country_ID.ToString() }
                    }).ToArray()
            };

            st = null;

            return jsonData;
        }

        //AddEditState
        public bool AddEditState(long ID, string Title, string Code, long Country_ID, bool IsAddin)
        {
            var st = (IsAddin) ? new State() : dataContext.States.SingleOrDefault(C => C.ID == ID);
            if (st == null) return false;

            st.Title = Title;
            st.Code = Code;
            st.Country_ID = Country_ID;

            if (IsAddin) dataContext.States.InsertOnSubmit(st);
            return SubmitChanges(dataContext);
        }

        //DeleteState
        public bool DeleteState(long ID)
        {
            var st = dataContext.States.SingleOrDefault(A => A.ID == ID);
            if (st == null) return false;
            dataContext.States.DeleteOnSubmit(st);
            return SubmitChanges(dataContext);
        }


        //GetAuctionStatusesForSearchSelect
        public string GetAuctionStatusesForSearchSelect()
        {
            var pt = (from UT in dataContext.AuctionStatus
                      orderby UT.ID ascending
                      select UT).ToList();
            var sb = new StringBuilder();
            sb.Append(":;");
            foreach (var p in pt)
                sb.AppendFormat("{0}:{1};", p.ID, p.Title);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        //GetMainCategoriesForSearchSelect
        public string GetMainCategoriesForSearchSelect()
        {
            var pt = (from UT in dataContext.MainCategories
                      orderby UT.Priority ascending, UT.Title ascending
                      select UT).ToList();
            var sb = new StringBuilder();
            sb.Append(":;");
            foreach (var p in pt)
                sb.AppendFormat("{0}:{1};", p.ID, p.Title);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }


        //PrintLabelsForItems    
        public string PrintLabelsForItems(Dictionary<long, byte> auction_count, long cons_id)
        {
            var ids = auction_count.Keys.ToList();
            var auctions = (from A in dataContext.Auctions
                            where ids.Contains(A.ID)
                            select A).ToList();
            ids = null;

            var cons = dataContext.Consignments.SingleOrDefault(C => C.ID == cons_id);
            if (cons == null) return String.Empty;

            var sb = new StringBuilder();
            if (cons.Specialist != null)
            {
                var str = cons.Specialist.FirstName.Split(' ');
                foreach (var s in str) sb.Append(s[0]);
                if (!String.IsNullOrEmpty(cons.Specialist.LastName))
                {
                    str = cons.Specialist.LastName.Split(' ');
                    foreach (var s in str) sb.Append(s[0]);
                }
            }

            var result = new StringBuilder();
            result.AppendLine("<html><body><div style='margin:-10 0 0 -10'>");
            Auction auc;
            for (var j = 0; j < auctions.Count(); j++)
            {
                auc = auctions[j];
                for (var i = 0; i < auction_count[auc.ID]; i++)
                {
                    result.AppendLine(
                        "<table style='width:300px; height:112px; padding:0px; font-size:9pt; border:solid 1px gray;margin:2px; font-family:Arial;' cellpadding='1' cellspacing='1'>");
                    result.AppendLine("<tr>");
                    result.AppendFormat(
                        "<td colspan='2' style='padding-left:2px;font-weight:bold'>{0} - A{1} - {2} - {3}</td>", auc.ID,
                        auc.Event_ID, cons_id, sb.ToString().ToUpper());
                    result.AppendLine("</tr>");
                    result.AppendLine("<tr>");
                    result.AppendLine("<td colspan='2' style='padding-left:2px;font-weight:bold'>" + auc.Title + "</td>");
                    result.AppendLine("</tr>");
                    result.AppendLine("<tr>");
                    result.AppendFormat(
                        "<td colspan='2' style='padding-left:2px'>Reserve: {0} Qnty: {1} Page Layout: {2}</td>",
                        auc.Price.GetCurrency(false), auc.Quantity, auc.ShortPriorityDescription);
                    result.AppendLine("</tr>");
                    result.AppendLine("<tr>");
                    result.AppendFormat("<td colspan='2' style='padding-left:2px'>{0} > {1}</td>",
                        auc.EventCategory.MainCategory.Title[0], auc.EventCategory.Category.Title);
                    result.AppendLine("</tr>");
                    result.AppendLine("<tr>");
                    result.AppendLine("<td style='border:1px solid gray;width:50%'>Writer</td>");
                    result.AppendLine("<td style='border:1px solid gray;width:50%'>Photographer</td>");
                    result.AppendLine("</tr>");
                    result.AppendLine("</table>");
                    //if (!(j + 1 == auctions.Count() && i + 1 == auction_count[auc.ID]))
                    //  result.AppendLine("<br clear=all style='page-break-before:always; height:1px;'>");
                }
                auc.IsPrinted = true;
            }
            result.AppendLine("<script type=\"text/javascript\">window.print()</script>");
            result.AppendLine("</div></body></html>");
            SubmitChanges(dataContext);
            return result.ToString();
        }

        //GetUserTypesForSearchSelect
        public string GetUserTypesForSearchSelect()
        {
            var pt = (from UT in dataContext.UserTypes
                      orderby UT.Title ascending
                      select UT).ToList<UserType>();
            var sb = new StringBuilder();
            sb.Append(":;");
            foreach (var p in pt)
                sb.AppendFormat("{0}:{1};", p.ID, p.Title);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        //GetUserStatusesForSearchSelect
        public string GetUserStatusesForSearchSelect()
        {
            var pt = (from UT in dataContext.UserStatus
                      orderby UT.Title ascending
                      select UT).ToList<UserStatus>();
            var sb = new StringBuilder();
            sb.Append(":;");
            foreach (var p in pt)
                sb.AppendFormat("{0}:{1};", p.ID, p.Title);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        //GetCommissionRatesForSearchSelect
        public string GetCommissionRatesForSearchSelect()
        {
            var pt = (from UT in dataContext.CommissionRates
                      orderby UT.Description ascending
                      select UT).ToList<CommissionRate>();
            var sb = new StringBuilder();
            sb.Append(":;");
            foreach (var p in pt)
                sb.AppendFormat("{0}:{1};", p.RateID, p.Description);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        //GetEmailImages
        public object GetEmailImages(string sidx, string sord, int page, int rows)
        {
            var files = Directory.GetFiles(DiffMethods.GetUploadedEmailImageDir(), "*.*");
            var fi = new List<FileInfo>();
            foreach (var s in files)
                fi.Add(new FileInfo(s));

            files = null;

            fi = fi.OrderBy(F => F.Name).ToList();

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = fi.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            fi = fi.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in fi
                    select new
                    {
                        i = query.Name,
                        cell = new string[]
                        {
                            query.Name,
                            String.Format("<img src='{0}' style='max-width:300px; max-height:300px' />",
                                DiffMethods.GetUploadedEmailImageWeb(query.Name)),
                            Path.GetFileName(query.FullName),
                            DiffMethods.GetUploadedEmailImageWeb(query.Name)
                        }
                    }).ToArray()
            };
            return jsonData;
        }

        //UploadImage
        public JsonExecuteResult UploadEmailImage(HttpPostedFileBase file)
        {
            try
            {
                var path = DiffMethods.GetUploadedEmailImageDir();
                System.Drawing.Image img;
                try
                {
                    img = System.Drawing.Image.FromStream(file.InputStream);
                    var fi = new FileInfo(Path.Combine(path, file.FileName));
                    if (fi.Exists) fi.Delete();
                    img.Save(fi.FullName);
                }
                catch (ExternalException ex)
                {
                    throw new ExternalException(ex.Message);
                }
                catch (Exception ex)
                {
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
                }
                img.Dispose();
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //DeleteEmailImages
        public JsonExecuteResult DeleteEmailImages(string filenames)
        {
            var sb = new StringBuilder();
            try
            {
                FileInfo fi;
                var files = filenames.Split(',');
                foreach (var f in files)
                {
                    fi = new FileInfo(Path.Combine(DiffMethods.GetUploadedEmailImageDir(), f));
                    if (!fi.Exists)
                    {
                        sb.Append(fi.Name + ",");
                        continue;
                    }
                    fi.Delete();
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return (sb.Length > 0)
                ? new JsonExecuteResult(JsonExecuteResultTypes.ERROR,
                    "Files doesn't exist: " + sb.Remove(sb.Length - 1, 1).ToString())
                : new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //GetDifferentImages
        public object GetDifferentImages(string sidx, string sord, int page, int rows)
        {
            var files = Directory.GetFiles(DiffMethods.GetUploadedDifferentImageDir(), "thmb_*.*");
            var fi = new List<FileInfo>();
            foreach (var s in files)
                fi.Add(new FileInfo(s));

            files = null;

            fi = fi.OrderBy(F => F.Name).ToList();

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = fi.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            fi = fi.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in fi
                    select new
                    {
                        i = query.Name,
                        cell = new string[]
                        {
                            query.Name.Replace("thmb_", String.Empty),
                            String.Format("<img src='{0}' style='max-width:120px; max-height:120px' />",
                                DiffMethods.GetUploadedDifferentImageWeb(query.Name)),
                            Path.GetFileName(query.FullName.Replace("thmb_", String.Empty)),
                            DiffMethods.GetUploadedDifferentImageWeb(query.Name.Replace("thmb_", String.Empty))
                        }
                    }).ToArray()
            };
            return jsonData;
        }

        //DeleteDifferentImage

        //UploadDifferentImage
        public JsonExecuteResult UploadDifferentImage(HttpPostedFileBase file)
        {
            try
            {
                var path = DiffMethods.GetUploadedDifferentImageDir();
                System.Drawing.Image img;
                try
                {
                    img = System.Drawing.Image.FromStream(file.InputStream);
                    var fi = new FileInfo(Path.Combine(path, file.FileName));
                    DeleteDifferentImage(fi);
                    img.Save(fi.FullName);
                    img = DiffMethods.ChangeImageSize(img, 100, null, false);
                    img.Save(Path.Combine(fi.DirectoryName, "thmb_" + fi.Name));
                }
                catch (ExternalException ex)
                {
                    throw new ExternalException(ex.Message);
                }
                catch (Exception ex)
                {
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
                }
                img.Dispose();
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //DeleteDifferentImages
        public JsonExecuteResult DeleteDifferentImages(string filenames)
        {
            var sb = new StringBuilder();
            try
            {
                FileInfo fi;
                var files = filenames.Split(',');
                foreach (var f in files)
                {
                    fi = new FileInfo(Path.Combine(DiffMethods.GetUploadedDifferentImageDir(), f));
                    if (!fi.Exists)
                    {
                        sb.Append(fi.Name + ",");
                        continue;
                    }
                    DeleteDifferentImage(fi);
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return (sb.Length > 0)
                ? new JsonExecuteResult(JsonExecuteResultTypes.ERROR,
                    "Files doesn't exist: " + sb.Remove(sb.Length - 1, 1).ToString())
                : new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //GetHomepageImages
        public object GetHomepageImages(string sidx, string sord, int page, int rows)
        {
            var result = dataContext.spImages_Homepage().ToList();

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = result.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            result = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in result
                    select new
                    {
                        i = query.ID,
                        cell = new string[]
                        {
                            query.ID.ToString(),
                            String.Format("<img src='{0}' style='max-width:200px; max-height:200px' />",
                                DiffMethods.GetUploadedHomepageImageWeb(query.ImgFileName)),
                            Path.GetFileName(query.ImgFileName),
                            query.ImgOrder.ToString(),
                            query.Link,
                            query.LinkTitle,
                            ((Consts.HomepageImageType) query.ImgType).ToString(),
                            query.IsEnabled.ToString()
                        }
                    }).ToArray()
            };
            return jsonData;
        }

        //UploadHomepageImage
        public JsonExecuteResult UploadHomepageImage(HttpPostedFileBase file)
        {
            try
            {
                var path = DiffMethods.GetUploadedHomepageImageDir();
                System.Drawing.Image img;
                try
                {
                    img = System.Drawing.Image.FromStream(file.InputStream);
                    var fi = new FileInfo(Path.Combine(path, file.FileName));
                    if (fi.Exists) fi.Delete();
                    img.Save(fi.FullName);
                    var hi = new HomepageImage();
                    hi.ImgFileName = fi.Name;
                    hi.ImgOrder = 99999;
                    hi.ImgType = (byte)Consts.HomepageImageType.Big;
                    hi.IsEnabled = false;
                    hi.Link = hi.LinkTitle = String.Empty;
                    dataContext.HomepageImages.InsertOnSubmit(hi);
                    SubmitChanges(dataContext);
                }
                catch (ExternalException ex)
                {
                    throw new ExternalException(ex.Message);
                }
                catch (Exception ex)
                {
                    return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
                }
                img.Dispose();
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //DeleteHomepageImages
        public JsonExecuteResult DeleteHomepageImages(long[] homepageimage_id)
        {
            var result = dataContext.spImages_Homepage().Where(i => homepageimage_id.Contains(i.ID)).ToList();
            try
            {
                FileInfo fi;
                foreach (var hpi in result)
                {
                    fi = new FileInfo(Path.Combine(DiffMethods.GetUploadedHomepageImageDir(), hpi.ImgFileName));
                    if (fi.Exists) fi.Delete();
                }
                dataContext.HomepageImages.DeleteAllOnSubmit(result);
                SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //UpdateHomepageImage
        public JsonExecuteResult UpdateHomepageImage(long homepageimage_id, int order, string link, string linktitle,
            int type, bool isenabled)
        {
            try
            {
                var result = dataContext.spImages_Homepage().Where(i => i.ID == homepageimage_id).FirstOrDefault();
                if (result == null) throw new Exception("The image doesn't exist.");
                result.ImgOrder = order;
                result.Link = link;
                result.LinkTitle = linktitle;
                result.ImgType = type;
                result.IsEnabled = isenabled;
                SubmitChanges(dataContext);
                CacheRepository.Remove(new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.HOTNEWS,
                    "GETHOMEPAGEIMAGES", new object[] { }, CachingExpirationTime.Minutes_10));
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //GetVauctionRegions

        //GetCacheList
        public object GetCacheList(string sidx, string sord, int page, int rows, bool issearch, string type,
            string region, string method)
        {
            if (!(CacheRepository is AppFabricCacheProviderSystemRegions)) return null;
            var result = (CacheRepository as AppFabricCacheProviderSystemRegions).AllCache();

            if (result == null) return null;

            if (issearch)
            {
                if (result.Count() > 0 && !String.IsNullOrEmpty(type))
                    result = result.Where(r => r.Type.ToString() == type).ToList();
                if (result.Count() > 0 && !String.IsNullOrEmpty(region))
                    result = result.Where(r => r.Region.ToLower().Contains(region.ToLower())).ToList();
                if (result.Count() > 0 && !String.IsNullOrEmpty(method))
                    result = result.Where(r => r.Method.ToLower().Contains(method.ToLower())).ToList();
            }

            switch (sidx)
            {
                case "Type":
                    result =
                        ((sord == "asc") ? result.OrderBy(d => d.Type) : result.OrderByDescending(d => d.Type)).ToList();
                    break;
                case "Region":
                    result =
                        ((sord == "asc") ? result.OrderBy(d => d.Region) : result.OrderByDescending(d => d.Region))
                            .ToList();
                    break;
                case "Method":
                    result =
                        ((sord == "asc") ? result.OrderBy(d => d.Method) : result.OrderByDescending(d => d.Method))
                            .ToList();
                    break;
                case "Data":
                    result =
                        ((sord == "asc") ? result.OrderBy(d => d.Data) : result.OrderByDescending(d => d.Data)).ToList();
                    break;
                case "TTL":
                    result =
                        ((sord == "asc") ? result.OrderBy(d => d.CacheTime) : result.OrderByDescending(d => d.CacheTime))
                            .ToList();
                    break;
            }

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;
            var totalRecords = result.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            result = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var count = 0;
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from query in result
                    select new
                    {
                        i = ++count,
                        cell = new string[]
                        {
                            String.Format("{0}~{1}~{2}", query.Type, query.Region, query.Method),
                            count.ToString(),
                            query.Type.ToString(),
                            query.Region,
                            query.Method,
                            query.Data.ToString(),
                            TimeSpan.FromSeconds(query.CacheTime).ToString()
                        }
                    }).ToArray()
            };
            return jsonData;
        }

        //InitRegions
        public JsonExecuteResult InitRegions()
        {
            try
            {
                if (!(CacheRepository is AFCDataProvider))
                    throw new Exception("You can init regions only when you are using AppFabric Cache.");
                (CacheRepository as AFCDataProvider).InitDataCacheRegions(GetVauctionRegions());
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //DeleteCache
        public JsonExecuteResult DeleteCache(string key)
        {
            try
            {
                if (!(CacheRepository is AppFabricCacheProviderSystemRegions))
                    throw new Exception("You can delete cache only when you are using AppFabric Cache.");
                var keys = key.Split('~');
                DataCacheType dct;
                switch (keys[0])
                {
                    case "RESOURSE":
                        dct = DataCacheType.RESOURCE;
                        break;
                    case "REFERENCE":
                        dct = DataCacheType.REFERENCE;
                        break;
                    default:
                        dct = DataCacheType.ACTIVITY;
                        break;
                }
                var dco = new DataCacheObject(dct, keys[1], keys[2]);
                (CacheRepository as AppFabricCacheProviderSystemRegions).Remove(dco);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //DeleteRegion
        public JsonExecuteResult ClearRegion(string type)
        {
            try
            {
                if (!(CacheRepository is AppFabricCacheProviderSystemRegions))
                    throw new Exception("You can delete cache only when you are using AppFabric Cache.");
                if (String.IsNullOrEmpty(type))
                {
                    var cache = GetVauctionRegions();
                    var tp = DataCacheType.REFERENCE.ToString();
                    cache[tp].ForEach(r => (CacheRepository as AppFabricCacheProviderSystemRegions).Clear(tp, r));
                    tp = DataCacheType.RESOURCE.ToString();
                    cache[tp].ForEach(r => (CacheRepository as AppFabricCacheProviderSystemRegions).Clear(tp, r));
                    tp = DataCacheType.ACTIVITY.ToString();
                    cache[tp].ForEach(r => (CacheRepository as AppFabricCacheProviderSystemRegions).Clear(tp, r));
                }
                else
                {
                    var regions = GetVauctionRegions()[type];
                    regions.ForEach(r => (CacheRepository as AppFabricCacheProviderSystemRegions).Clear(type, r));
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }


        //Temp_CreateSpreadsheets
        public List<string> Temp_CreateSpreadsheets(long event_id)
        {
            string file;
            var files = new List<string>();
            var ec = dataContext.EventCategories.Where(e => e.Event_ID == event_id).ToList();
            ushort row = 0;
            StringBuilder sbA, sbB;
            foreach (var e in ec)
            {
                if (e.Auctions.Count() == 0) continue;
                sbA = new StringBuilder(e.MainCategory.Title);
                sbA.Replace(" ", String.Empty);
                sbA.Replace("\"", "-");
                sbA.Replace("\'", "-");
                sbB = new StringBuilder(e.Category.Title);
                sbB.Replace(" ", String.Empty);
                sbB.Replace("\"", "-");
                sbB.Replace("\'", "-");
                file = HttpContext.Current.Server.MapPath("~/Pool/") +
                       String.Format("{0:yyyyMMdd}_{1}_{2}.xls", DateTime.Now, sbA, sbB);
                var workbook = new Workbook();
                var worksheet = new Worksheet(e.FullCategory);
                row = 0;
                worksheet.Cells.ColumnWidth[0] = 3000;
                worksheet.Cells.ColumnWidth[1] = 3000;
                worksheet.Cells.ColumnWidth[2] = 3000;
                worksheet.Cells.ColumnWidth[3] = 15000;
                worksheet.Cells.ColumnWidth[4] = 10000;
                worksheet.Cells.ColumnWidth[5] = 3000;
                worksheet.Cells.ColumnWidth[6] = 5000;
                worksheet.Cells.ColumnWidth[7] = 5000;
                worksheet.Cells.ColumnWidth[8] = 5000;
                worksheet.Cells.ColumnWidth[9] = 5000;
                worksheet.Cells.ColumnWidth[10] = 5000;
                worksheet.Cells.ColumnWidth[11] = 5000;
                worksheet.Cells.ColumnWidth[12] = 6000;
                worksheet.Cells.ColumnWidth[13] = 6000;
                worksheet.Cells.ColumnWidth[14] = 6000;
                worksheet.Cells.ColumnWidth[15] = 6000;
                worksheet.Cells.ColumnWidth[16] = 6000;
                worksheet.Cells.ColumnWidth[17] = 6000;
                worksheet.Cells.ColumnWidth[18] = 6000;
                worksheet.Cells.ColumnWidth[19] = 6000;

                worksheet.Cells[row, 0] = new Cell("Inventory#");
                worksheet.Cells[row, 1] = new Cell("Lot");
                worksheet.Cells[row, 2] = new Cell("Status");
                worksheet.Cells[row, 3] = new Cell("Title");
                worksheet.Cells[row, 4] = new Cell("Description");
                worksheet.Cells[row, 5] = new Cell("Reserve");
                worksheet.Cells[row, 6] = new Cell("Page Layout");
                worksheet.Cells[row, 7] = new Cell("Writing Step");
                worksheet.Cells[row, 8] = new Cell("Photographed");
                worksheet.Cells[row, 9] = new Cell("In Layout");
                worksheet.Cells[row, 10] = new Cell("Seller");
                worksheet.Cells[row, 11] = new Cell("Entered By");
                worksheet.Cells[row, 12] = new Cell("Date In");
                worksheet.Cells[row, 13] = new Cell("Last Updated");
                worksheet.Cells[row, 14] = new Cell("@Image");
                worksheet.Cells[row, 15] = new Cell("@Imagea");
                worksheet.Cells[row, 16] = new Cell("@Imageb");
                worksheet.Cells[row, 17] = new Cell("@Imagec");
                worksheet.Cells[row, 18] = new Cell("@Imaged");
                worksheet.Cells[row, 19] = new Cell("@Imagee");
                foreach (var a in e.Auctions.OrderBy(A => A.NotifiedOn))
                {

                    worksheet.Cells[++row, 0] = new Cell(a.ID.ToString(),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 1] = new Cell(a.Lot.HasValue ? a.Lot.Value.ToString() : String.Empty,
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 2] = new Cell(a.AuctionStatus.Title,
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 3] = new Cell(a.Title.Trim(),
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 4] =
                        new Cell(
                            a.Description.Length > 4090 ? string.Format("{0} [READ MORE]", a.Description.Substring(0, 4089)) : a.Description.Trim(), new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 5] = new Cell(a.Reserve, "#,##0.00");
                    worksheet.Cells[row, 6] = new Cell(a.PriorityDescription,
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 7] = new Cell(a.ListedStepDescription,
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 8] = new Cell(a.IsPhotographed ? "yes" : String.Empty,
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 9] = new Cell(a.IsInLayout ? "yes" : String.Empty,
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 10] = new Cell(a.User.Login,
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 11] = new Cell(a.EnteredByUser.Login,
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 12] = new Cell(a.NotifiedOn.Value.ToString(),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 13] = new Cell(a.LastUpdate.ToString(),
                        new CellFormat(CellFormatType.Text, "").FormatString);

                    Image r = a.Images.FirstOrDefault(i => i.Auction_ID == a.ID);
                    if (r != null)
                    {

                        foreach (var imge in a.Images)
                        {

                            if (imge.isChecked)
                            {

                                switch (imge.Order)
                                {
                                    case 1:
                                        {
                                            worksheet.Cells[row, 14] = new Cell(String.Format("{0}", imge.UploadedFileName),
                                                new CellFormat(CellFormatType.Text, "").FormatString);
                                        }
                                        break;

                                    case 2:
                                        {
                                            worksheet.Cells[row, 15] = new Cell(String.Format("{0}", imge.UploadedFileName),
                                                new CellFormat(CellFormatType.Text, "").FormatString);
                                        }
                                        break;
                                    case 3:
                                        {
                                            worksheet.Cells[row, 16] = new Cell(String.Format("{0}" , imge.UploadedFileName),
                                                new CellFormat(CellFormatType.Text, "").FormatString);
                                        }
                                        break;
                                    case 4:
                                        {
                                            worksheet.Cells[row, 17] = new Cell(String.Format("{0}",   imge.UploadedFileName),
                                                new CellFormat(CellFormatType.Text, "").FormatString);
                                        }
                                        break;
                                    case 5:
                                        {
                                            worksheet.Cells[row, 18] = new Cell(String.Format("{0}",  imge.UploadedFileName),
                                                new CellFormat(CellFormatType.Text, "").FormatString);
                                        }
                                        break;
                                    case 6:
                                        {
                                            worksheet.Cells[row, 19] = new Cell(String.Format("{0}",  imge.UploadedFileName),
                                                new CellFormat(CellFormatType.Text, "").FormatString);
                                        }
                                        break;

                                    default:
                                        break;
                                }

                                //worksheet.Cells[row, 14] = new Cell(String.Format("K:\\{1} auction\\psd\\{0}.psd", a.ID, event_id),
                                //    new CellFormat(CellFormatType.Text, "").FormatString);
                                //worksheet.Cells[row, 15] = new Cell(
                                //    String.Format("K:\\{1} auction\\psd\\{0}a.psd", a.ID, event_id),
                                //    new CellFormat(CellFormatType.Text, "").FormatString);
                                //worksheet.Cells[row, 16] = new Cell(
                                //    String.Format("K:\\{1} auction\\psd\\{0}b.psd", a.ID, event_id),
                                //    new CellFormat(CellFormatType.Text, "").FormatString);
                                //worksheet.Cells[row, 17] = new Cell(
                                //    String.Format("K:\\{1} auction\\psd\\{0}c.psd", a.ID, event_id),
                                //    new CellFormat(CellFormatType.Text, "").FormatString);
                                //worksheet.Cells[row, 18] = new Cell(
                                //    String.Format("K:\\{1} auction\\psd\\{0}d.psd", a.ID, event_id),
                                //    new CellFormat(CellFormatType.Text, "").FormatString);
                                //worksheet.Cells[row, 19] = new Cell(
                                //    String.Format("K:\\{1} auction\\psd\\{0}e.psd", a.ID, event_id),
                                //    new CellFormat(CellFormatType.Text, "").FormatString);
                            }
                        }
                    }
                }
                workbook.Worksheets.Add(worksheet);
                workbook.Save(file);
                files.Add(file);
            }
            return files;
        }

        //Temp_CreateSpreadsheets
        public List<string> Temp_CreateSpreadsheetsID(long event_id)
        {
            string file;
            var files = new List<string>();
            var ec = dataContext.EventCategories.Where(e => e.Event_ID == event_id).ToList();
            ushort row = 0;
            StringBuilder sbA, sbB;
            foreach (var e in ec)
            {
                if (e.Auctions.Count() == 0) continue;
                sbA = new StringBuilder(e.MainCategory.Title);
                sbA.Replace(" ", String.Empty);
                sbA.Replace("\"", "-");
                sbA.Replace("\'", "-");
                sbB = new StringBuilder(e.Category.Title);
                sbB.Replace(" ", String.Empty);
                sbB.Replace("\"", "-");
                sbB.Replace("\'", "-");
                file = HttpContext.Current.Server.MapPath("~/Pool/") +
                       String.Format("{0:yyyyMMdd}_{1}_{2}.xls", DateTime.Now, sbA, sbB);
                var workbook = new Workbook();
                var worksheet = new Worksheet(e.FullCategory);
                row = 0;
                worksheet.Cells.ColumnWidth[0] = 3000;
                worksheet.Cells.ColumnWidth[1] = 3000;
                worksheet.Cells.ColumnWidth[2] = 15000;
                worksheet.Cells.ColumnWidth[3] = 10000;
                worksheet.Cells.ColumnWidth[4] = 3000;
                worksheet.Cells.ColumnWidth[5] = 5000;
                worksheet.Cells.ColumnWidth[6] = 6000;
                worksheet.Cells.ColumnWidth[7] = 6000;
                worksheet.Cells.ColumnWidth[8] = 6000;
                worksheet.Cells.ColumnWidth[9] = 6000;
                worksheet.Cells.ColumnWidth[10] = 6000;
                worksheet.Cells.ColumnWidth[11] = 6000;

                worksheet.Cells[row, 0] = new Cell("Inventory#");
                worksheet.Cells[row, 1] = new Cell("Lot");
                worksheet.Cells[row, 2] = new Cell("Title");
                worksheet.Cells[row, 3] = new Cell("Description");
                worksheet.Cells[row, 4] = new Cell("Reserve");
                worksheet.Cells[row, 5] = new Cell("Page Layout");
                worksheet.Cells[row, 6] = new Cell("@Image");
                worksheet.Cells[row, 7] = new Cell("@Imagea");
                worksheet.Cells[row, 8] = new Cell("@Imageb");
                worksheet.Cells[row, 9] = new Cell("@Imagec");
                worksheet.Cells[row, 10] = new Cell("@Imaged");
                worksheet.Cells[row, 11] = new Cell("@Imagee");
                foreach (
                    var a in
                        e.Auctions.Where(A1 => A1.Status == (byte)Consts.AuctionStatus.Pending)
                            .OrderBy(A2 => A2.Priority)
                            .ThenByDescending(A3 => A3.Price)
                            .ThenBy(A4 => A4.Title))
                {
                    worksheet.Cells[++row, 0] = new Cell(a.ID.ToString(),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 1] = new Cell(a.Lot.HasValue ? a.Lot.Value.ToString() : String.Empty,
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 2] = new Cell(a.Title.Trim(),
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 3] = new Cell(a.Description.Replace("\n", " ").Replace("  ", " ").Trim(),
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 4] = new Cell(a.Reserve.GetCurrency().Replace(".00", ""),
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 5] = new Cell(a.PriorityDescription,
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 6] = new Cell(String.Format("K:\\{1} auction\\psd\\{0}.psd", a.ID, event_id),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 7] = new Cell(String.Format("K:\\{1} auction\\psd\\{0}a.psd", a.ID, event_id),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 8] = new Cell(String.Format("K:\\{1} auction\\psd\\{0}b.psd", a.ID, event_id),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 9] = new Cell(String.Format("K:\\{1} auction\\psd\\{0}c.psd", a.ID, event_id),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 10] = new Cell(
                        String.Format("K:\\{1} auction\\psd\\{0}d.psd", a.ID, event_id),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 11] = new Cell(
                        String.Format("K:\\{1} auction\\psd\\{0}e.psd", a.ID, event_id),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                }
                workbook.Worksheets.Add(worksheet);
                workbook.Save(file);
                files.Add(file);
            }
            return files;
        }

        //Temp_CreateSpreadsheetW
        public string Temp_CreateSpreadsheetW(long event_id)
        {
            var file = HttpContext.Current.Server.MapPath("~/Pool/") +
                       String.Format("{0:yyyyMMdd}_ForSpecialists.xls", DateTime.Now);
            var a =
                dataContext.Auctions.Where(A => A.Event_ID == event_id && A.ListedStep != 3)
                    .OrderBy(A2 => A2.ListedStep)
                    .ThenBy(A3 => A3.ID)
                    .GroupBy(A1 => A1.EnteredByUser.Login);
            var workbook = new Workbook();
            ushort row = 0;
            foreach (var b in a)
            {
                var worksheet = new Worksheet(b.Key);
                row = 0;
                worksheet.Cells[row, 0] = new Cell("Inventory#");
                worksheet.Cells[row, 1] = new Cell("Writing Step");
                worksheet.Cells[row, 2] = new Cell("Seller");
                worksheet.Cells[row, 3] = new Cell("Consignment#");
                worksheet.Cells[row, 4] = new Cell("Title");
                worksheet.Cells[row, 5] = new Cell("Description");
                worksheet.Cells[row, 6] = new Cell("Date In");
                worksheet.Cells.ColumnWidth[0] = 3000;
                worksheet.Cells.ColumnWidth[1] = 5000;
                worksheet.Cells.ColumnWidth[2] = 5000;
                worksheet.Cells.ColumnWidth[3] = 5000;
                worksheet.Cells.ColumnWidth[4] = 10000;
                worksheet.Cells.ColumnWidth[5] = 6000;
                worksheet.Cells.ColumnWidth[6] = 6000;
                foreach (var c in b)
                {
                    worksheet.Cells[++row, 0] = new Cell(c.ID.ToString(),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 1] = new Cell(c.ListedStepDescription,
                        new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 2] = new Cell(c.User.Login,
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 3] =
                        new Cell(
                            c.User.Consignments.Where(C => C.Event_ID == event_id).SingleOrDefault().ID.ToString(),
                            new CellFormat(CellFormatType.Text, "").FormatString);
                    worksheet.Cells[row, 4] = new Cell(c.Title.Trim(),
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 5] = new Cell(c.Description.Trim(),
                        new CellFormat(CellFormatType.Text, "text").FormatString);
                    worksheet.Cells[row, 6] = new Cell(c.NotifiedOn.Value.ToString(),
                        new CellFormat(CellFormatType.Text, "").FormatString);
                }
                workbook.Worksheets.Add(worksheet);
            }
            workbook.Save(file);
            return file;
        }

        //SendUserInvoices
        public void Temp_SendUserInvoices()
        {
            var ui = dataContext.UserInvoices.Where(UI => UI.Event_ID == 911).OrderBy(UI2 => UI2.ID).ToList();
            foreach (var _ui in ui)
            {
                try
                {
                    Mail.Temp_SendInvoiceInformationLetter(_ui.User.AddressCard_Billing.FirstName,
                        _ui.User.AddressCard_Billing.LastName, _ui.User.Email, _ui.ID.ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogInfo("Faild sending email to UI_ID: " + _ui.ID.ToString() + ". Error:" + ex.Message);
                }
            }
        }


        public int SendInvoices(long event_id)
        {
            var ui = dataContext.UserInvoices.Where(UI => UI.Event_ID == event_id).OrderBy(UI2 => UI2.ID).ToList();
            var evnt = dataContext.Events.FirstOrDefault(E => E.ID == event_id);
            var auctionends = evnt.DateEnd.ToString("MMMM dd");
            var eventname = evnt.Title.Replace(" ", "-");
            var count = 0;
            foreach (var _ui in ui)
            {
                try
                {
                    Mail.Temp_SendInvoiceInformationLetter2(_ui.User.AddressCard_Billing.FirstName,
                        _ui.User.AddressCard_Billing.LastName, _ui.User.Email, auctionends, _ui.ID.ToString(),
                        _ui.SaleDate.ToShortDateString(), eventname, _ui.Invoices.OrderBy(A => A.Auction.Lot).ToList(),
                        0);
                    count++;
                }
                catch (Exception ex)
                {
                    Logger.LogInfo("Faild sending email to UI_ID: " + _ui.ID + ". Error:" + ex.Message);
                }
            }
            return count;
        }

        public int SendConsignments(long event_id)
        {
            var consignments = (from C in dataContext.Consignments
                                join A in dataContext.Auctions on C.User_ID equals A.Owner_ID
                                where A.Event_ID == C.Event_ID && A.Event_ID == event_id
                                select C).Distinct().ToList();
            var evnt = dataContext.Events.FirstOrDefault(E => E.ID == event_id);
            var auctionends = evnt.DateEnd.ToString("MMMM dd");
            var eventname = evnt.Title.Replace(" ", "-");
            var count = 0;
            foreach (var consignment in consignments)
            {
                try
                {
                    Mail.Temp_SendConsignmentInformationLetter(consignment.User.AddressCard_Billing.FirstName,
                        consignment.User.AddressCard_Billing.LastName, consignment.User.Email, consignment.ID.ToString(),
                        eventname, auctionends);
                    count++;
                }
                catch (Exception ex)
                {
                    Logger.LogInfo("Faild sending email to Consignment_ID: " + consignment.ID.ToString() + ". Error:" +
                                   ex.Message);
                }
            }
            return count;
        }

        //GetTags
        public IEnumerable<Tag> GetTags()
        {
            return dataContext.Tags;
        }

        //UpdateTag
        public Tag UpdateTag(Tag tg)
        {
            var tag = dataContext.Tags.FirstOrDefault(t => t.ID == tg.ID);
            if (tag == null)
            {
                tag = new Tag();
                dataContext.Tags.InsertOnSubmit(tag);
            }
            tag.UpdateFields(tg.Title, tag.IsSystem, tg.IsViewable);
            SubmitChanges(dataContext);
            return tag;
        }

        //DeleteTag
        public void DeleteTag(long tagID)
        {
            var tag = dataContext.Tags.FirstOrDefault(t => t.ID == tagID);
            if (tag == null) return;
            if (tag.IsSystem) throw new Exception("You can't delete system tag.");
            dataContext.Tags.DeleteOnSubmit(tag);
            SubmitChanges(dataContext);
        }

        //GetCollections
        public IEnumerable<Collection> GetCollections()
        {
            return dataContext.Collections;
        }

        //UpdateCollection
        public Collection UpdateCollection(Collection c)
        {
            var collection = dataContext.Collections.FirstOrDefault(t => t.ID == c.ID);
            if (collection == null)
            {
                collection = new Collection();
                dataContext.Collections.InsertOnSubmit(collection);
            }
            collection.UpdateFields(c.Title, c.WebTitle, c.Description);
            SubmitChanges(dataContext);
            return collection;
        }

        //DeleteCollection
        public void DeleteCollection(long collectionID)
        {
            var collection = dataContext.Collections.FirstOrDefault(t => t.ID == collectionID);
            if (collection == null) return;
            dataContext.Collections.DeleteOnSubmit(collection);
            SubmitChanges(dataContext);
        }

        public static bool SubmitChanges(VauctionDataContext dataContext)
        {
            var res = true;
            try
            {
                dataContext.SubmitChanges();
            }
            catch (ChangeConflictException e)
            {
                Logger.LogWarning(e.Message);
                try
                {
                    foreach (var occ in dataContext.ChangeConflicts)
                    {
                        occ.Resolve(RefreshMode.KeepCurrentValues);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    res = false;
                }
            }
            return res;
        }

        private void DeleteDifferentImage(FileInfo fi)
        {
            if (fi.Exists) fi.Delete();
            fi = new FileInfo(Path.Combine(fi.DirectoryName, "thmb_" + fi.Name));
            if (fi.Exists) fi.Delete();
        }

        private Dictionary<string, List<string>> GetVauctionRegions()
        {
            var regions = new Dictionary<string, List<string>>();
            var reg = new List<string>() { "COUNTRIES", "STATES", "CATEGORIES", "HOTNEWS", "EVENTS" };
            regions.Add(DataCacheType.REFERENCE.ToString(), reg);
            reg = new List<string>() { "EVENTS", "AUCTIONS", "IMAGES", "AUCTIONLISTS" };
            regions.Add(DataCacheType.RESOURCE.ToString(), reg);
            reg = new List<string>() { "USERS", "WATCHLISTS", "EVENTREGISTRATIONS", "BIDS", "INVOICES" };
            regions.Add(DataCacheType.ACTIVITY.ToString(), reg);
            return regions;
        }

        #region for select

        //GetPaymentMethodsForSelect
        public string GetPaymentMethodsForSelect()
        {
            var sb = new StringBuilder();
            foreach (var p in dataContext.PaymentTypes.OrderBy(PT => PT.ID).ToList())
                sb.AppendFormat("{0}:{1};", p.ID, p.Title);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        #endregion

        #region for search

        //GetEventsListForSearchSelect
        public string GetEventsListForSearchSelect()
        {
            var sb = new StringBuilder();
            sb.Append(":;");
            foreach (var p in dataContext.Events.OrderByDescending(E => E.ID))
                sb.AppendFormat("{0}:{1};", p.ID, p.Title);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        //GetAuctionPriorityForSearchSelect
        public string GetAuctionPriorityForSearchSelect()
        {
            var sb = new StringBuilder();
            sb.Append(":;");
            sb.AppendFormat("{0}:{1};", 1, "Full Page (FP)");
            sb.AppendFormat("{0}:{1};", 2, "Half Page (HP)");
            sb.AppendFormat("{0}:{1};", 3, "Quarter Page (QP)");
            sb.AppendFormat("{0}:{1};", 4, "Standart Size (SD)");
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        #endregion

        #region get json

        //GetUserTypesJSON
        public object GetUserTypesJSON()
        {
            var jsonData = new
            {
                rows = (
                    from UT in dataContext.UserTypes
                    orderby UT.Title ascending
                    select new
                    {
                        val = UT.ID,
                        txt = UT.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetUserStatusesJSON
        public object GetUserStatusesJSON()
        {
            var jsonData = new
            {
                rows = (
                    from UT in dataContext.UserStatus
                    orderby UT.Title ascending
                    select new
                    {
                        val = UT.ID,
                        txt = UT.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetCommissionRatesJSON
        public object GetCommissionRatesJSON()
        {
            var jsonData = new
            {
                rows = (
                    from UT in dataContext.CommissionRates
                    orderby UT.MaxPercent ascending
                    select new
                    {
                        val = UT.RateID,
                        txt = UT.Description
                    }).ToArray()
            };
            return jsonData;
        }

        //GetCountryStatesJSON
        public object GetCountryStatesJSON()
        {
            var jsonData = new
            {
                rows = (
                    from UT in dataContext.States
                    orderby UT.Country_ID ascending, UT.Code ascending
                    select new
                    {
                        val = UT.ID,
                        txt = UT.Code,
                        gr = (UT.Country_ID > 0) ? UT.Country.Title : "Int. State"
                    }).ToArray()
            };
            return jsonData;
        }

        //GetCountryStatesJSON
        public object GetCountriesJSON()
        {
            var jsonData = new
            {
                rows = (
                    from UT in dataContext.Countries
                    orderby UT.Title
                    select new
                    {
                        val = UT.ID,
                        txt = UT.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetSpecialistsJSON
        public object GetSpecialistsJSON()
        {
            var jsonData = new
            {
                rows = (
                    from S in dataContext.Specialists
                    where S.IsActive
                    orderby S.FirstName + " " + S.LastName ascending
                    select new
                    {
                        val = S.ID,
                        txt = S.FullName
                    }).ToArray()
            };
            return jsonData;
        }

        //GetEventsJSON
        public object GetEventsJSON()
        {
            var jsonData = new
            {
                rows = (
                    from E in dataContext.Events
                    orderby E.ID descending
                    select new
                    {
                        val = E.ID,
                        txt = E.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetEventsViewableJSON
        public object GetEventsViewableJSON()
        {
            var jsonData = new
            {
                rows = (
                    from E in dataContext.Events
                    where E.IsViewable
                    orderby E.ID descending
                    select new
                    {
                        val = E.ID,
                        txt = E.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetPendingEventsJSON
        public object GetPendingEventsJSON()
        {
            var jsonData = new
            {
                rows = (
                    from E in dataContext.Events
                    where E.DateStart.CompareTo(DateTime.Now) >= 0 && E.CloseStep == 0
                    orderby E.DateStart ascending
                    select new
                    {
                        val = E.ID,
                        txt = E.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetAuctionStatusesJSON
        public object GetAuctionStatusesJSON()
        {
            var jsonData = new
            {
                rows = (
                    from AS in dataContext.AuctionStatus
                    orderby AS.ID
                    select new
                    {
                        val = AS.ID,
                        txt = AS.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetMainCategoriesJSON
        public object GetMainCategoriesJSON()
        {
            var jsonData = new
            {
                rows = (
                    from AS in dataContext.MainCategories
                    where AS.IsActive
                    orderby AS.Priority ascending, AS.Title ascending
                    select new
                    {
                        val = AS.ID,
                        txt = AS.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetMainCategoriesAndForSaleJSON
        public object GetMainCategoriesAndForSaleJSON()
        {
            var jsonData = new
            {
                rows = (
                    from AS in dataContext.MainCategories
                    where AS.IsActive || AS.ID == 0
                    orderby AS.Priority ascending, AS.Title ascending
                    select new
                    {
                        val = AS.ID,
                        txt = AS.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetEventsDateTimeJSON
        public object GetEventsDateTimeJSON()
        {
            var jsonData = new
            {
                rows = (
                    from E in dataContext.Events
                    orderby E.ID descending
                    select new
                    {
                        val = E.ID,
                        txt = E.Title + "&nbsp;&nbsp;&nbsp;" + E.StartEndTime
                    }).ToArray()
            };
            return jsonData;
        }

        //GetEventsClosedDateTimeJSON
        public object GetEventsClosedDateTimeJSON()
        {
            return new
            {
                rows = (
                    from E in dataContext.Events.ToList()
                    where E.CloseStep == 2
                    orderby E.DateEnd descending
                    select new
                    {
                        val = E.ID,
                        txt =
                            E.Title + "&nbsp;&nbsp;&nbsp;" +
                            String.Format("({0}-{1})", E.DateStart.ToShortDateString(), E.DateEnd.ToShortDateString())
                    }).ToArray()
            };
        }

        //GetEventsClosedDateTimeJSON
        public object GetEventsClosedDateTimeShortJSON()
        {
            var jsonData = new
            {
                rows = (
                    from E in dataContext.Events
                    where E.CloseStep == 2
                    orderby E.DateEnd descending
                    select new
                    {
                        val = E.ID,
                        txt = E.Title
                    }).ToArray()
            };
            return jsonData;
        }

        //GetPurchasedTypesJSON
        public object GetPurchasedTypesJSON(bool isNUll)
        {
            var result = dataContext.PurchasedTypes.OrderBy(a => a.Name).ToList();
            if (isNUll) result.Insert(0, new PurchasedType { ID = -1, Name = "" });
            return new
            {
                rows = (
                    from r in result
                    select new
                    {
                        val = r.ID,
                        txt = r.Name
                    }).ToArray()
            };
        }

        #endregion
    }
}