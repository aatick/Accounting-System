using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using AccountingSystem.Models.ViewModel;
using Microsoft.Ajax.Utilities;

namespace AccountingSystem.Models
{
    public class AccountingDbGateway
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["AccountingDbConnectionString"].ConnectionString;
        private SqlConnection aSqlConnection = new SqlConnection(connectionString);
        public string SqlQuery { private set; get; }
        public SqlCommand SqlCommand { private set; get; }
        public SqlDataReader SqlDataReader { private set; get; }

        private void InsertUpdateDelete(string query)
        {
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(query, aSqlConnection);
            SqlCommand.ExecuteNonQuery();
            aSqlConnection.Close();
        }
        public string GetOnlineConnectionString()
        {
            SqlQuery = "SELECT ConnStr FROM OnlineConn";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            SqlDataReader.Read();
            var con = SqlDataReader[0].ToString();
            aSqlConnection.Close();
            return con;
        }
        public List<User> GetAllUsers()
        {
            SqlQuery = "SELECT * FROM dbo.Users";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var users = new List<User>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var anUser = new User
                    {
                        UserId = Convert.ToInt32(SqlDataReader["UserID"]),
                        Name = SqlDataReader["Name"].ToString(),
                        Designation = SqlDataReader["Designation"].ToString(),
                        UserName = SqlDataReader["UName"].ToString(),
                        Password = SqlDataReader["PWord"].ToString(),
                        AccessRight = SqlDataReader["AccessRight"].ToString(),
                        CanApprove = Convert.ToBoolean(SqlDataReader["CanApprove"]),
                        ApproveRight = SqlDataReader["ApproveRight"].ToString(),
                        CanModifyAdmin = Convert.ToBoolean(SqlDataReader["CanModifyAdmin"]),
                        AccessReports = SqlDataReader["AccessReports"].ToString(),
                        ValidUser = Convert.ToBoolean(SqlDataReader["ValidUser"]),
                        AccountDep = Convert.ToBoolean(SqlDataReader["AccountDep"])
                    };
                    users.Add(anUser);
                }
            }
            aSqlConnection.Close();
            return users;
        }

        public List<User> GetApprovers()
        {
            SqlQuery = "Select UserId as id,name from Users where CanApprove=1 order by name";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var users = new List<User>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var anUser = new User
                    {
                        UserId = Convert.ToInt32(SqlDataReader[0]),
                        Name = SqlDataReader[1].ToString()
                    };
                    users.Add(anUser);
                }
            }
            aSqlConnection.Close();
            return users;
        }

        public List<User> GetSpecificusers()
        {
            SqlQuery = "Select UserId as id,name from Users where CanApprove=0 and AccessRight like '%1%' order by name;";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var users = new List<User>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var anUser = new User
                    {
                        UserId = Convert.ToInt32(SqlDataReader[0]),
                        Name = SqlDataReader[1].ToString()
                    };
                    users.Add(anUser);
                }
            }
            aSqlConnection.Close();
            return users;
        }

        public List<Ledger> GetAllLedger(string isAdmin, string isAccount)
        {
            SqlQuery = "USP_LedgerList '" + isAccount + "', '" + isAdmin + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var ledgers = new List<Ledger>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var aLedger = new Ledger
                    {
                        Id = Convert.ToInt32(SqlDataReader[0]),
                        GroupName = SqlDataReader[1].ToString()
                    };
                    ledgers.Add(aLedger);
                }
            }
            aSqlConnection.Close();
            return ledgers;
        }
        public List<Ledger> GetAllLedger()
        {
            SqlQuery = "SELECT * FROM dbo.Ledger";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var ledgers = new List<Ledger>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var aLedger = new Ledger
                    {
                        Id = Convert.ToInt32(SqlDataReader["Id"]),
                        GroupName = SqlDataReader["SBName"].ToString(),
                        Under = SqlDataReader["Under"].ToString(),
                        MaingroupName = SqlDataReader["MGroup"].ToString(),
                        LevelNo = Convert.ToInt32(SqlDataReader["LevelNo"]),
                        OpeningBalance = Convert.ToDouble(SqlDataReader["OpeningBalance"]),
                        IsLedgerAccount = Convert.ToBoolean(SqlDataReader["LedgerAcc"]),
                        Balance = Convert.ToDouble(SqlDataReader["Balance"]),
                        Account = SqlDataReader["Account"].ToString()
                    };

                    var date = SqlDataReader["OpeningDate"].ToString();
                    try
                    {
                        aLedger.OpeningDate = Convert.ToDateTime(date);
                    }
                    catch (Exception)
                    {
                        aLedger.OpeningDate = DateTime.Today;
                    }

                    ledgers.Add(aLedger);
                }
            }
            aSqlConnection.Close();
            return ledgers;
        }

        public void ActivateDeactivateUser(string id, string valid)
        {
            SqlQuery = "UPDATE users SET ValidUser='" + valid + "' WHERE UserID='" + id + "';";
            InsertUpdateDelete(SqlQuery);
        }


        public void InsertUpdateUser(string action, string name, int id, string username, string password, string designation,
            string admin, string account, string access, string approve, string accessright)
        {
            if (action == "update")
            {
                SqlQuery = "UPDATE users SET name='" + name + "',Designation='" + designation + "',uname='" + username +
                           "',pword='" + password + "',AccessRight='" + access + "',ApproveRight='" + approve +
                           "',CanModifyAdmin='" + admin + "',AccessReports='" + accessright + "', AccountDep='" +
                           account +
                           "' where UserID='" + id + "';";
            }
            else
            {
                SqlQuery =
                    "INSERT INTO users(UserID,name,Designation,uname,pword,AccessRight,ApproveRight,CanModifyAdmin,AccessReports,ValidUser,AccountDep) values(" +
                    id + ",'" + name + "','" + designation + "','" + username + "','" + password + "','" + access +
                    "','" + approve + "','" + admin + "','" + accessright + "',1,'" + account + "');";
            }
            InsertUpdateDelete(SqlQuery);
        }

        public void ChangeUserPassword(string id, string username, string newpassword)
        {
            SqlQuery = "UPDATE users SET pword='" + newpassword + "' WHERE UserID='" + id + "' AND uname='" + username +
                       "';";
            InsertUpdateDelete(SqlQuery);
        }

        public void SaveLedger(Ledger aLedger)
        {
            SqlQuery = "INSERT INTO dbo.Ledger(SBName,Under,MGroup,LevelNo,LedgerAcc) VALUES('" + aLedger.GroupName + "','" + aLedger.Under + "','" + aLedger.MaingroupName + "'," + aLedger.LevelNo + ",'" + aLedger.IsLedgerAccount + "')";
            InsertUpdateDelete(SqlQuery);
        }

        public void UpdateLedger(Ledger aLedger)
        {
            SqlQuery = "UPDATE dbo.Ledger SET SBName='" + aLedger.GroupName + "',Under='" + aLedger.Under + "',MGroup='" +
                       aLedger.MaingroupName + "',LevelNo=" + aLedger.LevelNo + ",LedgerAcc='" + aLedger.IsLedgerAccount +
                       "' WHERE Id=" + aLedger.Id;
            InsertUpdateDelete(SqlQuery);
        }

        public void DeleteLedger(int ledgerId)
        {
            SqlQuery = "DELETE FROM dbo.Ledger WHERE Id=" + ledgerId;
            InsertUpdateDelete(SqlQuery);
        }

        public Journal GetJournalBySId(int sId)
        {
            SqlQuery = "SELECT * FROM dbo.Journal WHERE Sid=" + sId;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            if (SqlDataReader.HasRows)
            {
                var aJournal = new Journal();
                while (SqlDataReader.Read())
                {
                    aJournal.Id = Convert.ToInt32(SqlDataReader["Id"]);
                    aJournal.JId = Convert.ToInt32(SqlDataReader["Jid"]);
                    aJournal.SId = Convert.ToInt32(SqlDataReader["Sid"]);
                    aJournal.Description = SqlDataReader["Description"].ToString();
                    aJournal.Debt = Convert.ToDouble(SqlDataReader["Debt"]);
                    aJournal.Credit = Convert.ToDouble(SqlDataReader["Credit"]);
                    aJournal.AccType = SqlDataReader["AccType"].ToString();
                    aJournal.Tno = Convert.ToInt32(SqlDataReader["Tno"]);
                    aJournal.Notify = SqlDataReader["Notify"].ToString();
                    aJournal.Lock = Convert.ToBoolean(SqlDataReader["Lock"]);
                    aJournal.UserId = Convert.ToInt32(SqlDataReader["UserID"]);
                    aJournal.ApprovedBy = Convert.ToInt32(SqlDataReader["ApprovedBy"]);
                    aJournal.UpdatedBy = Convert.ToInt32(SqlDataReader["UpdatedBy"]);
                    try
                    {
                        aJournal.PostDate = Convert.ToDateTime(SqlDataReader["PostDate"]);
                        aJournal.JDate = Convert.ToDateTime(SqlDataReader["JDate"]);
                        aJournal.ApprovalDate = Convert.ToDateTime(SqlDataReader["ApprovalDate"]);
                        aJournal.UpdatedDate = Convert.ToDateTime(SqlDataReader["UpdatedDate"]);
                    }
                    catch (Exception) { }

                }
                aSqlConnection.Close();
                return aJournal;
            }
            aSqlConnection.Close();
            return null;
        }

        public Company GetCompanyByName(string name)
        {
            SqlQuery = "SELECT * FROM dbo.Company WHERE Name='" + name + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            if (SqlDataReader.HasRows)
            {
                var aCompany = new Company();
                while (SqlDataReader.Read())
                {
                    aCompany.Id = Convert.ToInt32(SqlDataReader["Id"]);
                    aCompany.Name = SqlDataReader["Name"].ToString();
                    aCompany.Address = SqlDataReader["Address"].ToString();
                    aCompany.City = SqlDataReader["City"].ToString();
                    aCompany.Phone = SqlDataReader["Phone"].ToString();
                    aCompany.Email = SqlDataReader["Email"].ToString();
                    aCompany.Fax = SqlDataReader["Fax"].ToString();
                    aCompany.ContactPerson = SqlDataReader["Contact_Person"].ToString();
                    aCompany.Designation = SqlDataReader["Designation"].ToString();
                    aCompany.Balance = Convert.ToDouble(SqlDataReader["Balance"]);
                    aCompany.BlackListed = Convert.ToBoolean(SqlDataReader["BlackListed"]);
                    aCompany.CP_Id = Convert.ToInt32(SqlDataReader["CP_ID"]);
                    aCompany.AccContactName = SqlDataReader["AccContactName"].ToString();
                    aCompany.VatRegNo = SqlDataReader["VATRegNo"].ToString();
                    aCompany.VatRegAdd = SqlDataReader["VATRegAdd"].ToString();
                }
                aSqlConnection.Close();
                return aCompany;
            }
            aSqlConnection.Close();
            return null;
        }

        public Company GetCompanyById(int id)
        {
            SqlQuery = "SELECT * FROM dbo.Company WHERE Id='" + id + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            if (SqlDataReader.HasRows)
            {
                var aCompany = new Company();
                while (SqlDataReader.Read())
                {
                    aCompany.Id = Convert.ToInt32(SqlDataReader["Id"]);
                    aCompany.Name = SqlDataReader["Name"].ToString();
                    aCompany.Address = SqlDataReader["Address"].ToString();
                    aCompany.City = SqlDataReader["City"].ToString();
                    aCompany.Phone = SqlDataReader["Phone"].ToString();
                    aCompany.Email = SqlDataReader["Email"].ToString();
                    aCompany.Fax = SqlDataReader["Fax"].ToString();
                    aCompany.ContactPerson = SqlDataReader["Contact_Person"].ToString();
                    aCompany.Designation = SqlDataReader["Designation"].ToString();
                    aCompany.Balance = Convert.ToDouble(SqlDataReader["Balance"]);
                    aCompany.BlackListed = Convert.ToBoolean(SqlDataReader["BlackListed"]);
                    aCompany.CP_Id = Convert.ToInt32(SqlDataReader["CP_ID"]);
                    aCompany.AccContactName = SqlDataReader["AccContactName"].ToString();
                    aCompany.VatRegNo = SqlDataReader["VATRegNo"].ToString();
                    aCompany.VatRegAdd = SqlDataReader["VATRegAdd"].ToString();
                }
                aSqlConnection.Close();
                return aCompany;
            }
            aSqlConnection.Close();
            return null;
        }

        public List<ContactPerson> GetContactPersonsByCompanyId(int companyId)
        {
            SqlQuery = "SELECT * FROM dbo.ContactPersons WHERE CID='" + companyId + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var contactPersons = new List<ContactPerson>();
            while (SqlDataReader.Read())
            {
                var aPerson = new ContactPerson
                {
                    Id = Convert.ToInt32(SqlDataReader["Id"]),
                    CId = Convert.ToInt32(SqlDataReader["CID"]),
                    Name = SqlDataReader["Name"].ToString(),
                    Designation = SqlDataReader["Designation"].ToString(),
                    PType = SqlDataReader["PType"].ToString()
                };
                contactPersons.Add(aPerson);
            }
            aSqlConnection.Close();
            return contactPersons;
        }

        public List<Company> GetCompanyList()
        {
            SqlQuery = "USP_CompanyList";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection) { CommandType = CommandType.StoredProcedure };
            SqlDataReader = SqlCommand.ExecuteReader();
            var companies = new List<Company>();
            while (SqlDataReader.Read())
            {
                var aCompany = new Company
                {
                    Id = Convert.ToInt32(SqlDataReader["id"]),
                    Name = SqlDataReader["Name"].ToString(),
                    BlackListed = Convert.ToBoolean(SqlDataReader["BlackListed"])
                };
                companies.Add(aCompany);
            }
            aSqlConnection.Close();
            return companies;
        }

        public ContactPerson GetContactPersonById(int id)
        {
            SqlQuery = "SELECT * FROM dbo.ContactPersons WHERE Id='" + id + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var aPerson = new ContactPerson();
            while (SqlDataReader.Read())
            {
                aPerson.Id = Convert.ToInt32(SqlDataReader["Id"]);
                aPerson.CId = Convert.ToInt32(SqlDataReader["CID"]);
                aPerson.Name = SqlDataReader["Name"].ToString();
                aPerson.Designation = SqlDataReader["Designation"].ToString();
                aPerson.PType = SqlDataReader["PType"].ToString();
            }
            aSqlConnection.Close();
            return aPerson;
        }

        public void InsertOrUpdateCompany(Company aCompany)
        {
            SqlQuery = "INSERT_UPDATE_COMPANY";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlCommand.CommandType = CommandType.StoredProcedure;
            SqlCommand.Parameters.AddWithValue("@Name", aCompany.Name);
            SqlCommand.Parameters.AddWithValue("@Address", aCompany.Address);
            SqlCommand.Parameters.AddWithValue("@City", aCompany.City);
            SqlCommand.Parameters.AddWithValue("@Phone", aCompany.Phone);
            SqlCommand.Parameters.AddWithValue("@Email", aCompany.Email);
            SqlCommand.Parameters.AddWithValue("@Fax", aCompany.Fax);
            SqlCommand.Parameters.AddWithValue("@Contact_Person", aCompany.ContactPerson);
            SqlCommand.Parameters.AddWithValue("@Designation", aCompany.Designation);
            SqlCommand.Parameters.AddWithValue("@BlackListed", aCompany.BlackListed);
            SqlCommand.Parameters.AddWithValue("@CP_ID", aCompany.CP_Id);
            SqlCommand.Parameters.AddWithValue("@AccContactName", aCompany.AccContactName);
            SqlCommand.Parameters.AddWithValue("@VATRegNo", aCompany.VatRegNo);
            SqlCommand.Parameters.AddWithValue("@VATRegAdd", aCompany.VatRegAdd);
            SqlCommand.Parameters.AddWithValue("@Id", aCompany.Id);
            SqlCommand.ExecuteNonQuery();
            aSqlConnection.Close();
        }

        public void DeleteCompany(int id)
        {
            SqlQuery = "DELETE FROM dbo.Company WHERE Id='" + id + "';DELETE FROM dbo.ContactPersons WHERE CID='" + id + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public void AddPerson(ContactPerson aContactPerson)
        {
            SqlQuery = "INSERT INTO dbo.ContactPersons VALUES('" + aContactPerson.CId + "','" + aContactPerson.Name + "','" + aContactPerson.Designation + "','" + aContactPerson.PType + "');";
            InsertUpdateDelete(SqlQuery);
        }
        public void UpdatePerson(ContactPerson aContactPerson)
        {
            SqlQuery = "UPDATE dbo.ContactPersons SET Name='" + aContactPerson.Name + "', Designation='" + aContactPerson.Designation + "' WHERE Id='" + aContactPerson.Id + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public void DeletePerson(int id)
        {
            SqlQuery = "DELETE FROM dbo.ContactPersons WHERE Id='" + id + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public List<ScheduledInvoice> GetScheduledInvoicesByCompanyId(int companyId, char showAs, string date)
        {
            SqlQuery = "USP_DUES_INVOICE '" + showAs + "'," + companyId + ",'" + date + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection) { CommandType = CommandType.Text };
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<ScheduledInvoice>();
            while (SqlDataReader.Read())
            {
                var invoice = new ScheduledInvoice();
                invoice.Id = Convert.ToInt32(SqlDataReader["id"]);
                try
                {
                    invoice.Name = SqlDataReader["name"].ToString();
                }
                catch { }
                invoice.CId = Convert.ToInt32(SqlDataReader["cid"]);
                invoice.SbName = SqlDataReader["sbname"].ToString();
                invoice.SalesPrice = Convert.ToDouble(SqlDataReader["salesprice"]);
                invoice.InvshdlNo = Convert.ToInt32(SqlDataReader["invshdlno"]);
                invoice.ScheduleDate = Convert.ToDateTime(SqlDataReader["shdldate"]).ToShortDateString();
                invoice.Amount = Convert.ToDouble(SqlDataReader["amount"]);
                invoices.Add(invoice);
            }
            aSqlConnection.Close();
            return invoices;
        }

        public void DeleteSchedule(int invoiceId)
        {
            SqlQuery = "DELETE FROM dbo.invoiceSceduler WHERE Id='" + invoiceId + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public List<Ledger> GetProducts(int admin, int account, string groupname, string isAll, string isI, int isVatType)
        {
            SqlQuery = "USP_LedgerList " + admin + "," + account + ",'" + groupname + "','" + isAll + "','" + isI + "'," + isVatType;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var ledgers = new List<Ledger>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {

                    var aLedger = new Ledger();
                    if (isVatType == 0)
                    {
                        aLedger.Id = Convert.ToInt32(SqlDataReader["ID"]);
                        aLedger.GroupName = SqlDataReader["LadgerName"].ToString();
                    }
                    else
                    {
                        aLedger.Id = Convert.ToInt32(SqlDataReader["id"]);
                        aLedger.GroupName = SqlDataReader["SBName"].ToString();
                    }

                    ledgers.Add(aLedger);
                }
            }
            aSqlConnection.Close();
            return ledgers;
        }

        public List<Invoice> GetInvoices(int pageNo, int pageSize, int productId, int validity, string Operator,
            int fDuration, int tDuration, int fullPayment, int blacklisted, string order)
        {
            SqlQuery = "USP_LIST_OF_INVOICE " + pageNo + "," + pageSize + ",'" + productId + "','" + validity + "','" + Operator + "'" +
                       ",'" + fDuration + "','" + tDuration + "','" + fullPayment + "','" + blacklisted + "','" + order + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<Invoice>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var invoice = new Invoice
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        InvoiceNo = SqlDataReader["invoice_no"].ToString(),
                        CompanyName = SqlDataReader["name"].ToString(),
                        TAmount = Convert.ToDouble(SqlDataReader["tamount"]),
                        InvSendDt = Convert.ToDateTime(SqlDataReader["invsendDt"]).ToShortDateString(),
                        Sent = SqlDataReader["sent"].ToString(),
                        SendMode = SqlDataReader["SendMode"].ToString(),
                        Emailed = Convert.ToInt32(SqlDataReader["Emailed"]),
                        FullPayment = SqlDataReader["FullPayment"].ToString(),
                        TotalInvoices = Convert.ToInt64(SqlDataReader["TotalInvoice"])
                    };

                    invoices.Add(invoice);
                }
            }
            aSqlConnection.Close();
            return invoices;
        }

        public void DeleteUndeleteInvoice(int invoiceId, bool invalid)
        {
            SqlQuery = "UPDATE dbo.InvoiceList SET Invalid='" + invalid + "' WHERE Id='" + invoiceId + "'";
            InsertUpdateDelete(SqlQuery);
        }

        public List<InvoiceListReport> GetInvoiceListReport(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoiceList = new List<InvoiceListReport>();
            while (SqlDataReader.Read())
            {
                var invoice = new InvoiceListReport
                {
                    id = Convert.ToInt32(SqlDataReader["id"]),
                    ContactPerson = SqlDataReader["cp"].ToString(),
                    AccContactName = SqlDataReader["AccContactName"].ToString(),
                    invoice_no = SqlDataReader["invoice_no"].ToString(),
                    invsendDt = Convert.ToDateTime(SqlDataReader["invsendDt"]),
                    CompanyName = SqlDataReader["name"].ToString(),
                    phone = SqlDataReader["phone"].ToString(),
                    tamount = Convert.ToDouble(SqlDataReader["tamount"])
                };
                invoiceList.Add(invoice);
            }
            aSqlConnection.Close();
            return invoiceList;
        }

        public List<InvoiceReport> GetInvoiceReport(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<InvoiceReport>();
            while (SqlDataReader.Read())
            {
                var invoice = new InvoiceReport();
                invoice.Invoice_No = SqlDataReader["Invoice_No"].ToString();
                invoice.CName = SqlDataReader["CName"].ToString();
                invoice.InvSendDt = Convert.ToDateTime(SqlDataReader["InvSendDt"]);
                invoice.RefNo = SqlDataReader["RefNo"].ToString();
                invoice.VATRegNo = SqlDataReader["VATRegNo"].ToString();
                invoice.amount = Convert.ToDouble(SqlDataReader["amount"]);
                invoice.bname = SqlDataReader["bname"].ToString();
                invoice.comments = SqlDataReader["comments"].ToString();
                invoice.designation = SqlDataReader["designation"].ToString();
                invoice.sbname = SqlDataReader["sbname"].ToString();
                invoices.Add(invoice);
            }
            aSqlConnection.Close();
            return invoices;
        }

        public List<Invoice> GetInvoices(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<Invoice>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var invoice = new Invoice
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        InvoiceNo = SqlDataReader["ItemName"].ToString(),
                        TotalInvoices = Convert.ToInt64(SqlDataReader["NoOfRow"])
                    };
                    invoices.Add(invoice);
                }
            }
            aSqlConnection.Close();
            return invoices;
        }

        public List<LabelReport> GetLabelReport(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var labels = new List<LabelReport>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var aLabel = new LabelReport
                    {
                        Id = Convert.ToInt32(SqlDataReader["Id"]),
                        Addr = SqlDataReader["address"].ToString(),
                        City = SqlDataReader["city"].ToString(),
                        Des = SqlDataReader["designation"].ToString(),
                        ContactP = SqlDataReader["pname"].ToString(),
                        CompName = SqlDataReader["cname"].ToString()

                    };
                    labels.Add(aLabel);
                }
            }
            aSqlConnection.Close();
            return labels;
        }

        public List<Invoice> GetInvoicesForRemarks(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<Invoice>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var invoice = new Invoice
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        InvoiceNo = SqlDataReader["Invoice_No"].ToString(),
                        TAmount = Convert.ToInt64(SqlDataReader["TAmount"])
                    };
                    invoices.Add(invoice);
                }
            }
            aSqlConnection.Close();
            return invoices;
        }

        public List<Remark> GetRemarksForInvoice(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var remarks = new List<Remark>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var remark = new Remark
                    {
                        Id = Convert.ToInt32(SqlDataReader["ID"]),
                        RemarkDate = Convert.ToDateTime(SqlDataReader["RemarkDate"]).ToShortDateString(),
                        InvoiceId = Convert.ToInt32(SqlDataReader["InvoiceId"]),
                        Remarks = SqlDataReader["Remarks"].ToString()
                    };
                    remarks.Add(remark);
                }
            }
            aSqlConnection.Close();
            return remarks;
        }

        public void InsertUpdateDeleteRemark(string query)
        {
            SqlQuery = query;
            InsertUpdateDelete(SqlQuery);
        }

        public List<OutstandingInvoiceReport> GetOutstandingInvoiceReport(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<OutstandingInvoiceReport>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var invoice = new OutstandingInvoiceReport
                    {
                        id = Convert.ToInt32(SqlDataReader["id"]),
                        name = SqlDataReader["name"].ToString(),
                        Amount = Convert.ToDouble(SqlDataReader["Amount"]),
                        Email = SqlDataReader["Email"].ToString(),
                        address = SqlDataReader["address"].ToString(),
                        city = SqlDataReader["city"].ToString(),
                        sbname = SqlDataReader["sbname"].ToString(),
                        invoice_no = SqlDataReader["invoice_no"].ToString(),
                        phone = SqlDataReader["phone"].ToString()
                    };
                    invoices.Add(invoice);
                }
            }
            aSqlConnection.Close();
            return invoices;
        }

        public List<InvoiceRemarkReport> GetInvoiceReportWithRemark(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<InvoiceRemarkReport>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var invoice = new InvoiceRemarkReport
                    {
                        id = Convert.ToInt32(SqlDataReader["id"]),
                        designation = SqlDataReader["designation"].ToString(),
                        TAmount = Convert.ToDouble(SqlDataReader["TAmount"]),
                        Email = SqlDataReader["Email"].ToString(),
                        address = SqlDataReader["address"].ToString(),
                        city = SqlDataReader["city"].ToString(),
                        BCName = SqlDataReader["BCName"].ToString(),
                        invoice_no = SqlDataReader["invoice_no"].ToString(),
                        phone = SqlDataReader["phone"].ToString(),
                        RemarkDate = Convert.ToDateTime(SqlDataReader["RemarkDate"]),
                        CName = SqlDataReader["CName"].ToString(),
                        Remarks = SqlDataReader["Remarks"].ToString(),
                        UName = SqlDataReader["UName"].ToString()
                    };
                    invoices.Add(invoice);
                }
            }
            aSqlConnection.Close();
            return invoices;
        }

        public List<ProductForInvoice> Getproducts(string query, int type)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var products = new List<ProductForInvoice>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var product = new ProductForInvoice();

                    product.Id = Convert.ToInt32(SqlDataReader["id"]);
                    product.ListItem = SqlDataReader["ListItem"].ToString();
                    if (type == 0)
                    {
                        product.Comments = SqlDataReader["Comments"].ToString();
                        product.Amount = Convert.ToDouble(SqlDataReader["Amount"]);
                        product.SbName = SqlDataReader["SbName"].ToString();
                        product.EDate = Convert.ToDateTime(SqlDataReader["EDate"]).ToShortDateString();
                        product.Product = SqlDataReader["Product"].ToString();
                        product.LedgerId = Convert.ToInt32(SqlDataReader["LedgerId"]);
                        product.SDate = Convert.ToDateTime(SqlDataReader["SDate"]).ToShortDateString();
                        product.TNO = Convert.ToInt32(SqlDataReader["TNO"]);
                    }
                    else
                    {
                        product.SDate = Convert.ToDateTime(SqlDataReader["InvSendDt"]).ToShortDateString();
                        product.Amount = Convert.ToDouble(SqlDataReader["TAmount"]);
                        product.SbName = SqlDataReader["invoice_no"].ToString();
                    }
                    products.Add(product);
                }
            }
            aSqlConnection.Close();
            return products;
        }
        public List<Invoice> GetProductsDetails(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<Invoice>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var invoice = new Invoice
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        Comments = SqlDataReader["Comments"].ToString(),
                        TAmount = Convert.ToDouble(SqlDataReader["SchAmount"]),
                        CompanyName = SqlDataReader["Label"].ToString(),
                        InvSendDt = Convert.ToDateTime(SqlDataReader["InvSendDt"]).ToShortDateString(),
                        InvoiceNo = SqlDataReader["Invoice_No"].ToString(),
                        FullPayment = SqlDataReader["TotalAmount"].ToString()
                    };
                    invoices.Add(invoice);
                }
            }
            aSqlConnection.Close();
            return invoices;
        }

        public string GenerateInvoiceNumber(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoiceNumber = "";
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    invoiceNumber = SqlDataReader[0].ToString();
                }
            }
            aSqlConnection.Close();
            return invoiceNumber;
        }

        public void UpdateDeleteComments(string query)
        {
            SqlQuery = query;
            InsertUpdateDelete(SqlQuery);
        }

        public int[] CheckOnlineJobs(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var online = 0;
            var total = 0;
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    online = Convert.ToInt32(SqlDataReader["OnlineJobs"].ToString());
                    total = Convert.ToInt32(SqlDataReader["TotalJobs"].ToString());
                }
            }
            var data = new[] { online, total };
            aSqlConnection.Close();
            return data;
        }

        public bool CheckInvoiceNo(string invoiceNo)
        {
            SqlQuery = "SELECT * FROM dbo.InvoiceList WHERE Invoice_No='" + invoiceNo + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var isFound = SqlDataReader.HasRows;
            aSqlConnection.Close();
            return isFound;
        }

        public void SaveInvoice(string query)
        {
            SqlQuery = query;
            InsertUpdateDelete(SqlQuery);
        }

        public List<OnlineJob> GetOnlineJobList(string query)
        {
            SqlQuery = query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var joblist = new List<OnlineJob>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var job = new OnlineJob
                    {
                        Serial = Convert.ToInt32(SqlDataReader["Serial"]),
                        CpId = Convert.ToInt32(SqlDataReader["CP_ID"]),
                        Company = SqlDataReader["CName"].ToString(),
                        AccId = Convert.ToInt32(SqlDataReader["Acc_Id"]),
                        AddType = Convert.ToInt32(SqlDataReader["AddType"]),
                        CompanyStatus = SqlDataReader["CompanyStatus"].ToString(),
                        JobPosted = Convert.ToInt32(SqlDataReader["JobPosted"]),
                        TotalJobPosted = Convert.ToInt32(SqlDataReader["TotalJobPosted"]),
                        OpId = Convert.ToInt32(SqlDataReader["OPID"]),
                        Date = Convert.ToDateTime(SqlDataReader["PostingDate"]).ToShortDateString()
                    };
                    joblist.Add(job);
                }
            }
            aSqlConnection.Close();
            return joblist;
        }

        public List<Service> GetServices()
        {
            SqlQuery = "SELECT LedgerId, ServiceName, ServiceId FROM Service_List ORDER BY ServiceName";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var services = new List<Service>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var service = new Service
                    {
                        LedgerId = Convert.ToInt32(SqlDataReader["LedgerId"]),
                        ServiceId = Convert.ToInt32(SqlDataReader["ServiceId"]),
                        ServiceName = SqlDataReader["ServiceName"].ToString()
                    };
                    services.Add(service);
                }
            }
            aSqlConnection.Close();
            return services;
        }

        public List<Job> GetJobs(int cpId, string date)
        {
            SqlQuery = "Select jp_id, cname, Title, (CASE WHEN AddType = 1 THEN 'Stand-out' ELSE 'Basic' END) As Type, PostingDate, ValidDate, AddType from tmpJobs where TNO=0 and cp_id='" + cpId + "' And PostingDate = '" + date + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var Jobs = new List<Job>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var aJob = new Job
                    {
                        JpId = Convert.ToInt32(SqlDataReader["jp_id"]),
                        AddType = Convert.ToInt32(SqlDataReader["AddType"]),
                        CompanyName = SqlDataReader["cname"].ToString(),
                        PostingDate = Convert.ToDateTime(SqlDataReader["PostingDate"]).ToShortDateString(),
                        ValidDate = Convert.ToDateTime(SqlDataReader["ValidDate"]).ToShortDateString(),
                        Title = SqlDataReader["Title"].ToString(),
                        Type = SqlDataReader["Type"].ToString(),
                    };
                    Jobs.Add(aJob);
                }
            }
            aSqlConnection.Close();
            return Jobs;
        }

        public List<InvoiceForOnlineJob> GetInvoices(int cpId, string sDate, int ledgerId)
        {
            SqlQuery = "USP_UPLOAD_INVOICE_ONLINE " + ledgerId + ",'" + sDate + "'," + cpId;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<InvoiceForOnlineJob>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var invoice = new InvoiceForOnlineJob();
                    invoice.InvoiceNo = SqlDataReader["invoice_no"].ToString();
                    if (SqlDataReader["TAmount"] != DBNull.Value)
                        invoice.TotalAmount = Convert.ToDouble(SqlDataReader["TAmount"]);
                    if (cpId > 0)
                    {
                        invoice.AddType = Convert.ToInt32(SqlDataReader["AddType"]);
                        invoice.JpId = Convert.ToInt32(SqlDataReader["jp_id"]);
                        invoice.LedgerId = Convert.ToInt32(SqlDataReader["LedgerID"]);
                        invoice.Title = SqlDataReader["Title"].ToString();
                        invoice.Submitted = SqlDataReader["submitted"].ToString();
                        invoice.SalesPrice = Convert.ToDouble(SqlDataReader["salesprice"]);
                        invoice.BillingContact = SqlDataReader["BillingContact"].ToString();
                        invoice.OpId = Convert.ToInt32(SqlDataReader["OPID"]);
                    }
                    else
                    {
                        if (SqlDataReader["id"] != DBNull.Value)
                            invoice.Id = Convert.ToInt32(SqlDataReader["id"]);
                    }
                    invoices.Add(invoice);
                }
            }
            aSqlConnection.Close();
            return invoices;
        }

        public int GetOnlineLedgerId(string onlineproduct)
        {
            SqlQuery = "SELECT LedgerId FROM SERVICE_LIST WHERE ServiceName = '" + onlineproduct + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var onlineledgerId = 0;
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    onlineledgerId = Convert.ToInt32(SqlDataReader["LedgerId"]);
                }
            }
            else
                onlineledgerId = -1;
            aSqlConnection.Close();
            return onlineledgerId;
        }

        public void DeleteOnlineJob(int jpId)
        {
            SqlQuery = "DELETE FROM tmpJobs WHERE jp_id=" + jpId;
            InsertUpdateDelete(SqlQuery);
        }

        public bool IsAllUploaded()
        {
            SqlQuery = "SELECT id FROM tmpJobs WHERE Invoice_no!='' AND Submitted=0";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var isuploaded = !SqlDataReader.HasRows;
            aSqlConnection.Close();
            return isuploaded;
        }

        public bool CheckOnlineConnection()
        {
            var con = GetOnlineConnectionString();

            var onlineSqlConnection = new OleDbConnection(con);

            bool isOk;
            try
            {
                onlineSqlConnection.Open();
                isOk = true;
            }
            catch
            {
                isOk = false;
            }
            finally
            {
                onlineSqlConnection.Close();
            }
            return isOk;
        }

        public void DeleteTmpJobs()
        {
            SqlQuery = "DELETE FROM tmpjobs";
            InsertUpdateDelete(SqlQuery);
        }

        public object DownloadJobs(string fromDate, string toDate)
        {
            var cmdtext = "usp_Acc_Download_Jobs '" + fromDate + "','" + toDate + "';";
            var onlineSqlConnection = new OleDbConnection(GetOnlineConnectionString());
            var isOk = true;
            try
            {
                onlineSqlConnection.Open();
                var sqlcom = new OleDbCommand(cmdtext, onlineSqlConnection);
                var data = sqlcom.ExecuteReader();
                var count = 0;
                if (data != null)
                {
                    while (data.Read())
                    {
                        count++;
                        var query =
                            "Insert into tmpJobs(id,cp_id,jp_id,acc_id,cname,title,postingDate,validDate,tjobs,BillingContact,designation,OPID,AddType) values(" +
                            count + "," + Convert.ToInt32(data["cp_id"]) + "," +
                            Convert.ToInt32(data["jp_id"]) + "," + Convert.ToInt32(data["acc_id"]) +
                            ",'" +
                            data["name"].ToString().Replace("'", "`") + "','" +
                            data["JobTitle"].ToString().Replace("'", "`") + "','" +
                            data["P"].ToString() +
                            "','" + data["Deadline"].ToString() + "'," +
                            data["count_jp_id"].ToString() +
                            ",'" + data["BillingContact"].ToString().Replace("'", "`") + "','" +
                            data["Designation"].ToString().Replace("'", "`") + "'," +
                            Convert.ToInt32(data["OPID"]) + "," + Convert.ToInt32(data["AdType"]) +
                            ");";
                        aSqlConnection.Open();
                        var sql = new SqlCommand(query, aSqlConnection);
                        sql.ExecuteNonQuery();
                        aSqlConnection.Close();
                    }
                }
            }
            catch
            {
                isOk = false;
            }
            finally
            {
                onlineSqlConnection.Close();
            }
            return isOk;
        }

        public object GetContactPersonsOrJobTitle(string type, int? cId)
        {
            SqlQuery = "USP_ONLINE_SALE_INFO '" + type + "'," + cId;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var contactPersons = new List<ContactPerson>();
            var jobList = new List<Job>();
            while (SqlDataReader.Read())
            {
                if (type == "C")
                {
                    var aPerson = new ContactPerson
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        Name = SqlDataReader["name"].ToString(),
                        Designation = SqlDataReader["Designation"].ToString()
                    };
                    contactPersons.Add(aPerson);
                }
                else
                {
                    var aJob = new Job
                    {
                        JpId = Convert.ToInt32(SqlDataReader["jp_id"]),
                        CompanyName = SqlDataReader["BillingContact"].ToString(),
                        PostingDate = Convert.ToDateTime(SqlDataReader["postingDate"]).ToShortDateString(),
                        ValidDate = Convert.ToDateTime(SqlDataReader["ValidDate"]).ToShortDateString(),
                        Title = SqlDataReader["title"].ToString(),
                        Type = SqlDataReader["Designation"].ToString(),
                    };
                    jobList.Add(aJob);
                }
            }
            aSqlConnection.Close();
            if (type == "C")
                return contactPersons;
            return jobList;
        }

        public object CheckJobTitle(int productId)
        {
            SqlQuery = "USP_CheckJobTitle " + productId;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var ledgers = new List<Ledger>();
            var ledger = new Ledger();
            while (SqlDataReader.Read())
            {
                ledger.GroupName = SqlDataReader["SBName"].ToString();
                ledger.Id = Convert.ToInt32(SqlDataReader["ID"]);
                ledgers.Add(ledger);
            }
            aSqlConnection.Close();
            return ledgers;
        }

        public string GetClosingDate()
        {
            SqlQuery = "SELECT ClosingDate FROM CloseingDateInfo";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var date = "";
            while (SqlDataReader.Read())
            {
                date = Convert.ToDateTime(SqlDataReader["ClosingDate"]).ToShortDateString();
            }
            aSqlConnection.Close();
            return date;
        }

        public void SaveSale(int userId, int cId, int pCode, string fromdate, string toDate, string journalDate,
            double salesPrice, string billingPerson, string designation, string comment, int duration, int noOfInvoice,
            string refNo, int typeId, double vat, int jpId, string jobTitle, string workshopDate)
        {
            SqlQuery = "USP_INSERT_SALE " + userId + "," + cId + "," + pCode + ",'" + fromdate + "','" + toDate + "','" +
                       journalDate + "','" + salesPrice + "','" + billingPerson + "','" + designation + "','" + comment +
                       "','" + duration + "','" + noOfInvoice + "','" + refNo + "','" + typeId + "','" + vat + "','" +
                       jpId + "','" + jobTitle + "','" + workshopDate + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public object GetJournalClosing()
        {
            SqlQuery = "SELECT C.ClosingDate, C.SetDate, U.Name FROM CloseingDateInfo C INNER JOIN Users U ON C.UserID = U.UserID";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            string a = "", b = "", c = "";
            while (SqlDataReader.Read())
            {
                a = Convert.ToDateTime(SqlDataReader["ClosingDate"]).ToShortDateString();
                b = Convert.ToDateTime(SqlDataReader["SetDate"]).ToShortDateString();
                c = SqlDataReader["Name"].ToString();
            }
            aSqlConnection.Close();
            return new { ClosingDate = a, SetDate = b, Name = c };
        }

        public void SetJournalClosing(string closingDate, string setDate, int userId)
        {
            SqlQuery = "UPDATE CloseingDateInfo SET ClosingDate ='" + closingDate + "', SetDate = '" + setDate +
                       "', UserID = '" + userId + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public List<Invoice> GetVoucherList(int year, int month)
        {
            SqlQuery = "USP_GET_JOURNAL_VOUCHER '" + year + "','" + month + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var vouchers = new List<Invoice>();
            while (SqlDataReader.Read())
            {
                var voucher = new Invoice
                {
                    Id = Convert.ToInt32(SqlDataReader["JID"]),
                    InvoiceNo = SqlDataReader["VoucherNo"].ToString()
                };
                vouchers.Add(voucher);
            }
            aSqlConnection.Close();
            return vouchers;
        }
        public List<JournalVoucherReport> GetVoucherReport(int voucherId)
        {
            SqlQuery = "USP_JOURNAL_VOUCHER_RPT '" + voucherId + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var vouchers = new List<JournalVoucherReport>();
            while (SqlDataReader.Read())
            {
                var voucher = new JournalVoucherReport();
                voucher.ApprBy = SqlDataReader["ApprBy"].ToString();
                if (SqlDataReader["ApprovalDate"] != DBNull.Value)
                    voucher.ApprovalDate = Convert.ToDateTime(SqlDataReader["ApprovalDate"]);
                if (SqlDataReader["PostDate"] != DBNull.Value)
                    voucher.PostDate = Convert.ToDateTime(SqlDataReader["PostDate"]);
                if (SqlDataReader["jdate"] != DBNull.Value)
                    voucher.jdate = Convert.ToDateTime(SqlDataReader["jdate"]);
                voucher.Postedby = SqlDataReader["Postedby"].ToString();
                voucher.VoucherNo = SqlDataReader["VoucherNo"].ToString();
                if (SqlDataReader["credit"] != DBNull.Value)
                    voucher.credit = Convert.ToDouble(SqlDataReader["credit"]);
                if (SqlDataReader["debt"] != DBNull.Value)
                    voucher.debt = Convert.ToDouble(SqlDataReader["debt"]);
                voucher.des1 = SqlDataReader["des1"].ToString();
                voucher.des2 = SqlDataReader["des2"].ToString();
                voucher.description = SqlDataReader["description"].ToString();
                voucher.sbname = SqlDataReader["sbname"].ToString();
                vouchers.Add(voucher);
            }
            aSqlConnection.Close();
            return vouchers;
        }

        public object GetAssetCodes()
        {
            SqlQuery = "select id,AssetCode from FixedAsset order by assetcode;";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var codes = new List<Company>();
            while (SqlDataReader.Read())
            {
                var aCode = new Company
                {
                    Id = Convert.ToInt32(SqlDataReader["id"]),
                    Name = SqlDataReader["AssetCode"].ToString()
                };
                codes.Add(aCode);
            }
            aSqlConnection.Close();
            return codes;
        }

        public object GetAssetBankList()
        {
            SqlQuery = "EXEC USP_CASH_BANK_LIST 'Asset'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var banks = new List<Company>();
            while (SqlDataReader.Read())
            {
                var aBank = new Company
                {
                    Id = Convert.ToInt32(SqlDataReader["ID"]),
                    Name = SqlDataReader["Name"].ToString()
                };
                banks.Add(aBank);
            }
            aSqlConnection.Close();
            return banks;
        }

        public object GetFixedAssetItem()
        {
            SqlQuery = "USP_LOAD_FIXED_ASSETS_ITEM";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var items = new List<AssetItem>();
            while (SqlDataReader.Read())
            {
                var item = new AssetItem
                {
                    ItemId = Convert.ToInt32(SqlDataReader["ItemID"]),
                    ItemName = SqlDataReader["ItemName"].ToString(),
                    AssetType = SqlDataReader["AssetType"].ToString(),
                    AssetName = SqlDataReader["AssetName"].ToString()
                };
                items.Add(item);
            }
            aSqlConnection.Close();
            return items;
        }

        public object GetGeneratedAssetCode(int assetType, string type)
        {
            SqlQuery = "USP_GENERATE_ASSET_CODE '" + assetType + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            SqlDataReader.Read();
            var assetCode = SqlDataReader[0].ToString();
            aSqlConnection.Close();
            return assetCode;
        }

        public object GetAssetInfo(string assetCode)
        {
            SqlQuery = "Select * from FixedAsset where assetCode='" + assetCode + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var asset = new FixedAsset();
            while (SqlDataReader.Read())
            {
                asset.Id = Convert.ToInt32(SqlDataReader["Id"]);
                asset.AssetCode = SqlDataReader["AssetCode"].ToString();
                asset.AssetType = SqlDataReader["AssetType"].ToString();
                asset.AssetNo = Convert.ToInt32(SqlDataReader["AssetNo"]);
                try
                {
                    asset.PurchasedDate = Convert.ToDateTime(SqlDataReader["PurchasedDt"]).ToShortDateString();
                }
                catch { }
                asset.Price = Convert.ToDouble(SqlDataReader["Price"]);
                asset.DepRate = Convert.ToDouble(SqlDataReader["DepRate"]);
                try
                {
                    asset.DepStartDate = Convert.ToDateTime(SqlDataReader["DepStartDt"]).ToShortDateString();
                }
                catch { }
                asset.DepLife = SqlDataReader["DepLife"].ToString();
                try
                {
                    asset.DepEndDate = Convert.ToDateTime(SqlDataReader["DepEndDt"]).ToShortDateString();
                }
                catch { }
                try
                {
                    asset.Supplier = SqlDataReader["Supplier"].ToString();
                }
                catch { }
                try
                {
                    asset.InvoiceNo = SqlDataReader["InvoiceNo"].ToString();
                }
                catch { }
                try
                {
                    asset.LabelNo = SqlDataReader["LabelNo"].ToString();
                }
                catch { }
                asset.Description = SqlDataReader["Description"].ToString();
                try
                {
                    asset.LastPosted = Convert.ToDateTime(SqlDataReader["LastPosted"]).ToShortDateString();
                }
                catch { }
                asset.NoDep = Convert.ToBoolean(SqlDataReader["NoDep"]);
                asset.Approved = Convert.ToBoolean(SqlDataReader["Approved"]);
                asset.StopDep = SqlDataReader["StopDep"].ToString();
                asset.SoldAmount = Convert.ToDouble(SqlDataReader["SoldAmount"]);
                try
                {
                    asset.DisposalDate = Convert.ToDateTime(SqlDataReader["DisposalDate"]).ToShortDateString();
                }
                catch { }
                asset.Sold = Convert.ToBoolean(SqlDataReader["Sold"]);
                asset.Profit = Convert.ToDouble(SqlDataReader["Profit"]);
                asset.Remarks = SqlDataReader["Remarks"].ToString();
            }
            aSqlConnection.Close();
            return asset;
        }

        public void InsertUpdateAsset(string action, int userId, int noDep, string assetCode, string assetNo,
            string assetType, string purchasedDate, double price, string depStartDate, string supplier, string invoiceNo,
            string labelNo, string description, double depRate, int depLife, string depEndDate)
        {
            SqlQuery = "USP_INSERT_ASSET '" + action + "','" + userId + "','" + noDep + "','" + assetCode + "','" +
                       assetNo + "','" + assetType + "','" + purchasedDate + "','" + price + "','" + depStartDate +
                       "','" + supplier + "','" + invoiceNo + "','" + labelNo + "','" + description + "','" + depRate +
                       "','" + depLife + "','" + depEndDate + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public object CheckAssetNumber(string assetCode)
        {
            SqlQuery = "SELECT id,assetCode,DepStartDt,LastPosted,dependdt FROM FixedAsset WHERE assetCode='" +
                       assetCode + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var assets = new List<FixedAsset>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var asset = new FixedAsset();
                    asset.Id = Convert.ToInt32(SqlDataReader["id"]);
                    asset.AssetCode = SqlDataReader["assetCode"].ToString();
                    try
                    {
                        asset.DepStartDate = Convert.ToDateTime(SqlDataReader["DepStartDt"]).ToShortDateString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        asset.DepEndDate = Convert.ToDateTime(SqlDataReader["dependdt"]).ToShortDateString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        asset.LastPosted = Convert.ToDateTime(SqlDataReader["LastPosted"]).ToShortDateString();
                    }
                    catch
                    {
                    }
                    assets.Add(asset);
                }
            }
            aSqlConnection.Close();
            return assets;
        }

        public void MakeJournal(string userId, string noDep, string assetCode, string assetNo, string purchasedId,
            double price, string description)
        {
            SqlQuery = "USP_ASSET_JOURNAL '" + userId + "','" + noDep + "','" + assetCode + "','" + assetNo + "','" + purchasedId + "','" + price + "','" + description.Replace("'", "`") + "';";
            InsertUpdateDelete(SqlQuery);
        }
        public void InsertDisposalAsset(string userId, string assetNoId, string disposalDate, double amount,
            int sold, string soldId, string description)
        {
            SqlQuery = "USP_INSERT_DISPOSAL_ASSET '" + userId + "','" + assetNoId + "','" + disposalDate + "','" +
                       amount + "','" + sold + "','" + description.Replace("'", "`") + "','" + soldId + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public void DeleteAssetJournal(string assetCode)
        {
            SqlQuery = "Delete from FixedAsset where AssetCode='" + assetCode + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public object GetOnlineCompanyList(int radio)
        {
            SqlQuery = "USP_ONLINE_COMPANY_LIST";
            if (radio == 1)
                SqlQuery += " 'All'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var companies = new List<Company>();
            if (SqlDataReader.HasRows)
            {

                while (SqlDataReader.Read())
                {
                    var company = new Company
                    {
                        Id = Convert.ToInt32(SqlDataReader["acc_id"]),
                        Name = SqlDataReader["cname"].ToString(),
                        CP_Id = Convert.ToInt32(SqlDataReader["cp_id"])
                    };
                    companies.Add(company);
                }
            }
            aSqlConnection.Close();
            return companies;
        }

        public object GetOnlineCompanyInfo(int cpId, string connectionstr)
        {
            var cmdtext = "USP_Acc_Download_ComInfo '" + cpId + "';";
            var onlineSqlConnection = new OleDbConnection(connectionstr);
            var aCompany = new Company();
            try
            {
                onlineSqlConnection.Open();
                var sqlcom = new OleDbCommand(cmdtext, onlineSqlConnection);
                var data = sqlcom.ExecuteReader();
                if (data != null)
                {
                    while (data.Read())
                    {
                        aCompany.Id = Convert.ToInt32(data["acc_id"]);
                        aCompany.Name = data["name"].ToString();
                        aCompany.Address = data["address"].ToString();
                        aCompany.City = data["city"].ToString();
                        aCompany.Phone = data["phone"].ToString();
                        aCompany.Email = data["e_mail"].ToString();
                        aCompany.ContactPerson = data["ContactName"].ToString();
                        aCompany.Designation = data["designation"].ToString();
                        aCompany.AccCreatedDate = Convert.ToDateTime(data["acct_cr"]).ToShortDateString();
                    }
                }
                onlineSqlConnection.Close();
            }
            catch
            {
            }
            return aCompany;
        }

        public void InsertUpdateOnlineCompany(string action, string cpId, string name, string address, string city, string phone, string email, string cPerson, string designation, string companyId)
        {
            SqlQuery = "USP_INSERT_UPDATE_ONLINE_COMPANY '" + action + "','" + cpId + "','" + name + "','" + address +
                       "','" + city + "','" + phone + "','" + email + "','" + cPerson + "','" +
                       designation + "','" + companyId + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public void UpdateProfile(string action, string name, string cpId, string id)
        {
            var cId = 0;
            if (action == "INSERT")
            {
                SqlQuery = "SELECT id FROM company WHERE name='" + name + "' AND cp_id='" + cpId + "';";
                aSqlConnection.Open();
                SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
                SqlDataReader = SqlCommand.ExecuteReader();
                SqlDataReader.Read();
                cId = Convert.ToInt32(SqlDataReader["id"]);
                aSqlConnection.Close();
            }
            else
                cId = Convert.ToInt32(id);

            //var query = "Update dbo_Company_Profiles set acc_Id=" + cId + " where cp_id='" + cpId + "';";
            //var onlineSqlConnection = new OleDbConnection(GetOnlineConnectionString());
            //onlineSqlConnection.Open();
            //var sqlcom = new OleDbCommand(query, onlineSqlConnection);
            //sqlcom.ExecuteNonQuery();
            //onlineSqlConnection.Close();
        }

        public List<Ledger> GetLedgersWithBalance()
        {
            SqlQuery = "SELECT id,sbname,FORMAT(balance,'##,##0.00 '+account) As CurrentBalance FROM Ledger WHERE (LedgerAcc=1 or under like '3,1075%') ORDER BY sbname";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var ledgers = new List<Ledger>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var aLedger = new Ledger
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        GroupName = SqlDataReader["sbname"].ToString(),
                        Account = SqlDataReader["CurrentBalance"].ToString()
                    };
                    ledgers.Add(aLedger);
                }
            }
            aSqlConnection.Close();
            return ledgers;
        }

        public object GetMaxJournalId()
        {
            SqlQuery = "SELECT MAX(Jid) FROM Journal;";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            SqlDataReader.Read();
            var jId = Convert.ToInt32(SqlDataReader[0]);
            aSqlConnection.Close();
            return jId;
        }

        public void SaveJournals(List<Journal> journals)
        {
            foreach (Journal journal in journals)
            {
                journal.Description = !string.IsNullOrEmpty(journal.Description) ? journal.Description.Replace("'", "`") : "";
                if (journal.Tno != 0 && !string.IsNullOrEmpty(journal.Notify))
                {
                    SqlQuery =
                        "INSERT INTO journal(jid,sid,description,debt,credit,Jdate,PostDate,UserID,tno,notify) values(" +
                        journal.JId + "," + journal.SId + ",'" + journal.Description.Replace("'", "`") + "'," +
                        journal.Debt +
                        "," + journal.Credit + ",'" + journal.JDate.ToShortDateString() + "','" +
                        journal.PostDate.ToShortDateString() + "'," + journal.UserId + "," + journal.Tno + ",'" +
                        journal.Notify.Replace("'", "`") + "');";
                }
                else
                {
                    SqlQuery =
                        "INSERT INTO journal(jid,sid,description,debt,credit,Jdate,PostDate,UserID) values(" +
                        journal.JId + "," + journal.SId + ",'" + journal.Description + "'," +
                        journal.Debt +
                        "," + journal.Credit + ",'" + journal.JDate.ToShortDateString() + "','" +
                        journal.PostDate.ToShortDateString() + "'," + journal.UserId + ");";
                }
                InsertUpdateDelete(SqlQuery);
            }
        }

        public void UpdateLedger(List<Ledger> ledgers)
        {
            foreach (Ledger ledger in ledgers)
            {
                SqlQuery = "UPDATE Ledger SET balance='" + ledger.Balance + "',account='" + ledger.Account +
                           "' WHERE id='" + ledger.Id + "';";
                InsertUpdateDelete(SqlQuery);
            }
        }

        public void MakeJournalVoucher(int jId, string postDate)
        {
            SqlQuery = "USP_MakeJournalVoucher '" + jId + "','" + postDate + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public object GetSubreports()
        {
            SqlQuery = "SELECT id,sbname FROM ledger WHERE under LIKE '%,410' ORDER BY sbname;";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var ledgers = new List<Ledger>();
            while (SqlDataReader.Read())
            {
                var ledger = new Ledger
                {
                    Id = Convert.ToInt32(SqlDataReader["id"]),
                    GroupName = SqlDataReader["sbname"].ToString()
                };
                ledgers.Add(ledger);
            }
            aSqlConnection.Close();
            return ledgers;
        }
        public object GetInvoicesForCashCollection(string query)
        {
            SqlQuery = "usp_GetInvoiceList " + query;
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var invoices = new List<Invoice>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var invoice = new Invoice
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        InvoiceNo = SqlDataReader["invoice_no"].ToString(),
                        TAmount = Convert.ToDouble(SqlDataReader["TAmount"]),
                        FullPayment = SqlDataReader["FullPayment"].ToString(),
                        PostedNature = SqlDataReader["PostedNature"].ToString(),
                        UploadedPaymentStatus = SqlDataReader["UploadedPaymentStatus"].ToString()
                    };
                    invoices.Add(invoice);
                }
            }
            aSqlConnection.Close();
            return invoices;
        }

        public object GetSalesInfo(string invoiceNo)
        {
            SqlQuery = "SELECT s.TNO, l.SBName, i1.Amount, i1.Id, i1.comments FROM sales AS s, ledger AS l, InvoiceSceduler AS i1, InvoiceList AS i WHERE i.Id=i1.invoice_id AND i1.TNO=s.tno AND s.PCode=l.id AND i.Invoice_No='" + invoiceNo + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var sales = new List<Sale>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var sale = new Sale
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        Tno = SqlDataReader["TNO"].ToString(),
                        Amount = Convert.ToDouble(SqlDataReader["Amount"]),
                        Comment = SqlDataReader["comments"].ToString(),
                        SbName = SqlDataReader["SBName"].ToString()
                    };
                    sales.Add(sale);
                }
            }
            aSqlConnection.Close();
            return sales;
        }

        public object GetCashCollection(string id)
        {
            SqlQuery = "select id,cash,salesTax,receiveddate,posted,PaymentType,chequedetails,BadDebt,BankId from cash_Collection where InvoiceSchedulerId='" + id + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var collections = new List<CashCollection>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var collection = new CashCollection
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        Cash = Convert.ToDouble(SqlDataReader["cash"]),
                        SalesTax = Convert.ToDouble(SqlDataReader["salesTax"]),
                        ReceivedDate = Convert.ToDateTime(SqlDataReader["receiveddate"]).ToShortDateString(),
                        Posted = Convert.ToBoolean(SqlDataReader["posted"]),
                        BadDebt = Convert.ToBoolean(SqlDataReader["BadDebt"]),
                        BankId = Convert.ToInt32(SqlDataReader["BankId"]),
                        PaymentType = SqlDataReader["PaymentType"].ToString(),
                        ChequeDetails = SqlDataReader["chequedetails"].ToString()
                    };
                    collections.Add(collection);
                }
            }
            aSqlConnection.Close();
            return collections;
        }

        public object GetTotalOnlinePost()
        {
            SqlQuery = "SELECT COUNT(Invoice_No) AS Total FROM InvoiceList WHERE UploadedPaymentStatus='No'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            SqlDataReader.Read();
            var total = SqlDataReader[0].ToString();
            aSqlConnection.Close();
            return total;
        }

        public void InsertCashCollection(string type, string userId, string invoiceNo, string cash, string date, string tno, string invoiceShedulerId, string ledgerId, string chequeDetails, string companyName)
        {
            SqlQuery = "USP_INSERT_CASH_COLLECTION '" + type + "','" + userId + "','" + invoiceNo + "','" + cash + "','" +
                       date + "','" + tno + "','" + invoiceShedulerId + "','" + ledgerId + "','" + chequeDetails + "','" +
                       companyName + "';";
            InsertUpdateDelete(SqlQuery);
        }
        public object GetAccountReceivable(string tno)
        {
            SqlQuery = "SELECT AccReceivable FROM Sales WHERE TNO = '" + tno + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            SqlDataReader.Read();
            var acc = SqlDataReader[0].ToString();
            aSqlConnection.Close();
            return acc;
        }

        public void UpdateCashCollection(string type, string userId, string invoiceNo, string cash, string date, string tno, string invoiceShedulerId, string ledgerId, string chequeDetails, string companyName, string cashCollectionId)
        {
            SqlQuery = "USP_UPDATE_CASH_COLLECTION '" + type + "','" + userId + "','" + cashCollectionId + "','" + invoiceNo + "','" + cash + "','" +
                       date + "','" + tno + "','" + invoiceShedulerId + "','" + ledgerId + "','" + chequeDetails + "','" +
                       companyName + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public void UnpaidCashCollection(string userId, string ledgerId, string tno, string invoiceId, string invoiceNo, string collectionid, string amount, string companyName)
        {
            SqlQuery = "USP_UNPAID_CASH_COLLECTION '" + userId + "','" + ledgerId + "','" + tno + "','" + invoiceId +
                       "','" + invoiceNo + "','" + collectionid + "','" + amount + "','" + companyName + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public object GetJournals(string pageNo, string pageSize, string isPreview, string dateType, string startDate,
            string endDate, string ledgerId, string ledgerName, string companyId, string approvedBy, string postedBy,
            string isApproved)
        {
            SqlQuery = "USP_VIEW_JOURNAL_LIST '" + pageNo + "','" + pageSize + "','" + isPreview + "','" + dateType + "','" + startDate + "','" + endDate + "','" + ledgerId + "','" + ledgerName + "','" + companyId + "','" + approvedBy + "','" + postedBy + "','" + isApproved + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var journals = new List<Journal>();
            var journalsForReport = new List<JournalForReport>();
            while (SqlDataReader.Read())
            {
                if (isPreview == "0")
                {
                    var journal = new Journal
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        SId = Convert.ToInt32(SqlDataReader["sid"]),
                        Debt = Convert.ToDouble(SqlDataReader["Debt"]),
                        Credit = Convert.ToDouble(SqlDataReader["Credit"]),
                        Description = SqlDataReader["Description"].ToString(),
                        JId = Convert.ToInt32(SqlDataReader["jid"]),
                        Journaldate = Convert.ToDateTime(SqlDataReader["jDate"]).ToShortDateString(),
                        UserId = Convert.ToInt32(SqlDataReader["UserID"]),
                        ApprovedBy = Convert.ToInt32(SqlDataReader["ApprovedBy"]),
                        Notify = SqlDataReader["Notify"].ToString(),
                        Tno = Convert.ToInt32(SqlDataReader["tno"]),
                        UpdatedBy = Convert.ToInt32(SqlDataReader["UpdatedBy"]),
                        AccType = SqlDataReader["SBName"].ToString(),
                        TotalRecord = Convert.ToInt32(SqlDataReader["TotalRecords"])
                    };
                    try
                    {
                        journal.ApprovalDate = Convert.ToDateTime(SqlDataReader["ApprovalDate"]);
                    }
                    catch
                    {
                    }
                    try
                    {
                        journal.UpdatedDate = Convert.ToDateTime(SqlDataReader["UpdatedDate"]);
                    }
                    catch
                    {
                    }
                    journals.Add(journal);
                }
                else
                {
                    var journal = new JournalForReport
                    {
                        id = Convert.ToInt32(SqlDataReader["id"]),
                        lid = Convert.ToInt32(SqlDataReader["lid"]),
                        jid = Convert.ToInt32(SqlDataReader["jid"]),
                        Description = SqlDataReader["Description"].ToString(),
                        Approval = SqlDataReader["Approval"].ToString(),
                        Users = SqlDataReader["Users"].ToString(),
                        sbname = SqlDataReader["sbname"].ToString(),
                        Debt = Convert.ToDouble(SqlDataReader["Debt"]),
                        Credit = Convert.ToDouble(SqlDataReader["Credit"]),
                        PostDate = Convert.ToDateTime(SqlDataReader["PostDate"]),
                        jDate = Convert.ToDateTime(SqlDataReader["jDate"])
                    };
                    journalsForReport.Add(journal);
                }
            }
            aSqlConnection.Close();
            if (isPreview == "0")
                return journals;
            return journalsForReport;
        }
        public object ApprovedJournals(string userId, string dateType, string startDate, string endDate, string ledgerId, string ledgerName, string companyId, string approvedBy, string postedBy, string approved)
        {
            SqlQuery = "USP_APPROVE_JOURNAL '" + userId + "','" + dateType + "','" + startDate + "','" + endDate + "','" + ledgerId + "','" + ledgerName + "','" + companyId + "','" + approvedBy + "','" + postedBy + "','" + approved + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            SqlDataReader.Read();
            var result = SqlDataReader[0].ToString();
            aSqlConnection.Close();
            return result;
        }

        public object GetSales(string pageNo, string pageSize, int cId, int tno)
        {
            SqlQuery = "USP_VIEW_SALES '" + pageNo + "','" + pageSize + "','" + cId + "','" + tno + "'";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var sales = new List<Sale>();
            var totalAmount = 0.00;
            var totalDues = 0.00;
            while (SqlDataReader.Read())
            {
                if (cId != 0 && tno == 0)
                {
                    var sale = new Sale
                    {
                        Id = Convert.ToInt32(SqlDataReader["id"]),
                        SbName = SqlDataReader["sbname"].ToString(),
                        Tno = SqlDataReader["TNO"].ToString(),
                        FromDate = Convert.ToDateTime(SqlDataReader["From"]).ToShortDateString(),
                        ToDate = Convert.ToDateTime(SqlDataReader["To"]).ToShortDateString(),
                        Amount = Convert.ToDouble(SqlDataReader["salesPrice"]),
                        TotalRecords = Convert.ToInt32(SqlDataReader["TotalRecord"]),
                        AccReceivale = Convert.ToDouble(SqlDataReader["AccReceivable"]),
                        IsPosted = Convert.ToBoolean(SqlDataReader["Posted"]),
                        Duration = Convert.ToInt32(SqlDataReader["Duration"]),
                        Tax = Convert.ToDouble(SqlDataReader["Tax"]),
                        BillingContactId = Convert.ToInt32(SqlDataReader["BillContactId"]),
                        RefNo = SqlDataReader["RefNo"].ToString(),
                        TaxId = Convert.ToInt32(SqlDataReader["TaxID"])
                    };
                    totalAmount = Convert.ToDouble(SqlDataReader["TotalAmount"]);
                    totalDues = Convert.ToDouble(SqlDataReader["DuesAmount"]);
                    sales.Add(sale);
                }
                else
                {
                    var sale = new Sale
                    {
                        Tno = SqlDataReader["tno"].ToString(),
                        SbName = SqlDataReader["sbname"].ToString(),
                        CompanyName = SqlDataReader["name"].ToString(),
                        Amount = Convert.ToDouble(SqlDataReader["salesprice"]),
                        SDate = Convert.ToDateTime(SqlDataReader["sdate"]).ToShortDateString(),
                        TotalRecords = Convert.ToInt32(SqlDataReader["TotalRecord"])
                    };
                    sales.Add(sale);
                }
            }
            aSqlConnection.Close();
            if (cId == 0)
                return sales;
            return new { Sales = sales, TotalAmount = totalAmount, DuesAmount = totalDues };
        }
        public void UpdateJournal(string id, string sid, string description, string debt, string credit, string jdate, string updatedBy, string updatedDate, string notify)
        {
            SqlQuery = "UPDATE Journal SET Sid='" + sid + "', Description='" + description.Replace("'", "`") +
                       "', Debt='" + debt + "', Credit='" + credit + "', JDate='" + jdate + "', UpdatedDate='" +
                       updatedDate + "', UpdatedBy='" + updatedBy + "', Notify='" + notify + "' WHERE Id='" + id + "';";
            InsertUpdateDelete(SqlQuery);
        }
        public void DeleteJournal(string id, string jid)
        {
            if (jid == "-1")
            {
                SqlQuery = "DELETE FROM Journal WHERE Id='" + id + "';";
            }
            else
            {
                SqlQuery = "DELETE FROM Journal WHERE JId='" + jid + "';";
            }
            InsertUpdateDelete(SqlQuery);
        }
        public object GetJournalsForTrialBalance(string pageNo, string pageSize, string tno, string fromDate, string endDate)
        {
            SqlQuery = "USP_VIEW_SALE_WISE_JOURNAL_LIST '" + pageNo + "','" + pageSize + "','" + fromDate + "','" +
                       endDate + "','" + tno + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var journals = new List<Journal>();
            double totalDebt = 0.00, totalCredit = 0.00;
            while (SqlDataReader.Read())
            {
                var journal = new Journal
                {
                    Id = Convert.ToInt32(SqlDataReader["id"]),
                    SId = Convert.ToInt32(SqlDataReader["sid"]),
                    Debt = Convert.ToDouble(SqlDataReader["Debt"]),
                    Credit = Convert.ToDouble(SqlDataReader["Credit"]),
                    Description = SqlDataReader["description"].ToString(),
                    Journaldate = Convert.ToDateTime(SqlDataReader["JDate"]).ToShortDateString(),
                    AccType = SqlDataReader["sbName"].ToString(),
                    TotalRecord = Convert.ToInt32(SqlDataReader["TotalRecord"])
                };
                totalDebt = Convert.ToDouble(SqlDataReader["TotalDebt"]);
                totalCredit = Convert.ToDouble(SqlDataReader["TotalCredit"]);
                journals.Add(journal);
            }
            aSqlConnection.Close();
            return new { Journals = journals, TotalDebt = totalDebt, TotalCredit = totalCredit };
        }
        public List<Journal> GetPreviewReport(string type, string fromdate, string endDate)
        {
            SqlQuery = "USP_PREVIEW_TRIAL_BALANCE '" + type + "', '" + fromdate + "', '" + endDate + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var journals = new List<Journal>();
            while (SqlDataReader.Read())
            {
                var journal = new Journal
                {
                    Id = Convert.ToInt32(SqlDataReader["id"]),
                    JId = Convert.ToInt32(SqlDataReader["Jid"]),
                    Debt = Convert.ToDouble(SqlDataReader["Debt"]),
                    Credit = Convert.ToDouble(SqlDataReader["Credit"]),
                    Description = SqlDataReader["description"].ToString(),
                    JDate = Convert.ToDateTime(SqlDataReader["JDate"]),
                    AccName = SqlDataReader[2].ToString(),
                    Group = SqlDataReader["Group"].ToString()
                };
                journals.Add(journal);
            }
            aSqlConnection.Close();
            return journals;
        }
        public object DeleteSale(string tno)
        {
            SqlQuery = "USP_DELETE_SALE_INFO '" + tno + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            SqlDataReader.Read();
            var data = SqlDataReader[0].ToString();
            aSqlConnection.Close();
            return data;
        }
        public void MakeJournalOfSale(string sId, string amount, string jDate, string duration, string tno, string description, string salesdate, string taxId, string tax, string userId)
        {
            SqlQuery = "USP_SALES_JOURNAL '" + sId + "','" + amount + "','" + jDate + "','" + duration + "','" + tno +
                       "','" + description.Replace("'", "`") + "','" + salesdate + "','" + taxId + "','" + tax + "','" +
                       userId + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public object GetNumberOfId(string tno)
        {
            return
                new
                {
                    First = GetFirst("select COUNT(tno) TotalCount from Cash_Collection where tno = '" + tno + "';"),
                    Second =
                        GetFirst("select COUNT(tno) TotalCount from journal where ApprovedBy>0 And tno = '" + tno + "';")
                };
        }

        public int GetFirst(string query)
        {
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(query, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            SqlDataReader.Read();
            var count = Convert.ToInt32(SqlDataReader[0]);
            aSqlConnection.Close();
            return count;
        }

        public void UpdateSaleInfo(string salesPrice, string accReceivable, string duration, string tax, string sDate, string eDate, string tno, string contactId, string refNo)
        {
            SqlQuery = "Update sales set SalesPrice='" + salesPrice + "',AccReceivable='" + accReceivable +
                       "',Duration='" + duration + "',Tax='" + tax + "',Sdate='" + sDate + "',edate='" + eDate +
                       "',BillContactID='" + contactId + "',RefNo='" + refNo + "' where tno='" + tno + "';";
            InsertUpdateDelete(SqlQuery);
        }
        public void UpdateSalesJournal(string sid, string vatId, string tno, string oldDuration,
            string newDuration, string oldAmount, string newAmount, string oldVatAmount, string newVatAmount,
            string fromDate, string description, string userId)
        {
            SqlQuery = "USP_SALES_JOURNAL_UPDATE_D '" + sid + "','" + vatId + "','" + tno + "','" + oldDuration + "','" +
                       newDuration + "','" + oldAmount + "','" + newAmount + "','" + oldVatAmount + "','" + newVatAmount +
                       "','" + fromDate + "','" + description.Replace("'", "`") + "','" + userId + "';";
            InsertUpdateDelete(SqlQuery);
        }

        public void UpdateSalePosted(string sid, string newAmount, string fromdate, string newDuration, string tno, string description, string dateFrom, string vatId, string newVatAmount, string userId)
        {
            SqlQuery = "USP_SALES_JOURNAL '" + sid + "','" + newAmount + "','" + fromdate + "','" + newDuration + "','" +
                       tno + "','" + description + "','" + dateFrom + "','" + vatId + "','" + newVatAmount + "','" +
                       userId + "';UPDATE Sales SET posted=1 WHERE tno='" + tno + "';";
            InsertUpdateDelete(SqlQuery);
        }
        public void UpdateSaleProduct(string oldSid, string tno, string newSid)
        {
            SqlQuery = "UPDATE sales SET pcode='" + newSid + "' WHERE tno='" + tno + "';UPDATE journal SET sid='" + newSid + "' WHERE tno='" + tno + "' AND sid='" + oldSid + "' AND notify IS NOT NULL";
            InsertUpdateDelete(SqlQuery);
        }

        public void UpdateSaleContactPersonAndRefNo(string personId, string refNo, string tno)
        {
            SqlQuery = "UPDATE sales SET BillContactID='" + personId + "',RefNo='" + refNo + "' WHERE tno='" + tno +
                       "';";
            InsertUpdateDelete(SqlQuery);
        }

        public object GetMoneyReceipt(string invoices)
        {
            SqlQuery = "USP_COLLECTION_NOTE '" + invoices + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var receipts = new List<MoneyReceipt>();
            while (SqlDataReader.Read())
            {
                var receipt = new MoneyReceipt
                {
                    Invoice_No = SqlDataReader["Invoice_No"].ToString(),
                    MoneyReceiptNo = SqlDataReader["MoneyReceiptNo"].ToString(),
                    Name = SqlDataReader["Name"].ToString(),
                    TAmount = SqlDataReader["TAmount"].ToString(),
                    sbname = SqlDataReader["sbname"].ToString()
                };
                receipts.Add(receipt);
            }
            aSqlConnection.Close();
            return receipts;
        }
        public object GetTrialBalanceReport(string type, string startingDate, string endDate)
        {
            SqlQuery = "USP_TRIAL_BALANCE_RPT '" + type + "','" + startingDate + "','" + endDate + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var ledgers = new List<Ledger>();
            while (SqlDataReader.Read())
            {
                var ledger = new Ledger
                {
                    mgroup = SqlDataReader["mgroup"].ToString(),
                    sbname = SqlDataReader["sbname"].ToString(),
                    Total = Convert.ToDouble(SqlDataReader["Total"])
                };
                ledgers.Add(ledger);
            }
            aSqlConnection.Close();
            return ledgers;
        }
        public object GetAccountReceivableReport(string type)
        {
            SqlQuery = "USP_AccountsReceivableTodayRPT '" + type + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var products = new List<ProductForInvoice>();
            while (SqlDataReader.Read())
            {
                var product = new ProductForInvoice
                {
                    Company = SqlDataReader["Name2"].ToString(),
                    Product = SqlDataReader["Name1"].ToString(),
                    TD = Convert.ToDouble(SqlDataReader["TD"]),
                    sdate = Convert.ToDateTime(SqlDataReader["sdate"])
                };
                products.Add(product);
            }
            aSqlConnection.Close();
            return products;
        }

        public object GetReportRpt(string startingDate, string enddate, string type, int num, int diff)
        {
            switch (num)
            {
                case 0:
                    SqlQuery = "USP_AccountsReceivableRPT '" + startingDate + "','" + enddate + "', '" + type + "';";
                    break;
                case 1:
                    SqlQuery = "USP_RevenueRPT '" + startingDate + "','" + enddate + "', '" + type + "';";
                    break;
                case 2:
                    SqlQuery = "USP_ExpenseRPT '" + startingDate + "','" + enddate + "';";
                    break;
                case 3:
                    SqlQuery = "USP_CashCollectionRPT '" + startingDate + "','" + enddate + "', '" + type + "';";
                    break;
                case 4:
                    SqlQuery = "USP_IncomeStatementRPT '" + startingDate + "','" + enddate + "', '" + type + "';";
                    break;
                case 5:
                    SqlQuery = "USP_CashBankDetailRPT '" + startingDate + "','" + enddate + "', '" + type + "';";
                    break;
                case 6:
                    SqlQuery = "USP_GL_CB_RPT_V3_NEW '" + startingDate + "','" + enddate + "', '" + type + "';";
                    break;
                case 7:
                    switch (type)
                    {
                        case "MB":
                            SqlQuery = "USP_BalanceSheetRPT '" + startingDate + "','" + enddate + "', " + GetRevExpString(startingDate, enddate, diff) + ";";
                            break;
                        case "OL":
                            SqlQuery = "USP_BalanceSheet  '" + startingDate + "','" + enddate + "', '" + type + "', " + GetRevExpString(startingDate, enddate, diff) + ";";
                            break;
                        default:
                            SqlQuery = "USP_BalanceSheet  '" + startingDate + "','" + enddate + "', '" + type + "';";
                            break;
                    }
                    break;
                case 8:
                    SqlQuery = "USP_GL_CB_CM_RPT_V1_NEW '" + startingDate + "','" + enddate + "', '" + type + "','" + diff + "';";
                    break;
                case 9:
                    SqlQuery = "USP_FIXED_ASSETS_RPT '" + startingDate + "';";
                    break;
            }

            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var products = new List<ReportRpt>();
            if (SqlDataReader.HasRows)
            {
                while (SqlDataReader.Read())
                {
                    var product = new ReportRpt();

                    try
                    {
                        product.Name1 = SqlDataReader["Name1"].ToString();
                    }
                    catch
                    {
                        try
                        {
                            product.Name1 = SqlDataReader["N1"].ToString();
                        }
                        catch
                        {
                            product.Name1 = "";
                        }
                    }
                    try
                    {
                        product.Name2 = SqlDataReader["Name2"].ToString();
                    }
                    catch
                    {
                        try
                        {
                            product.Name2 = SqlDataReader["N2"].ToString();
                        }
                        catch
                        {
                            product.Name2 = "";
                        }
                    }
                    var a = 0;
                    var b = 0;
                    try
                    {
                        product.Name3 = SqlDataReader["Name3"].ToString();
                    }
                    catch
                    {
                        try
                        {
                            product.Name3 = SqlDataReader["N3"].ToString();
                        }
                        catch
                        {
                            product.Name3 = "";
                        }
                    }
                    try
                    {
                        product.Id = Convert.ToInt32(SqlDataReader["ID"]);
                    }
                    catch
                    {
                        product.Id = 0;
                    }
                    try
                    {
                        product.DateS1 = Convert.ToDateTime(SqlDataReader["DateS1"]);
                        a = 1;
                    }
                    catch
                    {
                        a = 0;
                    }
                    try
                    {
                        product.DateS2 = Convert.ToDateTime(SqlDataReader["DateS2"]);
                        b = 1;
                    }
                    catch
                    {
                        b = 0;
                    }
                    try
                    {
                        product.DateS = Convert.ToDateTime(SqlDataReader["DateS"]);
                    }
                    catch
                    {

                    }
                    try
                    {
                        product.M1 = Convert.ToDouble(SqlDataReader["M1"]);
                    }
                    catch
                    {
                        product.M1 = 0;
                    }
                    try
                    {
                        product.M2 = Convert.ToDouble(SqlDataReader["M2"]);
                    }
                    catch
                    {
                        product.M2 = 0;
                    }
                    try
                    {
                        product.M3 = Convert.ToDouble(SqlDataReader["M3"]);
                    }
                    catch
                    {
                        product.M3 = 0;
                    }
                    try
                    {
                        product.M4 = Convert.ToDouble(SqlDataReader["M4"]);
                    }
                    catch
                    {
                        product.M4 = 0;
                    }
                    try
                    {
                        product.M5 = Convert.ToDouble(SqlDataReader["M5"]);
                    }
                    catch
                    {
                        product.M5 = 0;
                    }
                    try
                    {
                        product.M6 = Convert.ToDouble(SqlDataReader["M6"]);
                    }
                    catch
                    {
                        product.M6 = 0;
                    }
                    try
                    {
                        product.M7 = Convert.ToDouble(SqlDataReader["M7"]);
                    }
                    catch
                    {
                        product.M7 = 0;
                    }
                    try
                    {
                        product.M8 = Convert.ToDouble(SqlDataReader["M8"]);
                    }
                    catch
                    {
                        product.M8 = 0;
                    }
                    try
                    {
                        product.M9 = Convert.ToDouble(SqlDataReader["M9"]);
                    }
                    catch
                    {
                        product.M9 = 0;
                    }
                    try
                    {
                        product.M10 = Convert.ToDouble(SqlDataReader["M10"]);
                    }
                    catch
                    {
                        product.M10 = 0;
                    }
                    try
                    {
                        product.M11 = Convert.ToDouble(SqlDataReader["M11"]);
                    }
                    catch
                    {
                        product.M11 = 0;
                    }
                    try
                    {
                        product.M12 = Convert.ToDouble(SqlDataReader["M12"]);
                    }
                    catch
                    {
                        product.M12 = 0;
                    }
                    if (num == 6)
                    {
                        var str = 0;
                        if (a == 1 && b == 1)
                            str = 3;
                        else if (a == 1 && b == 0)
                            str = 0;
                        else if (a == 0 && b == 1)
                            str = 1;
                        else
                            str = 2;
                        product.Id = str;
                    }
                    try
                    {
                        product.CVoucherNo = SqlDataReader["CVoucherNo"].ToString();
                    }
                    catch { }
                    try
                    {
                        product.DVoucherNo = SqlDataReader["DVoucherNo"].ToString();
                    }
                    catch { }
                    products.Add(product);
                }
            }
            else
            {
                var product = new ReportRpt();
                product.Name1 = product.Name2 = product.Name3 = product.CVoucherNo = product.DVoucherNo = "";
                product.M1 =
                    product.M2 =
                        product.M3 =
                            product.M4 =
                                product.M5 =
                                    product.M6 =
                                        product.M7 =
                                            product.M8 = product.M9 = product.M10 = product.M11 = product.M12 = 0;
                products.Add(product);
            }
            aSqlConnection.Close();
            return products;
        }
        public string GetRevExpString(string start, string end, int diff)
        {
            SqlQuery = "USP_RevenueExpense '" + start + "','" + end + "','Revenue';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var revList = new List<double>();
            while (SqlDataReader.Read())
            {
                for (var i = 1; i <= 12; i++)
                {
                    var m = 0.00;
                    try
                    {
                        m = Convert.ToDouble(SqlDataReader["M" + i]);
                    }
                    catch
                    {
                        m = 0.00;
                    }
                    revList.Add(m);
                }
            }
            aSqlConnection.Close();
            SqlQuery = "USP_RevenueExpense '" + start + "','" + end + "','Expense';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var expList = new List<double>();
            while (SqlDataReader.Read())
            {
                for (var i = 1; i <= 12; i++)
                {
                    var m = 0.00;
                    try
                    {
                        m = Convert.ToDouble(SqlDataReader["M" + i]);
                    }
                    catch
                    {
                        m = 0.00;
                    }
                    expList.Add(m);
                }
            }
            aSqlConnection.Close();
            var str = "";
            for (var i = 0; i <= 11; i++)
            {
                var m = revList[i] - expList[i];
                str += "," + m;
            }
            return str.Substring(1);
        }
        public object GetLedgers(string mainGroup, string companyId)
        {
            SqlQuery = "USP_LEDGER_LIST_LR '" + mainGroup + "','" + companyId + "';";
            aSqlConnection.Open();
            SqlCommand = new SqlCommand(SqlQuery, aSqlConnection);
            SqlDataReader = SqlCommand.ExecuteReader();
            var ledgers = new List<Ledger>();
            while (SqlDataReader.Read())
            {
                var ledger = new Ledger
                {
                    Id = Convert.ToInt32(SqlDataReader[0]),
                    GroupName = SqlDataReader[1].ToString()
                };
                ledgers.Add(ledger);
            }
            aSqlConnection.Close();
            return ledgers;
        }
    }
}