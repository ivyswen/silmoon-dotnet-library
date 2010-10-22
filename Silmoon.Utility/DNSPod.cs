using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Windows.Forms;
using System.Collections;
using System.Web;
using System.IO;

namespace Silmoon.Utility
{
    [Serializable()]
    public class DNSPod
    {
        protected internal string _baseAPIUri = "https://www.dnspod.com/";
        string _username;
        string _password;
        string _token;
        bool _isLogin = false;
        string _user_agent = "Unknown_SilmoonAssembly/0.0.0.0";
        public string _result = "";
        int _userID = 0;
        public bool BlackCase = false;
        public ArrayList APIHeaders = new ArrayList();

        /// <summary>
        /// ��ȡ״̬�Ƿ����Ѿ���¼
        /// </summary>
        public bool IsLogin
        {
            get { return _isLogin; }
            private set { _isLogin = value; }
        }
        /// <summary>
        /// ��ȡ�����õ�ǰ�û���
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        /// <summary>
        /// ��ȡ�������û�����
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        /// <summary>
        /// ���ð������ʹ�õ�Token
        /// </summary>
        public string Token
        {
            get { return _token; }
            private set { _token = value; }
        }
        /// <summary>
        /// ��ȡ������DNSPod API·��
        /// </summary>
        public string BaseAPIUri
        {
            get { return _baseAPIUri; }
            set { _baseAPIUri = value; }
        }
        /// <summary>
        /// ��ȡUA
        /// </summary>
        public string User_Agent
        {
            get { return _user_agent; }
        }
        /// <summary>
        /// ��ȡ�û�ID
        /// </summary>
        public int UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }

        /// <summary>
        /// ���͹�����
        /// </summary>
        public DNSPod()
        {

        }
        /// <summary>
        /// ���͹�������ָ��ʹ�õ�UA
        /// </summary>
        /// <param name="user_agent"></param>
        public DNSPod(string user_agent)
        {
            _user_agent = user_agent;
        }

        /// <summary>
        /// �����������POST����
        /// </summary>
        /// <param name="apiField">API�ֶ�</param>
        /// <param name="data">POST����</param>
        /// <returns>���������ص�����</returns>
        public string GetDNSPodServerXml(string apiField, string data)
        {
            WebClient _wclit = new WebClient();
            try
            {
                _wclit.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                _wclit.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                foreach (string s in APIHeaders)
                    _wclit.Headers.Add(s);
                _wclit.Headers.Add("User-Agent", "(SM)" + _user_agent);
                byte[] bytes = _wclit.UploadData(new Uri(_baseAPIUri + "API/" + apiField), Encoding.UTF8.GetBytes(data));
                _result = Encoding.UTF8.GetString(bytes);
                _wclit.Dispose();
                return Encoding.UTF8.GetString(bytes);
            }
            catch (WebException ex)
            {
                try
                {
                    File.WriteAllText(@"C:\DNSPodClientErrXmlString.xml.txt", DateTime.Now + "\r\n\r\n" + ex + "\r\n\r\n");
                    if (ex.Response != null)
                        File.AppendAllText(@"C:\DNSPodClientErrXmlString.xml.txt", new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                }
                catch { }
                _wclit.Dispose();
                return "!" + ex.ToString();
            }
        }

        /// <summary>
        /// ָ��ʹ�õ�UA
        /// </summary>
        /// <param name="user_agent"></param>
        public void SetUserAgent(string user_agent)
        {
            _user_agent = user_agent;
        }
        /// <summary>
        /// ָ��DNSPod API·��
        /// </summary>
        /// <param name="apiUrl"></param>
        public void SetAPIUrl(string apiUrl)
        {
            _baseAPIUri = apiUrl;
        }
        /// <summary>
        /// ��¼
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <returns></returns>
        public UserInfo Login(string username, string password)
        {
            BlackCase = false;

            _username = username;
            _password = password;
            return GetUserInfo();
        }
        /// <summary>
        /// ���õ�¼״̬
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <param name="login">�Ƿ�����Ϊ�Ѿ���¼</param>
        public void SetLoginState(string username, string password, bool login)
        {
            BlackCase = false;
            _username = username;
            _password = password;
            _isLogin = login;
        }
        /// <summary>
        /// ���������¼
        /// </summary>
        /// <param name="token">TOKEN</param>
        /// <param name="validate">�Ƿ���֤TOKEN</param>
        public UserInfo BlackCaseLogin(int userID, string token, bool validate = false)
        {
            BlackCase = true;
            UserID = userID;
            Token = token;
            if (validate)
            {
                return GetUserInfo();
            }
            return null;
        }
        /// <summary>
        /// ���õ�¼״̬
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="password">����</param>
        /// <param name="login">�Ƿ�����Ϊ�Ѿ���¼</param>
        public void SetBlackLoginState(int userID, string token, bool login)
        {
            BlackCase = true;
            UserID = userID;
            Token = token;
            _isLogin = login;
        }

        /// <summary>
        /// ��ȡ�û�����������
        /// </summary>
        /// <returns></returns>
        public DomainInfo[] GetDomains()
        {
            return GetDomains(DomainListType.all);
        }
        /// <summary>
        /// ��ȡ�û�ָ�����͵�����
        /// </summary>
        /// <param name="type">������������</param>
        /// <returns></returns>
        public DomainInfo[] GetDomains(DomainListType type)
        {
            XmlDocument _xml = new XmlDocument();

            string resultXml = GetDNSPodServerXml("Domain.List", AuthConnection() + "&type=" + type.ToString());
            if (resultXml == "") return null;

            LoadXml(ref resultXml, ref _xml);
            ArrayList binfoArr = new ArrayList();
            XmlNode domainsNode = _xml["dnspod"]["domains"];

            foreach (XmlNode node in domainsNode)
            {
                DomainInfo newInfo = new DomainInfo();
                newInfo.DomainName = node["name"].InnerText;
                newInfo.State = SmString.StringToBool(node["status"].InnerText);
                newInfo.Records = int.Parse(node["records"].InnerText);
                newInfo.ID = int.Parse(node["id"].InnerText);
                newInfo.Grade = StringToGrade(node["grade"].InnerText);
                if (node["shared_from"] != null)
                    newInfo.ShardForm = node["shared_from"].InnerText;
                newInfo.Validate = DNSPodUnitValidate.FromDNSPod;
                binfoArr.Add(newInfo);
            }
            return (DomainInfo[])binfoArr.ToArray(typeof(DomainInfo));
        }
        /// <summary>
        /// ��ȡ�û�������¼
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <returns>������¼����</returns>
        public RecordInfo[] GetRecords(int domainID)
        {
            XmlDocument _xml = new XmlDocument();
            ArrayList array = new ArrayList();

            string resultXml = GetDNSPodServerXml("Record.List", AuthConnection() + "&domain_id=" + domainID);
            if (resultXml == "") return null;
            LoadXml(ref resultXml, ref _xml);

            if (_xml.GetElementsByTagName("code")[0].InnerText == "1" || _xml.GetElementsByTagName("code")[0].InnerText == "7")
            {
                XmlNode recordsNode = _xml["dnspod"]["records"];
                foreach (XmlNode node in recordsNode)
                {
                    RecordInfo newRecord = new RecordInfo();
                    newRecord.Enable = SmString.StringToBool(node["enabled"].InnerText);
                    newRecord.ID = int.Parse(node["id"].InnerText);
                    newRecord.Isp = node["line"].InnerText;
                    newRecord.MXLevel = int.Parse(node["mx"].InnerText);
                    newRecord.Subname = node["name"].InnerText;
                    newRecord.TTL = int.Parse(node["ttl"].InnerText);
                    newRecord.Type = DNSPod.StringToRecordType(node["type"].InnerText);
                    newRecord.Value = node["value"].InnerText;
                    newRecord.Validate = DNSPodUnitValidate.FromDNSPod;
                    array.Add(newRecord);
                }
            }
            return (RecordInfo[])array.ToArray(typeof(RecordInfo));
        }
        /// <summary>
        /// ������ID�ͼ�¼ID��ȡ��¼��Ϣ
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="recordID">��¼ID</param>
        /// <returns></returns>
        public RecordInfo GetRecord(int domainID, int recordID)
        {
            string s = GetDNSPodServerXml("Record.Info", AuthConnection() + "&domain_id=" + domainID + "&recordid=" + recordID);
            XmlDocument xml = new XmlDocument();
            LoadXml(ref s, ref xml);
            RecordInfo result = null;
            if (xml["dnspod"]["status"]["code"].InnerText == "1")
            {
                XmlNode node = xml["dnspod"]["record"];
                result = new RecordInfo();
                result.Enable = SmString.StringToBool(node["enabled"].InnerText);
                result.ID = int.Parse(node["id"].InnerText);
                result.Subname = node["sub_domain"].InnerText;
                result.Isp = node["record_line"].InnerText;
                result.Type = DNSPod.StringToRecordType(node["record_type"].InnerText);
                result.Validate = DNSPodUnitValidate.FromDNSPod;
                result.Value = node["value"].InnerText;
                result.MXLevel = int.Parse(node["mx"].InnerText);
                result.TTL = int.Parse(node["ttl"].InnerText);
            }
            return result;
        }
        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="enable">�Ƿ�����</param>
        /// <returns></returns>
        public StateFlag SetDomainState(int domainID, bool enable)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string enableArgs = "";
            if (enable) enableArgs = "enable"; else enableArgs = "disable";
            string resultXml = GetDNSPodServerXml("Domain.Status", AuthConnection() + "&domain_id=" + domainID + "&status=" + enableArgs);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="domain">Ҫ��ӵ�����</param>
        /// <returns></returns>
        public StateFlag CreateDomain(string domain)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string resultXml = GetDNSPodServerXml("Domain.Create", AuthConnection() + "&domain=" + domain);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="domainID">Ҫɾ��������ID</param>
        /// <returns></returns>
        public StateFlag RemoveDomain(int domainID)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string resultXml = GetDNSPodServerXml("Domain.Remove", AuthConnection() + "&domain_id=" + domainID);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ��Ӽ�¼
        /// </summary>
        /// <param name="record">������¼��Ϣ</param>
        /// <param name="domainID">����ID</param>
        /// <returns>״̬��</returns>
        public StateFlag CreateRecord(RecordInfo record, int domainID)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string urlArgs = AuthConnection();
            urlArgs += "&domain_id=" + domainID;
            urlArgs += "&sub_domain=" + record.Subname;
            urlArgs += "&record_type=" + record.Type.ToString();
            urlArgs += "&record_line=" + record.Isp.ToString().ToLower();
            urlArgs += "&value=" + HttpUtility.UrlEncode(record.Value);
            urlArgs += "&mx=" + record.MXLevel;
            urlArgs += "&ttl=" + record.TTL;
            string resultXml = GetDNSPodServerXml("Record.Create", urlArgs);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            if (_xml.GetElementsByTagName("id").Count == 0)
                result.ID = 0;
            else
                result.ID = int.Parse(_xml.GetElementsByTagName("id")[0].InnerText);

            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// �༭������¼
        /// </summary>
        /// <param name="record">������¼��Ϣ</param>
        /// <param name="domainID">����ID</param>
        /// <returns>״̬��</returns>
        public StateFlag ModifyRecord(RecordInfo record, int domainID)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string urlArgs = AuthConnection();
            urlArgs += "&domain_id=" + domainID;
            urlArgs += "&record_id=" + record.ID;
            urlArgs += "&sub_domain=" + record.Subname;
            urlArgs += "&record_type=" + record.Type.ToString();
            urlArgs += "&record_line=" + record.Isp.ToString().ToLower();
            urlArgs += "&value=" + HttpUtility.UrlEncode(record.Value);
            urlArgs += "&mx=" + record.MXLevel;
            urlArgs += "&ttl=" + record.TTL;
            string resultXml = GetDNSPodServerXml("Record.Modify", urlArgs);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ɾ��������¼
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="recordID">��¼ID</param>
        /// <returns>�����</returns>
        public StateFlag RemoveRecord(int domainID, int recordID)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string resultXml = GetDNSPodServerXml("Record.Remove", AuthConnection() + "&record_id=" + recordID + "&domain_id=" + domainID);
            if (resultXml == "")
            {
                result.DoubleStateFlag = false;
                result.IntStateFlag = -99;
                result.Message = "server error";
                return result;
            }

            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ����������¼״̬
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="recordID">��¼ID</param>
        /// <param name="enable">�Ƿ�����</param>
        /// <returns>�����</returns>
        public StateFlag SetRecordState(int domainID, int recordID, bool enable)
        {
            XmlDocument _xml = new XmlDocument();

            StateFlag result = new StateFlag();
            string enableArgs = "";
            if (enable) enableArgs = "enable"; else enableArgs = "disable";
            string resultXml = GetDNSPodServerXml("Record.Status", AuthConnection() + "&record_id=" + recordID + "&domain_id=" + domainID + "&status=" + enableArgs);
            LoadXml(ref resultXml, ref _xml);
            result.IntStateFlag = int.Parse(_xml.GetElementsByTagName("code")[0].InnerText);
            if (result.IntStateFlag == 1) result.DoubleStateFlag = true;
            result.Message = _xml.GetElementsByTagName("message")[0].InnerText;
            return result;
        }
        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <returns>������Ϣ</returns>
        public DomainInfo GetDomainInfo(int domainID)
        {
            DomainInfo[] domains = GetDomains();
            foreach (DomainInfo domain in domains)
            {
                if (domain.ID == domainID)
                    return domain;
            }
            return null;
        }
        /// <summary>
        /// ��ȡ������Ϣ��������Ϣ������
        /// </summary>
        /// <param name="domainID">����ID</param>
        /// <param name="domains">������Ϣ����</param>
        /// <returns>������Ϣ</returns>
        public DomainInfo GetDomainInfo(int domainID, DomainInfo[] domains)
        {
            foreach (DomainInfo domain in domains)
            {
                if (domain.ID == domainID)
                    return domain;
            }
            return null;
        }
        /// <summary>
        /// ��ȡ������Ϣ
        /// </summary>
        /// <param name="domain">����</param>
        /// <returns>������Ϣ</returns>
        public DomainInfo GetDomainInfo(string domain)
        {
            DomainInfo[] domains = GetDomains();
            foreach (DomainInfo domainInfo in domains)
            {
                if (domainInfo.DomainName.ToLower() == domain.ToLower())
                    return domainInfo;
            }
            return null;
        }
        /// <summary>
        /// ��ȡ������Ϣ��������Ϣ������
        /// </summary>
        /// <param name="domain">����</param>
        /// <param name="domains">������Ϣ����</param>
        /// <returns>������Ϣ</returns>
        public DomainInfo GetDomainInfo(string domain, DomainInfo[] domains)
        {
            foreach (DomainInfo domainInfo in domains)
            {
                if (domainInfo.DomainName.ToLower() == domain.ToLower())
                    return domainInfo;
            }
            return null;
        }
        /// <summary>
        /// ��ȡ�û���Ϣ
        /// </summary>
        /// <returns></returns>
        public UserInfo GetUserInfo()
        {
            XmlDocument _xml = new XmlDocument();

            UserInfo userInfo = new UserInfo();
            string resultXml = GetDNSPodServerXml("User.Info", AuthConnection());
            if (resultXml == "")
                userInfo.StateCode = -99;
            else
            {
                LoadXml(ref resultXml, ref _xml);
                userInfo.StateCode = int.Parse(_xml["dnspod"]["status"]["code"].InnerText);
                userInfo.Message = _xml["dnspod"]["status"]["message"].InnerText;
                if (userInfo.StateCode == 1)
                {
                    userInfo.LoginOK = true;
                    IsLogin = true;
                    userInfo.UserID = int.Parse(_xml["dnspod"]["user"]["id"].InnerText);
                    userInfo.Username = _xml["dnspod"]["user"]["email"].InnerText;
                    Username = userInfo.Username;
                    UserID = userInfo.UserID;
                }
                else
                {
                    userInfo.LoginOK = false;
                    IsLogin = false;
                }
            }

            return userInfo;
        }
        /// <summary>
        /// ��ȡDNSPodһ��������
        /// </summary>
        /// <returns>һ��������</returns>
        public string GetOncePassword()
        {
            XmlDocument _xml = new XmlDocument();

            string resultXml = GetDNSPodServerXml("Login.Key", AuthConnection());
            LoadXml(ref resultXml, ref _xml);
            if (_xml.GetElementsByTagName("code")[0].InnerText == "1")
            {
                return _xml.GetElementsByTagName("key")[0].InnerText;
            } return "";
        }
        /// <summary>
        /// ��ȡ�����Ŀ�����·
        /// </summary>
        /// <param name="domainInfo">������Ϣ</param>
        /// <returns></returns>
        public string[] GetDomainNetworkType(DomainInfo domainInfo)
        {
            string s = GetDNSPodServerXml("Record.Line", AuthConnection() + "&domain_grade=" + domainInfo.Grade);
            XmlDocument xml = new XmlDocument();
            ArrayList array = new ArrayList();

            LoadXml(ref s, ref xml);
            if (xml["dnspod"]["status"]["code"].InnerText == "1")
            {
                XmlNode xmlNode = xml["dnspod"]["lines"];
                foreach (XmlNode item in xmlNode)
                {
                    array.Add(item.InnerText);
                }
            }
            return (string[])array.ToArray(typeof(string));
        }
        /// <summary>
        /// Load Xml from string
        /// </summary>
        /// <param name="xmlString">string</param>
        /// <param name="xmlDoc">xml object</param>
        public void LoadXml(ref string xmlString, ref XmlDocument xmlDoc)
        {
            try
            {
                xmlDoc.LoadXml(xmlString);
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public static RecordType StringToRecordType(string type)
        {
            switch (type.ToLower())
            {
                case "a":
                    return RecordType.A;
                case "cname":
                    return RecordType.CNAME;
                case "mx":
                    return RecordType.MX;
                case "url":
                    return RecordType.URL;
                case "ns":
                    return RecordType.NS;
                case "txt":
                    return RecordType.TXT;
                case "aaaa":
                    return RecordType.AAAA;
                case "cn":
                    return RecordType.CNAME;
                case "v6":
                    return RecordType.AAAA;
                default:
                    return RecordType.A;
            }
        }
        public static DomainGrade StringToGrade(string grade)
        {
            switch (grade.ToLower())
            {
                case "d_free":
                    return DomainGrade.D_Free;
                case "d_express":
                    return DomainGrade.D_Express;
                case "d_plus":
                    return DomainGrade.D_Plus;
                case "d_extra":
                    return DomainGrade.D_Extra;
                case "d_expert":
                    return DomainGrade.D_Expert;
                case "d_ultra":
                    return DomainGrade.D_Ultra;
                default:
                    return DomainGrade.D_Free;
            }
        }

        public string AuthConnection()
        {
            if (BlackCase)
            {
                return "login_id=" + UserID + "&login_token=" + Token + "&format=xml";
            }
            else
            {
                return "login_email=" + HttpUtility.UrlEncode(_username) + "&login_password=" + HttpUtility.UrlEncode(_password) + "&format=xml";
            }
        }

        public enum DomainListType
        {
            all, share, mine
        }
    }
    public class DomainInfo
    {
        public string DomainName;
        public bool State;
        public int Records;
        public DomainGrade Grade;
        public int ID;
        public string ShardForm;
        public DNSPodUnitValidate Validate = DNSPodUnitValidate.New;
    }
    public enum DomainGrade
    {
        D_Free = 0,
        D_Express = 1,
        D_Plus = 2,
        D_Extra = 3,
        D_Expert = 4,
        D_Ultra = 5
    }
    public class RecordInfo
    {
        public int ID;
        public string Subname;
        public string Isp = "Ĭ��";
        public RecordType Type;
        public int TTL = 3600;
        public string Value;
        public int MXLevel = 5;
        public bool Enable = true;
        public DNSPodUnitValidate Validate = DNSPodUnitValidate.New;
        public string Note;
        public RecordInfo() { }
        public RecordInfo(string subname, string isp, RecordType type, string value)
        {
            Subname = subname;
            Isp = isp;
            Type = type;
            Value = value;
        }
    }
    public class UserInfo
    {
        public string Username;
        public int UserID;
        public int StateCode = -99;
        public bool LoginOK = false;
        public string Message;
    }
    public enum AgentGrade
    {
        unknown = 0,
        bronze = 1,
        silver = 2,
        gold = 3,
        diamond = 4,
    }

    public enum RecordType
    {
        A = 1,
        CNAME = 2,
        MX = 3,
        URL = 4,
        NS = 5,
        TXT = 6,
        AAAA = 7,
    }
    public enum DNSPodUnitValidate
    {
        FromDNSPod,
        Invalid,
        New,
    }
}